using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols.Devices
{
    /// <summary>
    ///  获取分站设备历史单点控制信息，即D0命令（设备->上位机）
    /// </summary>
    public class QueryHistoryControlResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 表示剩余的报警控制信息条数
        /// </summary>
        public ushort AlarmTotal;
        /// <summary>
        /// 表示当前回复的报警信息记录数
        /// </summary>
        public List<DeviceHistoryControlItem> HistoryControlItems { get; set; }
    }
    /// <summary>
    /// 设备历史控制记录信息
    /// </summary>
    public class DeviceHistoryControlItem
    {
        /// <summary>
        /// 存盘时间
        /// </summary>
        public DateTime SaveTime;
        /// <summary>
        /// 表示设备的通道号（=0表示采集设备本身，=1表示此采集设备下的1号设备….）
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 表示设备的地址号（单参数传感器此值为0，多参数传感器此值为其地址号）
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 实时值（即产生控制时的实时值；）
        /// </summary>
        public string RealData { get; set; }
        /// <summary>
        /// 数据状态描述
        /// </summary>
        public ItemState State { get; set; }
        /// <summary>
        /// 设备性质枚举
        /// </summary>
        public ItemDevProperty DeviceProperty { get; set; }
        /// <summary>
        /// 表示当前设备触发时，控制量的执行情况，bit0为1表示控制了1号控制设备
        /// </summary>
        public ushort ControlDevice;
    }
}
