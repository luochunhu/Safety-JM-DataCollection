using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 查询缓存数据应答
    /// </summary>
    public class QueryCacheDataResponse
    {
        /// <summary>
        /// 设备（测点）列表
        /// </summary>
        public List<DeviceInfo> DeviceList { get; set; }

        /// <summary>
        /// 设备类型列表
        /// </summary>
        public List<DeviceTypeInfo> DeviceTypeList { get; set; }

        /// <summary>
        /// 网络设备（MAC）列表
        /// </summary>
        public List<NetworkDeviceInfo> NetworkDeviceList { get; set; }

        /// <summary>
        /// 设备交叉控制列表
        /// </summary>
        public List<DeviceAcrossControlInfo> DeviceAcrossControlList { get; set; }
    }
}
