using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols.Devices
{    /// <summary>
     /// 表示获取分站4小时历史统计数据（设备->上位机）
     /// </summary>
    public class QueryHistoryRealDataResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 表示剩余的五分钟历史数据数量
        /// </summary>
        public ushort MinuteDataTotal { get; set; }
        /// <summary>
        /// 表示当前回复的五分钟记录数量
        /// </summary>
        public List<DeviceHistoryRealDataItem> HistoryRealDataItems { get; set; }
    }
    /// <summary>
    /// 历史数据---20180921
    /// </summary>
    public class DeviceHistoryRealDataItem
    {
        /// <summary>
        /// 历史数据时间
        /// </summary>
        public DateTime HistoryDate { get; set; }
        /// <summary>
        /// 分支号：=1表示智能口1；=2表示智能口2；=3表示智能口3；=4表示智能口4；=5表示扩展的智能开停；=6表示挂接的模拟量采集板
        /// </summary>
        public byte BranchNumber { get; set; }
        ///// <summary>
        ///// 表示此设备是否为自适应设备=true为是自适应，=false为非已定义的设备
        ///// </summary>
        //public bool DeviceAutoDefine { get; set; }
        /// <summary>
        /// 表示设备的通道号（=0表示采集设备本身，=1表示此采集设备下的1号设备….）;人员定位：kh
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 表示设备的地址号（单参数传感器此值为0，多参数传感器此值为其地址号） 人员定位：bh
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 实时值（表示监测设备的实时值，如果此分站为人员定位系统下的识别器就表示为卡号；）；人员定位：接收时间
        /// </summary>
        public string RealData { get; set; }
        /// <summary>
        /// 数据状态描述
        /// </summary>
        public ItemState State { get; set; }
        /// <summary>
        /// 传感器更换标记(0:未更换，1：更换中)
        /// </summary>
        public int ChangeSenior { get; set; }
        /// <summary>
        /// 传感器分级报警等级
        /// </summary>
        public int SeniorGradeAlarm { get; set; }
        /// <summary>
        /// 电压值描述（传感器时跟的是电压等级，分站如果挂接的智能电源箱跟的是电源箱的电压值）；人员定位：系统类型标志:0—人员,1—机车
        /// </summary>
        public string Voltage { get; set; }
        /// <summary>
        /// 设备型号编码
        /// </summary>
        public int DeviceTypeCode { get; set; }
        /// <summary>
        /// 设备性质枚举
        /// </summary>
        public ItemDevProperty DeviceProperty { get; set; }
        /// <summary>
        /// 回控状态（0无电 1无电）;  人员定位：是否呼叫
        /// </summary>
        public string FeedBackState { get; set; }
        /// <summary>
        /// 馈电状态（1馈电成功 2 馈电失败 3 复电成功 4 负电失败） ；  人员定位：是否为补传
        /// </summary>
        public string FeedState { get; set; }
    }
}
