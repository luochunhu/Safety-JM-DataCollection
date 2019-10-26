using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 异常中止升级流程
    /// </summary>
    public class UpdateCancleRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 升级的设备类型编码
        /// </summary>
        public byte DeviceId;
        /// <summary>
        /// 需进行升级操作的硬件版本号
        /// </summary>
        public byte HardVersion;
        /// <summary>
        /// 本次升级文件的软件版本号
        /// </summary>
        public byte FileVersion;
    }
}
