using Basic.Framework.Logging;
using Sys.DataCollection.Common.Driver;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Services;
using DC.Communication.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Communications.Provider
{
    /// <summary>
    /// C8962通讯模块
    /// </summary>
    public class C8962Communication : ICommunication
    {
        Dictionary<string, long> _macDic;
        SocketTCPServer _c8962Server;

        public event NetDataArrivedEventHandler OnNetDataArrived;
        public event CommunicationStateChangeEventHandler OnCommunicationStateChange;
        public string CommunicationCode { get; set; }
        public C8962Communication()
        {
            _macDic = new Dictionary<string, long>();

            _c8962Server = new SocketTCPServer();
            _c8962Server.OnAccept += OnAccept;
            _c8962Server.OnConnectClose += OnConnectClose;
            _c8962Server.OnDataArrive += OnDataArrive;
            _c8962Server.OnError += OnError;
        }

        /// <summary>
        /// 接收新连接事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAccept(object sender, NetEventArgs e)
        {
            if (_macDic.ContainsKey(e.IP))
            {
                _macDic[e.IP] = e.ConnectID;
            }
            else
            {
                _macDic.Add(e.IP, e.ConnectID);
            }
            if (this.OnCommunicationStateChange != null)
            {
                OnCommunicationStateChange(sender, new CommunicationStateChangeArgs() { CommunicationCode = this.CommunicationCode, CommunicationState = CommunicationState.Connect, RemoteIp = e.IP, RemotePort = e.Port, UniqueCode = e.IP, ConntecionId = e.ConnectID });
            }

            LogHelper.Debug(string.Format("建立新的网络连接 ConnectionId{0} MAC:{1} IP:{2} 端口号:{3} ", e.ConnectID, e.MAC, e.IP, e.Port));
        }

        /// <summary>
        /// 连接断开处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectClose(object sender, NetEventArgs e)
        {
            if (_macDic.ContainsKey(e.IP))
            {
                if (e.ConnectID == _macDic[e.IP])
                    _macDic.Remove(e.IP);
                else
                {
                    LogHelper.Debug(string.Format("设备网络连接断开未执行 IP:{0} OLD:{1} NEW:{2} ",   e.IP, e.ConnectID, _macDic[e.IP]));
                    return;
                }
            }

            if (this.OnCommunicationStateChange != null)
            {
                OnCommunicationStateChange(sender, new CommunicationStateChangeArgs() { CommunicationCode = this.CommunicationCode, CommunicationState = CommunicationState.Disconnect, RemoteIp = e.IP, RemotePort = e.Port, UniqueCode = e.IP, ConntecionId = e.ConnectID });
            }

            LogHelper.Debug(string.Format("设备网络连接断开 ConnectionId{0} MAC:{1} IP:{2} 端口号:{3} ", e.ConnectID, e.MAC, e.IP, e.Port));
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
                    string errorString = string.Format("OnNetDataArrived 出错，UniqueCode：{0} RemoteIp{1} Data:{2} \nERROR：{3}", e.MAC, e.RemoteIp, ByteToString(e.Data), ex.ToString());
                    LogHelper.Error(errorString);
                }
            }
        }
        private string ByteToString(byte[] InBytes)
        {
            if (InBytes == null || InBytes.Length == 0)
            {
                return "";
            }

            string StringOut = "";
            foreach (byte InByte in InBytes)
            {
                StringOut = StringOut + String.Format("{0:X2}-", InByte);
            }
            if (StringOut.Contains("-"))
            {
                StringOut = StringOut.Substring(0, StringOut.Length - 1);
            }
            return StringOut;
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
            if (!_macDic.ContainsKey(target))
            {
                return false;
            }

            long connectId = _macDic[target];
            _c8962Server.Send(connectId, data, data.Length);
            return true;
        }

        /// <summary>
        /// 启动8962网络模块
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Start(string ip, int port)
        {
            _c8962Server.LocalIP = ip;
            _c8962Server.LocalPort = port;
            _c8962Server.Listen();

            return true;
        }

        /// <summary>
        /// 停止8962网络模块
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            _c8962Server.StopListen();
            _c8962Server.CloseAll();
            return true;
        }
        public void ConnetionServer(List<ClientConntion> lstConnetion)
        {
        }
    }

}
