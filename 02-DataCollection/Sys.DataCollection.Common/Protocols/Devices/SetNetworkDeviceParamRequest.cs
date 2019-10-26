using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 设置指定IP模块的相关参数
    /// </summary>
    public class SetNetworkDeviceParamRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// =1表示设置分站，=0表示设置交换机
        /// </summary>
        public string StationFind { get; set; }
        /// <summary>
        /// 指定Mac地址
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// 设置模块参数信息
        /// </summary>
        public NetDeviceSettingInfo NetworkDeviceParam;
    }

    /// <summary>
    /// 设置指定IP模块的相关参数
    /// </summary>
    public class SetNetworkDeviceParamCommRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        public int CommPort { get; set; }
        /// <summary>
        /// 指定Mac地址
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// 设置模块参数信息
        /// </summary>
        public NetDeviceSettingInfo NetworkDeviceParam;
    }
}
