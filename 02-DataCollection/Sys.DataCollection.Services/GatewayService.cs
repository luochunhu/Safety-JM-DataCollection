
using Basic.Framework.Logging;
using Basic.Framework.Rpc;
using Sys.DataCollection.Cache;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Rpc;
using Sys.DataCollection.Communications;
using Sys.DataCollection.Communications.Provider;
using Sys.DataCollection.Driver;
using Sys.DataCollection.Rpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sys.DataCollection.Services
{
    /// <summary>
    /// 网关服务
    /// </summary>
    public class GatewayService
    {  
        static HeartbeatManager _heartbeatManager;//心跳管理器
       public static int _serverOrClient = 1;//表示是客户端模式还是服务端模式
        static string _rpcRemoteIp = "";
        static int _rpcRemotePort = 0;
        static string _rpcLocalIp = "";
        static int _rpcLocalPort = 0;
        static int _heartbeatInterval = 30;
        static bool _isRun = false;
        /// <summary>
        /// 本地日志自动清除线程
        /// </summary>
        private static Thread log4netClearThread;

        static GatewayService()
        {
           // LogHelper.Warn("static GatewayService()");
        }

        /// <summary>
        /// 网关服务启动
        /// </summary>
        /// <returns></returns>
        public static bool Start()
        {
            if (_isRun)
            {
                return true;
            }

            try
            {
                LogHelper.SystemInfo("正在启动安全监控系统采集程序,请等待。。。");
                _isRun = true;

                InitConfigData();

                InitRpc();

                InitHeartbeat();

                InitCache();

                InitDrivers();

                InitCommunicationChannel();

                //这里一定要顺序初始化程序，不然有可能后面的模块还没有初始化完，数据就上来了。
                GatewayManager.RpcManager.StartRpcServer();

                //增加本地日志自动清除功能
                log4netClearThread = new Thread(ClearLog4netLog);
                log4netClearThread.IsBackground = true;
                log4netClearThread.Start();

                LogHelper.SystemInfo("采集程序启动成功");

                AnalogTest.DoAnalogTest();
            }
            catch (Exception ex)
            {
                _isRun = false;

                LogHelper.SystemInfo("网关程序启动失败，具体查看错误日志。");
                LogHelper.Error("网关程序启动失败，错误原因：" + ex.ToString());
            }

            return _isRun;
        }

        /// <summary>
        /// 网关服务停止
        /// </summary>
        /// <returns></returns>
        public static bool Stop()
        {
            if (!_isRun)
            {
                return true;
            }
            try
            {
                _isRun = false;

                if (GatewayManager.CommunicationManager != null)
                {
                    GatewayManager.CommunicationManager.Stop();
                }

                if (GatewayManager.RpcManager != null)
                {
                    GatewayManager.RpcManager.StopRpcServer();
                }

                if (GatewayManager.CacheManager != null)
                {
                    GatewayManager.CacheManager.Stop();
                }

                if (GatewayManager.DriverManager != null)
                {
                    GatewayManager.DriverManager.Stop();
                }

                if (_heartbeatManager != null)
                {
                    _heartbeatManager.Stop();
                }

                _isRun = false;

                LogHelper.SystemInfo("******************网关程序停止成功******************");
            }
            catch (Exception ex)
            {
                _isRun = true;

                LogHelper.SystemInfo("网关程序停止失败，具体查看错误日志。");
                LogHelper.Error("网关程序停止失败，错误原因：" + ex.ToString());
            }           

            return _isRun;
            
        }

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        private static void InitConfigData()
        {
            //LogHelper.SystemInfo("正在初始化配置");

            _rpcRemoteIp = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("RpcRemoteIp", "127.0.0.1");
            _rpcRemotePort = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("RpcRemotePort", 10000);

            _rpcLocalIp = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetString("RpcLocalIp", "127.0.0.1");
            _rpcLocalPort = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("RpcLocalPort", 10001);


            _serverOrClient = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("ServerOrClient", 1);

            _heartbeatInterval = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("HeartbeatInterval", 30);

            //LogHelper.SystemInfo("初始化配置完成");
        }

        /// <summary>
        /// 初始化RPC
        /// </summary>
        private static void InitRpc()
        {
            //LogHelper.SystemInfo("正在初始化RPC进程通讯模块");
            GatewayManager.RpcManager = new RpcManager(_rpcRemoteIp, _rpcRemotePort, _rpcLocalIp, _rpcLocalPort);           
        }

        /// <summary>
        /// 正在初始化心跳模块
        /// </summary>
        private static void InitHeartbeat()
        {
            //LogHelper.SystemInfo("正在初始化心跳检测模块");

            _heartbeatManager = new HeartbeatManager(_heartbeatInterval);           
            _heartbeatManager.Start();
        }

        /// <summary>
        /// 初始化缓存 
        /// </summary>
        private static void InitCache()
        {
            //LogHelper.SystemInfo("正在初始化缓存模块");

            GatewayManager.CacheManager = new CacheManager();
            GatewayManager.CacheManager.Start();
        }

        /// <summary>
        /// 初始化通讯
        /// </summary>
        private static void InitCommunicationChannel()
        {
            GatewayManager.CommunicationManager = new CommunicationManager();
            GatewayManager.CommunicationManager.Start();
            //LogHelper.SystemInfo("启动网络通讯模块完成");
        }

        /// <summary>
        /// 初始化驱动
        /// </summary>
        private static void InitDrivers()
        {
            //LogHelper.SystemInfo("正在加载驱动模块");

            GatewayManager.DriverManager = new DriverManager();
            GatewayManager.DriverManager.Start();
        }

        /// <summary>
        /// Log4net日志自动清除功能
        /// </summary>
        private static void ClearLog4netLog()
        {
            string sondirsonDate = string.Empty;
            DateTime sondirsonDateTime = new DateTime();
            TimeSpan ts = new TimeSpan();
            string AutoClearLog4netLog = System.Configuration.ConfigurationManager.AppSettings["AutoClearLog4netLog"].ToString().ToLower();
            string Log4netFilePath = System.Configuration.ConfigurationManager.AppSettings["Log4netFilePath"].ToString();
            string ClearTimeLongAgo = System.Configuration.ConfigurationManager.AppSettings["ClearTimeLongAgo"].ToString();

            while (_isRun)
            {
                try
                {
                    //清除同步debug日志 
                    if (AutoClearLog4netLog == "true")
                    {
                        if (Directory.Exists(Log4netFilePath))
                        {
                            DirectoryInfo dir = new DirectoryInfo(Log4netFilePath);
                            DirectoryInfo[] dirs = dir.GetDirectories();
                            foreach (DirectoryInfo sondir in dirs)
                            {
                                DirectoryInfo[] sondirsons = sondir.GetDirectories();
                                foreach (DirectoryInfo sondirson in sondirsons)
                                {
                                    sondirsonDate = sondirson.Name.Substring(0, 4) + "-" + sondirson.Name.Substring(4, 2) + "-" + sondirson.Name.Substring(6, 2);
                                    sondirsonDateTime = DateTime.Parse(sondirsonDate);
                                    ts = DateTime.Now - sondirsonDateTime;
                                    if (ts.TotalDays > int.Parse(ClearTimeLongAgo))
                                    {
                                        Directory.Delete(sondirson.FullName, true);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogHelper.Error("清除mysql同步日志失败,详细信息" + ex.Message + ex.StackTrace);
                }
                Thread.Sleep(3600000);//每小时执行一次
            }
        }
    }

    /// <summary>
    /// 网关模块管理器
    /// </summary>
    public class GatewayManager
    {
        /// <summary>
        /// 和核心服务层 进程间Rpc通讯
        /// </summary>
        internal static RpcManager RpcManager { get; set; }
        /// <summary>
        /// /驱动管理器
        /// </summary>
        internal static DriverManager DriverManager { get; set; }
        /// <summary>
        /// 缓存管理器
        /// </summary>
        public static CacheManager CacheManager { get; set; }        
        /// <summary>
        /// 数据通讯管理器
        /// </summary>
        internal static CommunicationManager CommunicationManager { get; set; }
    }

    /// <summary>
    /// 网关映射关系管理器   
    /// </summary>
    public class GatewayMapper
    {
        //todo 待具体实现按配置加载
        static Dictionary<string, string> _dicMapper = new Dictionary<string, string>();

        public static void LoadMapper()
        {
            //todo 待实现加载具体的映射管理器
        }

        /// <summary>
        /// 根据驱动编号获取通讯模块编号
        /// </summary>
        /// <param name="driverCode">驱动编号</param>
        /// <returns>通讯模块编号</returns>
        public static string GetCommunicationCode(int CommPort)
        {
            string ret="SC001";
            switch (CommPort)
            {
                case 1:
                    ret = "SC001";
                    break;
                case 2:
                    ret = "SC002";
                    break;
                case 3:
                    ret = "SC003";
                    break;
                case 4:
                    ret = "SC004";
                    break;
            }
            if(GatewayService._serverOrClient==1)
            {
                ret = "CS001";
            }
            return ret;
        }

        /// <summary>
        /// 根据通讯模块编码获取驱动编号
        /// </summary>
        /// <param name="CommunicationCode">通讯模块编号</param>
        /// <returns>驱动编号</returns>
        public static string GetDriverCode(string CommunicationCode)
        {
            //todo 待实现
            return "DC001";
        }

        /// <summary>
        /// 根据设备编号获取驱动编号
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <returns>驱动编号</returns>
        public static string GetDriverCodeByDeviceCode(string deviceCode)
        {
            //todo 待实现
            return "DC001";
        }

    }


    public class AnalogTest
    {

        public static void DoAnalogTest()
        {
            Thread t = new Thread(new ThreadStart(DoTest));
            t.IsBackground = true;
            t.Start();
        }

        public static void DoTest()
        {
            string testFilePath = AppDomain.CurrentDomain.BaseDirectory + "/AnalogTestData.txt";
            if (!File.Exists(testFilePath))
            {
                return;
            }

            var dataLines = File.ReadAllLines(testFilePath);
            while (true)
            {
                for (int i = 0; i < dataLines.Length; i = i + 2)
                {
                    try
                    {
                        string mac = dataLines[i].Replace("-", ".").Trim();
                        byte[] data = StringToByte(dataLines[i + 1]);

                        GatewayManager.DriverManager.HandleNetData("DC001", mac, data);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        Thread.Sleep(300);
                    } 
                }
                
               // Thread.Sleep(5000);
            }
        }

        public static byte[] StringToByte(string InString)
        {
            InString = InString.Trim();
            if (InString.LastIndexOf("-") == InString.Length-1)
            {
                InString = InString.Remove(InString.Length - 1);
            }
            string[] ByteStrings;
            ByteStrings = InString.Split("-".ToCharArray());
            byte[] ByteOut;
            ByteOut = new byte[ByteStrings.Length];
            for (int i = 0; i <= ByteStrings.Length - 1; i++)
            {
                ByteOut[i] = Convert.ToByte(ByteStrings[i], 16);   /*Convert.ToByte(("0x" + ByteStrings[i]));*/
            }
            return ByteOut;
        }
    }
   


}
