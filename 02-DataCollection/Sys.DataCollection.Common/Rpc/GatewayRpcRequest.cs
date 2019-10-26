using Sys.DataCollection.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Rpc
{
    /// <summary>
    /// 网关类RPC请求
    /// </summary>
    public class GatewayRpcRequest : Basic.Framework.Rpc.BaseRequest
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="requestType">请求类型</param>
        public GatewayRpcRequest(RequestType requestType) : base((int)requestType)
        {

        }

        /// <summary>
        /// 协议对象
        /// </summary>
        public MasProtocol MasProtocol { get; set; }        
    }
}
