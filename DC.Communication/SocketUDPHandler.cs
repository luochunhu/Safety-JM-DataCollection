using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace DC.Communication.Components
{
    /// <summary>
    /// 设备搜索管理类
    /// </summary>
    public class SocketUDPHandler
    {
        int _localPort = 65534;
        int _remotePort = 65533;

        byte[] _buffer;

        Socket _socket;
        EndPoint _remotePoint;

        public event DataArriveEventHandler OnDataArrive;

        public SocketUDPHandler()
        {
        }

        /// <summary>
        /// 打开指定UDP监听
        /// </summary>
        /// <param name="bindIp">指定固定的绑定IP发送数据</param>
        public void OpenSocket(string bindIp = "")
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                if (string.IsNullOrEmpty(bindIp) || bindIp == "127.0.0.1")
                {
                    _remotePoint = new IPEndPoint(IPAddress.Any, _localPort);
                }
                else
                {
                    _remotePoint = new IPEndPoint(System.Net.IPAddress.Parse(bindIp), _localPort);
                }
                _socket.Bind(_remotePoint);

                BeginReceive();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 关闭UDP端口
        /// </summary>
        public void CloseSocket()
        {
            if (_socket != null)
            {
                try
                {
                    _socket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }

        /// <summary>
        /// 发送UDP数据，广播的形式到255.255.255.255
        /// 发送端口必须为32762,否则设备收到其它端口数据不会回复
        /// </summary>
        /// <param name="data"></param>
        /// <param name="number "></param>
        public void Send(byte[] data, int number)
        {
            try
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse("255.255.255.255"), _remotePort);
                for (int i = 0; i < number; i++)
                {
                    _socket.SendTo(data, iep);
                    Thread.Sleep(10);
                }
            }
            catch (System.ObjectDisposedException ex)
            { }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 开始异步接收数据
        /// </summary>
        private void BeginReceive()
        {
            try
            {
                _buffer = new byte[1024];
                if (_socket != null)
                {
                    _socket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref _remotePoint, new AsyncCallback(EndReceive), null);
                }
            }
            catch (System.ObjectDisposedException ex)
            { }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 结束接收数据
        /// </summary>
        /// <param name="ar"></param>
        private void EndReceive(IAsyncResult ar)
        {
            int cnt = 0;
            try
            {
                if (this._socket == null)
                {
                    return;
                }
                cnt = this._socket.EndReceiveFrom(ar, ref this._remotePoint);
            }
            catch (System.ObjectDisposedException ex)
            { }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            try
            {
                if (cnt > 0)
                {
                    byte[] temp = new byte[cnt];
                    Buffer.BlockCopy(_buffer, 0, temp, 0, cnt);
                    if (temp.Length < 3)
                    {
                        return;
                    }

                    if (this.OnDataArrive != null)
                    {
                        this.OnDataArrive(this, new DataArriveEventArgs(0, temp, temp.Length, "", "", 0));
                    }
                }
            }
            finally
            {
                if (_socket != null)
                {
                    BeginReceive();
                }
            }
        }
    }
}
