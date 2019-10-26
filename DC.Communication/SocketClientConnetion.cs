using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DC.Communication.Components
{

    /// <summary>
    /// 用于本机为客户端模式的时候，连接远程设备的服务端
    /// </summary>
    public class SocketClientConnetion
    {
        private Socket _client;
        private string _IP;
        private string _Port;
        private static long _lastSocketID = 0; //0xf4240;

        public event NewSocketEventHandler OnNewSocketAccept;

        /// <summary>
        /// 获取一个新连接号
        /// </summary>
        private static long NextSocketID
        {
            get
            {
                return Interlocked.Increment(ref _lastSocketID);
            }
        }
        /// <summary>
        /// 获取和设置起始的连接号，一般不需要设置
        /// </summary>
        public static long StartSocketID
        {
            get { return _lastSocketID; }
            set { _lastSocketID = value; }
        }
        
        public bool ClientConnetion(string _ip,string _port)
        {
            bool result = false;

            IPEndPoint DdeServerIp = new IPEndPoint(IPAddress.Parse(_ip), Convert.ToInt32(_port));
            _IP = _ip;
            _Port = _port;
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {

                //_client.SendTimeout = 6000;

                //_client.ReceiveTimeout = 6000;

                _client.SendBufferSize = 1024;

                _client.Connect(DdeServerIp);

                Basic.Framework.Logging.LogHelper.Info(string.Format(" socket log: IP【{0}】-Port【{1}】连接成功！", _ip, _port));

                if (OnNewSocketAccept != null)
                {
                    OnNewSocketAccept(NextSocketID, _client);
                }
                
                result = true;

            }
            catch (Exception ex)
            {
                Basic.Framework.Logging.LogHelper.Error(string.Format(" socket log: IP【{0}】-Port【{1}】连接失败！【{2}】！", _ip, _port, ex.Message));
            }

            return result;
        }
    }
}
