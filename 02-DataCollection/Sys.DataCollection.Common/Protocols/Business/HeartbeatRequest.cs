using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 心跳请求
    /// </summary>
    public class HeartbeatRequest
    {
        /// <summary>
        /// 网关状态；0：不正常；1：正常；
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 请求发起时间
        /// </summary>
        public DateTime RequestTime { get; set; }
    }
}
