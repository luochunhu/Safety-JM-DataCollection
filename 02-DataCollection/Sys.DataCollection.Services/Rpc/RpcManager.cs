using Basic.Framework.Logging;
using Basic.Framework.Rpc;
using Sys.DataCollection.BusinessManager;
using Sys.DataCollection.Cache;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Rpc;
using Sys.DataCollection.Communications.Provider;
using Sys.DataCollection.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Rpc
{
    //public delegate void DeviceMessageArrivedEventHandler(MasProtocol masProtocol);

    /// <summary>
    /// RPC进程通讯管理器
    /// </summary>
    public class RpcManager
    {
        IRpcClient _client;
        IRpcServer _server;
        string _remoteIp = "127.0.0.1";//远程RPC服务器IP
        int _remotePort = 10000;//远程RPC服务器端口号

        public string _localIp = "127.0.0.1";//自己做为RPC服务器的IP
        public int _localPort = 10001;//自己做为RPC服务器的端口号

        RpcModel _rpcModel = RpcModel.WebApiModel;

        /// <summary>
        /// 初始化RPC管理器
        /// </summary>
        /// <param name="remoteIp">远程RPC服务器IP</param>
        /// <param name="remotePort">远程RPC服务器端口号</param>
        /// <param name="localIp">自己做为RPC服务器的IP</param>
        /// <param name="localPort">自己做为RPC服务器的端口号</param>
        public RpcManager(string remoteIp, int remotePort, string localIp, int localPort)
        {
            _remoteIp = remoteIp;
            _remotePort = remotePort;
            _localIp = localIp;
            _localPort = localPort;

            System.Reflection.Assembly.Load("Sys.DataCollection.WebApi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            int rpcModel = Basic.Framework.Configuration.ConfigurationManager.FileConfiguration.GetInt("RpcModel", 1);
            if (rpcModel == 1)
            {
                _rpcModel = RpcModel.WebApiModel;
            }
            else if (rpcModel == 2)
            {
                _rpcModel = RpcModel.gRPCModel;
            }

            _client = RpcFactory.CreateRpcClient(_rpcModel, _remoteIp, _remotePort);
            _server = RpcFactory.CreateRpcServer(_rpcModel);
            _server.RegistCallback(HandleRpcMessage);
        }

        /// <summary>
        /// 收到RPC消息处理
        /// </summary>
        /// <param name="rpcRequest"></param>
        /// <returns></returns>
        private RpcResponse HandleRpcMessage(RpcRequest rpcRequest)
        {
            if (rpcRequest.RequestType == (int)RequestType.DeviceRequest)
            {
                //设备类请求，直接交给驱动处理
                return HandleDeviceRequest(rpcRequest);
            }
            else if (rpcRequest.RequestType == (int)RequestType.BusinessRequest)
            {
                //业务类请求，网关直接处理 如心跳、获取业务数据或者交互处理 等
                return HandleBusinessRequest(rpcRequest);
            }
            else if (rpcRequest.RequestType == (int)RequestType.DeviceUdpRequest)
            {
                /// 设备（UDP）类请求
                /// 这里是kj73n特殊的一类命令，主要用于搜索网络设备及网络参数设置               
                var masProtocol = rpcRequest.ToRequest<GatewayRpcRequest>().MasProtocol;
                return C8962UdpCommunication.HandleDeviceUdpRequest(masProtocol);
            }

            return null;
        }

        /// <summary>
        /// 设备类请求处理
        /// </summary>
        /// <param name="rpcRequest"></param>
        /// <returns></returns>
        private RpcResponse HandleDeviceRequest(RpcRequest rpcRequest)
        {
            var masProtocol = rpcRequest.ToRequest<GatewayRpcRequest>().MasProtocol;
            //调用驱动 处理业务 
            DeviceProtocol deviceProtocol = masProtocol.Deserialize<DeviceProtocol>();
            string driverCode = GatewayMapper.GetDriverCodeByDeviceCode(deviceProtocol.DeviceCode);

            GatewayManager.DriverManager.HandleProtocolData(driverCode, masProtocol);

            return RpcResponse.Response<GatewayRpcResponse>(new GatewayRpcResponse());
        }

        /// <summary>
        /// 业务类请求处理
        /// </summary>
        /// <param name="rpcRequest"></param>
        /// <returns></returns>
        private RpcResponse HandleBusinessRequest(RpcRequest rpcRequest)
        {
            var masProtocol = rpcRequest.ToRequest<GatewayRpcRequest>().MasProtocol;
            //处理业务
            switch (masProtocol.ProtocolType)
            {
                case ProtocolType.UpdateCacheDataRequest://安全监控、人员定位缓存同步
                    GatewayManager.CacheManager.HandleUpdateCacheDataRequest(masProtocol);
                    break;
                //新增广播系统服务端业务请求处理  20171229
                case ProtocolType.EndPaTaskRequest://结束广播任务                    
                    return BroadCastBusinessManager.endPaTask(masProtocol);                   
                case ProtocolType.HangupCallRequest://挂断呼叫
                    return BroadCastBusinessManager.hangupCall(masProtocol);                     
                case ProtocolType.MakeCallRequest://发起呼叫
                    return BroadCastBusinessManager.makeCall(masProtocol); 
                case ProtocolType.MonitorCallRequest://监听呼叫
                    return BroadCastBusinessManager.monitorCall(masProtocol);
                case ProtocolType.MusicControlRequest://音乐管理
                    return BroadCastBusinessManager.musicManage(masProtocol);
                case ProtocolType.PartitionControlRequest://分区管理
                    return BroadCastBusinessManager.zoneManage(masProtocol);
                case ProtocolType.PlayListControlRequest://播放列表管理
                    return BroadCastBusinessManager.playListManage(masProtocol);
                case ProtocolType.PlayListMusicControlRequest://播放列表音乐管理
                    return BroadCastBusinessManager.playListMusicManage(masProtocol);
                case ProtocolType.StartPaMusicTaskRequest://启动音乐广播任务
                    return BroadCastBusinessManager.startPaMusicTask(masProtocol);
                case ProtocolType.StartPaTtsTaskRequest://启动文字广播任务
                    return BroadCastBusinessManager.startPaTtsTask(masProtocol);
                case ProtocolType.StartPaVoiceTaskRequest://启动喊话广播任务
                    return BroadCastBusinessManager.startPaVoiceTask(masProtocol);
                case ProtocolType.TerminalControlRequest://终端管理
                    return BroadCastBusinessManager.termManage(masProtocol);
                case ProtocolType.QureyTerminalRequest://查询终端
                    return BroadCastBusinessManager.queryTerm(masProtocol);
            }

            return RpcResponse.Response<GatewayRpcResponse>(new GatewayRpcResponse());
        }


        /// <summary>
        /// 启动RPC服务器
        /// </summary>
        /// <returns></returns>
        public bool StartRpcServer()
        {
            _server.Start(_localIp, _localPort);
            //LogHelper.Debug("启动RPC服务器成功");
            return true;
        }

        /// <summary>
        /// 停止RPC服务器
        /// </summary>
        /// <returns></returns>
        public bool StopRpcServer()
        {
            _server.Stop();
            LogHelper.Debug("停止RPC服务器成功");
            return true;
        }

        /// <summary>
        /// 发送数据到远程服务器
        /// </summary>
        /// <typeparam name="TResult">返回的结果类型</typeparam>
        /// <param name="masProtocol">待发送的协议</param>
        /// <param name="requestType">请求的类型</param>
        /// <returns>调用结果</returns>
        public TResult Send<TResult>(MasProtocol masProtocol, RequestType requestType)
        {
            GatewayRpcRequest request = new GatewayRpcRequest(requestType);
            request.MasProtocol = masProtocol;
            var response = _client.Send<GatewayRpcRequest, TResult>(request);
            if (response.IsSuccess)
            {
                //todo
            }
            return response.Data;
        }
    }
}
