using DC.Communication.Components;
using Sys.DataCollection.Common.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Communications
{
    /// <summary>
    /// 通讯接口类
    /// </summary>
    public interface ICommunication
    {
        /// <summary>
        /// 通讯模块编码
        /// </summary>
        string CommunicationCode { get; set; }
        /// <summary>
        /// 网络数据到达事件
        /// </summary>
        event NetDataArrivedEventHandler OnNetDataArrived;
        /// <summary>
        /// 通讯网络状态变化事件
        /// </summary>
        event CommunicationStateChangeEventHandler OnCommunicationStateChange;
        /// <summary>
        /// 启动通讯模块
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        bool Start(string ip, int port);
        /// <summary>
        /// 停止通讯模块
        /// </summary>
        /// <returns></returns>
        bool Stop();
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="target">唯一标识（可为Mac,手机号,IP地址等）</param>
        /// <param name="data">数据包</param>
        /// <returns></returns>
        bool Send(string target, byte[] data);
        /// <summary>
        /// 扩展用于客户端连接井下服务端设备时使用
        /// </summary>
        /// <param name="lstConnetion"></param>
        /// <returns></returns>

        void ConnetionServer(List<ClientConntion> lstConnetion);

    }

    /// <summary>
    /// 网络数据到达参数
    /// </summary>
    public class NetDataArrivedEventArgs : EventArgs
    {
        /// <summary>
        /// 网络模块编号
        /// </summary>
        public string CommunicationCode { get; set; }
        /// <summary>
        /// 远程IP
        /// </summary>
        public string RemoteIp { get; set; }
        /// <summary>
        /// 远程端口号
        /// </summary>
        public int RemotePort { get; set; }
        /// <summary>
        /// 数据包
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 连接唯一标识
        /// 针对安全监控系统，这是MAC；
        /// 针对GPRS，此为手机号
        /// </summary>
        public string UniqueCode { get; set; }
    }
    /// <summary>
    /// 网络数据到达委托
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">数据参数</param>
    public delegate void NetDataArrivedEventHandler(object sender, NetDataArrivedEventArgs args);

    /// <summary>
    /// 通讯状态变化（连接、断开连接） 委托
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">通讯状态变化参数</param>

    public delegate void CommunicationStateChangeEventHandler(object sender, CommunicationStateChangeArgs args);





}
