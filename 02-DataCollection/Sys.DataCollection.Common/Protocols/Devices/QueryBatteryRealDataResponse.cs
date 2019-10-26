using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 电源箱实时数据回复（设备->上位机）
    /// </summary>
    public class QueryBatteryRealDataResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 获取电源箱电压时间
        /// </summary>
        public DateTime BatteryDateTime { get; set; }
        /// <summary>
        /// 区分回发数据是交换机还是分站的电源箱数据
        /// </summary>
        public ItemDevProperty DeviceProperty { get; set; }
        /// <summary>
        /// 电源箱详细信息
        /// </summary>
        public List<BatteryRealDataItem> BatteryRealDataItems { get; set; }
    }
    //20180921,此结构进行了修改
    public class BatteryRealDataItem
    {
        /// <summary>
        /// 表示设备的通道号/MAC地址（=0表示采集设备本身，=1表示此采集设备下的1号设备….）
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 表示设备的地址号/IP地址（单参数传感器此值为0，多参数传感器此值为其地址号）
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 设备性质枚举
        /// </summary>
        public ItemDevProperty DeviceProperty { get; set; }
        /// <summary>
        /// 温度T1
        /// </summary>
        public float DeviceTemperature1 { get; set; }
        /// <summary>
        /// 温度T2
        /// </summary>
        public float DeviceTemperature2{ get; set; }
        /// <summary>
        /// 电源箱交直流状态40 交流（40） 直流（41 和01）
        /// </summary>
        public int BatteryACDC { get; set; }
        /// <summary>
        /// 总电压
        /// </summary>
        public float TotalVoltage { get; set; }
        ///// <summary>
        ///// 电源箱电池电压---长度为6.
        ///// </summary>
        public float[] BatteryVOL { get; set; }
        ///// <summary>
        ///// 电池控制状态（0不放电，1放电）
        ///// </summary>
        //public byte BatteryState { get; set; }
        ///// <summary>
        ///// 电源箱过热（true 过热；false正常）
        ///// </summary>
        //public bool BatteryTooHot { get; set; }
        ///// <summary>
        /////电源箱欠压（true 欠压；false正常）
        ///// </summary>
        //public bool BatteryUndervoltage { get; set; }
        ///// <summary>
        /////电源箱过充（true 过充；false正常）
        ///// </summary>
        //public bool BatteryOverCharge { get; set; }
        ///// <summary>
        ///// 电源箱充放电状态0 均衡中=1表示条件为真
        ///// </summary>
        //public int BatteryPackStateJh { get; set; }
        ///// <summary>
        ///// 电源箱充放电状态充电中=1表示条件为真
        ///// </summary>
        //public int BatteryPackStateCd { get; set; }
        ///// <summary>
        ///// 电源箱充放电状态 放电中 =1表示条件为真
        ///// </summary>
        //public int BatteryPackStateFd { get; set; }
        ///// <summary>
        ///// 电源箱电量
        ///// </summary>
        //public byte BatteryPackVOL { get; set; }
        ///// <summary>
        /////电源箱交直流状态（0 断线，1交流、2直流）
        ///// </summary>
        //public int BatteryACDC { get; set; }
        ///// <summary>
        ///// 电源箱负载电流
        ///// </summary>
        //public float BatteryPackMA { get; set; }
        ///// <summary>
        ///// 电源箱电池电压
        ///// </summary>
        //public float[] BatteryVOL { get; set; }

    }
}
