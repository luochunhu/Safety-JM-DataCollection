using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 启动文字广播任务
    /// </summary>
    public class StartPaMusicTaskResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 响应代码, 0表示成功, 其他表示失败
        /// </summary>
        public string retCode { get; set; }
        /// <summary>
        /// 广播任务标识
        /// </summary>
        public string taskId { get; set; }
    }
}
