using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Rpc
{
    /// <summary>
    /// 网关请求应答
    /// </summary>
    public class GatewayRpcResponse
    {
        public GatewayRpcResponse()
        {
            IsSuccess = true;
            Message = "调用成功";
        }
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}
