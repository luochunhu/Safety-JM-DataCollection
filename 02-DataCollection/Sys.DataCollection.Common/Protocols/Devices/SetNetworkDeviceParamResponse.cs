using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 对设置网络参数的应答
    /// </summary>
    public class SetNetworkDeviceParamResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 返回执行结果，=1表示成功，=0表示失败
        /// </summary>
        public byte ExeRtn;
    }
    /// <summary>
    /// 对设置网络参数的应答
    /// </summary>
    public class SetNetworkDeviceParamCommResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 返回执行结果，=1表示成功，=0表示失败
        /// </summary>
        public byte ExeRtn;
    }
}
