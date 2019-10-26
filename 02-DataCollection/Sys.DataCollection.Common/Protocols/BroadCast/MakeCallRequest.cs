using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 发起呼叫
    /// </summary>
    public class MakeCallRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 控制话机号码
        /// </summary>
        public string agentDN { get; set; }

        /// <summary>
        /// 被叫号码
        /// </summary>
        public string calledDN { get; set; }
    }
}
