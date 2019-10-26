using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Driver
{
    /// <summary>
    /// 通讯状态变化参数基类
    /// </summary>
    public class CommunicationStateChangeArgs : EventArgs
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
        /// 连接唯一标识
        /// 针对安全监控系统，这是MAC；
        /// 针对GPRS，此为手机号
        /// </summary>
        public string UniqueCode { get; set; }

        /// <summary>
        /// 通讯状态
        /// </summary>
        public CommunicationState CommunicationState { get; set; }

        /// <summary>
        /// 连接号
        /// </summary>
        public long ConntecionId { get; set; }
    }

    /// <summary>
    /// 通讯状态枚举
    /// </summary>
    public enum CommunicationState
    {
        /// <summary>
        /// 连接成功
        /// </summary>
        Connect = 1,
        /// <summary>
        /// 断开连接
        /// </summary>
        Disconnect = 2
    }
}
