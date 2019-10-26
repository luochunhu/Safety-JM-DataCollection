using Basic.Framework.Logging;
using Basic.Framework.Rpc;
using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Driver;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Rpc;
using Sys.DataCollection.Rpc;
using Sys.DataCollection.Services;
using Sys.DataCollection.WebApiAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DC.Communication.Components;

namespace Sys.DataCollection.Communications.Provider
{
    /// <summary>
    /// Http通讯处理类(目前主要用于广播系统)
    /// </summary>
    public class HttpCommunication : ICommunication
    {
        bool _isRun = false;
        
        /// <summary>
        /// 定时刷新登录线程
        /// </summary>
        private Thread refreshThread;
        /// <summary>
        /// 最后刷新登录时间
        /// </summary>
        private DateTime lastRefreshTime;
        /// <summary>
        /// 刷新时间，在此时间内需调用服务器刷新接口进行刷新（单位：秒）
        /// </summary>
        private int refreshTick;
        /// <summary>
        /// 广播服务器登录状态
        /// </summary>
        private bool loginState = false;
        /// <summary>
        /// 监控广播事件的WebApi Url地址
        /// </summary>
        private string callbackUrl;

        public string CommunicationCode
        {
            get;
            set;
        }

        public event NetDataArrivedEventHandler OnNetDataArrived;

        public event CommunicationStateChangeEventHandler OnCommunicationStateChange;

        public bool Start(string ip, int port)
        {
            //启动
            _isRun = true;

            //生成回调url，取的是网关的RpcLocalIp,RpcLocalPort为Webapi的监听ip,端口
            callbackUrl = string.Format("http://{0}:{1}/", GatewayManager.RpcManager._localIp, GatewayManager.RpcManager._localPort);

            LogHelper.Debug("启动广播系统Http远程连接");

            //登录广播服务器
            Login();
            //开启刷新线程
            refreshThread = new Thread(RefLoginStateThread);
            refreshThread.Start();

            //注册终端状态事件
            Sys.DataCollection.WebApi.BroadCastUpEvent.OnBraodCastProtocolDataCreated += Communication_BroadCastProtocolDataArrived;

            return true;
        }
        /// <summary>
        /// 广播系统终端状态事件（用于接收广播系统服务器发过来的状态变动数据）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Communication_BroadCastProtocolDataArrived(object sender, ProtocolDataCreatedEventArgs args)
        {
            //将数据通过Rpc发送到核心服务层
            var response = GatewayManager.RpcManager.Send<GatewayRpcResponse>(args.MasProtocol, RequestType.DeviceRequest);
            if (response != null && response.IsSuccess)
            {

            }
        }
        /// <summary>
        /// 登录广播服务器
        /// </summary>
        private void Login()
        {
            try
            {
                LoginRequest loginrequest = new LoginRequest();
                loginrequest.id = "admin";
                loginrequest.secret = Basic.Framework.Common.MD5Helper.MD5Encrypt("admin");
                loginrequest.callbackUrl = callbackUrl;
                LoginResponse response =BroadCastControllerProxy.broadCastControllerProxy.Login(loginrequest);
                if (response.retCode == "0")
                {
                    lastRefreshTime = DateTime.Now;
                    BroadCastControllerProxy.broadCastControllerProxy.Token = response.accessToken;//获取令牌
                    loginState = true;
                    int.TryParse(response.expires, out refreshTick);
                    if (refreshTick < 1)
                    {
                        refreshTick = 7200;//默认2小时刷新一次
                    }
                    LogHelper.Debug("Http远程登录广播系统服务器成功");
                }
                else
                {
                    loginState = false;
                    LogHelper.Error("广播系统远程登录接口调用异常，错误码:" + response.retCode);
                }
            }
            catch (Exception ex)
            {
                loginState = false;
                LogHelper.Error("登录广播系统Http远程服务器异常," + ex);
            }
        }
        private void Refresh()
        {
            try
            {
                RefreshRequest refreshRequest = new RefreshRequest();
                refreshRequest.id = "admin";
                RefreshResponse response = BroadCastControllerProxy.broadCastControllerProxy.Refresh(refreshRequest);
                if (response.retCode == "0")
                {
                    lastRefreshTime = DateTime.Now;
                    BroadCastControllerProxy.broadCastControllerProxy.Token = response.accessToken;//获取令牌
                    int.TryParse(response.expires, out refreshTick);
                    if (refreshTick < 1)
                    {
                        refreshTick = 7200;//默认2小时刷新一次
                    }
                    LogHelper.Debug("Http远程刷新登录状态成功");
                }
                else
                {
                    LogHelper.Error("广播系统远程刷新接口调用异常，错误码:" + response.retCode);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("刷新广播系统Http远程服务器异常," + ex);
            }
        }
        private void RefLoginStateThread()
        {
            TimeSpan ts;
            while (_isRun)
            {
                try
                {
                    if (!loginState)
                    {//登录异常，重新进行登录
                        Login();
                    }
                    ts = DateTime.Now - lastRefreshTime;
                    if (ts.TotalMinutes >= refreshTick * 0.8)//提前20时间进行连接，以保证在规定时间范围内刷新
                    {
                        //调用刷新接口
                        if (loginState)
                        {
                            Refresh();
                        }
                        lastRefreshTime = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {

                }
                Thread.Sleep(10000);
            }
        }

        public bool Stop()
        {
            //结束
            _isRun = false;

            return true;
        }

        public bool Send(string target, byte[] data)
        {
            //暂不支持

            return false;
        }

        public void ConnetionServer(List<DC.Communication.Components.ClientConntion> lstConnetion)
        {
            throw new NotImplementedException();
        }
    }
}
