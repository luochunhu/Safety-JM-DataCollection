using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 获取分站的工作状态(设备->上位机)
    /// </summary>
    public class GetStationUpdateStateResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 软件版本号
        /// </summary>
        public double GetSoftVersion;
        /// <summary>
        /// 远程升级状态
        /// </summary>
        public byte GetUpdateState;
        /// <summary>
        /// 设备类型
        /// </summary>
        public byte GetDevType;
        /// <summary>
        /// 设备硬件版本号
        /// </summary>
        public double GetHardVersion;
        /// <summary>
        /// 升级版本号
        /// </summary>
        public byte UpdateVersion;
    }

}
