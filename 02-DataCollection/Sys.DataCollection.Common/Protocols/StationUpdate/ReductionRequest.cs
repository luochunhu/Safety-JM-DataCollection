using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 远程还原最近一次备份程序
    /// </summary>
    public class ReductionRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 需进行恢复操作的设备类型编码；=0：强制任何类型的设备恢复备份；
        /// </summary>
        public byte DeviceId;
        /// <summary>
        /// 需进行恢复操作的硬件版本号；=0：强制任何类型的设备恢复备份
        /// </summary>
        public byte HardVersion;
        /// <summary>
        /// 响应本次恢复操作的程序软件版本号;=0：强制任何版本的软件恢复备份
        /// </summary>
        public byte SoftVersion;
    }
}
