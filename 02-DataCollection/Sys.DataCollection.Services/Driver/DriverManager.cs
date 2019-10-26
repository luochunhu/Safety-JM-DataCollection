using Basic.Framework.Logging;
using DC.Communication.Components;
using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Driver;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Rpc;
using Sys.DataCollection.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver
{
    /// <summary>
    /// 驱动管理器
    /// </summary>
    public class DriverManager
    {
        //驱动缓存
        Dictionary<string, IDeviceDriver> _driverCache = new Dictionary<string, IDeviceDriver>();

        //public Dictionary<string, IDeviceDriver> DriverCache
        //{
        //    get
        //    {
        //        return _driverCache;
        //    }
        //}

        /// <summary>
        /// 启动驱动模块
        /// </summary>
        public void Start()
        {
            LoadDrivers();
            InitDrivers();
        }

        /// <summary>
        /// 停止驱动模块
        /// </summary>
        public void Stop()
        {
            _driverCache.Clear();
        }


        /// <summary>
        /// 初始化驱动
        /// </summary>
        private void LoadDrivers()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            var files = Directory.GetFiles(path, "Sys.DataCollection.Drivers.*.dll", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var assembly = Assembly.Load(File.ReadAllBytes(file));
                var driverTypes = assembly.GetTypes().Where(type => type.GetInterface("IDeviceDriver") != null).ToList();
                if (driverTypes == null || driverTypes.Count <= 0)
                {
                    continue;
                }

                IDeviceDriver deviceDriver = Activator.CreateInstance(driverTypes[0]) as IDeviceDriver;
                if (deviceDriver == null)
                {
                    //todo write log
                    continue;
                }
                else
                {
                    string deviceCode = deviceDriver.DriverCode;
                    if (_driverCache.ContainsKey(deviceCode))
                    {
                        _driverCache[deviceCode] = deviceDriver;
                        //todo 待确认驱动编码重复的处理
                    }
                    else
                    {
                        _driverCache.Add(deviceCode, deviceDriver);
                    }
                }             
            }
        }

        /// <summary>
        /// 初始化驱动
        /// </summary>
        private void InitDrivers()
        {            
            foreach (var driver in _driverCache.Values)
            {
                driver.CacheManager = GatewayManager.CacheManager;
                driver.OnNetDataCreated += Driver_OnNetDataCreated;
                driver.OnProtocolDataCreated += Driver_OnProtocolDataCreated;
                driver.OnExcuteDriverCommand += Driver_OnExcuteDriverCommand;
            }
        }

        /// <summary>
        /// 处理驱动命令事件（特殊处理）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool Driver_OnExcuteDriverCommand(object sender, DriverCommandEventArgs args)
        {
            if (args.CommandType == 1)
            {
                DeviceResetCommand resetCommand = Basic.Framework.Common.JSONHelper.ParseJSONString<DeviceResetCommand>(args.JsonData);
                return Sys.DataCollection.Communications.Provider.C8962UdpCommunication.ResetDevice(resetCommand.Mac, resetCommand.Timeout);
            }
            else if (args.CommandType == 2)
            {//20181013
                List<ClientConntion> connCommand = Basic.Framework.Common.JSONHelper.ParseJSONString<List<ClientConntion>>(args.JsonData);
                GatewayManager.CommunicationManager.ConnetionServer("CS001",connCommand);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 处理向上的数据事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Driver_OnProtocolDataCreated(object sender, ProtocolDataCreatedEventArgs args)
        {
            var response = GatewayManager.RpcManager.Send<GatewayRpcResponse>(args.MasProtocol, RequestType.DeviceRequest);
            if (response != null && response.IsSuccess)
            {

            }
        }

        /// <summary>
        /// 处理向下的数据事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Driver_OnNetDataCreated(object sender, NetDataEventCreatedArgs args)
        {
            string communicationCode = GatewayMapper.GetCommunicationCode(args.CommPort);

            GatewayManager.CommunicationManager.Send(communicationCode, args.DeviceCode, args.Data);
        }

        /// <summary>
        /// 调用驱动处理 核心服务层到驱动的命令
        /// </summary>
        /// <param name="driverCode"></param>
        /// <param name="masProtocol"></param>
        public void HandleProtocolData(string driverCode, MasProtocol masProtocol)
        {
            if (!_driverCache.ContainsKey(driverCode))
            {
                LogHelper.Error("DriverManager.HandleProtocolData() 错误，driverCode：" + driverCode + " 驱动编号不存在");
                return;
            }

            _driverCache[driverCode].HandleProtocolData(masProtocol);
        }

        /// <summary>
        /// 调用驱动处理业务包
        /// </summary>
        /// <param name="driverCode"></param>
        /// <param name="uniqueCode"></param>
        /// <param name="data"></param>
        public void HandleNetData(string driverCode, string uniqueCode, byte[] data)
        {
            if (!_driverCache.ContainsKey(driverCode))
            {
                LogHelper.Error("DriverManager.HandleNetData() 错误，driverCode：" + driverCode + " 驱动编号不存在");
                return;
            }

            _driverCache[driverCode].HandleNetData(data, uniqueCode);
        }

        /// <summary>
        /// 处理网关
        /// </summary>
        /// <param name="stateChangeArgs">通讯状态变化参数</param>
        public void HandleCommunicationStateChange(string driverCode, CommunicationStateChangeArgs stateChangeArgs)
        {
            if (!_driverCache.ContainsKey(driverCode))
            {
                LogHelper.Error("DriverManager.HandleNetData() 错误，driverCode：" + driverCode + " 驱动编号不存在");
                return;
            }

            _driverCache[driverCode].CommunicationStateChangeNotify(stateChangeArgs);
        }


    }
}
