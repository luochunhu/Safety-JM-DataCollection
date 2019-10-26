using Basic.Framework.Logging;
using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    /// <summary>
    /// 执行中心站设备初始化处理
    /// </summary>
    public class InitializeRequestCommand : InitializeRequest
    {
        /// <summary>
        /// 表示所有的设备类型集合。
        /// </summary>
        public List<DeviceTypeInfo> DeviceTypeListItem = null;
        /// <summary>        
        /// 当前分站及下属设备对象
        /// </summary>
        public List<DeviceInfo> DeviceListItem = null;
        /// <summary>
        /// 传入的分站设备定义信息
        /// </summary>
        public DeviceInfo def = null;
        /// <summary>
        /// 命令下发的字节数
        /// </summary>
        private int BufferLength = 12;
        /// <summary>
        /// 表示命令版本，即=13表示kj306-f(16)h智能分站；=1表示大分站；=14表示抽放分站；
        /// </summary>
        public byte OrderVersion;
        ///// <summary>
        ///// 表示有多少个设备类型需要下发
        ///// </summary>
        //private int DeviceTypeCount = 0;
        ///// <summary>
        ///// 定义的传感器类型
        ///// </summary>
        //private int[] DeviceTypeList = new int[16];
        /// <summary>
        /// m_devDefineMark[0] 模拟量定义标志 1 表示有定义 0 表示无定义
        /// m_devDefineMark[1] 开关量定义标志
        /// m_devDefineMark[2] 控制量定义标志
        /// m_devDefineMark[3] 智能开停定义标志
        /// m_devDefineMark[4] 多参数定义标志 
        /// m_devDefineMark[5] 智能断电器定义标志
        /// </summary>
        private ushort[] NdevDefineMark = new ushort[6];
        public byte[] ToBytes()
        {
          
            byte[] bytes = null;
            try
            {
                bytes = GetInitializeCapacity();
                CommandUtil.AddSumToBytes(bytes, 4, bytes.Length);//累加和
            }
            catch(Exception ex)
            {
                Basic.Framework.Logging.LogHelper.Error(ex.Message+ex.StackTrace);
            }
            return bytes;
        }
        /// <summary>
        /// 智能分站初始化主函数
        /// </summary>
        /// <returns></returns>
        private byte[] GetInitializeCapacity()
        {
            byte[] bytes = null;
            int Index = 0;
            GetDeviceDefineMark(def.Fzh);//得到设备的定义标记
            BufferLength = GetInitCommandLengthByIntelligence();//得到长度
            bytes = new byte[BufferLength];
            GetInitializeBufferHead(bytes);//得到包头
            CommandUtil.ConvertInt32ToByte(GetModelSwitchCtrolDefine(), bytes, 9, false);//模开控定义标记：9，10,11,12
            Index = GetAnalogDerailCapacity(bytes, 13);//得到模拟量和开关量的下发初始化信息。       
            if (OrderVersion == 14)
            {
                Index = SetChouFDefine(bytes, Index);
                //传输累计量偏差值 
                Index = SetLjlValue(bytes, Index);
            }
            if (Index + 2 != bytes.Length)
            {
                LogHelper.Info(string.Format("-GetInitializeCapacity【字节填充不对{0}-{1}】", Index + 2, bytes.Length));
            }
            return bytes;
        }
        private int SetLjlValue(byte[] bytes, int index)
        {
            ushort shrtvalue = 0xFFFF;
            byte[] fbyte;
            float flt;
            CommandUtil.ConvertInt16ToByte(shrtvalue, bytes, index, false);
            index += 2;
            string[] str;
            if (def.Bz6 != null && def.Bz6 != "")
            {
                str = def.Bz6.Split('|');
                for (int i = 0; i < str.Length; i++)
                {
                    flt = Convert.ToSingle(str[i]);
                    fbyte = BitConverter.GetBytes(flt);
                    Buffer.BlockCopy(bytes, index, fbyte, 0, 4);
                    index += 4;
                    if (i >= 15) break;
                }
            }
            return index;
        }
        private int SetChouFDefine(byte[] bytes, int index)
        {
            List<DeviceInfo> items;//测点设备集合
            ushort ljlDefine = 0;
            ushort analogderaildefine = CommandUtil.ConvertByteToInt16(bytes, 9, false);//定义的模开控
            for (int i = 0; i < 16; i++)
            {
                if ((analogderaildefine & (1 << i)) == (1 << i))//此设备有定义
                {
                    items = DeviceListItem.FindAll(delegate(DeviceInfo p) { return p.Fzh == def.Fzh && p.Kh == i + 1 && ((p.DevPropertyID == 1 || p.DevPropertyID == 2) || (p.DevPropertyID == 3 && p.Dzh != 0)); });
                    if (items == null || items.Count == 0 || items.Count > 1)
                    {//表示前面有这个测点现在没有这个测点了，此时，把对应的定义标记设置为0
                        continue;
                    }
                    if (items[0].Bz20 == "1")
                    {
                        ljlDefine += (ushort)(1 << i);
                    }
                }
            }
            CommandUtil.ConvertInt16ToByte(ljlDefine, bytes, index, false);

            index += 2;
            return index;
        }
        private byte GetControlByteCapacity(ushort controlDefine, ushort controlValue)
        {
            byte tempcur = 0;
            byte index = 0;
            bool Flag = false;
            for (int i = 0; i < 16; i++)
            {
                if ((controlValue & (1 << i)) == (1 << i))//此设备关联了此地址号的控制；
                {
                    index = 0;
                    Flag = false;
                    for (int j = 0; j < 16; j++)
                    {
                        if ((controlDefine & (1 << j)) == (1 << j))//此设备关联了此地址号的控制；
                        {
                            if (j == i)
                            {
                                Flag = true;
                                break;
                            }
                            index++;
                        }
                    }
                    if (Flag)
                    {
                        tempcur += (byte)(1 << index);
                    }
                    else
                    {
                        LogHelper.Info("定义的控制口和下发的控制口不同步。分站号：" + def.Point + "【" + controlDefine + "," + controlValue + "】");
                    }
                }
            }
            return tempcur;
        }
        /// <summary>
        /// 智能开停的初始化下发
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        //private int GetZNDerailControlCapacity(byte[] bytes, int index)
        //{
        //    ushort controldefine =0;
        //    ushort znDefine = 0;
        //    controldefine = CommandUtil.ConvertByteToInt16(bytes, 11);//定义的控制口
        //    znDefine = bytes[13];
        //    List<DeviceInfo> items;//测点设备集合
        //    for (int i = 0; i < 8; i++)
        //    {
        //        if ((znDefine & (1 << i)) == (1 << i))//定义的智能开停
        //        {
        //            items = DeviceListItem.FindAll(delegate(DeviceInfo p) { return p.Fzh == def.Fzh && p.Kh == i + 17 && p.DevPropertyID == 2; });
        //            if (items == null || items.Count == 0)
        //            {
        //                bytes[index++] = 0;//0态
        //                bytes[index++] = 0;//1态
        //                bytes[index++] = 0;//2态
        //                continue;
        //            }
        //            bytes[index++] = GetControlByteCapacity(controldefine, (ushort)((items[0].K1 & 0xFF) | (items[0].K1 >> 8)));//0态
        //            bytes[index++] = GetControlByteCapacity(controldefine, (ushort)((items[0].K2 & 0xFF) | (items[0].K2 >> 8)));//1态
        //            bytes[index++] = GetControlByteCapacity(controldefine, (ushort)((items[0].K3 & 0xFF) | (items[0].K3 >> 8)));//2态
        //        }
        //    }
        //    return index;
        //}
        /// <summary>
        /// 得到智能分站模拟量和开关量的下发初始化信息。
        /// </summary>
        /// <param name="bytes">下发的数组</param>
        /// <param name="index">填充的开始位置</param>
        /// <returns></returns>
        private int GetAnalogDerailCapacity(byte[] bytes, int index)
        {
            List<DeviceInfo> items;//测点设备集合
            //ushort allcontroldefine = CommandUtil.ConvertByteToInt16(bytes, 11);//定义的控制口
            uint analogderaildefine = CommandUtil.ConvertByteToInt(bytes, 9, false);//定义的模开控
            for (int i = 0; i < 32; i++)
            {
                if ((analogderaildefine & (1 << i)) == (1 << i))//此设备有定义
                {
                    items = DeviceListItem.FindAll(delegate(DeviceInfo p) { return p.Fzh == def.Fzh && p.Kh == i + 1 && ((p.DevPropertyID == 1 || p.DevPropertyID == 2) || (p.DevPropertyID == 3 && p.Dzh != 0)); });
                    if (items == null || items.Count == 0)
                    {//表示前面有这个测点现在没有这个测点了，此时，把对应的定义标记设置为0
                        if (i > 23)
                            bytes[12] = (byte)(bytes[12] & (~(1 << (i - 24))));
                        else if (i > 15)
                            bytes[11] = (byte)(bytes[11] & (~(1 << (i - 16))));
                        else if (i > 7)//高在前
                            bytes[10] = (byte)(bytes[10] & (~(1 << (i - 8))));
                        else
                            bytes[9] = (byte)(bytes[9] & (~(1 << i)));
                        continue;
                    }
                    items = items.OrderBy(temp => temp.Dzh).ToList();//按照通道号排序，默认第一个参数上的型号为此设备的型号，多参时。                   
                    for (int j = 0; j < items.Count; j++)//处理多参数传感器
                    {
                        switch (items[j].DevPropertyID)
                        {
                            case 1://模拟量
                                bytes[index++] = (byte)items[j].Kh;//地址号   
                                bytes[index++] = (byte)items[j].DevModelID;//设备大类   
                                if (items.Count == 1) bytes[index] = 0;//标记字节
                                else
                                    bytes[index] = (byte)(items[j].Dzh);//后续还有参数
                                index++;
                                index = GetAnalogCapacity(bytes, index, items[j]);
                                break;
                            case 2://开关量
                                bytes[index++] = (byte)items[j].Kh;//地址号   
                                bytes[index++] = (byte)items[j].DevModelID;//设备大类  
                                if (items.Count == 1) bytes[index++] = 0;//标记字节
                                else
                                    bytes[index++] = (byte)(j + 1);//后续还有参数
                                CommandUtil.ConvertInt16ToByte((ushort)items[j].K1, bytes, index, false);//
                                index += 2;
                                CommandUtil.ConvertInt16ToByte((ushort)items[j].K2, bytes, index, false);//
                                index += 2;
                                CommandUtil.ConvertInt16ToByte((ushort)items[j].K3, bytes, index, false);//
                                index += 2;
                                break;
                            case 3://智能断电器
                                bytes[index++] = (byte)items[j].Kh;//地址号   
                                bytes[index++] = (byte)items[j].DevModelID;//设备大类 
                                break;
                        }
                        if (j == 0 && items[0].DevPropertyID == 3)//表示为智能断电器，只有1个字节强制性退出。
                            break;
                    }
                }
            }
            return index;
        }
        /// <summary>
        /// 得到智能分站模拟量的初始化下发数据信息
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <param name="tempDef"></param>
        /// <returns></returns>
        private int GetAnalogCapacity(byte[] bytes, int index, DeviceInfo tempDef)
        {
            ushort controldefine = CommandUtil.ConvertByteToInt16(bytes, 11);//定义的控制口
            byte decimalCount = 0;//小数位数
            ushort sensorlc = 0xFFFF;//量程
            ushort tempValue = 0;
            float[] tempalarm = new float[14];
            string strCur = "";
            string[] spltstr;
            float tempstrCur = 0;

            GetSignCapacity(bytes, index, tempDef, ref decimalCount, ref sensorlc);//获取标记            
            tempalarm[0] = tempDef.Z2;
            tempalarm[1] = tempDef.Z3;
            tempalarm[2] = tempDef.Z4;
            tempalarm[3] = tempDef.Z6;
            tempalarm[4] = tempDef.Z7;
            tempalarm[5] = tempDef.Z8;
            if (tempalarm[0] == 0 && tempalarm[1] == 0 && tempalarm[2] == 0)
            {
                tempalarm[0] = 65535;
                tempalarm[1] = 65535;
                tempalarm[2] = 65535;
            }
            if (tempalarm[3] == 0 && tempalarm[4] == 0 && tempalarm[5] == 0)
            {
                tempalarm[3] = 65535;
                tempalarm[4] = 65535;
                tempalarm[5] = 65535;
            }
            strCur = tempDef.Bz8;//4级报警值,3级报警值,2级报警值,1级报警值
            if (strCur == null)
            {
                strCur = "65535,65535,65535,65535";
            }
            spltstr = strCur.Split(',');
            if (spltstr.Length < 4)
            {
                spltstr = new string[4] { "65535", "65535", "65535", "65535" };
            }
            for (int i = 0; i < 4; i++)
            {
                float.TryParse(spltstr[i].ToString(), out tempstrCur);
                try
                {
                    tempalarm[6 + i] = tempstrCur;
                }
                catch
                {
                }
            }
            strCur = tempDef.Bz9;//1级报警时间,2级报警时间,3级报警时间,4级报警时间（单位：分钟）
            if (strCur == null || strCur=="0")
            {
                strCur = "255,255,255,255";
            }
            spltstr = strCur.Split(',');
            if (spltstr.Length < 4)
            {
                spltstr = new string[4] { "255", "255", "255", "255" };
            }
            for (int i = 0; i < 4; i++)
            {
                float.TryParse(spltstr[i].ToString(), out tempstrCur);
                try
                {
                    tempalarm[10 + i] = tempstrCur;
                }
                catch
                {

                }
            }
            for (int i = 0; i < 10; i++)
            {
                if (tempalarm[i] >65534.8)
                {
                    bytes[index]= 0xFF;
                    bytes[index+1] = 0xFF;
                }
                else
                {
                    tempValue = Convert.ToUInt16(Math.Abs(tempalarm[i]) * Math.Pow(10, decimalCount));
                    CommandUtil.ConvertInt16ToByte(tempValue, bytes, index, false);//
                    if (tempalarm[i] < 0)//Bit15表示正负，=0表示正，=1表示负
                        bytes[index + 1] |= 0x80;
                    else bytes[index + 1] &= 0x7F;
                }
                index += 2;
            }
            for (int i = 10; i < 14; i++)
            {
                if (tempalarm[i] > 254.8)
                {
                    bytes[index++] = 0xFF;
                }
                else
                {
                    tempValue = Convert.ToUInt16(Math.Abs(tempalarm[i]));
                    bytes[index++] = Convert.ToByte(tempValue);
                }
            }
            CommandUtil.ConvertInt16ToByte((ushort)tempDef.K1, bytes, index, false);//
            index += 2;
            CommandUtil.ConvertInt16ToByte((ushort)tempDef.K2, bytes, index, false);//
            index += 2;
            CommandUtil.ConvertInt16ToByte((ushort)tempDef.K3, bytes, index, false);//
            index += 2;
            CommandUtil.ConvertInt16ToByte((ushort)tempDef.K4, bytes, index, false);//
            index += 2;
            CommandUtil.ConvertInt16ToByte((ushort)tempDef.K7, bytes, index, false);//
            index += 2;
            index += 4;

            return index;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <param name="tempDef"></param>
        /// <param name="decimalCount"></param>
        /// <param name="sensorlc"></param>
        private void GetSignCapacity(byte[] bytes, int index, DeviceInfo tempDef, ref byte decimalCount, ref ushort sensorlc)
        {
            decimalCount = 0;//小数位数需要返回
            sensorlc = 0xFFFF;//量程需要返回
            DeviceTypeInfo dev = null;
            short LC1;
            float LC2;
            ushort tempValue;// = Convert.ToUInt16(Math.Abs(tempalarm[i]) * Math.Pow(10, decimalCount));

            sensorlc = 0;
            dev = DeviceTypeListItem.Find(delegate(DeviceTypeInfo p) { return p.Devid == tempDef.Devid; });
            if (dev != null)
            {
                LC1 = dev.LC;//高值
                if (string.IsNullOrEmpty(dev.Bz11))
                {
                    LC2 = 0;//默认为0
                }
                else
                {
                    LC2 = Convert.ToSingle(dev.Bz11);
                }
                decimalCount = Convert.ToByte(dev.Bz1);
            }
            else
            {
                decimalCount = 0;
                LC1 = 0;
                LC2 = 0;
                sensorlc = 0;
                LogHelper.Error("测点号：" + tempDef.Point + ",未找到设备类型" + tempDef.Devid);
            }
            bytes[index - 1] |= (byte)((decimalCount & 0x03) << 4);//放到大类的高两位
            tempValue = Convert.ToUInt16(Math.Abs(LC1) * Math.Pow(10, decimalCount));
            bytes[index + 34] = (byte)(tempValue & 0xFF);//量程高值
            bytes[index + 35] = (byte)((tempValue >> 8) & 0xFF);//量程高值
            if (LC1 < 0)
                bytes[index + 35] |= 0x80;//量程高值
            else
                bytes[index + 35] &= 0x7F;//量程高值
            tempValue = Convert.ToUInt16(Math.Abs(LC2) * Math.Pow(10, decimalCount));

            bytes[index + 36] |= (byte)(tempValue & 0xFF);//量程低值
            bytes[index + 37] |= (byte)((tempValue >> 8) & 0xFF);//量程低值
            if (LC2 < 0)
                bytes[index + 37] |= 0x80;//量程高值
            else
                bytes[index + 37] &= 0x7F;//量程高值
        }

        /// <summary>
        /// 获取设备设置的参数的小数位数
        /// </summary>
        /// <param name="tempDef"></param>
        /// <returns></returns>
        private byte GetFloatDecimalCountCapacity(DeviceInfo tempDef)
        {
            string strCur = "";
            string[] spltstr;
            byte Count = 0, tempcount = 0;//判断 上限的报警，上限断电，上限复电等状态的小数位数
            float[] tempcur = new float[10];
            /*上限预警值/上限报警值/上限断电值/上限恢复值/下限预警值/下限报警值/下限断电值/下限恢复值*/
            tempcur[0] = tempDef.Z2;
            tempcur[1] = tempDef.Z3;
            tempcur[2] = tempDef.Z4;
            tempcur[3] = tempDef.Z6;
            tempcur[4] = tempDef.Z7;
            tempcur[5] = tempDef.Z8;
            strCur = tempDef.Bz8;//4级报警值,3级报警值,2级报警值,1级报警值
            if (strCur == null)
            {
                strCur = "0,0,0,0";
            }
            spltstr = strCur.Split(',');
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    tempcur[6 + i] = Convert.ToSingle(strCur[i]);
                }
                catch
                {

                }
            }
            for (int i = 0; i < 10; i++)
            {
                if (tempcur[i].ToString().IndexOf(".") > 0)
                {
                    tempcount = (byte)tempcur[i].ToString().Substring(
                                    tempcur[i].ToString().IndexOf(".") + 1).Length;
                    if (tempcount > Count) Count = tempcount;
                }
            }
            if (Count > 3) Count = 3;//最多支持3位小数
            return Count;
        }

        private void GetInitializeBufferHead(byte[] bytes)
        {
            bytes[0] = 0x3E;
            bytes[1] = 0xE3;
            bytes[2] = 0x80;
            bytes[3] = 0x08;
            bytes[4] = (byte)def.Fzh;
            bytes[5] = 0x49;
            CommandUtil.ConvertInt16ToByte((ushort)(BufferLength - 4), bytes, 6, false);//长度,6,7
            Buffer.BlockCopy(BitConverter.GetBytes(GetSignByte()), 0, bytes, 8, 1);//标志 8
        }
        /// <summary>
        /// 设置32个字节的风电闭锁字节
        /// </summary>
        /// <param name="bytes">下发的数组</param>
        /// <param name="Index">开始填充的索引位置</param>
        /// <returns>返回当前已处理到的下一个位置点</returns>
        private int GetFDBSControlByte(byte[] bytes, int Index)
        {
            try
            {
                ushort allcontroldefine = CommandUtil.ConvertByteToInt16(bytes, 11);//定义的控制口                
                ushort tempvalue = 0;
                string[] ControlBytes = null;
                byte[] tempbuffer = new byte[2];
                if (((def.Bz3 & 0x1) == 0x1) ||
                    ((def.Bz3 & 0x2) == 0x2))//与前面的定义标记同步，避免出问题。生效为有逻辑控制或者风电控制  //if ((def.Bz3 & 0x1) == 0x1)
                {
                    if (!string.IsNullOrEmpty(def.Bz11))
                    {
                        ControlBytes = def.Bz11.Split(',');
                        if (ControlBytes.Length == 36)
                        {//新风电闭锁36个字节，要转换成34个字节 bytes[0],bytes[1]需要进行处理成一个字节。bytes[33][34]需要处理成一个字节
                            tempbuffer[0] = Convert.ToByte(ControlBytes[0]);
                            tempbuffer[1] = Convert.ToByte(ControlBytes[1]);
                            tempvalue = CommandUtil.ConvertByteToInt16(tempbuffer, 0);
                            bytes[Index++] = GetControlByteCapacity(allcontroldefine, tempvalue);//甲烷电闭锁控制口
                            for (int i = 2; i < ControlBytes.Length; i++)
                            {
                                if (i == 33)
                                {
                                    tempbuffer[0] = Convert.ToByte(ControlBytes[33]);
                                    tempbuffer[1] = Convert.ToByte(ControlBytes[34]);
                                    tempvalue = CommandUtil.ConvertByteToInt16(tempbuffer, 0);
                                    bytes[Index++] = GetControlByteCapacity(allcontroldefine, tempvalue);//甲烷电闭锁控制口
                                    i++;
                                }
                                else
                                {
                                    bytes[Index++] = Convert.ToByte(ControlBytes[i]);
                                }
                            }
                        }
                        else
                        {
                            Index += 34;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info(ex.Message + "-KJ73NDriver-风电闭锁判断");
            }
            return Index;
        }
        /// <summary>
        /// 得到模开智能断电器的定义标记 
        /// </summary>
        /// <returns></returns>
        private uint GetModelSwitchCtrolDefine()
        {
            ushort CurShort = 0;
            CurShort += NdevDefineMark[0];//模拟量定义标记
            CurShort += NdevDefineMark[1];//开关量定义标记
            CurShort += NdevDefineMark[5];//智能断电器定义标记
            return CurShort;
        }
        /// <summary>
        /// 得到下发的标志控制字节
        /// </summary>      
        /// <returns></returns>
        private byte GetSignByte()
        {
            byte Sign = 0;

            Sign = (byte)(LastAcceptFlag & 0x01);//接收标记    
            if ((def.Bz3 & 0x4) == 0x4)
            {
                Sign += 0x02; //故障闭锁
            }
            if ((def.Bz3 & 0x1) == 0x1)
            {
                Sign += 0x04; //风电闭锁
            }
            if (OrderVersion == 14)
            {
                Sign += (byte)(1 << 3);
                if (def.Bz6 != null && def.Bz6 != "")
                {
                    Sign += (byte)(1 << 4);
                }
            }

            return Sign;
        }
        /// <summary>
        /// 获取设备的定义标记
        /// </summary>
        /// <param name="fzh"></param>
        private void GetDeviceDefineMark(short fzh)
        {
            List<DeviceInfo> items;//测点设备集合
            int DoubleDeviceCount = 0;
            try
            {
                for (int i = 0; i < NdevDefineMark.Length; i++)
                    NdevDefineMark[i] = 0;
                for (int i = 0; i < 16; i++)
                {
                    items = DeviceListItem.FindAll(delegate(DeviceInfo p) { return p.Fzh == def.Fzh && p.Kh == i + 1 && ((p.DevPropertyID == 1 || p.DevPropertyID == 2) || (p.DevPropertyID == 3 && p.Dzh != 0)); });
                    if (items != null)
                    {
                        if (items.Count > 0)
                        {
                            items.OrderBy(temp => temp.Dzh);
                            if (items[0].DevPropertyID == 1)
                            {
                                NdevDefineMark[0] += (ushort)(1 << i); //模拟量
                            }
                            else if (items[0].DevPropertyID == 2)
                            {
                                NdevDefineMark[1] += (ushort)(1 << i);//开关量
                            }
                            else if (items[0].DevPropertyID == 3)
                            {
                                if (i > 7)//屏蔽本地的触点控制
                                {
                                    NdevDefineMark[5] += (ushort)(1 << i);//智能断电器
                                }
                            }
                            if (items[0].DevPropertyID != 3)
                            {
                                if (items.Count > 1)
                                {
                                    if (DoubleDeviceCount < 4)
                                    {
                                        NdevDefineMark[4] += (ushort)(1 << i); //多参数
                                        DoubleDeviceCount++;
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 16; i < 24; i++)
                {
                    items = DeviceListItem.FindAll(delegate(DeviceInfo p) { return p.Fzh == def.Fzh && p.Kh == i + 1 && (p.DevPropertyID == 2); });
                    if (items != null && items.Count > 0)
                    {
                        NdevDefineMark[3] += (ushort)(1 << (i - 16));//智能开停
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    items = DeviceListItem.FindAll(delegate(DeviceInfo p) { return p.Fzh == def.Fzh && p.Kh == i + 1 && p.DevPropertyID == 3 && p.Dzh == 0; });
                    if (items != null && items.Count > 0)
                    {
                        NdevDefineMark[2] += (ushort)(1 << i);//触点控制
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetNdevDefineMark:" + ex);
            }
        }
        private int GetInitCommandLengthByIntelligence()
        {
            List<DeviceInfo> items;//测点设备集合
            int commandLength = 15;//16个包头和累加和
            int tempCur = 0;
            int index = 0;
            uint tempDefine = 0;
            if (NdevDefineMark[3] > 0)//表示有智能开停，每个智能开停3个字节；
            {
                for (index = 0; index < 8; index++)
                {
                    if ((NdevDefineMark[3] & (1 << index)) == (1 << index))
                        tempCur++;
                }
            }
            commandLength += tempCur * 8;
            tempDefine = GetModelSwitchCtrolDefine();//模开智能断电器定义标记
            for (index = 0; index < 16; index++)
            {
                if ((tempDefine & (1 << index)) == (1 << index))
                {//此位有定义
                    items = DeviceListItem.FindAll(delegate(DeviceInfo p) { return p.Fzh == def.Fzh && p.Kh == index + 1 && (p.DevPropertyID == 1 || p.DevPropertyID == 2 || (p.DevPropertyID == 3 && p.Dzh != 0)); });
                    items = items.OrderBy(temp => temp.Dzh).ToList();
                    for (int temp = 0; temp < items.Count; temp++)
                    {
                        if (temp > 6)
                        {
                            break; // 协议仅支持7参数
                        }
                        if (items[temp].DevPropertyID == 3) { commandLength += 2; break; }//智能断电器
                        if (items[temp].DevPropertyID == 1)//模拟量
                        {
                            commandLength += 41;
                        }
                        else if (items[temp].DevPropertyID == 2)
                        {
                            commandLength += 9;
                        }
                        //if (manydefinecount >= 4) break;//最多4个多参数传感器不处理；
                        //if (temp == items.Count - 1 && temp > 0) manydefinecount++;
                    }
                }
            }

            if (OrderVersion == 14)//多两个字节的定义通道
            {
                commandLength += 2;
                if (def.Bz6 != null && def.Bz6 != "")
                {
                    commandLength += 2;
                    commandLength += 16 * 4;
                }
            }
            return commandLength;
        }
    }
}
