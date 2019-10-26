using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 设备初始化应答（设备->上位机）
    /// </summary>
    public class InitializeResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 初始化应答的CRC
        /// </summary>
        public ushort StationCrc { get; set; }
    }
}
