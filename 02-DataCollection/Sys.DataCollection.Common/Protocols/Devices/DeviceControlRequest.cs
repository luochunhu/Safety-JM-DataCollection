using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 设备控制命令下发（上位机->设备）
    /// </summary>
    public class DeviceControlRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 表示控制链表
        /// </summary>
        public List<DeviceControlItem> ControlChanels { get; set; }
        /// <summary>
        /// 唯一性编码确认链表
        /// </summary>
        public List<DeviceControlItem> SoleCodingChanels { get; set; }
        /// <summary>
        /// 表示控制分站下发报警、断电、复电值至传感器(=1表示分站要同步参数至传感器，=0表示不同步
        /// </summary>
        public byte SensorParaControl { get; set; }
          /// <summary>
        /// 瓦电3分强制解锁标记（=1表示要3分强制解锁，=0表示不进行3分锅制解锁）
        /// </summary>
        public byte GasThreeUnlockContro { get; set; }
        /// <summary>
        /// 强制获取设备唯一编码信息
        /// </summary>
        public byte GetDeviceInfoCoding { get; set; }
        /// <summary>
        /// <summary>
        /// 手动放电 0不进行操作，1取消维护性放电，2维护性放电
        /// </summary>
        public byte BDisCharge { get; set; }
        /// <summary>
        /// 清除分站历史数据-20180921
        /// </summary>
        public byte ClearHistoryData { get; set; }
        /// <summary>
        /// 得到历史数据-20180921
        /// </summary>
        public byte GetHistoryData { get; set; }
    }

    public class DeviceControlItem
    {
        /// <summary>
        /// 控制口号
        /// </summary>
        public int Channel;
        /// <summary>
        /// 控制类型（0解控   1控制   2强制解控）
        /// </summary>
        public int ControlType;
    }
}
