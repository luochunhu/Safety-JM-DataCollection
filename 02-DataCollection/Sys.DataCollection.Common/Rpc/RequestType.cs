using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Rpc
{
    /// <summary>
    /// 请求类型枚举
    /// </summary>
    public enum RequestType
    {
        /// <summary>
        /// 设备类请求
        /// </summary>
        DeviceRequest = 1,
        /// <summary>
        /// 业务类请求
        /// </summary>
        BusinessRequest = 2,
        /// <summary>
        /// 设备（UDP）类请求
        /// 这里是kj73n特殊的一类命令，主要用于搜索网络设备及网络参数设置
        /// </summary>
        DeviceUdpRequest = 3
    }
}
