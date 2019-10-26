using Sys.DataCollection.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Driver
{
    /// <summary>
    /// 上行数据生成事件参数
    /// </summary>
    public class ProtocolDataCreatedEventArgs : DriverEventArgs
    {
        /// <summary>
        /// 设备唯一标识
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 生成的上行协议对象
        /// </summary>
        public MasProtocol MasProtocol { get; set; }
    }
}
