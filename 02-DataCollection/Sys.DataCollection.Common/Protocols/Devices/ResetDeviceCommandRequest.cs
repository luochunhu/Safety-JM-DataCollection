using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 下发复位命令（上位机->分站设备）
    /// </summary>
    public class ResetDeviceCommandRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 指设备的地址号
        /// </summary>
        public string Mac { get; set; }
    }
}
