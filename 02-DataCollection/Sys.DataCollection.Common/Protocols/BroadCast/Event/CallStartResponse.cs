using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 呼叫开始事件
    /// </summary>
    public class CallStartResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 呼叫唯一标识
        /// </summary>
        public string callId { get; set; }
        /// <summary>
        /// 主叫号码
        /// </summary>
        public string callerDN { get; set; }
        /// <summary>
        /// 被叫号码
        /// </summary>
        public string calledDN { get; set; }
    }
}
