using Basic.Framework.Web;
using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 终端查询
    /// </summary>
    public class TerminalQueryRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 归属分区标识(新增、修改、删除)
        /// </summary>
        public string zoneId { get; set; }
        /// <summary>
        /// 终端号码(新增、修改、删除)
        /// </summary>
        public string termDN { get; set; }
    }
}
