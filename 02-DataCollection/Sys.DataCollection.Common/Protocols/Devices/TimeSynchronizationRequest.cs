using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 下发时间同步命令（上位机->设备）
    /// </summary>
    public class TimeSynchronizationRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 下发至设备的时间
        /// </summary>
        public DateTime SyncTime { get; set; }
        /// <summary>
        /// 串口号
        /// </summary>
        public int CommPort { get; set; }
    }
}
