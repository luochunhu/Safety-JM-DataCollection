using Basic.Framework.Logging;
using DC.Communication.Components;
using Sys.DataCollection.Common.Driver;
using Sys.DataCollection.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Services.Communications.Provider.C8962Provider
{
    public class SocketTcpClientCommunication: ICommunication
    {
        Dictionary<string, long> _macDic;
        SocketTCPClient _socketClient;

        public event NetDataArrivedEventHandler OnNetDataArrived;
        public event CommunicationStateChangeEventHandler OnCommunicationStateChange;
        public string CommunicationCode { get; set; }
        public SocketTcpClientCommunication()
        {
            _macDic = new Dictionary<string, long>();

            _socketClient = new SocketTCPClient();
            _socketClient.OnAccept += OnAccept;
            _socketClient.OnConnectClose += OnConnectClose;
            _socketClient.OnDataArrive += OnDataArrive;
            _socketClient.OnError += OnError;
        }

        /// <summary>
        /// 接收新连接事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAccept(object sender, NetEventArgs e)
        {
            string str1 = e.IP;
            if (_macDic.ContainsKey(str1))
            {
                _macDic[str1] = e.ConnectID;
            }
            else
            {
                _macDic.Add(str1, e.ConnectID);
            }
            if (this.OnCommunicationStateChange != null)
            {
                OnCommunicationStateChange(sender, new CommunicationStateChangeArgs() { CommunicationCode = this.CommunicationCode, CommunicationState = CommunicationState.Connect, RemoteIp = e.IP, RemotePort = e.Port, UniqueCode = e.IP, ConntecionId = e.ConnectID });
            }

            LogHelper.Debug(string.Format("建立新的网络连接 TcpClient ConnectionId{0} MAC:{1} IP:{2} 端口号:{3} ", e.ConnectID, e.MAC, e.IP, e.Port));
        }

        /// <summary>
        /// 连接断开处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectClose(object sender, NetEventArgs e)
        {
            //if (_macDic.ContainsKey(e.IP))
            //{
            //    if (e.ConnectID == _macDic[e.IP])
            //        _macDic.Remove(e.IP);
            //    else
            //    {
            //        LogHelper.Debug(string.Format("设备网络连接断开未执行 IP:{0} OLD:{1} NEW:{2} ", e.IP, e.ConnectID, _macDic[e.IP]));
            //        return;
            //    }
            //}
            try
            {
                string str1 = e.IP;
                if (_macDic.ContainsKey(str1))
                {
                    if (e.ConnectID == _macDic[str1])
                        _macDic.Remove(str1);
                    else
                    {
                        LogHelper.Debug(string.Format("设备网络连接断开未执行 IP:{0} OLD:{1} NEW:{2} ", e.IP, e.ConnectID, _macDic[str1]));
                        return;
                    }
                }
                if (this.OnCommunicationStateChange != null)
                {
                    OnCommunicationStateChange(sender, new CommunicationStateChangeArgs() { CommunicationCode = this.CommunicationCode, CommunicationState = CommunicationState.Disconnect, RemoteIp = e.IP, RemotePort = e.Port, UniqueCode = e.IP, ConntecionId = e.ConnectID });
                }

                LogHelper.Debug(string.Format("TcpClient 设备网络连接断开 ConnectionId{0} MAC:{1} IP:{2} 端口号:{3} ", e.ConnectID, e.MAC, e.IP, e.Port));
            }
            catch
            {

            }
        }
        /// <summary>
        /// 收到数据包处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataArrive(object sender, DataArriveEventArgs e)
        {
            if (this.OnNetDataArrived != null)
            {
                var data = e.Data;
                try
                {
                    OnNetDataArrived(sender, new NetDataArrivedEventArgs { CommunicationCode = this.CommunicationCode, Data = data, UniqueCode = e.RemoteIp, RemoteIp = e.RemoteIp, RemotePort = e.Port });
                }
                catch (Exception ex)
                {
                    string errorString = string.Format("TcpClient OnNetDataArrived 出错，UniqueCode：{0} RemoteIp{1} Data:{2} \nERROR：{3}", e.MAC, e.RemoteIp,BitConverter.ToString(e.Data), ex.ToString());
                    LogHelper.Error(errorString);
                }
            }
        }
        private void OnError(object sender, NetEventArgs e)
        {
            //todo write log
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="target"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Send(string target, byte[] data)
        {
            try
            {
                if (!_macDic.ContainsKey(target))
                {
                    return false;
                }

                long connectId = _macDic[target];
                _socketClient.Send(connectId, data, data.Length);
            }
            catch
            {

            }
            return true;
        }

        /// <summary>
        /// 启动TcpClient网络模块
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Start(string ip, int port)
        {
            return true;
        }

        /// <summary>
        /// 停止TcpClient网络模块
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            _socketClient.CloseAll();
            return true;
        }
        public void ConnetionServer(List<ClientConntion> lstConnetion)
        {
            _socketClient.ConnetionServer(lstConnetion);
        }
    }
}
