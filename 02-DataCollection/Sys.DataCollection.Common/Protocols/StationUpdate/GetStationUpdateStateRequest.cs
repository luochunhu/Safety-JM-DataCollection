using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 获取分站的工作状态
    /// </summary>
    public class GetStationUpdateStateRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// =1表示获取软件版本号
        /// </summary>
        public byte GetSoftVersion;
        /// <summary>
        /// =1表示获取远程升级状态
        /// </summary>
        public byte GetUpdateState;
        /// <summary>
        /// =1 表示获取设备类型
        /// </summary>
        public byte GetDevType;
        /// <summary>
        /// =1表示获取设备硬件版本号
        /// </summary>
        public byte GetHardVersion;
    }
}
