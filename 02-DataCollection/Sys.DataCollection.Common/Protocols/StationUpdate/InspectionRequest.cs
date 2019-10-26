using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 巡检单台分站的文件接收情况
    /// </summary>
    public class InspectionRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
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
        /// 缺失文件编号
        /// </summary>
        public int LostFileNum;
        /// <summary>
        /// 文件数据体
        /// </summary>
        public byte[] FileBuffer;
    }
}
