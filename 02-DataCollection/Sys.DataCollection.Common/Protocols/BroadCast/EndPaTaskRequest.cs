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
    /// 终端管理
    /// </summary>
    public class EndPaTaskRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 广播任务标识
        /// </summary>
        public string taskId { get; set; }

    }
}
