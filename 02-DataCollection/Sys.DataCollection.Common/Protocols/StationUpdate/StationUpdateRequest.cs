using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 请求分站远程升级
    /// </summary>
    public class StationUpdateRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
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
        /// <summary>
        /// 字节9：升级版本上限；
        /// </summary>
        public byte maxVersion;
        /// <summary>
        /// 字节10：升级版本下限；
        /// </summary>
        public byte minVersion;
        /// <summary>
        /// 高在前，低在后；已下发升级文件的总的片段数
        /// </summary>
        public int FileCount;
        /// <summary>
        /// 升级文件的CRC32校验码
        /// </summary>
        public long Crc;
    }
}
