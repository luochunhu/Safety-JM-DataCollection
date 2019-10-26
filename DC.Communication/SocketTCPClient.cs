using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Basic.Framework.Logging;

namespace DC.Communication.Components
{

    /// <summary>
    /// PC为客户端时候的管理操作类
    /// </summary>
    public class SocketTCPClient
    {
        /// <summary>
        /// 用于解决，连接速度慢的问题。
        /// </summary>
        public class ConnectionThread
        {
            private string IP;
            private string port;
            private SocketClientConnetion _socketClient;
            private Thread tThread;
            public ConnectionThread(string _ip,string _port, SocketClientConnetion _cls)
            {
                IP = _ip;
                port = _port;
                _socketClient = _cls;
                tThread = new Thread(new ThreadStart(Conntion));
                tThread.Start();
            }

            private void Conntion()
            {
                _socketClient.ClientConnetion(IP, port);
            }
        }
        public class ConntionTime
        {
            public string IP;
            public int Count;
            public ConntionTime(string _ip,int _count)
            {
                IP = _ip;
                Count = _count;
            }
        }
        //需要建立连接的对象
        private List<ClientConntion> _lstclient;
        // 表示当前处于与服务器建立连接的过程中
        private bool _clientNow;
        //用于建立连接时使用的线程
        private Thread _threadclient;
        //读写锁
        private ReaderWriterLock _rwLock;
        //SOCKET缓存
        private Dictionary<long, SocketTCPHandler> _socketsCache;
        //TCP连接器
        SocketClientConnetion _socketClient;

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
        /// <summary>
        /// 用于表示连接的超时时间，是否重新开起连接
        /// </summary>
        private List<ConntionTime> LstConntionTime = new List<ConntionTime>();

        #endregion

        /// <summary>
        /// TCP客户端构造函数
        /// </summary>
        public SocketTCPClient()
        {
            _rwLock = new ReaderWriterLock();
            _socketsCache = new Dictionary<long, SocketTCPHandler>();
            _socketClient = new SocketClientConnetion();
            _socketClient.OnNewSocketAccept += new NewSocketEventHandler(_socketListener_OnNewSocketAccept);

            _threadclient = new Thread(new ThreadStart(ThreadClientConnetion));
            _threadclient.Start();
        }
        /// 用于检查IP地址或域名是否可以使用TCP/IP协议访问(使用Ping命令),true表示Ping成功,false表示Ping失败 
        /// </summary>
        /// <param name="strIpOrDName">输入参数,表示IP地址或域名</param>
        /// <returns></returns>
        private bool PingIP(string DoNameOrIP)
        {
            try
            {
                Ping objPingSender = new Ping();
                PingOptions objPinOptions = new PingOptions();
                objPinOptions.DontFragment = true;
                string data = "FData";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int intTimeout = 10;
                PingReply objPinReply = objPingSender.Send(DoNameOrIP,intTimeout, buffer, objPinOptions);
                string strInfo = objPinReply.Status.ToString();
                if (strInfo == "Success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void ThreadClientConnetion()
        {
            SocketTCPHandler tcpHandler;
            ConnectionThread ctCls;
            for (; ; )
            {
                try
                {
                    if (_clientNow)
                    {

                        for (int i = _socketsCache.Values.Count - 1; i >= 0; i--)
                        {
                            tcpHandler = _socketsCache.Values.ToList()[i];
                            if (_lstclient.FindIndex(p => p._ip == tcpHandler.IP) < 0)
                            {
                                try
                                {
                                    if (tcpHandler != null)
                                    {
                                        tcpHandler.Disconnect();
                                    }
                                }
                                catch
                                {
                                }
                            }

                        }
                        for (int i = 0; i < _lstclient.Count; i++)
                        {
                            if (_socketsCache.Values.ToList().FindIndex(p => (p.IP == _lstclient[i]._ip) && (p.Port.ToString() == _lstclient[i]._port)) < 0)
                            {//以前没有，就重新建立连接
                                if (LstConntionTime.FindIndex(p => p.IP == _lstclient[i]._ip) < 0)
                                {
                                    if (PingIP(_lstclient[i]._ip))
                                    {
                                        _socketClient = new SocketClientConnetion();
                                        _socketClient.OnNewSocketAccept += new NewSocketEventHandler(_socketListener_OnNewSocketAccept);
                                        ctCls = new ConnectionThread(_lstclient[i]._ip, _lstclient[i]._port, _socketClient);
                                        LstConntionTime.Add(new ConntionTime(_lstclient[i]._ip, 0));
                                    }
                                }
                            }

                        }
                    }
                    _clientNow = false;
                    for (int i = 0; i < LstConntionTime.Count; i++)
                    {
                        LstConntionTime[i].Count++;
                        if (LstConntionTime[i].Count > 30)
                        {
                            LstConntionTime.RemoveAt(i);
                            i--;
                        }
                    }                    
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    Basic.Framework.Logging.LogHelper.Error(string.Format(" socket log: ThreadClientConnetion失败--{0}！", ex.Message));
                }
            }
        }
        /// <summary>
        /// 用于外部定时建立连接和关闭连接
        /// </summary>
        /// <param name="lstConnetion"></param>
        public void ConnetionServer(List<ClientConntion> lstConnetion)
        {
            if (!_clientNow)
            {
                _lstclient = lstConnetion;
                _clientNow = true;
            }
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
        public bool Send(long connectID, byte[] data, long dataLength)
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
                if (!_socketsCache.ContainsKey(socketId))//添加到缓存
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
                List<long> removlst = new List<long>();
                foreach (var item in _socketsCache)
                {
                    if (item.Value.IP == e.IP)
                    {
                        removlst.Add(item.Key);
                    }
                }
                foreach(long i in removlst)
                {
                    var socket = _socketsCache[i];
                    socket.OnConnectClose -= socketHandler_OnConnectClose;
                    socket.OnDataArrive -= socketHandler_OnDataArrive;
                    _socketsCache.Remove(i);
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
    }
}
