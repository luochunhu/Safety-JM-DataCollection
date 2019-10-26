using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace DC.Communication.Components
{
     
    /// <summary>
    /// TCP 服务管理类
    /// </summary>
    public class SocketTCPServer
    {
        //读写锁
        private ReaderWriterLock _rwLock;
        //SOCKET缓存
        private Dictionary<long, SocketTCPHandler> _socketsCache;
        //TCP监听器
        SocketListener _socketListener;
        //心跳检查定时器
        System.Threading.Timer timer = null;

        //心跳检测时间，单位秒
       int _heartbeatInterval = 10;

        #region 对外事件

        /// <summary>
        /// 新连接事件
        /// </summary>
        public event NetEventHandler OnAccept;
        
        /// <summary>
        /// 关闭连接事件
        /// </summary>
        public event NetEventHandler OnConnectClose;

        /// <summary>
        /// 发生错误事件
        /// </summary>
        public event NetEventHandler OnError;

        /// <summary>
        /// 数据到达事件
        /// </summary>
        public event DataArriveEventHandler OnDataArrive;

        #endregion

        #region 属性
        /// <summary>
        /// 服务端监听端口号
        /// </summary>
        public int LocalPort
        { get; set; }

        /// <summary>
        /// 服务端IP
        /// </summary>
        public string LocalIP
        { get; set; }

        #endregion

        /// <summary>
        /// TCP服务器构造函数
        /// </summary>
        /// <param name="heartbeatPeriod">心跳检测时间; 单位秒,默认10秒</param>
        public SocketTCPServer(int heartbeatInterval = 10)
        {
            _heartbeatInterval = heartbeatInterval;
            _rwLock = new ReaderWriterLock();
            _socketsCache = new Dictionary<long, SocketTCPHandler>();

            _socketListener = new SocketListener();
            _socketListener.OnNewSocketAccept += new NewSocketEventHandler(_socketListener_OnNewSocketAccept);

           // timer = new System.Threading.Timer(new TimerCallback(AutoCheckHeartbeat), null, 1000 * _heartbeatInterval * 1, 1000 * _heartbeatInterval * 1);
        }

        /// <summary>
        /// 打开监听端口
        /// </summary>
        /// <returns></returns>
        public bool Listen()
        {
            return _socketListener.Start(LocalPort);
        }

        /// <summary>
        /// 停止监听，停止后，不再接收新的TCP连接
        /// </summary>
        /// <returns></returns>
        public bool StopListen()
        {
            bool result = false;
            try
            {
                _socketListener.StopListen();
                result = true;
            }
            catch 
            {
                //todo write log
            }
            
            return result;
        }        

        /// <summary>
        /// 关闭指定连接
        /// </summary>
        /// <param name="connectID">连接号</param>
        /// <returns></returns>
        public bool CloseConnect(long connectID)
        {
            bool result = false;

            SocketTCPHandler tcpHandler = null;

            _rwLock.AcquireReaderLock(-1);

            try
            {
                if (_socketsCache.ContainsKey(connectID))
                {
                    tcpHandler = _socketsCache[connectID];                   
                }
            }           
            finally
            {
                _rwLock.ReleaseReaderLock();//获取集合中的对象后，立即释放锁，不然会在后面的CLOSE事件里引起死锁
            }

            try
            {
                if (tcpHandler != null)
                {
                    tcpHandler.Disconnect();
                    result = true;
                }
            }
            catch 
            {
            }

            return result;
        }
       
             
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connectID">连接号</param>
        /// <param name="data">数据包</param>
        /// <param name="dataLength">长度</param>
        /// <returns></returns>
        public bool Send(long connectID,  byte[] data, long dataLength)
        {
            bool result = false;

            SocketTCPHandler handler = GetHandlerById(connectID);
            if (handler != null)
            {
                try
                {
                    handler.Send(data);
                    result = true;
                }
                catch { };
            }

            return result;
        }

        /// <summary>
        /// 根据连接号获取连接MAC
        /// </summary>
        /// <param name="connectID">连接号</param>
        /// <returns></returns>
        public string GetMAC(long connectID)
        {
            string result = "";

            _rwLock.AcquireReaderLock(-1);

            try
            {
                if (_socketsCache.ContainsKey(connectID))
                {
                    result = _socketsCache[connectID].MAC;
                }
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }

            return result;
        }

        /// <summary>
        /// 根据连接号获取连接IP
        /// </summary>
        /// <param name="connectID">连接号</param>
        /// <returns></returns>
        public string GetPeerIP(long connectID)
        {
            string result = "";
            _rwLock.AcquireReaderLock(-1);

            try
            {
                if (_socketsCache.ContainsKey(connectID))
                {
                    result = _socketsCache[connectID].IP;
                }
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }

            return result;
        }

        /// <summary>
        /// 根据连接号获取连接端口
        /// </summary>
        /// <param name="connectID">连接号</param>
        /// <returns></returns>
        public long GetPeerPort(long connectID)
        {
            long result = 0;
            _rwLock.AcquireReaderLock(-1);

            try
            {
                if (_socketsCache.ContainsKey(connectID))
                {
                    result = _socketsCache[connectID].Port;
                }
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }

            return result;
        }

         /// <summary>
        /// 新的TCP连接处理
        /// </summary>
        /// <param name="socketId"></param>
        /// <param name="newSocket"></param>
        void _socketListener_OnNewSocketAccept(long socketId, Socket newSocket)
        {

            SocketTCPHandler socketHandler = new SocketTCPHandler(socketId, newSocket);
            bool result = socketHandler.ReceiveAuth();//建立连接后，第一次数据的连接效验  todo

            if (result == false)//如果效验不成功，则直接关闭连接
            {
                socketHandler.Disconnect();
                return;
            }

            _rwLock.AcquireWriterLock(-1);

            try
            {
                if (!_socketsCache.ContainsKey(socketId) )//添加到缓存
                {
                    //避免由于外面申请锁的过程中（可能时间较长或者其它原因），在此期间socket连接异常断开，此种情况则不把socket放入_socketsCache缓存
                    //（应该很出现这种情况）
                    if (socketHandler.IsConnected)
                    {
                        _socketsCache.Add(socketId, socketHandler);
                    }
                    else
                    {
                        //socket断开后，断开连接处理
                        socketHandler.Disconnect();
                        socketHandler = null;
                        return;
                    }               
                }
                else
                {
                    //socketId id 重复(理论上不可能出现)
                    //todo write error log
                }
            }
            catch
            {
                //如果在新增对象到缓存出错，则直接返回；  20171214
                return;
            }
            finally
            {
                _rwLock.ReleaseWriterLock();
            }

            socketHandler.OnConnectClose += socketHandler_OnConnectClose; //new NetEventHandler(socketHandler_OnConnectClose);//注册事件
            socketHandler.OnDataArrive += socketHandler_OnDataArrive;// new DataArriveEventHandler(socketHandler_OnDataArrive);

            if (this.OnAccept != null)
            {
                try
                {
                    //抛出新连接事件
                    this.OnAccept(this, new NetEventArgs(socketId, socketHandler.IP, socketHandler.MAC, socketHandler.Port, "新连接已经建立！"));
                }
                catch 
                {
                    //todo write log,
                }
            }
        }

        /// <summary>
        /// 数据到达处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void socketHandler_OnDataArrive(object sender, DataArriveEventArgs e)
        {
            if (this.OnDataArrive != null)
            {
                this.OnDataArrive(sender, e);
            }
        }

        /// <summary>
        /// SOCKET事件断开处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void socketHandler_OnConnectClose(object sender, NetEventArgs e)
        {
            _rwLock.AcquireWriterLock(-1);

            try
            {
                if (_socketsCache.ContainsKey(e.ConnectID))
                {
                    //断开连接后，注销此对象的订阅的事件
                    var socket = _socketsCache[e.ConnectID];                   
                    socket.OnConnectClose -= socketHandler_OnConnectClose;
                    socket.OnDataArrive -= socketHandler_OnDataArrive;
                    _socketsCache.Remove(e.ConnectID);
                    socket = null;
                }
            }
            catch { }
            finally
            {
                _rwLock.ReleaseWriterLock();
            }

            if (this.OnConnectClose != null)
            {
                try
                {
                    //抛出断开连接事件
                    this.OnConnectClose(sender, e);
                }
                catch 
                {
                    //todo write log
                }
            }

        }

    

        /// <summary>
        /// 关闭所有连接
        /// </summary>
        /// <returns></returns>
        public bool CloseAll()
        {
            _rwLock.AcquireReaderLock(-1);
            List<long> list;
            try
            {
                list = new List<long>(_socketsCache.Keys);
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }

            if (list == null || list.Count <= 0)
            {
                return true;
            }

            foreach (long socketId in list)
            {
                SocketTCPHandler handler = GetHandlerById(socketId);
                if (handler == null)
                {
                    continue;
                }
                try
                {
                    handler.Disconnect();
                }
                catch { }
            }

            return true;           
        }

        /// <summary>
        /// 根据SOCKET ID获取处理器
        /// </summary>
        /// <param name="socketId"></param>
        /// <returns></returns>
        private SocketTCPHandler GetHandlerById(long socketId)
        {
            SocketTCPHandler result = null;

            _rwLock.AcquireReaderLock(-1);

            try
            {
                if (_socketsCache.ContainsKey(socketId))
                {
                    result = _socketsCache[socketId];
                }
            }
            catch 
            {
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }

            return result;
        }
              

        /// <summary>
        /// 心跳检查
        /// </summary>
        /// <param name="state"></param>
        private void AutoCheckHeartbeat(object state)
        {
            _rwLock.AcquireReaderLock(-1);
            List<long> list;
            try
            {
                list = new List<long>(_socketsCache.Keys);
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }

            if (list == null || list.Count <= 0)
            {
                return;
            }

            foreach (long socketId in list)
            {
                SocketTCPHandler handler = GetHandlerById(socketId);
                if (handler == null)
                {
                    continue;
                }
                try
                {
                    var result = handler.CheckHeartbeat();
                    if(!result)
                    {
                        //如果发送心跳包时失败，直接调用对象的断开连接接口。
                        handler.Disconnect();
                    }
                }
                catch { }
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
    /// <summary>
    /// 数据到达事件
    /// </summary>
    public class DataArriveEventArgs : EventArgs
    {
        private byte[] _data;
        private int _length;       
        private long _connectID;

        private string _remoteIp;
        private string _mac;     
        private int _port;

        public DataArriveEventArgs()
        {
        }
        public DataArriveEventArgs(long connectID, byte[] data, int length, string mac, string remoteIp, int port)
        {
            _connectID = connectID;
            _data = data;
            _length = length;

            _mac = mac;
            _remoteIp = remoteIp;
            _port = port;
        }

        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public long ConnectID
        {
            get { return _connectID; }
            set { _connectID = value; }
        }

        public string RemoteIp
        {
            get { return _remoteIp; }
            set { _remoteIp = value; }
        }
        public string MAC
        {
            get { return _mac; }
            set { _mac = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

    }

    public class NetEventArgs : EventArgs
    {       
        private long _connectID;
        private string _ip;
        private string _mac;
        private string _message;
        private int _port;
       
        public NetEventArgs()
        {
        }

        public NetEventArgs(long connectId,string ip,string mac,int port, string msg)
        {
            _connectID = connectId;
            _ip = ip;
            _mac = mac;
            _port = port;

            _message = msg;
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public long ConnectID
        {
            get { return _connectID; }
            set { _connectID = value; }
        }

        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        public string MAC
        {
            get { return _mac; }
            set { _mac = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }       
    }

   

    public delegate void DataArriveEventHandler(object sender, DataArriveEventArgs e);
    public delegate void NetEventHandler(object sender, NetEventArgs e);
}
