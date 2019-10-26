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
    public class PartitionControlResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 响应代码, 0表示成功, 其他表示失败
        /// </summary>
        public string retCode { get; set; }
        /// <summary>
        /// 分区标识(新增分区时返回)
        /// </summary>
        public string zoneId { get; set; }
    }

}
