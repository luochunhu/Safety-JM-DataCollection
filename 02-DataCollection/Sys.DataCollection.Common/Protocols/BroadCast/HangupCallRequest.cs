using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 挂断呼叫
    /// </summary>
    public class HangupCallRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 呼叫标识
        /// </summary>
        public string callId { get; set; }
    }
}
