using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Driver
{
    /// <summary>
    /// 下行数据生成事件参数
    /// </summary>
    public class NetDataEventCreatedArgs : DriverEventArgs
    {
        /// <summary>
        /// 设备唯一标识
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 生成的下行数据包
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// NewAdd，用于指定当前分站是某个IP下面的那个通讯端口
        /// </summary>
        public int CommPort { get; set; }
    }
}
