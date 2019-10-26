using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols.Devices
{
    /// <summary>
    /// 表示获取分站4小时历史统计数据（上位机->设备）
    /// </summary>
    public class QueryHistoryRealDataRequest: Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 查询五分钟时间点
        /// 以5分站为单位，倒推；
        ///例如取值=1时，表示取最近5分钟的数据；
        ///例如取值=48时，表示取过去4h的数据；
        ///=0表示查询全部
        /// </summary>
        public byte QueryMinute;
        /// <summary>
        /// 生成命令时的顺序号，当切换QueryMinute时发生改变，且保证与上一次下发的顺序号不一致
        /// </summary>
        public byte SerialNumber;
    }
}
