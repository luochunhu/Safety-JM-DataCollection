
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 获取电源箱实时数据回复（上位机->设备）
    /// </summary>
    public class QueryBatteryRealDataRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 放电时，切换的百分比
        /// </summary>
        public byte PowerPercentum { get; set; }
        /// <summary>
        /// 设备性质枚举
        /// </summary>
        public ItemDevProperty DevProperty { get; set; }
        /// <summary>
        /// 表示对交换机电源箱的远程操作，0不进行操作，1取消维护性放电，2维护性放电
        /// </summary>
        public byte BatteryControl { get; set; }
    }
}
