using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 呼叫结束事件
    /// </summary>
    public class CallEndResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 呼叫唯一标识
        /// </summary>
        public string callId { get; set; }
    }
}
