using Sys.DataCollection.Common.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Driver.Commands;
using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Driver.Driver;
using Sys.DataCollection.Common.Cache;
using Basic.Framework.Logging;
using Sys.DataCollection.Common.Utils;
using Sys.DataCollection.Driver.Driver.Net_8962;
using Sys.DataCollection.Common.Protocols.Devices;
using System.Threading;
using Basic.Framework.Common;
using System.IO;
namespace Sys.DataCollection.Driver
{
    /// <summary>
    /// 矿山类驱动集合之8962主控
    /// </summary>
    public class MineNet8962Driver : IDeviceDriver
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MineNet8962Driver()
        {
            CoreBllObj = new Driver.NetCoreBllRealizer();
        }
        /// <summary>
        /// 是否为第一次加载驱动
        /// </summary>
        public bool IsFirstLoad = true;
        /// <summary>
        /// 下行数据生成对象
        /// </summary>
        private NetCoreBllRealizer CoreBllObj;
        /// <summary>
        /// 驱动产生下行数据事件
        /// </summary>
        public event NetDataEventHandler OnNetDataCreated;
        /// <summary>
        /// 驱动产生上行对象事件
        /// </summary>
        public event ProtocolDataEventHandler OnProtocolDataCreated;
        /// <summary>
        /// 驱动在网关程序中执行特殊命令事件
        /// </summary>
        public event DriverCommandEventHandler OnExcuteDriverCommand;
        /// <summary>
        /// 驱动编号（唯一，可重复）
        /// </summary>
        public string DriverCode { get { return "DC001"; } }
        /// <summary>
        /// 是否需要启动复位处理线程
        /// </summary>
        public bool IsResetNetWork = true;
        /// <summary>
        /// 复位线程
        /// </summary>
        private Thread NetWorkThread = null;
        /// <summary>
        /// 作为启动后，多久开始加载分站上面的CRC信息
        /// </summary>
        private int NetReSetTime = 10;
        /// <summary>
        /// 重新发送的时间间隔-网络模块
        /// </summary>
        private int ReDoSetTime = 60;
        public ICacheManager CacheManager
        {
            get { return null; }
            set
            {
                Cache.CacheManager = value;
                if (IsFirstLoad)
                {
                    IsFirstLoad = false;
                    if (IsResetNetWork)//开起复位处理线程
                    {
                        if (NetWorkThread == null)
                        {
                            NetWorkThread = new Thread(new ThreadStart(NetWorkResetThread));
                            NetWorkThread.IsBackground = true;
                            NetWorkThread.Start();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 网络模块复位处理线程
        /// </summary>
        private void NetWorkResetThread()
        {
            CommSendData commdata = new CommSendData();
            int iclientCount = 0;
            List<ClientConntion> lstConnetion;
            if (Directory.Exists("C:\\NetSource\\") == false)
            {
                Directory.CreateDirectory("C:\\NetSource\\");
            }
            string strCur = "";
            DateTime t_dayTime = new DateTime(1999, 1, 1);
            string[] strP = { "debug", "error", "info", "system", "warn" };
            int i = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            for (; ; )
            {
                try
                {
                    sw.Restart();

                    #region 更新四舍五入标记、源码输出
                    try
                    {
                        System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                        strCur = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("B4c5r", "1");
                        if (strCur.Trim() == "2")
                            Cache.B4c5r = true;
                        else
                            Cache.B4c5r = false;
                        strCur = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("FeedTimeOut", "5");
                        Cache.FeedTimeOut = Convert.ToInt32(strCur);

                        strCur = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("ShowYmOut", "0");
                        Cache.ShowYmOut = Convert.ToInt32(strCur);
                        if (Cache.ShowYmOut == 1)//有源码输出才进行相关的处理
                        {
                            while (Cache.LstCommData.Count > 0)
                            {
                                lock (Cache.LockCommData)
                                {
                                    commdata = Basic.Framework.Common.ObjectConverter.DeepCopy<CommSendData>(Cache.LstCommData[0]);
                                    Cache.LstCommData.RemoveAt(0);
                                }
                                ShowNetDataToComputer(commdata.data, commdata.Mac, commdata.Flag);
                            }
                        }
                        else//不带源码输出直接清空
                        {
                            lock (Cache.LockCommData)
                            {
                                Cache.LstCommData.Clear();
                            }
                        }

                        strCur = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("FeedComplexFailure", "1");
                        if (strCur == "1")
                        {
                            Cache.FeedComplexFailure = true;
                        }
                        else
                        {
                            Cache.FeedComplexFailure = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("更新四舍五入、源码输出标记错误：" + ex.Message);
                    }
                    #endregion

                    #region 如处于客户端模式，定期进行连接和关闭判断

                    iclientCount++;
                    strCur = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("ServerOrClient", "1");
                    if ((iclientCount >= 3) && (strCur == "1"))
                    {
                        iclientCount = 0;
                        if (OnExcuteDriverCommand != null)//修正一下，如果是第一次进来时，连接两次
                        {
                            DriverCommandEventArgs args = new DriverCommandEventArgs();
                            DeviceResetCommand resetcommand = new DeviceResetCommand();
                            args.CommandType = 2;
                            lstConnetion = GetFzhIP();
                            args.JsonData = Basic.Framework.Common.JSONHelper.ToJSONString(lstConnetion);
                            OnExcuteDriverCommand(this, args);
                        }
                    }

                    #endregion

                    #region   自动删除数据
                    //自动删除 20个月前的源码数据
                    if (t_dayTime.Day != DateTime.Now.Day)
                    {
                        t_dayTime = DateTime.Now;
                        try
                        {
                            DateTime dt = new DateTime();
                            foreach (string d in Directory.GetFileSystemEntries("C:\\NetSource\\"))
                            {
                                if (File.Exists(d))
                                {
                                    FileInfo fi = new FileInfo(d);
                                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                                        fi.Attributes = FileAttributes.Normal;

                                    dt = new DateTime(Convert.ToInt32(fi.Name.Substring(0, 4)), Convert.ToInt32(fi.Name.Substring(4, 2)), Convert.ToInt32(fi.Name.Substring(6, 2)));
                                    if ((DateTime.Now - dt).TotalDays >= 20)
                                        File.Delete(d);//直接删除其中的文件  
                                }
                            }
                            for (i = 0; i < 5; i++)
                            {
                                strCur = "C:\\log\\Sys.DataCollection.ConsoleHost\\"+ strP[i];
                                //判断文件夹是否还存在
                                if (Directory.Exists(strCur))
                                {
                                    foreach (string f in Directory.GetFileSystemEntries(strCur))
                                    {
                                        strCur = f.ToString();
                                        strCur = strCur.Substring(strCur.LastIndexOf('\\') + 1);
                                        dt = new DateTime(Convert.ToInt32(strCur.Substring(0, 4)), Convert.ToInt32(strCur.Substring(4, 2)), Convert.ToInt32(strCur.Substring(6, 2)));
                                        if ((DateTime.Now - dt).TotalDays >= 20)
                                        {
                                            foreach (string f1 in Directory.GetFileSystemEntries(f))
                                            {
                                                if (File.Exists(f1))
                                                {//如果有子文件删除文件
                                                    File.Delete(f1);
                                                }
                                            }
                                            Directory.Delete(f);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("NetWorkReset【删除源码文件失败】" + ex.ToString());
                        }
                    }
                    #endregion
                    sw.Stop();
                    if (sw.ElapsedMilliseconds > 3000)
                    {//操作了3秒就输出日志出来
                        LogHelper.Info("网关超时判断:" +  sw.ElapsedMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("NetWorkReset【NetWorkResetThread】:" + ex.ToString());
                }
                Thread.Sleep(1000);
            }
        }
        private List<ClientConntion> GetFzhIP()
        {//增加交换机获了电源箱时的IP地址，并默认设置其端口号
            List<ClientConntion> lst = new List<ClientConntion>();
            string port = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("ClientPort", "1901");
            List<DeviceInfo> lstdevice = Cache.CacheManager.Query<DeviceInfo>(p => p.Activity == "1" && p.Kh == 0, true);
            ClientConntion item = null;
            if (lstdevice != null)
            {
                for (int i = 0; i < lstdevice.Count; i++)
                {
                    if (lst.FindIndex(p => p._ip == lstdevice[i].Jckz2) < 0)
                    {
                        item = new ClientConntion();
                        item._ip = lstdevice[i].Jckz2;
                        item._port = port;
                        lst.Add(item);
                    }
                }
            }
            //加交换机---------------
            List<NetworkDeviceInfo> lstNet = new List<NetworkDeviceInfo>();
            lstNet = Cache.CacheManager.Query<NetworkDeviceInfo>(p => p.Bz6 != "" && p.Bz6 != null && (p.Upflag == "1"), true);
            if (lstNet != null)
            {
                for (int i = 0; i < lstNet.Count; i++)
                {
                    if (lst.FindIndex(p => p._ip == lstNet[i].Bz6) < 0)
                    {
                        item = new ClientConntion();
                        item._ip = lstNet[i].Bz6;
                        item._port = port;
                        lst.Add(item);
                    }
                }
            }
            return lst;
        }
        /// <summary>
        /// 复位委托函数，调用外部接口执行。
        /// </summary>
        /// <param name="netMacObject">网络模块</param>
        /// <param name="resetTwo">表示是否需要连续复位2次</param>
        /// <param name="setType">表示此次复位是什么条件发起的=1 表示强制重启，=2表示无连接号超时，重启；=3表示有连接但是超时无数据</param>
        private void ResetNetWork(NetworkDeviceInfo netMacObject, byte setType, bool resetTwo = false)
        {
            if (OnExcuteDriverCommand != null)//修正一下，如果是第一次进来时，连接两次
            {
                bool result = false;
                DriverCommandEventArgs args = new DriverCommandEventArgs();
                DeviceResetCommand resetcommand = new DeviceResetCommand();
                resetcommand.Mac = netMacObject.MAC;
                resetcommand.Timeout = 2000;
                args.CommandType = 1;
                args.JsonData = Basic.Framework.Common.JSONHelper.ToJSONString(resetcommand);
                result = OnExcuteDriverCommand(this, args);
                if (resetTwo && !result)//如果失败需要连续复位，在此执行
                {
                    Thread.Sleep(200);
                    result = OnExcuteDriverCommand(this, args);
                }
                if (result)
                {
                    LogHelper.Info("对" + netMacObject.MAC + "-" + netMacObject.IP + "复位成功-" + setType.ToString());
                }
                else
                {
                    LogHelper.Info("对" + netMacObject.MAC + "-" + netMacObject.IP + "复位失败-" + setType.ToString());
                }
            }
        }
        private DateTime SetNetWorkNetID(NetworkDeviceInfo net)
        {
            DateTime result = DateTime.Now;
            lock (Cache.LockWorkNet)
            {
                CacheNetWork curnet = Cache.LstWorkNet.Find(delegate(CacheNetWork p) { return p.IP == net.IP; });
                if (curnet != null)
                {
                    net.NetID = curnet.NetID;
                    result = curnet.DttBridgeReceiveTime;
                }
            }
            return result;
        }

        #region 收到核心服务层数据处理(向下)
        /// <summary>
        /// 处理服务端下发的命令协议数据（对象转Buffer，下行）     
        /// </summary>
        /// <param name="masProtocol">下发的协议对象</param>
        public void HandleProtocolData(MasProtocol masProtocol)
        {
            byte[] data = new byte[1];//向下的数据包Buffer
            string deviceCode = "000";//向下对应的MAC地址
            int commPort = 1;//表示IP模块下面的串口编号
            if (masProtocol == null || masProtocol.Protocol == null)//数据效验
            {
                return;
            }
            switch (masProtocol.ProtocolType)
            {
                case ProtocolType.GetSwitchboardParamCommRequest://获取交换机的数据处理
                    commPort = 1;
                    var GetSwicthInfo = masProtocol.Deserialize<GetSwitchboardParamCommRequest>();
                    data = CoreBllObj.HandGetSwitchInfo(masProtocol, GetSwicthInfo, ref deviceCode);
                    break;
                case ProtocolType.DeviceControlRequest://设备控制命令
                case ProtocolType.QueryRealDataRequest://设备实时数据获取命令
                case ProtocolType.QueryDeviceInfoRequest://获取设备唯一编码信息
                    var deviceControl = masProtocol.Deserialize<DeviceControlRequest>();
                    data = CoreBllObj.HandDeviceControl(masProtocol, deviceControl, ref deviceCode, ref commPort);
                    break;
                case ProtocolType.InitializeRequest://初始化命令
                    var Initialize = masProtocol.Deserialize<InitializeRequest>();
                    data = CoreBllObj.HandInitialize(masProtocol, Initialize, ref deviceCode, ref commPort);
                    break;
                case ProtocolType.QueryBatteryRealDataRequest://查询电源箱的命令
                    var batteryRealData = masProtocol.Deserialize<QueryBatteryRealDataRequest>();
                    data = CoreBllObj.HandQueryBatteryRealData(masProtocol, batteryRealData, ref deviceCode, ref commPort);
                    break;
                case ProtocolType.ResetDeviceCommandRequest://设备复位
                    var resetDevice = masProtocol.Deserialize<ResetDeviceCommandRequest>();
                    data = CoreBllObj.HandResetDeviceCommand(masProtocol, resetDevice, ref deviceCode, ref commPort);
                    break;
                case ProtocolType.TimeSynchronizationRequest://时间同步
                    var timeSynchronization = masProtocol.Deserialize<TimeSynchronizationRequest>();
                    data = CoreBllObj.HandTimeSynchronization(masProtocol, timeSynchronization, ref deviceCode, ref commPort);
                    break;
                case ProtocolType.ModificationDeviceAdressRequest://传感器地址修改
                    var modificationdeviceadress = masProtocol.Deserialize<ModificationDeviceAdressRequest>();
                    data = CoreBllObj.HandModificationDeviceAdress(masProtocol, modificationdeviceadress, ref deviceCode, ref commPort);
                    break;
                case ProtocolType.QueryHistoryControlRequest://查询分站历史控制记录
                    var queryhistorycontrol = masProtocol.Deserialize<QueryHistoryControlRequest>();
                    data = CoreBllObj.HandQueryHistoryControl(masProtocol, queryhistorycontrol, ref deviceCode, ref commPort);
                    break;
                case ProtocolType.QueryHistoryRealDataRequest://查询分站历史4小时数据记录
                    var queryhistoryrealdata = masProtocol.Deserialize<QueryHistoryRealDataRequest>();
                    data = CoreBllObj.HandQueryHistoryRealData(masProtocol, queryhistoryrealdata, ref deviceCode, ref commPort);
                    break;
                case ProtocolType.SetSensorGradingAlarmRequest://传感器分级报警下发
                    var sensorgradingalarm = masProtocol.Deserialize<SetSensorGradingAlarmRequest>();
                    data = CoreBllObj.HandSensorGradingAlarm(masProtocol, sensorgradingalarm, ref deviceCode, ref commPort);
                    break;
                case ProtocolType.CallPersonRequest://井下人员呼叫
                    var callperson = masProtocol.Deserialize<CallPersonRequest>();
                    data = CoreBllObj.HandCallPersonCommand(masProtocol, callperson, ref deviceCode);
                    break;
                case ProtocolType.GetStationUpdateStateRequest:
                    var getStationUpdateStateRequest = masProtocol.Deserialize<GetStationUpdateStateRequest>();
                    data = CoreBllObj.HandGetStationUpdateState(masProtocol, getStationUpdateStateRequest, ref deviceCode);
                    break;
                case ProtocolType.InspectionRequest:
                    var inspectionResponse = masProtocol.Deserialize<InspectionRequest>();
                    data = CoreBllObj.HandInspection(masProtocol, inspectionResponse, ref deviceCode);
                    break;
                case ProtocolType.ReductionRequest:
                    var reductionRequest = masProtocol.Deserialize<ReductionRequest>();
                    data = CoreBllObj.HandReduction(masProtocol, reductionRequest, ref deviceCode);
                    break;
                case ProtocolType.RestartRequest:
                    var restartRequest = masProtocol.Deserialize<RestartRequest>();
                    data = CoreBllObj.HandRestart(masProtocol, restartRequest, ref deviceCode);
                    break;
                case ProtocolType.StationUpdateRequest:
                    var stationUpdateRequest = masProtocol.Deserialize<StationUpdateRequest>();
                    data = CoreBllObj.HandStationUpdate(masProtocol, stationUpdateRequest, ref deviceCode);
                    break;
                case ProtocolType.UpdateCancleRequest:
                    var updateCancleRequest = masProtocol.Deserialize<UpdateCancleRequest>();
                    data = CoreBllObj.HandUpdateCancle(masProtocol, updateCancleRequest, ref deviceCode);
                    break;
                case ProtocolType.SendUpdateBufferRequest:
                    var sendUpdateBufferRequest = masProtocol.Deserialize<SendUpdateBufferRequest>();
                    data = CoreBllObj.HandSendUpdateBuffer(masProtocol, sendUpdateBufferRequest, ref deviceCode);
                    break;
            }
            if (deviceCode != "000" && data != null)//通过事件抛出向下的数据包
            {
                LogHelper.Info("网络模块:" + deviceCode + "  分站：" + CoreBllObj.Point + "  下发：" + masProtocol.ProtocolType);
                if (Cache.ShowYmOut == 1)//有源码输出才进行相关的处理
                    Cache.AddCommData(data, deviceCode, 2);//下发源码输出
                if (OnNetDataCreated != null)
                {
                    OnNetDataCreated(null, new NetDataEventCreatedArgs() { DeviceCode = deviceCode, Data = data, DriverCode = this.DriverCode, CommPort = commPort });
                }
            }
        }
        #endregion

        #region 收到网络数据处理(向上)
        /// <summary>
        /// 源码输出函数
        /// </summary>
        /// <param name="data">输出BUffer</param>
        /// <param name="mac">网络模块地址</param>
        /// <param name="Flag">=1表示回发数据，=2表示下发数据</param>
        private void ShowNetDataToComputer(byte[] data, string mac, byte Flag = 1)
        {
            try
            {
                string strout = "";
                if(Flag==2)strout="【FS】：";
                else strout="【JS】：";
                strout+=BitConverter.ToString(data);
                System.IO.StreamWriter sw = null;
                sw = new System.IO.StreamWriter("C:\\NetSource\\" + DateTime.Now.ToString("yyyyMMdd") + "-" + mac + ".txt", true);
                sw.WriteLine(" ");
                sw.Write(DateTime.Now.ToString() + strout);
                sw.Close();
               
            }
            catch (Exception ex)
            {
                LogHelper.Error("ShowNetDataToComputer:" + ex.Message);
            }
        }
        /// <summary>
        /// 处理网络接收的数据（上行）
        /// </summary>
        /// <param name="data">收到的网络数据包</param>
        /// <param name="net">接收到此数据包的网络模块</param>
        public void HandleNetData(byte[] data, string ip)
        {
            CacheNetWork curnet;
            bool tcnet = false;
            NetworkDeviceInfo net = Cache.CacheManager.QueryFrist<NetworkDeviceInfo>(p => p.IP == ip, true);
            //增加交换机处理
            if(net==null)
            {//再找一下交换机对应的串口服务器IP
                tcnet = true;
                net = Cache.CacheManager.QueryFrist<NetworkDeviceInfo>(p => p.Bz6 == ip, true);
            }
            if (Cache.ShowYmOut == 1)//有源码输出才进行相关的处理
                Cache.AddCommData(data, ip, 1);//源码输出
            if (net == null) return;
            KJ73NCommand command = KJ73NCommand.ToCommand(data, net);
            if (!command.IsSuccess)
            {
                lock (Cache.LockWorkNet)//用缓存的网络设备进行操作
                {
                    curnet = Cache.LstWorkNet.Find(delegate(CacheNetWork p) { return p.IP == net.IP; });
                    if (curnet != null)
                    {
                        curnet.BBridgeRevMark = false;
                    }
                }
                LogHelper.Info(command.errorMessage);
                return;
            }
            SendNetStateToCenter(net); //发送网络模块的实时数据至中心站；
            lock (Cache.LockWorkNet)//用缓存的网络设备进行操作
            {
                if (tcnet)
                {
                    curnet = Cache.LstWorkNet.Find(delegate (CacheNetWork p) { return p.IP == net.Bz6; });
                }
                else
                    curnet = Cache.LstWorkNet.Find(delegate (CacheNetWork p) { return p.IP == net.IP; });
                if (curnet != null)
                {
                    curnet.BBridgeRevMark = true;//表示数据包接收正常
                    curnet.DttBridgeReceiveTime = DateTime.Now;//用于存储最新接收到数据时的时间
                }
                else
                {//是不正常情况，输出日志，但是在此，也先行进行处理。
                    LogHelper.Error("错误日志:" + net.MAC + "  当数据包上行时，未找到未相应的缓存网络模块链表！！");
                    curnet = new Driver.CacheNetWork();
                    curnet.State = 3;
                    curnet.MAC = net.MAC;
                    curnet.IP = net.IP;
                    curnet.NetID = 0;
                    curnet.DttBridgeReceiveTime = DateTime.Now;//用于存储最新接收到数据时的时间
                    curnet.BBridgeRevMark = false;
                    Cache.LstWorkNet.Add(curnet);
                }
            }
            switch (command.PackageType)
            {
                case 1://上行的数据包
                    SendDevicDataAffirmToCenter(net, data);
                    LogHelper.Info("收到数据包:" + net.IP + "-" + net.MAC + "  长度：" + data.Length);
                    break;
                case 2://交换机信息
                    SendSwicthBaseInfo(net, data);
                    break;
            }
        }

        /// <summary>
        /// 表示数据包回发处理后至核心业务逻辑层
        /// </summary>
        /// <param name="net"></param>
        /// <param name="data"></param>
        private void SendDevicDataAffirmToCenter(NetworkDeviceInfo net, byte[] data)
        {
            int index = 0, deviceCode;
            byte[] fzhBuffer = new byte[1];
            ushort receiveDeviceCount = 0;//每包的长度
            DeviceInfo def;
            ProtocolDataCreatedEventArgs upData = new ProtocolDataCreatedEventArgs();
            DeviceDataConrtol dataConrtol = new DeviceDataConrtol();
            if (data.Length >= 8)
            {
                receiveDeviceCount = CommandUtil.ConvertByteToInt16(data, 6,false);//读长度,后续字节长度为-4
                deviceCode = data[4];
                def = Cache.CacheManager.QueryFrist<DeviceInfo>(p => p.Fzh == deviceCode && p.DevPropertyID == 0, true);
                if (def != null)
                {
                    if (def.Jckz2 == net.IP)//只处理分站定义相对应的IP。
                    {
                        if (4 + receiveDeviceCount > data.Length)
                        {//数据长度不足
                            LogHelper.Error("回发长度不足:" + net.MAC + "  回发长度：" + data.Length + " 判断长度：" + (4 + receiveDeviceCount).ToString());
                            return;
                        }
                        fzhBuffer = new byte[data.Length - 4];
                        for (index = 4; index < data.Length; index++)
                        {
                            fzhBuffer[index - 4] = data[index];
                        }
                        upData.DriverCode = this.DriverCode;
                        upData.DeviceCode = def.Point;
                        upData.MasProtocol = new MasProtocol((SystemType)(def.Sysid), DirectionType.Up, ProtocolType.QueryRealDataResponse);
                        upData.MasProtocol.DeviceNumber = Convert.ToUInt16(def.Fzh);
                        dataConrtol.def = def;
                        dataConrtol.HandleDeviceData(fzhBuffer, upData.MasProtocol);
                        OnProtocolData(upData);
                    }
                }
            }
        }
        /// <summary>
        /// 表示交换机电源的确认
        /// </summary>
        /// <param name="net"></param>
        /// <param name="data"></param>
        private void SendBatteryAffirmToCenter(NetworkDeviceInfo net, byte[] data)
        {
            //7F-03-2A-00-00-01-8A-01-8A-01-8A-01-8A-01-8A-01-8A-00-00-00-00-09-41-00-40-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-E8-07

            ProtocolDataCreatedEventArgs upData = new ProtocolDataCreatedEventArgs();
            QueryBatteryRealDataResponse cData = new QueryBatteryRealDataResponse();//获取交换机电源箱的应答数据；
            //需要反过来找一下，串口服务器对应的交换机IP地址。
            upData.DeviceCode = net.IP;
            upData.DriverCode = this.DriverCode;
            upData.MasProtocol = new MasProtocol(SystemType.Security, DirectionType.Up, ProtocolType.QueryBatteryRealDataResponse);
            cData.DeviceCode = net.IP;
            cData.DeviceProperty = ItemDevProperty.Switches;
            cData.BatteryRealDataItems = new List<BatteryRealDataItem>();
            BatteryRealDataItem BatteryItem = new BatteryRealDataItem();
            cData.BatteryDateTime = DateTime.Now;
            Cache.HandleDeviceBattery(data, 5, BatteryItem, true);//解析电源箱的数据  
            cData.BatteryRealDataItems.Add(BatteryItem);
            upData.MasProtocol.Protocol = cData;
            OnProtocolData(upData);
        }
        private void SendSwicthBaseInfo(NetworkDeviceInfo net, byte[] data)
        {
            if (data[0] == 0x7F&& data[1] == 0x10)
            {
                //ProtocolDataCreatedEventArgs upData = new ProtocolDataCreatedEventArgs();
                //upData.DeviceCode = net.MAC;
                //upData.DriverCode = this.DriverCode;
                //upData.MasProtocol = new MasProtocol(SystemType.Security, DirectionType.Up, ProtocolType.GetSwitchboardParamCommResponse);
                //GetSwitchboardParamCommResponseCommand.HandleSwitchInfo(data, upData, net);
                //OnProtocolData(upData);
            }
            else if (data[0] == 0x7F && data[1] == 0x03)
            {
                SendBatteryAffirmToCenter(net, data);
            }
        }
        /// <summary>
        /// 网关应答中心站控制及取数命令
        /// </summary>
        /// <param name="Fzh"></param>
        private void SendConrtolAffirmToCenter(string Fzh)
        {
            ProtocolDataCreatedEventArgs upData = new ProtocolDataCreatedEventArgs();
            DeviceControlResponse cData = new DeviceControlResponse();//控制应答数据
            DeviceInfo def = Cache.CacheManager.QueryFrist<DeviceInfo>(p => p.Fzh == Convert.ToInt16(Fzh) && p.DevPropertyID == 0, true);
            if (def == null) return;
            upData.DriverCode = this.DriverCode;
            upData.DeviceCode = def.Point;
            upData.MasProtocol = new MasProtocol(SystemType.Security, DirectionType.Up, ProtocolType.DeviceControlResponse);
            upData.MasProtocol.DeviceNumber = Convert.ToUInt16(Fzh);
            cData.DeviceCode = def.Point;
            upData.MasProtocol.Protocol = cData;
            OnProtocolData(upData);
        }
        /// <summary>
        /// 应答核心业务逻辑层的初始化接收确认。
        /// </summary>
        /// <param name="Fzh"></param>
        private void SendInitializeAffirmToCenter(string Fzh)
        {
            ProtocolDataCreatedEventArgs upData = new ProtocolDataCreatedEventArgs();
            InitializeResponse cData = new InitializeResponse();//初始化应答数据
            DeviceInfo def = Cache.CacheManager.QueryFrist<DeviceInfo>(p => p.Fzh == Convert.ToInt16(Fzh) && p.DevPropertyID == 0, true);
            if (def == null) return;
            upData.DriverCode = this.DriverCode;
            upData.DeviceCode = def.Point;
            upData.MasProtocol = new MasProtocol(SystemType.Security, DirectionType.Up, ProtocolType.InitializeResponse);
            upData.MasProtocol.DeviceNumber = Convert.ToUInt16(Fzh);
            cData.DeviceCode = def.Point;
            upData.MasProtocol.Protocol = cData;
            OnProtocolData(upData);
        }
        /// <summary>
        /// 回发网络模块的状态至核心业务逻辑层
        /// </summary>
        /// <param name="net">网络模块IP</param>
        private void SendNetStateToCenter(NetworkDeviceInfo net)
        {
            ProtocolDataCreatedEventArgs upData = new ProtocolDataCreatedEventArgs();
            NetworkDeviceDataRequest cData = new NetworkDeviceDataRequest();//网络模块自身的实时值数据
            lock (Cache.LockWorkNet)//用缓存的网络设备进行操作
            {
                CacheNetWork curnet = Cache.LstWorkNet.Find(delegate(CacheNetWork p) { return p.IP == net.IP; });
                if (curnet != null)
                {
                    net.NetID = curnet.NetID;
                    net.State = curnet.State;
                }
            }
            upData.DeviceCode = net.IP;
            upData.DriverCode = this.DriverCode;
            upData.MasProtocol = new MasProtocol(SystemType.Security, DirectionType.Up, ProtocolType.NetworkDeviceDataRequest);
            cData.DeviceCode = net.IP;
            cData.RealData = net.NetID.ToString();
            cData.Address = net.IP;
            cData.Channel = net.MAC;
            if (net.State == 4) //直流
            {
                cData.State = ItemState.EquipmentDC;
            }
            else if (net.State == 3) //交流
            {
                cData.State = ItemState.EquipmentAC; ;
            }
            upData.MasProtocol.Protocol = cData;
            OnProtocolData(upData);
        }
        /// <summary>
        /// 接收上行数据处理的接口
        /// </summary>
        /// <param name="pdcEvents"></param>
        public void OnProtocolData(ProtocolDataCreatedEventArgs pdcEvents)
        {
            if (OnProtocolDataCreated != null)
            {
                //这里是例子MasProtocol
                OnProtocolDataCreated(null, pdcEvents);
            }
        }
        /// <summary>
        /// 网关通讯状态变化通知
        /// </summary>
        /// <param name="stateChangeArgs">通讯状态变化参数</param>
        public void CommunicationStateChangeNotify(CommunicationStateChangeArgs stateChangeArgs)
        {
            bool flag = false;
            CacheNetWork curnet;
            int curid = 0;
            if (stateChangeArgs.CommunicationState == CommunicationState.Connect)
            {
                //网络连接成功处理
                flag = true;
                NetworkDeviceInfo net = Cache.CacheManager.QueryFrist<NetworkDeviceInfo>(p => p.IP == stateChangeArgs.UniqueCode, true);
                if (net == null)
                {
                    net = new NetworkDeviceInfo();
                }
                net.NetID = (int)stateChangeArgs.ConntecionId;
                net.State = 3;
                net.IP = stateChangeArgs.UniqueCode;
                lock (Cache.LockWorkNet)
                {
                    curnet = Cache.LstWorkNet.Find(delegate(CacheNetWork p) { return p.IP == stateChangeArgs.UniqueCode; });
                    flag = false;
                    if (curnet == null)
                    {
                        flag = true;
                        curnet = new Driver.CacheNetWork();
                    }
                    curnet.DttBridgeReceiveTime = DateTime.Now;
                    curnet.State = 3;
                    curnet.IP = stateChangeArgs.UniqueCode;
                    curnet.NetID = (int)stateChangeArgs.ConntecionId;
                    curnet.BBridgeRevMark = false;
                    if (flag) Cache.LstWorkNet.Add(curnet);
                }
                SendNetStateToCenter(net);
                LogHelper.Info("收到" + curnet.IP + "-请求连接命令！" );

            }
            else if (stateChangeArgs.CommunicationState == CommunicationState.Disconnect)
            {
                //网络断开处理
                NetworkDeviceInfo net = Cache.CacheManager.QueryFrist<NetworkDeviceInfo>(p => p.IP == stateChangeArgs.UniqueCode, true);
                if (net == null) { net = new NetworkDeviceInfo(); }
                curid = GetMacConnectionID(stateChangeArgs.UniqueCode);
                if (curid > 0)
                {
                    if (stateChangeArgs.ConntecionId < curid)
                    {
                        LogHelper.Info("断开连接不处理：ConnectIdOld=" + curid + ",ConnectIdNew=" + stateChangeArgs.ConntecionId);
                        return;
                    }
                }
                net.IP = stateChangeArgs.UniqueCode;
                net.NetID = 0;
                net.State = (short)ItemState.EquipmentInterrupted;

                lock (Cache.LockWorkNet)//更新网络模块缓存
                {
                    curnet = Cache.LstWorkNet.Find(delegate(CacheNetWork p) { return p.IP == stateChangeArgs.UniqueCode; });
                    if (curnet != null)
                    {
                        curnet.DttBridgeReceiveTime = DateTime.Now.AddSeconds(-(ReDoSetTime - 10));
                        curnet.State = 0;
                        curnet.NetID = 0;
                        curnet.BBridgeRevMark = false;
                    }
                }
                SendNetStateToCenter(net);
                LogHelper.Info("收到" + curnet.IP + "-断开连接请求！");
            }
        }
        private int GetMacConnectionID(string mac)
        {
            int result = -1;
            lock (Cache.LockWorkNet)
            {
                CacheNetWork curnet = Cache.LstWorkNet.Find(delegate(CacheNetWork p) { return p.MAC == mac; });
                if (curnet != null)
                {
                    result = curnet.NetID;
                }
            }
            return result;
        }
        #endregion
    }

    public class Cache
    {
        /// <summary>
        /// 访问脉冲计数链表加锁
        /// </summary>
        public static object LocklstLJL = new object();
        /// <summary>
        /// 表示脉冲计数链表
        /// </summary>
        public static List<McAddTotal> lstLJL = new List<McAddTotal>();
        /// <summary>
        /// 源码输出默认不输出
        /// </summary>
        public static int ShowYmOut = 0;
        /// <summary>
        /// 上传时间与系统时间差值,确认为补传,不采用以前的时间对比，如超过用当前时间---加config
        /// </summary>
        public static int PassUpTime = 30;//以分为单位
        /// <summary>
        /// 人员定位重复记录列表
        /// </summary>
        public static List<RyMore> RyNowCf = new List<RyMore>();
        /// <summary>
        /// 欠压卡号列表
        /// </summary>
        public static List<int> qykhlst = new List<int>();
        /// <summary>
        /// 最小允许采集识别卡号
        /// </summary>
        public static ushort MinId = 1;
        /// <summary>
        /// 最大允许采集识别卡号
        /// </summary>
        public static ushort MaxId = 30000;
        /// <summary>
        /// 机车最小采集卡号
        /// </summary>
        public static ushort JcMinId = 0;
        /// <summary>
        /// 机车最大采集卡号
        /// </summary>
        public static ushort JcMaxId = 0;
        /// <summary>
        /// 用于馈电计算的容错时间为5秒
        /// </summary>
        public static int FeedTimeOut = 5;
        /// <summary>
        /// 是否计算复电失败  true: 是，false不是
        /// </summary>
        public static bool FeedComplexFailure { get; set; }
        /// <summary>
        /// 用于断电、复电失败加锁
        /// </summary>
        public static object LockControlInfo = new object();
        /// <summary>
        /// 断电、复电失败的链表
        /// </summary>
        public static List<ControlInfo> LstControlInfo = new List<ControlInfo>();
        /// <summary>
        /// 用于源码输出加锁
        /// </summary>
        public static object LockCommData = new object();
        /// <summary>
        /// 源码输出列表
        /// </summary>
        public static List<CommSendData> LstCommData = new List<CommSendData>();
        /// <summary>
        /// 分站设备加锁标记
        /// </summary>
        public static object LockCacheDevice = new object();
        /// <summary>
        /// 表示当前的驱动缓存的关于设备相关的更新信息
        /// 当此链表无对应分站数据时，以定义为主，如果有相关信息以缓存为主
        /// 此缓存在设备回发时更新相关参数，在收到此设备中断后清除相关信息。
        /// 保存分站下发初始化的命令类型，当为新分站，上一次发送为I命令时，需要切换成J命令
        /// </summary>
        //public static List<CacheDevice> LstDeviceCache = new List<CacheDevice>();
        /// <summary>
        /// 访问网络缓存加锁
        /// </summary>
        public static object LockWorkNet = new object();
        /// <summary>
        /// 表示当前缓存的交换机模块队列
        /// </summary>
        public static List<CacheNetWork> LstWorkNet = new List<CacheNetWork>();
        /// <summary>
        /// 缓存的测点定义信息
        /// </summary>
        public static ICacheManager CacheManager { get; set; }
        /// <summary>
        ///  是否按照量程进行四舍五入 true: 是，false不是
        /// </summary>
        public static bool B4c5r { get; set; }

        /// <summary>
        /// 处理交换机以及分站的电源箱回发的数据，通用函数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex">表示电源箱的地址号</param>
        /// <param name="batteryItem"></param>
        public static void HandleDeviceBattery(byte[] data, byte startIndex, BatteryRealDataItem batteryItem, bool switchType = false)
        {
            ushort tempValue = 6;
            batteryItem.BatteryVOL = new float[6];
            batteryItem.Address = "0";
            if (switchType)
            {
                batteryItem.Channel = "250";
            }
            else
                batteryItem.Channel = data[startIndex++].ToString();
            for (int i = 0; i < tempValue; i++)
            {
                if (switchType)
                    batteryItem.BatteryVOL[i] = (float)(CommandUtil.ConvertByteToInt16(data, startIndex, true) / 100.0);
                else
                    batteryItem.BatteryVOL[i] = (float)(CommandUtil.ConvertByteToInt16(data, startIndex, false) / 100.0);
                startIndex += 2;
            }

            if (switchType)
                batteryItem.DeviceTemperature1 = (float)(CommandUtil.ConvertByteToInt16(data, startIndex, true) / 100.0);
            else
                batteryItem.DeviceTemperature1 = (float)(CommandUtil.ConvertByteToInt16(data, startIndex, false) / 100.0);
            startIndex += 2;
            batteryItem.DeviceTemperature1 += 17.0f;
            if (switchType)
                batteryItem.DeviceTemperature2 = (float)(CommandUtil.ConvertByteToInt16(data, startIndex, true) / 100.0);
            else
                batteryItem.DeviceTemperature2 = (float)(CommandUtil.ConvertByteToInt16(data, startIndex, false) / 100.0);
            batteryItem.DeviceTemperature2 += 17.0f;

            startIndex += 2;
            if (switchType)
                batteryItem.TotalVoltage = (float)(CommandUtil.ConvertByteToInt16(data, startIndex, true) / 100.0);
            else
                batteryItem.TotalVoltage = (float)(CommandUtil.ConvertByteToInt16(data, startIndex, false) / 100.0);

            startIndex += 2;
            if (switchType)
            {
                if ((data[startIndex + 1] & 0x01) == 0)
                    batteryItem.BatteryACDC = 1;//交直流状态 （0 断线，1交流、2直流）
                else
                    batteryItem.BatteryACDC = 2;//交直流状态 （0 断线，1交流、2直流）
            }
            else
            {
                if (data[startIndex] == 0)
                    batteryItem.BatteryACDC = 1;//交直流状态 （0 断线，1交流、2直流）
                else
                    batteryItem.BatteryACDC = 2;//交直流状态 （0 断线，1交流、2直流）
            }
        }
       
        public static ItemState GetSensorState(ushort SensorState, byte deviceCommunicationType)
        {
            ItemState SensorTempState = ItemState.EquipmentCommOK;//设备状态
            switch (SensorState)
            {
                case 0:
                    SensorTempState = ItemState.EquipmentCommOK;
                    break;
                case 1:
                    SensorTempState = ItemState.EquipmentStart;
                    break;
                case 2:
                    SensorTempState = ItemState.EquipmentAdjusting;
                    break;
                case 3:
                    SensorTempState = ItemState.EquipmentInfrareding;
                    break;
                case 4:
                    SensorTempState = ItemState.EquipmentBiterror;
                    break;
                case 5:
                    SensorTempState = ItemState.EquipmentDown;
                    break;
                case 6:
                    if (deviceCommunicationType == 0x26)
                        SensorTempState = ItemState.EquipmentTypeError;
                    else
                        SensorTempState = ItemState.EquipmentHeadDown;
                    break;
                case 7:
                    SensorTempState = ItemState.EquipmentCommOK;
                    break;
            }
            return SensorTempState;
        }
        public static DateTime TurnTimeFromInt(uint value)
        {
            DateTime dtime = new DateTime();
            uint tempValue = value;
            try
            {
                int year, month, day, minute, second, hour;
                second = (int)(value & 0x3F);
                value >>= 6;
                minute = (int)(value & 0x3F);
                value >>= 6;
                hour = (int)(value & 0x1F);
                value >>= 5;
                day = (int)(value & 0x1F);
                value >>= 5;
                month = (int)(value & 0x0F);
                value >>= 4;
                year = (int)(value & 0x3F);
                year += 2018;
                dtime = new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                LogHelper.Error("TurnTimeFromInt Error:value = " + tempValue);  
            }
            return dtime;
        }
        /// <summary>
        /// 用于增加网络源码输出对象
        /// </summary>
        /// <param name="data">输出BUffer</param>
        /// <param name="mac">网络模块地址</param>
        /// <param name="Flag">=1表示回发数据，=2表示下发数据</param>
        public static void AddCommData(byte[] data, string mac, byte Flag = 1)
        {
            try
            {
                CommSendData cur = new Driver.CommSendData();
                cur.data = data;
                cur.Mac = mac;
                cur.Flag = Flag;
                lock (LockCommData)
                {
                    Cache.LstCommData.Add(cur);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("AddCommData:" + ex.Message);
            }
        }
    }
    /// <summary>
    /// 客户端进行连接操作时的对象
    /// </summary>
    public class ClientConntion
    {
        public string _ip;
        public string _port;
    }

}
