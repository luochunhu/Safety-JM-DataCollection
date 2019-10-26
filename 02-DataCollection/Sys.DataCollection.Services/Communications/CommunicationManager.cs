using Basic.Framework.Logging;
using Basic.Framework.Rpc;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.DataCollection.Common.Driver;
using DC.Communication.Components;

namespace Sys.DataCollection.Communications
{


    /// <summary>
    /// 通讯管理类
    /// </summary>
    public class CommunicationManager
    {
        Dictionary<string, ICommunication> _communicationCache = null;
        Dictionary<string, CommunicationInfo> _communicationConfig = null;

        public CommunicationManager()
        {
            _communicationCache = new Dictionary<string, ICommunication>();
            _communicationConfig = new Dictionary<string, CommunicationInfo>();
        }

        /// <summary>
        /// 通讯管理模块启动
        /// </summary>
        public void Start()
        {
            //加载通讯模块
            LoadCommunicationConfig();

            foreach (var config in _communicationConfig.Values)
            {
                //初始化和启动通讯模块
                ICommunication communication = CommunicationFactory.CreateCommunication(config.CommunicationType);
                communication.CommunicationCode = config.CommunicationCode;
                communication.OnNetDataArrived += Communication_OnNetDataArrived;
                communication.OnCommunicationStateChange += Communication_OnCommunicationStateChange;
                communication.Start(config.ServerIp, config.ServerPort);
                _communicationCache.Add(config.CommunicationCode, communication);
            }

            LogHelper.Debug("启动网络模块完成");
        }


        /// <summary>
        /// 停止通讯模块
        /// </summary>
        public void Stop()
        {
            LogHelper.SystemInfo("正在停止网络通讯模块");

            foreach (var communication in _communicationCache.Values)
            {
                try
                {
                    communication.Stop();
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("停止 {0} 网络通讯模块出错，错误原因：{1}", communication.CommunicationCode, ex.ToString()));
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="communicationCode">网络模块编号</param>
        /// <param name="target">目标唯一编号（可能是ip、手机号等）</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public bool Send(string communicationCode, string target, byte[] data)
        {
            //这里根据映射配置调用网络通讯模块发送数据            
            return _communicationCache[communicationCode].Send(target, data);
        }

        public void ConnetionServer(string communicationCode, List<ClientConntion> lstConnetion)
        {
             _communicationCache[communicationCode].ConnetionServer(lstConnetion);
        }

        /// <summary>
        /// 加载通讯配置
        /// </summary>
        private void LoadCommunicationConfig()
        {
            LogHelper.Debug("正在初始化网络通讯配置");
            if (GatewayService._serverOrClient != 1)//20181013
            {
                List<CommunicationInfo> communicationlst = GetDefaultCommunicationLst();
                for (int i = 0; i < communicationlst.Count; i++)
                {
                    _communicationConfig.Add(communicationlst[i].CommunicationCode, communicationlst[i]);
                }
            }
            else
            {
                CommunicationInfo cvalue = new CommunicationInfo()
                {
                    CommunicationCode = "CS001",
                    DriverCode = "DC001",
                    ServerIp = "127.0.0.1",
                    ServerPort = 1901,
                    SocketType = SocketType.TCP,
                    CommunicationType = CommunicationType.socketClient,
                    DeviceList = new List<string>()
                };
                _communicationConfig.Add(cvalue.CommunicationCode, cvalue);
            }
            //增加广播系统http通讯
            //bool broadCastEnable = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetBool("BroadCastEnable", false);
            //if (broadCastEnable)
            //{
            //    CommunicationInfo broadcastCommunication = GetBroadCastDefaultCommunication();
            //    _communicationConfig.Add(broadcastCommunication.CommunicationCode, broadcastCommunication);
            //}
        }

        ///// <summary>
        ///// 获取安全监控默认通讯配置（这里代码暂时写死，未走配置 20170614 ）
        ///// </summary>
        ///// <returns></returns>
        //private CommunicationInfo GetKJ73NDefaultCommunication()
        //{
        //    string netServerIp = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("NetServerIp", "127.0.0.1");
        //    int netServerPort = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("NetServerPort", 7300);

        //    CommunicationInfo ci = new CommunicationInfo()
        //    {
        //        CommunicationCode = "SC001",
        //        DriverCode = "DC001",
        //        ServerIp = netServerIp,
        //        ServerPort = netServerPort,
        //        SocketType = SocketType.TCP,
        //        CommunicationType = CommunicationType.C8962,
        //        DeviceList = new List<string>()
        //    };

        //    return ci;
        //}
        /// <summary>
        /// 获取安全监控默认通讯配置
        /// </summary>
        /// <returns></returns>
        private List< CommunicationInfo> GetDefaultCommunicationLst()
        {
            List<CommunicationInfo> lstObject = new List<CommunicationInfo>();
            string netServerIp = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("NetServerIp", "127.0.0.1");
            int netServerPort = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("NetServerPort", 7301);
            int netServerPort2 = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("NetServerPort2", 7302);
            int netServerPort3 = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("NetServerPort3", 7303);
            int netServerPort4 = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("NetServerPort4", 7304);
            CommunicationInfo ci = null;
            if (netServerPort != 0)
            {
                ci = new CommunicationInfo()
                {
                    CommunicationCode = "SC001",
                    DriverCode = "DC001",
                    ServerIp = netServerIp,
                    ServerPort = netServerPort,
                    SocketType = SocketType.TCP,
                    CommunicationType = CommunicationType.C8962,
                    DeviceList = new List<string>()
                };
                lstObject.Add(ci);
            }
            if (netServerPort2 != 0)
            {
                ci = new CommunicationInfo()
                {
                    CommunicationCode = "SC002",
                    DriverCode = "DC001",
                    ServerIp = netServerIp,
                    ServerPort = netServerPort2,
                    SocketType = SocketType.TCP,
                    CommunicationType = CommunicationType.C8962,
                    DeviceList = new List<string>()
                };
                lstObject.Add(ci);
            }
            if (netServerPort3 != 0)
            {
                ci = new CommunicationInfo()
                {
                    CommunicationCode = "SC003",
                    DriverCode = "DC001",
                    ServerIp = netServerIp,
                    ServerPort = netServerPort3,
                    SocketType = SocketType.TCP,
                    CommunicationType = CommunicationType.C8962,
                    DeviceList = new List<string>()
                };
                lstObject.Add(ci);
            }
            if (netServerPort4 != 0)
            {
                ci = new CommunicationInfo()
                {
                    CommunicationCode = "SC004",
                    DriverCode = "DC001",
                    ServerIp = netServerIp,
                    ServerPort = netServerPort4,
                    SocketType = SocketType.TCP,
                    CommunicationType = CommunicationType.C8962,
                    DeviceList = new List<string>()
                };
                lstObject.Add(ci);
            }
            return lstObject;
        }
        //private CommunicationInfo GetBroadCastDefaultCommunication()
        //{
        //    //string netServerIp = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("BroadCastServerIp", "127.0.0.1");
        //    //int netServerPort = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("BroadCastServerPort", 7300);

        //    //CommunicationInfo ci = new CommunicationInfo()
        //    //{
        //    //    CommunicationCode = "BC001",
        //    //    DriverCode = "",
        //    //    ServerIp = netServerIp,
        //    //    ServerPort = netServerPort,
        //    //    SocketType = SocketType.TCP,
        //    //    CommunicationType = CommunicationType.Http,
        //    //    DeviceList = new List<string>()
        //    //};
        //    //return ci;
        //}

        /// <summary>
        /// 收到网络数据处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Communication_OnNetDataArrived(object sender, NetDataArrivedEventArgs args)
        {
            //根据通讯编号找驱动映射关系
            string driverCode = GatewayMapper.GetDriverCode(args.CommunicationCode);

            //交给驱动处理业务包
            GatewayManager.DriverManager.HandleNetData(driverCode, args.UniqueCode, args.Data);
        }

        /// <summary>
        /// 网络 状态变化 处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Communication_OnCommunicationStateChange(object sender, CommunicationStateChangeArgs args)
        {
            //根据通讯编号找驱动映射关系
            string driverCode = GatewayMapper.GetDriverCode(args.CommunicationCode);

            //交给驱动处理业务包
            GatewayManager.DriverManager.HandleCommunicationStateChange(driverCode, args);
        }
    }

    public enum SocketType
    {
        TCP,
        UDP
    }
    public class CommunicationInfo
    {
        public string CommunicationCode { get; set; }
        public string DriverCode { get; set; }
        public List<string> DeviceList { get; set; }


        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public CommunicationType CommunicationType { get; set; }
        public SocketType SocketType { get; set; }
    }
}
