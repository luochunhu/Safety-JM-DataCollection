using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 搜索指定IP模块参数回发
    /// </summary>
    public class QuerytNetworkDeviceParamResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 返回指定IP模块的参数集合
        /// </summary>
        public NetDeviceSettingInfo NetworkDeviceParam;
    }
}
