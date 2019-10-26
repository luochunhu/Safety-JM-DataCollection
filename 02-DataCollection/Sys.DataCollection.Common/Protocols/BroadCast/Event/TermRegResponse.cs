using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 终端注册
    /// </summary>
    public class TermRegResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
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
        public int regState { get; set; }

    }

}
