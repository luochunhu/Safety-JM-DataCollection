using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 搜索指定IP模块的参数
    /// </summary>
    public class QuerytNetworkDeviceParamRequest  : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 获取的Mac地址
        /// </summary>
        public string Mac;
        ///// <summary>
        ///// 获取信息时，待待时间
        ///// </summary>
        //public uint WaitTime;
    }
}
