using Basic.Framework.Logging;
using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Protocols.Devices;
using Sys.DataCollection.Common.Utils;
using Sys.DataCollection.Driver.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    public class RealDataResponseCommand : QueryRealDataRequest
    {
        /// <summary>
        /// 表示当前数据帧，存储开关量对应的状态值，主要用于馈电异常状态生成
        /// </summary>
        private byte[] DerailState = new byte[46];
        /// <summary>
        /// 传入的分站设备定义信息
        /// </summary>
        public DeviceInfo def = null;
        /// <summary>
        /// 表示命令版本，即=13表示kj306-f(16)h智能分站；=1表示大分站；=14表示抽放分站；
        /// </summary>
        public byte OrderVersion;
        /// <summary>
        /// 用于字符串转换为字节数组
        /// </summary>
        /// <param name="InString"></param>
        /// <returns></returns>
        public byte[] StringToByte(string InString)
        {
            string[] ByteStrings;
            ByteStrings = InString.Split("-".ToCharArray());
            byte[] ByteOut;
            ByteOut = new byte[ByteStrings.Length];
            for (int i = 0; i <= ByteStrings.Length - 1; i++)
            {
                ByteOut[i] = Convert.ToByte(ByteStrings[i], 16);
            }
            return ByteOut;
        }
        /// <summary>
        /// 处理回发的数据体，以分站为对象
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ProtocolData"></param>
        /// <param name="startIndex">当前为分站的下标索引 </param>
        public void HandleRealData(byte[] data, MasProtocol protocol, ushort startIndex, byte orderVersion)
        {
            QueryRealDataResponse RealDataObject = new QueryRealDataResponse();//返回列表
            RealDataObject.DeviceCode = def.Point;
            RealDataObject.RealDataItems = new List<RealDataItem>();
            RealDataObject.HistoryRealDataItems = new List<DeviceHistoryRealDataItem>();
            this.OrderVersion = orderVersion;
            RealDataObject.DeviceCommperType = 0;
            HandleRealDataCapacity(data, startIndex, RealDataObject);
            protocol.ProtocolType = ProtocolType.QueryRealDataResponse;
            protocol.Protocol = RealDataObject;
        }
        /// <summary>
        /// 生成实时值对象
        /// </summary>
        /// <param name="RealDataObject"></param>
        /// <param name="Channel"></param>
        /// <param name="Address"></param>
        /// <param name="DeviceTypeCode"></param>
        /// <param name="DeviceProperty"></param>
        /// <param name="RealValue"></param>
        /// <param name="State"></param>
        /// <param name="Voltage"></param>
        /// <param name="FeedBackState"></param>
        /// <param name="FeedState"></param>
        /// <param name="DeviceOnlyCode"></param>
        private void AddRealData(QueryRealDataResponse RealDataObject, string Channel, string Address, int DeviceTypeCode, ItemDevProperty DeviceProperty,
          string RealValue, ItemState State, string Voltage, string FeedBackState, string FeedState, string DeviceOnlyCode, byte checkpower = 0, byte seniorGradeAlarm = 0)
        {
            RealDataItem RealData = new RealDataItem();
            RealData = new RealDataItem();
            RealData.Address = Address;
            RealData.Channel = Channel;
            RealData.DeviceProperty = DeviceProperty;
            RealData.DeviceTypeCode = DeviceTypeCode;
            RealData.RealData = RealValue;
            RealData.State = State;
            RealData.Voltage = Voltage;
            RealData.SoleCoding = DeviceOnlyCode;
            RealData.FeedState = FeedState;
            RealData.FeedBackState = FeedBackState;
            RealData.ChangeSenior = checkpower;
            RealData.SeniorGradeAlarm = seniorGradeAlarm;
            RealDataObject.RealDataItems.Add(RealData);
        }

        /// <summary>
        /// 处理F命令上去的唯一性编码
        /// </summary>
        private ushort HandleSoleCoding(int soleCodingCount, byte[] data, ushort startIndex, QueryRealDataResponse realDataObject)
        {
            int index = 0;
            byte branch;//分支号
            byte address;//地址号
            RealDataItem item;
            UInt32 soleCoding;//唯一性编码；
            ushort SensorDevID = 0;//设备的类型
            ItemState SensorTempState = ItemState.EquipmentCommOK;//设备状态
            for (; index < soleCodingCount; index++)//有多少个唯一性编码
            {
                SensorTempState = ItemState.EquipmentCommOK;//设备状态
                branch = data[startIndex++];//分支号及地址号
                SensorDevID = data[startIndex++];//设备型号
                soleCoding = CommandUtil.ConvertByteToInt(data, startIndex);//唯一性编码
                address = (byte)(branch >> 3);//地址号
                branch = (byte)(branch & 0x07);//分支号
                branch += 1;
                item = new RealDataItem();
                item.DeviceProperty = ItemDevProperty.SoleCoding;
                item.DeviceTypeCode = SensorDevID;//设备型号
                item.BranchNumber = branch;//分支号
                if ((soleCoding & 0xFFFFFF) != 0x00)
                {
                    item.SoleCoding = (((soleCoding >> 19) & 0x1F) + 2018).ToString().PadLeft(4, '0')
                        + ((soleCoding >> 15) & 0x0F).ToString().PadLeft(2, '0')
                        + ((soleCoding >> 10) & 0x1F).ToString().PadLeft(2, '0')
                        + (soleCoding & 0x3FF).ToString().PadLeft(4, '0');
                }
                else
                {
                    item.SoleCoding = "0";
                }
                item.Address = "0";
                item.Channel = address.ToString();//地址号
                item.RealData = "0";
                item.State = SensorTempState;
                realDataObject.RealDataItems.Add(item);
                startIndex += 4;
            }
            return startIndex;
        }
        private void HandleLocalControl(byte localControlValue, QueryRealDataResponse realDataObject)
        {
            List<DeviceInfo> items = null;
            DeviceInfo itemderail = null;
            DeviceInfo itemcontrol = null;
            Driver.ControlInfo itemcinfo = null;
            bool ishaveupdate = false;//是否更新馈电状态
            byte FeedState = 3;//1断电成功 2 断电失败 3 复电成功 4 复电失败）
            byte DeciveState = 0;//1断开，0接通
            string strKd = "";

            items = Cache.CacheManager.Query<DeviceInfo>(p => p.Fzh == def.Fzh && p.Dzh == 0 && p.DevPropertyID == 3, true);//是否有触点控制的定义
            if (items == null || items.Count == 0) return;
            for (int i = 0; i < items.Count; i++)
            {
                itemcontrol = items[i];
                try
                {
                    if (itemcontrol.K1 > 0 && itemcontrol.K2 > 0)
                    {
                        //有馈电量
                        itemderail = Cache.CacheManager.QueryFrist<DeviceInfo>(p => p.Fzh == itemcontrol.K1 && p.Kh == itemcontrol.K2 && p.Dzh == 0 && p.DevPropertyID == 2, true);//对应的馈电开关  

                        if ((localControlValue & (1 << (byte)(itemcontrol.Kh - 1))) == (1 << (byte)(itemcontrol.Kh - 1)))
                        {//1--断开
                            #region ----控制量断开（断电）----

                            DeciveState = 1;//断开标记
                            FeedState = 1;//默认断电成功
                            lock (Cache.LockControlInfo)
                            {
                                itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 4));
                                if (itemcinfo != null)//如果有复电失败，清除掉
                                {
                                    Cache.LstControlInfo.Remove(itemcinfo);
                                }
                            }
                            if (itemderail != null)
                            {
                                //if (DerailState[itemcontrol.K2 - 1] == 2)//馈电为有电即2态时，表明为断电失败    
                                if (DerailState[itemcontrol.K2 - 1] == 2)
                                    strKd = itemderail.Bz8;
                                else if (DerailState[itemcontrol.K2 - 1] == 1)
                                    strKd = itemderail.Bz7;
                                else
                                    strKd = itemderail.Bz6;
                                //if (DerailState[itemcontrol.K2 - 1] == 2)//馈电为有电即2态时，表明为断电失败    
                                if (strKd == "有电")
                                {
                                    //进行容错判断
                                    FeedState = 5;//默认不更新馈电
                                    ishaveupdate = false;
                                    lock (Cache.LockControlInfo)
                                    {
                                        itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 2));
                                    }
                                    if (itemcinfo != null)
                                    {
                                        if ((DateTime.Now - itemcinfo.FailTime).TotalSeconds >= Cache.FeedTimeOut)
                                        {
                                            ishaveupdate = true;
                                        }
                                    }
                                    else
                                    {
                                        itemcinfo = new Driver.ControlInfo();
                                        itemcinfo.Pid = itemcontrol.Fzh * 50 + itemcontrol.Kh;
                                        itemcinfo.FailTime = DateTime.Now;
                                        itemcinfo.FeedState = 2;
                                        lock (Cache.LockControlInfo)
                                        {
                                            Cache.LstControlInfo.Add(itemcinfo);
                                        }
                                    }
                                    if (ishaveupdate)//条件满足可以进行更新
                                    {
                                        FeedState = 2;
                                    }
                                }
                                else
                                {
                                    lock (Cache.LockControlInfo)
                                    {
                                        itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 2));
                                        if (itemcinfo != null)
                                            Cache.LstControlInfo.Remove(itemcinfo);
                                    }
                                }
                            }
                            else
                            {
                                lock (Cache.LockControlInfo)
                                {
                                    itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 2));
                                    if (itemcinfo != null)
                                        Cache.LstControlInfo.Remove(itemcinfo);
                                }
                            }

                            #endregion
                        }
                        else
                        {//0--接通
                            #region ----控制量接通（复电）----

                            DeciveState = 0;//接通.
                            FeedState = 3;//复电成功
                            lock (Cache.LockControlInfo)
                            {
                                itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 2));
                                if (itemcinfo != null)
                                    Cache.LstControlInfo.Remove(itemcinfo);
                            }
                            if (itemderail != null)
                            {
                                //if (DerailState[itemcontrol.K2 - 1] == 1)//馈电为无电即1态时，表明为复电失败 
                                if (DerailState[itemcontrol.K2 - 1] == 2)
                                    strKd = itemderail.Bz8;
                                else if (DerailState[itemcontrol.K2 - 1] == 1)
                                    strKd = itemderail.Bz7;
                                else
                                    strKd = itemderail.Bz6;
                                //if (DerailState[itemcontrol.K2 - 1] == 1)//馈电为无电即1态时，表明为复电失败 
                                if (strKd == "无电")
                                {
                                    FeedState = 5;//默认不更新馈电
                                    //进行容错判断
                                    ishaveupdate = false;
                                    lock (Cache.LockControlInfo)
                                    {
                                        itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 4));
                                    }
                                    if (itemcinfo != null)
                                    {
                                        if ((DateTime.Now - itemcinfo.FailTime).TotalSeconds >= Cache.FeedTimeOut)
                                        {
                                            ishaveupdate = true;
                                        }
                                    }
                                    else
                                    {
                                        itemcinfo = new Driver.ControlInfo();
                                        itemcinfo.Pid = itemcontrol.Fzh * 50 + itemcontrol.Kh;
                                        itemcinfo.FailTime = DateTime.Now;
                                        itemcinfo.FeedState = 4;
                                        lock (Cache.LockControlInfo)
                                        {
                                            Cache.LstControlInfo.Add(itemcinfo);
                                        }
                                    }
                                    if (ishaveupdate)//条件满足可以进行更新
                                    {
                                        FeedState = 4;
                                    }
                                }
                                else
                                {
                                    lock (Cache.LockControlInfo)
                                    {
                                        itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 4));
                                        if (itemcinfo != null)
                                            Cache.LstControlInfo.Remove(itemcinfo);
                                    }
                                }
                            }
                            else
                            {
                                lock (Cache.LockControlInfo)
                                {
                                    itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 4));
                                    if (itemcinfo != null)
                                        Cache.LstControlInfo.Remove(itemcinfo);
                                }
                            }

                            #endregion
                        }
                    }
                    else
                    {//如果没有馈电量，不写馈电状态标记
                        DeciveState = 0;
                        FeedState = 3;
                        if ((localControlValue & (1 << (int)(itemcontrol.Kh - 1))) == (1 << (int)(itemcontrol.Kh - 1)))//1是断开
                        {
                            DeciveState = 1;//位运算得出相应通道是否为1.
                            FeedState = 1;
                        }
                        lock (Cache.LockControlInfo)
                        {
                            itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 4));
                            if (itemcinfo != null)
                                Cache.LstControlInfo.Remove(itemcinfo);

                            itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 2));
                            if (itemcinfo != null)
                                Cache.LstControlInfo.Remove(itemcinfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("判断出错：" + ex.Message);
                }
                if (!Cache.FeedComplexFailure)//判断如果配置了不处理复电失败，则直接返回  20181213
                {
                    if (FeedState == 4)
                    {
                        AddRealData(realDataObject, itemcontrol.Kh.ToString(), itemcontrol.Dzh.ToString(), 0, ItemDevProperty.Control, DeciveState.ToString(),
                          ItemState.EquipmentCommOK, "0", "", "", "");
                    }
                    else
                    {
                        AddRealData(realDataObject, itemcontrol.Kh.ToString(), itemcontrol.Dzh.ToString(), 0, ItemDevProperty.Control, DeciveState.ToString(),
                            ItemState.EquipmentCommOK, "0", "", FeedState.ToString(), "");
                    }
                }
                else
                {
                    AddRealData(realDataObject, itemcontrol.Kh.ToString(), itemcontrol.Dzh.ToString(), 0, ItemDevProperty.Control, DeciveState.ToString(),
                           ItemState.EquipmentCommOK, "0", "", FeedState.ToString(), "");
                }
            }
        }

        /// <summary>
        /// 得到馈电的状态
        /// </summary>
        /// <param name="UintValue"></param>
        /// <returns></returns>
        private byte GetTicklingState(uint UintValue, DeviceInfo itemcontrol)
        {
            byte DeviceKdState = 3;
            Driver.ControlInfo itemcinfo = null;

            if (((UintValue & 0x01) == 0x01) && ((UintValue & 0x02) == 0x02)) //断电成功
            {
                DeviceKdState = 1;
            }
            else if (((UintValue & 0x01) == 0x01) && ((UintValue & 0x02) == 0x00)) //断电失败
            {
                DeviceKdState = 2;
            }
            else if (((UintValue & 0x01) == 0x00) && ((UintValue & 0x02) == 0x00))//复电成功
            {
                DeviceKdState = 3;
            }
            else if (((UintValue & 0x01) == 0x00) && ((UintValue & 0x02) == 0x02))//复电失败
            {
                DeviceKdState = 4;
            }
            if ((UintValue & 0x01) == 0x01)
            {//断开
                lock (Cache.LockControlInfo)//先删除复电失败的缓存
                {
                    itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 4));
                    if (itemcinfo != null)
                    {
                        Cache.LstControlInfo.Remove(itemcinfo);
                    }
                }
                if (DeviceKdState == 2)//如果为断电失败才进行时间容错处理
                {
                    lock (Cache.LockControlInfo)
                    {
                        itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 2));
                    }
                    if (itemcinfo != null)
                    {
                        if ((DateTime.Now - itemcinfo.FailTime).TotalSeconds < Cache.FeedTimeOut)//时间不到，不更新馈电
                        {
                            DeviceKdState = 5;//默认不更新馈电
                        }
                    }
                    else
                    {
                        DeviceKdState = 5;//默认不更新馈电 2017.11.1 by AI
                        itemcinfo = new Driver.ControlInfo();
                        itemcinfo.Pid = itemcontrol.Fzh * 50 + itemcontrol.Kh;
                        itemcinfo.FailTime = DateTime.Now;
                        itemcinfo.FeedState = 2;
                        lock (Cache.LockControlInfo)
                        {
                            Cache.LstControlInfo.Add(itemcinfo);
                        }
                    }
                }
                else
                {
                    lock (Cache.LockControlInfo)
                    {
                        itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 2));
                        if (itemcinfo != null)
                            Cache.LstControlInfo.Remove(itemcinfo);
                    }
                }
            }
            else
            {//接通
                lock (Cache.LockControlInfo)
                {
                    itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 2));
                    if (itemcinfo != null)
                        Cache.LstControlInfo.Remove(itemcinfo);
                }
                if (DeviceKdState == 4)//如果为复电失败才进行时间容错处理
                {
                    lock (Cache.LockControlInfo)
                    {
                        itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 4));
                    }
                    if (itemcinfo != null)
                    {
                        if ((DateTime.Now - itemcinfo.FailTime).TotalSeconds < Cache.FeedTimeOut)
                        {
                            DeviceKdState = 5;//默认不更新馈电
                        }
                    }
                    else
                    {
                        DeviceKdState = 5;//默认不更新馈电 2017.11.1 by AI
                        itemcinfo = new Driver.ControlInfo();
                        itemcinfo.Pid = itemcontrol.Fzh * 50 + itemcontrol.Kh;
                        itemcinfo.FailTime = DateTime.Now;
                        itemcinfo.FeedState = 4;
                        lock (Cache.LockControlInfo)
                        {
                            Cache.LstControlInfo.Add(itemcinfo);
                        }
                    }
                }
                else
                {
                    lock (Cache.LockControlInfo)
                    {
                        itemcinfo = Cache.LstControlInfo.Find(a => (a.Pid == (itemcontrol.Fzh * 50 + itemcontrol.Kh)) && (a.FeedState == 4));
                        if (itemcinfo != null)
                            Cache.LstControlInfo.Remove(itemcinfo);
                    }
                }
            }
            return DeviceKdState;
        }
        /// <summary>
        /// 主要处理智能的控制量，为智能断电器&KXB18
        /// </summary>
        /// <param name="UintValue"></param>
        /// <param name="SensorTempState"></param>
        /// <param name="SensorDevID"></param>
        /// <param name="item"></param>
        /// <param name="RealDataObject"></param>
        private void GetZNControlCapacity(uint UintValue, ItemState SensorTempState, ushort SensorDevID, List<DeviceInfo> items, QueryRealDataResponse RealDataObject)
        {
            byte DeviceKdState = 0;
            //智能控制设备，第一个参数为控制，第二个参数为开关量。
            if (SensorTempState == ItemState.EquipmentCommOK ||
                SensorTempState == ItemState.EquipmentInfrareding ||
                SensorTempState == ItemState.EquipmentStart ||
                SensorTempState == ItemState.EquipmentAdjusting ||
                SensorTempState == ItemState.EquipmentTypeError)
            {
                if (items[0].Devid == "17") //处理 KXB180xB2
                {
                    AddRealData(RealDataObject, items[0].Kh.ToString(), items[0].Dzh.ToString(), SensorDevID, ItemDevProperty.Control, ((UintValue & 0x4) >> 2).ToString(),
          SensorTempState, items[0].Voltage.ToString(), "", "", "");
                }
                else
                {
                    DeviceKdState = GetTicklingState(UintValue, items[0]);//统一改成由馈电进行计算
                    AddRealData(RealDataObject, items[0].Kh.ToString(), items[0].Dzh.ToString(), SensorDevID, ItemDevProperty.Control, (UintValue & 0x1).ToString(),
                                SensorTempState, items[0].Voltage.ToString(), "", DeviceKdState.ToString(), "");
                }
            }
            else
            {
                AddRealData(RealDataObject, items[0].Kh.ToString(), items[0].Dzh.ToString(), SensorDevID, ItemDevProperty.Control, (UintValue & 0x1).ToString(),
          ItemState.EquipmentControlDown, items[0].Voltage.ToString(), "", "", "");
            }
            GetZNControlDerailCapacity(UintValue, SensorTempState, SensorDevID, items, RealDataObject);//处理第二个参数
        }
        /// <summary>
        /// 得到智能控制设备后续的开关量信息
        /// </summary>
        private void GetZNControlDerailCapacity(uint UintValue, ItemState SensorTempState, ushort SensorDevID, List<DeviceInfo> items, QueryRealDataResponse RealDataObject)
        {
            if (items[1] == null)
            {
                return;
            }
            if (SensorTempState == ItemState.EquipmentCommOK ||
                SensorTempState == ItemState.EquipmentInfrareding ||
                SensorTempState == ItemState.EquipmentStart ||
                SensorTempState == ItemState.EquipmentAdjusting)
            {
                if (items[1].Devid == "17") //处理KXB18
                {
                    AddRealData(RealDataObject, items[1].Kh.ToString(), items[1].Dzh.ToString(), SensorDevID, ItemDevProperty.Derail, (UintValue & 0x03).ToString(),
              SensorTempState, items[0].Voltage.ToString(), "", "", "");
                }
                else
                {
                    //0表示有电 1表示无电
                    if (((UintValue & 0x2) >> 1) == 0)
                    {
                        AddRealData(RealDataObject, items[1].Kh.ToString(), items[1].Dzh.ToString(), SensorDevID, ItemDevProperty.Derail, "2",
                                    SensorTempState, items[0].Voltage.ToString(), "", "", "");
                    }
                    else if (((UintValue & 0x2) >> 1) == 1)
                    {
                        AddRealData(RealDataObject, items[1].Kh.ToString(), items[1].Dzh.ToString(), SensorDevID, ItemDevProperty.Derail, "1",
                                    SensorTempState, items[0].Voltage.ToString(), "", "", "");
                    }
                }
            }
            else
            {
                AddRealData(RealDataObject, items[1].Kh.ToString(), items[1].Dzh.ToString(), SensorDevID, ItemDevProperty.Derail, "0",
                            SensorTempState, items[0].Voltage.ToString(), "", "", "");
            }
        }
        private void GetDerailFromCapacity(uint UintValue, ItemState SensorTempState, ushort SensorDevID, DeviceInfo item, QueryRealDataResponse RealDataObject)
        {
            if (UintValue < 0 || UintValue > 2)
            {
                UintValue = 0;
            }
            if (SensorTempState == ItemState.EquipmentHeadDown ||
                SensorTempState == ItemState.EquipmentDown ||
                SensorTempState == ItemState.EquipmentBiterror)
            {
                UintValue = 0;
            }
            DerailState[item.Kh - 1] = (byte)UintValue;
            AddRealData(RealDataObject, item.Kh.ToString(), item.Dzh.ToString(), SensorDevID, ItemDevProperty.Derail, UintValue.ToString(),
           SensorTempState, item.Voltage.ToString(), "", "", "");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">回发数据包</param>
        /// <param name="startIndex">分站的下标索引</param>
        /// <param name="realDataObject">回发的数据集合</param>
        private void HandleRealDataCapacity(byte[] data, ushort startIndex, QueryRealDataResponse realDataObject)
        {
            byte[] cumulantbyte = new byte[100];
            byte FzhType = 0;
            ItemState SensorTempState = ItemState.EquipmentCommOK;//设备状态
            float FloatValue = 0;
            byte dataTypeCount = 0;//回发信息域个数
            ushort dataLen = 0;//信息区域数据字节长度
            UInt32 deviceDefineFlag = 0;//设备定义标记
            ushort controlValue = 0;//表示上传的控制通道的断开，接通信息

            realDataObject.DeviceCommperType = 0x26;
            FzhType = data[startIndex + 4];
            dataTypeCount = data[startIndex + 5];
            startIndex += 6;
            for (int i = 0; i < dataTypeCount; i++)
            {
                dataLen = CommandUtil.ConvertByteToInt16(data, startIndex + 1, false);//得到本次信息区域的总长度
                if (startIndex + dataLen > data.Length)
                {
                    LogHelper.Error("数据处理异常（长度不足）：分站号=" + data[startIndex - 6] + ",数据处理类型=" + data[startIndex] + ",数据总长:" + data.Length + "=" + startIndex + dataLen);
                    continue;
                }
                switch (data[startIndex])
                {
                    case 1://分站基本信息
                        #region
                        
                        startIndex += 3;
                        if ((data[startIndex] & 0x01) == 0x01)
                            SensorTempState = ItemState.EquipmentDC;
                        else if ((data[startIndex] & 0x02) == 0x02)//红外遥控中                
                            SensorTempState = ItemState.EquipmentInfrareding;
                        else
                            SensorTempState = ItemState.EquipmentAC;//
                        if ((data[startIndex] & 0x04) == 0x04)
                        {
                            realDataObject.StationDyType = 1;
                        }
                        else
                            realDataObject.StationDyType = 0;
                        startIndex++;
                        FloatValue = data[startIndex] / 10.0f;
                        //19.2-25.2 小于19.2按照19.2处理，大于25.2按照25.2处理 通过电源电压来算电量 电压-19.2*100/(25.2-19.2)  这个算出来的值就是电量剩余百分比
                        if (FloatValue < 19.2)
                            FloatValue = 19.2f;
                        else if (FloatValue > 25.2)
                            FloatValue = 25.2f;
                        FloatValue = (FloatValue - 19.2f) * 100 / (25.2f - 19.2f);
                        AddRealData(realDataObject, "0", "0", 0, ItemDevProperty.Substation, "",
                               SensorTempState, FloatValue.ToString(), "", "", "");
                        startIndex++;
                        controlValue = CommandUtil.ConvertByteToInt16(data, startIndex, false);//得控制量的控制信息
                        startIndex += 2;
                        //新增加4个字节的日期
                        realDataObject.DeviceTime= CommandUtil.ConvertByteToDate(data, startIndex);
                        startIndex += 4;
                        #endregion
                        break;
                    case 2://实时监测数据
                    case 4://未定义设备信息
                        startIndex += 1;
                        deviceDefineFlag = CommandUtil.ConvertByteToInt16(data, startIndex, false);//得到长度
                        startIndex += 2;
                        deviceDefineFlag = (deviceDefineFlag - 3) / 7;//得到个数
                        if (deviceDefineFlag > 0)//有设备回传数据
                        {
                            startIndex = AnalogDerailControl(deviceDefineFlag, data, realDataObject, startIndex, controlValue);//模开智能断电器解析
                        }
                        break;

                    case 3://历史数据                     
                        startIndex += 1;
                        deviceDefineFlag = CommandUtil.ConvertByteToInt16(data, startIndex, false);//得到长度
                        startIndex += 2;
                        deviceDefineFlag = (deviceDefineFlag - 3) / 11;//得到个数
                        if (deviceDefineFlag > 0)
                        {
                            HistoryHandle(deviceDefineFlag, data, startIndex, realDataObject);
                            startIndex += (ushort)(deviceDefineFlag * 11);
                        }

                        break;
                    case 5://抽放数据
                        startIndex += 3;
                        for (int j = startIndex; j < startIndex + 84; j++)//68+16
                            cumulantbyte[j - startIndex] = data[j];
                        HandleCumulant(cumulantbyte, realDataObject);
                        startIndex += 84;
                        break;
                }
            }
            if (startIndex + 3 < data.Length)
            {
                realDataObject.StationCrc = CommandUtil.ConvertByteToInt16(data, startIndex, false);
            }
            HandleLocalControl((byte)controlValue, realDataObject);//本地触点控制解析,需要等馈电量解析完成后执行。

        }
        private void HistoryHandle(uint hiscount, byte[] bytes, ushort startIndex, QueryRealDataResponse RealDataObject)
        {//处理历史数据，startIndex为年的开始位置
            ushort UintValue = 0;//2个字节的值
            byte seniorType = 0;//表示的传感器型号
            int DecimalDigits = 0;//小数点位数
            decimal FloatCurValue = 0;//模拟量数值
            ItemState SensorTempState = ItemState.EquipmentCommOK;//设备状态
            byte checkPower = 0;// 无线传感器更换电池标记
            byte senorState = 0;//传感器状态字节
            byte seniorGrad = 0;//分级报警标记
            DateTime dtime = new DateTime();
            DeviceHistoryRealDataItem item = null;
            byte DevPropertyID = 0;
            for (int i = 0; i < hiscount; i++)
            {//传感器地址号+实时信息区域中的 “设备类型“-“数值”进行传输。
                item = new DeviceHistoryRealDataItem();
                dtime = CommandUtil.ConvertByteToDate(bytes, startIndex);
                if (dtime == new DateTime(1900, 01, 01))
                {
                    return;
                }
                startIndex += 4;
                item.Channel = bytes[startIndex++].ToString();
                item.HistoryDate = dtime;
                item.Address = (bytes[startIndex + 3] & 0x03).ToString();

                seniorType = bytes[startIndex++];//获取设备型号

                if ((seniorType >= 12 && seniorType <= 16) || (seniorType == 27) || (seniorType == 53) || (seniorType >= 100 && seniorType <= 150))
                    DevPropertyID = 2;
                else
                    DevPropertyID = 1;
                UintValue = bytes[startIndex++];//电压字节
                item.Voltage = Convert.ToString(UintValue / 10.0f);
                senorState = bytes[startIndex++];//设备状态
                if ((senorState & 0x80) == 0x80)
                {
                    checkPower = 1;//有无线传感器更换电池
                }
                SensorTempState = ItemState.EquipmentCommOK;//传感器状态
                if ((senorState & 0x20) == 0x20)
                {//----开机中----
                    SensorTempState = ItemState.EquipmentStart;
                }
                else if ((senorState & 0x40) == 0x40)
                {//红外遥控中-----
                    SensorTempState = ItemState.EquipmentInfrareding;
                }
                else if ((senorState & 0x08) == 0x08)
                {//断线
                    SensorTempState = ItemState.EquipmentDown;
                }
                else if ((senorState & 0x10) == 0x10)
                {//断线
                    SensorTempState = ItemState.EquipmentHeadDown;
                }
                else if ((senorState & 0x04) == 0x04)
                {//标效
                    SensorTempState = ItemState.EquipmentAdjusting;
                }
                DecimalDigits = senorState & 0x03;//数值放大倍数每2位表示一个参数
                seniorGrad = (byte)(bytes[startIndex++] >> 4);
                UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                startIndex += 2;
                switch (DevPropertyID)
                {
                    case 1:
                        FloatCurValue = (decimal)((UintValue & 0x7FFF) / Math.Pow(10, DecimalDigits));
                        if ((UintValue & 0x8000) == 0x8000)//负数
                            FloatCurValue = -FloatCurValue;
                        item.BranchNumber = 0;
                        item.ChangeSenior = checkPower;
                        item.DeviceProperty = ItemDevProperty.Analog;
                        item.DeviceTypeCode = seniorType;
                        item.FeedBackState = "0";
                        item.FeedState = "0";
                        item.RealData = FloatCurValue.ToString();
                        item.SeniorGradeAlarm = seniorGrad;
                        item.State = SensorTempState;
                        break;
                    case 2:
                        if (UintValue < 0 || UintValue > 2)
                        {
                            UintValue = 0;
                        }
                        if (SensorTempState == ItemState.EquipmentHeadDown ||
                            SensorTempState == ItemState.EquipmentDown ||
                            SensorTempState == ItemState.EquipmentBiterror)
                        {
                            UintValue = 0;
                        }
                        item.BranchNumber = 0;
                        item.ChangeSenior = checkPower;
                        item.DeviceProperty = ItemDevProperty.Derail;
                        item.DeviceTypeCode = seniorType;
                        item.FeedBackState = "0";
                        item.FeedState = "0";
                        item.RealData = UintValue.ToString();
                        item.SeniorGradeAlarm = 0;
                        item.State = SensorTempState;
                        break;
                    default:
                        break;
                }
                RealDataObject.HistoryRealDataItems.Add(item);
            }
        }
        private ushort DeviceBaseinfoHandle(uint deviceCount, byte[] bytes, ushort startIndex, QueryRealDataResponse RealDataObject)
        {//:处理终端信息
            byte dtype = 0, DecimalDigits;
            ushort UintValue;
            float FloatCurValue;
            DeviceInfoMation item = null;
            //3E-E3-90-09-C8-46-21-00-E9-04
            //-01-07-00-00-B4-00-00
            //-02-03-00-
            //03 -03-00-
            //04 -0A-00-02-01-B5-02-00-19-00
            //-00-00-C4-03
            for (int i = 0; i < deviceCount; i++)
            {
                item = new DeviceInfoMation();
                item.Address = "0";

                dtype = bytes[startIndex++];
                dtype >>= 7;
                switch (dtype)
                {
                    case 0://模拟量
                        item.DeviceProperty = ItemDevProperty.Analog;
                        #region
                        item.Channel = bytes[startIndex++].ToString();
                        item.DeviceTypeCode = bytes[startIndex++];
                        DecimalDigits = bytes[startIndex++];
                        UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        if (UintValue != 0xFFFF)
                        {
                            FloatCurValue = (float)(UintValue / Math.Pow(10, DecimalDigits));
                            item.UpAarmValue = FloatCurValue;
                        }
                        else
                            item.UpAarmValue = 0xFFFF;

                        UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        if (UintValue != 0xFFFF)
                        {
                            FloatCurValue = (float)(UintValue / Math.Pow(10, DecimalDigits));
                            item.UpDdValue = FloatCurValue;
                        }
                        else
                            item.UpDdValue = 0xFFFF;

                        UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        if (UintValue != 0xFFFF)
                        {
                            FloatCurValue = (float)(UintValue / Math.Pow(10, DecimalDigits));
                            item.UpHfValue = FloatCurValue;
                        }
                        else
                            item.UpHfValue = 0xFFFF;

                        UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        if (UintValue != 0xFFFF)
                        {
                            FloatCurValue = (float)(UintValue / Math.Pow(10, DecimalDigits));
                            item.DownAarmValue = FloatCurValue;
                        }
                        else
                            item.DownAarmValue = 0xFFFF;

                        UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        if (UintValue != 0xFFFF)
                        {
                            FloatCurValue = (float)(UintValue / Math.Pow(10, DecimalDigits));
                            item.DownDdValue = FloatCurValue;
                        }
                        else
                            item.DownDdValue = 0xFFFF;

                        UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        if (UintValue != 0xFFFF)
                        {
                            FloatCurValue = (float)(UintValue / Math.Pow(10, DecimalDigits));
                            item.DownHfValue = FloatCurValue;
                        }
                        else
                            item.DownHfValue = 0xFFFF;
                        item.LC1 = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        item.LC2 = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;

                        UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        FloatCurValue = (float)(UintValue / Math.Pow(10, DecimalDigits));
                        item.SeniorGradeAlarmValue1 = FloatCurValue;

                        UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        FloatCurValue = (float)(UintValue / Math.Pow(10, DecimalDigits));
                        item.SeniorGradeAlarmValue2 = FloatCurValue;

                        UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        FloatCurValue = (float)(UintValue / Math.Pow(10, DecimalDigits));
                        item.SeniorGradeAlarmValue3 = FloatCurValue;

                        UintValue = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        FloatCurValue = (float)(UintValue / Math.Pow(10, DecimalDigits));
                        item.SeniorGradeAlarmValue4 = FloatCurValue;

                        item.SeniorGradeTimeValue1 = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;

                        item.SeniorGradeTimeValue2 = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;

                        item.SeniorGradeTimeValue3 = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;

                        item.SeniorGradeTimeValue4 = CommandUtil.ConvertByteToInt16(bytes, startIndex, false);
                        startIndex += 2;
                        #endregion
                        break;
                    case 1://1表示开关量
                        item.DeviceProperty = ItemDevProperty.Derail;
                        #region
                        item.Channel = bytes[startIndex++].ToString();
                        item.DeviceTypeCode = bytes[startIndex++];
                        #endregion
                        break;
                }
                RealDataObject.DeviceInfoItems.Add(item);
            }
            return startIndex;
        }
        private void HandleCumulant(byte[] bytes, QueryRealDataResponse RealDataObject)
        {
            DateTime uploadTime = DateTime.Now;//协议上传抽放时间
            int i; //下标
            string Strs = "";//格式化字符串
            float accumulatValue;
            try
            {
                if ((bytes[1] > 12) || (bytes[1] < 1) || (bytes[2] > 31) || (bytes[2] < 1) || (bytes[3] > 23)) return;
                if (bytes[0] == 0)
                {
                    LogHelper.Info(string.Format("【" + def.Point + "】抽采上传日期不对，{0}-{1}-{2}-{3}", bytes[0], bytes[1], bytes[2], bytes[3]));
                    bytes[0] = (Byte)(DateTime.Now.Year - 2000);
                    bytes[1] = (Byte)(DateTime.Now.Month);
                    bytes[2] = (Byte)(DateTime.Now.Day);
                }
                uploadTime = DateTime.Parse(string.Format("{0:0000}-{1:00}-{2:00} {3:00}:00:00", bytes[0] + 2000, bytes[1], bytes[2], bytes[3]));//
                RealDataObject.CumulantTime = uploadTime;//抽采的上传时间
                //先处理4个分钟的瞬间流量值
                for (i = 0; i < 4; i++)
                {
                    accumulatValue = BitConverter.ToSingle(bytes, 4 + i * 4);
                    accumulatValue = (float)Math.Round(accumulatValue, 2);//强制性处理为2位小数

                    AddRealData(RealDataObject, (40 + i).ToString(), "0", 0, ItemDevProperty.Analog, accumulatValue.ToString("F2"),
                      ItemState.EquipmentCommOK, "0", "", "", "");
                }
                for (i = 0; i < 16; i++) //13号口 标况混合量 14号口 标况纯流量 15号口 工况混合量 16号口 工况纯流量
                {
                    accumulatValue = BitConverter.ToSingle(bytes, 20 + i * 4);
                    accumulatValue = (float)Math.Round(accumulatValue, 2);//强制性处理为2位小数
                    if (i < 8) Strs = "F2";
                    else Strs = "F1";
                    AddRealData(RealDataObject, Convert.ToString(i + 1), "0", 0, ItemDevProperty.Accumulation, accumulatValue.ToString(Strs), ItemState.EquipmentCommOK, "", "", "", "");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("HandleCumulant:" + ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceDefineFlag"></param>
        /// <param name="data"></param>
        /// <param name="RealDataObject"></param>
        /// <param name="StartIndex">表示设备回发数据的第一个字节</param>
        /// <param name="controlValue">表示上传控制设备的状态</param>
        /// <returns></returns>
        private ushort AnalogDerailControl(uint DeviceDefineFlag, byte[] data, QueryRealDataResponse RealDataObject, ushort StartIndex, ushort controlValue)
        {
            byte tdValue = 0, kh;//表示单一设备的通道号
            int index = 0;
            ushort UintValue = 0;//2个字节的值
            List<DeviceInfo> items = null;
            DeviceInfo nProl = null;//表示没有找到定义时的默认值
            byte seniorType = 0;//表示的传感器型号
            int DecimalDigits = 0;//小数点位数
            decimal FloatCurValue = 0;//模拟量数值
            ItemState SensorTempState = ItemState.EquipmentCommOK;//设备状态
            byte checkPower = 0;// 无线传感器更换电池标记
            byte senorState = 0;//传感器状态字节
            byte seniorGrad = 0;//分级报警标记
            DeviceTypeInfo dev;
            decimal LC2;
            for (index = 0; index < DeviceDefineFlag; index++)
            {
                tdValue = (byte)(data[StartIndex + 4] & 0x07); ;
                kh = data[StartIndex++];
                items = Cache.CacheManager.Query<DeviceInfo>(p => p.Fzh == def.Fzh && p.Kh == kh && tdValue == p.Dzh && ((p.DevPropertyID < 3) || (p.DevPropertyID == 3 && p.Dzh != 0)), true);
                if (items == null || items.Count == 0)//定义已经删除，移动下标位置
                {//每个参数占固定的7个字节，因此要循环判断移徐,设备固定头占4个字节
                    items = new List<DeviceInfo>();
                    nProl = new DeviceInfo();
                    items.Add(nProl);
                    items[0].Kh = kh;
                    items[0].Dzh = tdValue;
                }
                else
                    nProl = null;

                seniorType = data[StartIndex++];//获取设备型号
                if (nProl != null)
                {
                    if ((seniorType >= 12 && seniorType <= 16) || (seniorType == 27) || (seniorType == 53) || (seniorType >= 100 && seniorType <= 150))
                        items[0].DevPropertyID = 2;
                    else
                        items[0].DevPropertyID = 1;
                }
                UintValue = data[StartIndex++];//电压字节
                items[0].Voltage = UintValue / 10.0f;
                senorState = data[StartIndex++];//设备状态
                if ((senorState & 0x80) == 0x80)
                {
                    checkPower = 1;//有无线传感器更换电池
                }
                else
                {
                    checkPower = 0;//有无线传感器更换电池
                }
                SensorTempState = ItemState.EquipmentCommOK;//传感器状态
                if ((senorState & 0x20) == 0x20)
                {//----开机中----
                    SensorTempState = ItemState.EquipmentStart;
                }
                else if ((senorState & 0x08) == 0x08)
                {//断线
                    SensorTempState = ItemState.EquipmentDown;
                }
                else if ((senorState & 0x10) == 0x10)
                {//头子断线
                    SensorTempState = ItemState.EquipmentHeadDown;
                }
                else if ((senorState & 0x04) == 0x04)
                {//标效
                    SensorTempState = ItemState.EquipmentAdjusting;
                }
                else if ((senorState & 0x40) == 0x40)
                {//红外遥控中-----
                    SensorTempState = ItemState.EquipmentInfrareding;
                }
                DecimalDigits = senorState & 0x03;//数值放大倍数每2位表示一个参数
                seniorGrad = (byte)(data[StartIndex++] >> 4);
                if ((seniorGrad & 0x08) == 0x08) seniorGrad = 4;
                else if ((seniorGrad & 0x04) == 0x04) seniorGrad = 3;
                else if ((seniorGrad & 0x02) == 0x02) seniorGrad = 2;
                else if ((seniorGrad & 0x01) == 0x01) seniorGrad = 1;

                UintValue = CommandUtil.ConvertByteToInt16(data, StartIndex, false);
                StartIndex += 2;
                switch (items[0].DevPropertyID)
                {
                    case 1:
                        if (UintValue == 0xFFFF)
                        {//溢出
                            FloatCurValue = 0;
                            SensorTempState = ItemState.EquipmentOverrange;
                        }
                        else
                        {
                            FloatCurValue = (decimal)((UintValue & 0x7FFF) / Math.Pow(10, DecimalDigits));
                            if ((UintValue & 0x8000) == 0x8000)//负数
                                FloatCurValue = -FloatCurValue;                            
                            dev = Cache.CacheManager.QueryFrist<DeviceTypeInfo>(p => p.Devid == items[0].Devid, true);
                            if(dev!=null)
                            {
                                if (FloatCurValue > dev.LC)
                                {//正数
                                    FloatCurValue = dev.LC;
                                }
                                else
                                {//下限
                                    if (string.IsNullOrEmpty(dev.Bz11))
                                    {
                                        LC2 = 0;//默认为0
                                    }
                                    else
                                    {
                                        LC2 = Convert.ToDecimal(dev.Bz11);
                                    }
                                    if (FloatCurValue < LC2)
                                        FloatCurValue = LC2;
                                }
                            }
                        }
                        AddRealData(RealDataObject, items[0].Kh.ToString(), items[0].Dzh.ToString(), seniorType, ItemDevProperty.Analog, FloatCurValue.ToString("f" + DecimalDigits),
                        SensorTempState, items[0].Voltage.ToString(), "", "", "", checkPower, seniorGrad);
                        break;
                    case 2:
                        GetDerailFromCapacity(UintValue, SensorTempState, seniorType, items[0], RealDataObject);
                        break;
                    case 3:
                        if (items[0].Devid == "62") //只处理智能断电器0xB2
                        {//用前面上传的控制口状态，更新智能断电器输出的状态信息。
                            UintValue |= (byte)((controlValue >> index) & 0x01);//第0位表示接通断开，直接以前面传输的控制字节为准。
                        }
                        GetZNControlCapacity(UintValue, SensorTempState, seniorType, items, RealDataObject);
                        break;
                    default:
                        break;
                }
            }
            return StartIndex;
        }
    }
}
