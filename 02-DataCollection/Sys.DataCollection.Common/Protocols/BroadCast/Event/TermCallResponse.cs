using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 终端呼叫状态对象
    /// </summary>
    public class TermCallResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 归属分区标识
        /// </summary>
        public string zoneId { get; set; }
        /// <summary>
        /// 终端号码
        /// </summary>
        public string termDN { get; set; }
        /// <summary>
        /// 注册状态(参见关联定义)
        /// </summary>
        public ItemCallState callState { get; set; }

    }

}
