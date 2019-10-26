using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 获取分站的工作状态(设备->上位机)
    /// </summary>
    public class PlayListControlResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 响应代码, 0表示成功, 其他表示失败
        /// </summary>
        public string retCode { get; set; }
        /// <summary>
        /// 播放列表唯一标识
        /// </summary>
        public string plstId { get; set; }
    }

}
