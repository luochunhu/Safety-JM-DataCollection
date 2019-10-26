using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace DC.Communication.Components
{
    public class SocketTCPHandler
    {
        Socket _socket;
        long _socketId;

        const int BUFFERSIZE = 1024;
        byte[] _readBuffer = null;

        private volatile bool _sending;
        private Queue<byte[]> _sendQueue;
        private ReaderWriterLock _rwLock;

        public string IP { get; set; }
        public string MAC { get; set; }
        public int Port { get; set; }
        

        public event NetEventHandler OnConnectClose;       
        public event DataArriveEventHandler OnDataArrive;
 
        public SocketTCPHandler(long socketId, Socket socket)
        {
            this._socketId = socketId;

            this._socket = socket;

            _sending = false;
            _sendQueue = new Queue<byte[]>();
            _readBuffer = new byte[BUFFERSIZE];
            _rwLock = new ReaderWriterLock();
        }

        public bool ReceiveAuth()
        {
            bool result = true;

            try
            {
                MAC = "";
                IP = ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
                Port = ((IPEndPoint)_socket.RemoteEndPoint).Port;

                //int rec = _socket.Receive(_readBuffer, 0, BUFFERSIZE, SocketFlags.None);

                //if (_readBuffer[0] != 0x5A || _readBuffer[1] != 0xA5 || _readBuffer[2] != 0x3C || _readBuffer[3] != 0xC3)
                //{
                //    Basic.Framework.Logging.LogHelper.Debug(" socket log: ReceiveAuth() 数据包头不对:" + IP);

                //    byte[] tempData = new byte[rec];
                //    Buffer.BlockCopy(_readBuffer, 0, tempData, 0, rec);
                //    Basic.Framework.Logging.LogHelper.Debug(" socket log: _socket.Receive data:" + string.Join(" - ", tempData));

                //    this.Disconnect();
                //    return false;
                //}
                
                //MAC = BitConverter.ToString(_readBuffer, 6, 6).Replace('-', '.');
                
                this.BeginReceive();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 开始接收数据
        /// </summary>
        private  void BeginReceive()
        {
            try
            {
                _readBuffer = new byte[BUFFERSIZE];
                _socket.BeginReceive(_readBuffer, 0, BUFFERSIZE, SocketFlags.None, new AsyncCallback(EndReceive), null);
            }
            catch(Exception ex)
            {
                Basic.Framework.Logging.LogHelper.Error("socket log: BeginReceive() " + ex.ToString());
            }
        }

        /// <summary>
        /// 收到异步数据
        /// </summary>
        /// <param name="ar"></param>
        private void EndReceive(IAsyncResult ar)
        {
            try
            {
                int nBytes;
                nBytes = _socket.EndReceive(ar);
                
                if (nBytes <= 0)//todo:
                {
                    Disconnect();
                    return;
                }

                if (nBytes > 4) //&& _readBuffer[0] == 0x5A && _readBuffer[1] == 0xA5 && _readBuffer[2] == 0x3C && _readBuffer[3] == 0xC3)
                {

                    if (this.OnDataArrive != null)
                    {
                        byte[] data = new byte[nBytes];
                        Buffer.BlockCopy(_readBuffer, 0, data, 0, data.Length);
                        
                        try
                        {
                            this.OnDataArrive(this, new DataArriveEventArgs(this._socketId, data, data.Length, MAC, IP, Port));
                        }
                        catch
                        {
                            //todo write log
                        }
                    }
                }

                if (this.IsConnected)
                {
                    this.BeginReceive();
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (SocketException ex)
            {
                // Basic.Framework.Logging.LogHelper.Error("c8962 socket log: EndReceive() " + ex.ToString());
                Disconnect();
            }
            catch
            {
                Disconnect();
            }

            
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (_socket == null)
            {
                if (this.OnConnectClose != null)
                {
                    this.OnConnectClose(this, new NetEventArgs(this._socketId, this.IP, this.MAC, this.Port, "Socket连接被关闭"));
                }
               
                if (this._sendQueue != null)
                {
                    this._sendQueue.Clear();
                }
                this._sending = false;
                return;
            }

            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch(Exception ex )
            {
                 Basic.Framework.Logging.LogHelper.Error("Disconnect1: " + ex.ToString());

            }

            try
            {
                _socket.Disconnect(false);
                _socket.Close();
            }
            catch (Exception ex)
            {
                Basic.Framework.Logging.LogHelper.Error("Disconnect2:" + ex.ToString());

            }
            _socket.Dispose();

            this._sending = false;
            this._sendQueue.Clear();

            if (this.OnConnectClose != null)
            {
                this.OnConnectClose(this, new NetEventArgs(this._socketId, this.IP, this.MAC, this.Port, "Socket连接被关闭"));
            }
            Basic.Framework.Logging.LogHelper.Info(string.Format(" socket log: IP【{0}】-Port【{1}】  断开连接！", this.IP, this.Port));

        }

        /// <summary>
        /// 发送业务数据，自动增加0xfa处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Send(byte[] data)
        {
            return SendToSocket(data);
        }
                

        /// <summary>
        /// 发送心跳检查包
        /// </summary>
        /// <returns></returns>
        public bool CheckHeartbeat()
        {
            byte[] data = new byte[] { 0xfa, 0xfa };

            return SendToSocket(data);
        }

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SendToSocket(byte[] data)
        {
            if (!IsConnected)
            {
                return false;
            }
            try
            {
                if (_sending)
                {
                    //如果有数据在发送中，加锁入队列 
                    _rwLock.AcquireWriterLock(-1);
                    try
                    {
                        this._sendQueue.Enqueue(data);
                    }
                    catch { }
                    finally
                    {
                        _rwLock.ReleaseWriterLock();
                    }
                }
                else
                {
                    _sending = true;
                    _socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(EndSend), null);
                }
            }
            catch
            {
                return false;
            }            

            return true;
        }

        /// <summary>
        /// 异步发送完成
        /// </summary>
        /// <param name="ar"></param>
        private void EndSend(IAsyncResult ar)
        {
            if (_socket == null)
            {
                return;
            }

            try
            {
                _socket.EndSend(ar);

                byte[] data = null;

                _rwLock.AcquireWriterLock(-1);//20170309 加写锁

                if (_sendQueue.Count > 0)
                {
                    data = _sendQueue.Dequeue();
                }

                _rwLock.ReleaseWriterLock();


                if (data != null)//如果取到数据，继续异步发送
                {
                    try
                    {
                        _socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(EndSend), null);
                    }
                    catch (Exception e)
                    {
                       // Logger.WriteLog("EndSend() _socket.BeginSend 出错：" + e.ToString());
                    }
                }
                else
                {
                    //如果未有数据，则重置信号状态
                    _sending = false;
                }
            }
            catch (Exception ex)
            {
               // Logger.WriteLog("EndSend() 出错：" + ex.ToString());
            }
        }


        public bool IsConnected
        {
            get
            {
                if (_socket == null)
                    return false;
                return _socket.Connected;
            }
        }

       
    }
}
