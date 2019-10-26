using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace DC.Communication.Components
{
    public delegate void NewSocketEventHandler(long socketId,Socket newSocket);

    /// <summary>
    /// socket 监听处理类
    /// </summary>
    public class SocketListener
    {     

        private Socket _listener;
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

        public void SocketListenner()
        {
            
        }
              
        /// <summary>
        /// 启动监听
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Start(int port)
        {
            bool result = false;
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);
            _listener = new Socket(iep.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _listener.Bind(iep);

                _listener.Listen(255);

                Basic.Framework.Logging.LogHelper.Debug(" socket log: " + port + " 端口监听已经打开，等待TCP连接");

                _listener.BeginAccept(new AsyncCallback(EndAccept), null);

                result = true;
            }
            catch (Exception e)
            {
                Basic.Framework.Logging.LogHelper.Error(" socket log: " + e.ToString());
            }

            return result;
        }

        /// <summary>
        /// 收到一个新的连接
        /// </summary>
        /// <param name="ar"></param>
        protected void EndAccept(IAsyncResult ar)
        {
            try
            {
                Socket socket = _listener.EndAccept(ar);

                Basic.Framework.Logging.LogHelper.Debug(" socket log: " + string.Format("新的连接{0}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString()));
                               
                if (OnNewSocketAccept != null)
                {
                    OnNewSocketAccept(NextSocketID, socket);
                }              
            }
            catch (Exception ex)
            {
                Basic.Framework.Logging.LogHelper.Error("c8962 socket log: " + ex.ToString());
            }
            finally
            {
                try
                {
                    _listener.BeginAccept(new AsyncCallback(EndAccept), null);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// 关闭监听
        /// </summary>
        public void StopListen()
        {
            try
            {
                if ((this._listener != null) && this._listener.Connected)
                {
                    this._listener.Shutdown(SocketShutdown.Both);
                }
            }
            catch (SocketException ex)
            {

            }
            catch (Exception ex)
            {

            }
            try
            {
                if (this._listener != null)
                {
                    this._listener.Close();
                }
            }
            catch (SocketException ex)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {
                this._listener = null;
            }
        }
               
    }
}
