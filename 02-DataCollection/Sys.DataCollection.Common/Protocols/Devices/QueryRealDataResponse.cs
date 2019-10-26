using Sys.DataCollection.Common.Protocols.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 设备回发实时数据(设备->上位机)
    /// </summary>
    public class QueryRealDataResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 表示实时数据上行的分站类型,0x02,0x16表示老的智能分站，0x26表示新智能分站，0x00表示大分站。
        /// </summary>
        public byte DeviceCommperType { get; set; }
        /// <summary>
        /// 表示抽采数据的时间
        /// </summary>
        public DateTime CumulantTime { get; set; }
        /// <summary>
        /// 实时数据
        /// </summary>
        public List<RealDataItem> RealDataItems { get; set; }
        /// <summary>
        /// 历史数据列表---20180921
        /// </summary>
        public List<DeviceHistoryRealDataItem> HistoryRealDataItems { get; set; }
        /// <summary>
        /// 设备基本信息列表20180921
        /// </summary>
        public List<DeviceInfoMation> DeviceInfoItems { get; set; }
        /// <summary>
        /// 表示分站向上发送的CRC
        /// </summary>
        public ushort StationCrc { get; set; }
        /// <summary>
        /// 表示设备上传的时间，如果服务端判断时间间隔超过30秒自动下发时间同步
        /// </summary>
        public DateTime DeviceTime { get; set; }
        /// <summary>
        /// 分站电源状态，=1表示分站电源箱和分站通讯故障。
        /// </summary>
        public byte StationDyType { get; set; }
    }

    public class RealDataItem
    {
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
        /// 传感器分级报警等级，bit0 表示1级报警，bit1表示2级报警，bit2表示3级报警，bit3表示4级报警
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
        /// <summary>
        /// 电池控制状态（0不放电，1放电）
        /// </summary>
        public int BatteryState { get; set; }
        /// <summary>
        /// 表示设备的唯一编码；人员定位：欠压标记
        /// </summary>
        public string SoleCoding { get; set; }

    }
    /// <summary>
    /// 设备基础信息--新增20180921
    /// </summary>
    public class DeviceInfoMation
    {
        /// <summary>
        /// 设备性质枚举
        /// </summary>
        public ItemDevProperty DeviceProperty { get; set; }
        /// <summary>
        /// 表示设备的通道号（=0表示采集设备本身，=1表示此采集设备下的1号设备….）;人员定位：kh
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 表示设备的地址号（单参数传感器此值为0，多参数传感器此值为其地址号） 人员定位：bh
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 设备型号编码
        /// </summary>
        public int DeviceTypeCode { get; set; }
        /// <summary>
        /// 上报值
        /// </summary>
        public float UpAarmValue { get; set; }
        /// <summary>
        /// 下报值
        /// </summary>
        public float DownAarmValue { get; set; }
        /// <summary>
        /// 上断值
        /// </summary>
        public float UpDdValue { get; set; }
        /// <summary>
        /// 下断值
        /// </summary>
        public float DownDdValue { get; set; }
        /// <summary>
        /// 上恢复值
        /// </summary>
        public float UpHfValue { get; set; }
        /// <summary>
        /// 下恢复值
        /// </summary>
        public float DownHfValue { get; set; }
        /// <summary>
        /// 量程开始值
        /// </summary>
        public float LC1 { get; set; }
        /// <summary>
        /// 量程结始值
        /// </summary>
        public float LC2 { get; set; }
        /// <summary>
        /// 分级报警1的值
        /// </summary>
        public float SeniorGradeAlarmValue1 { get; set; }
        /// <summary>
        /// 分级报警2的值
        /// </summary>
        public float SeniorGradeAlarmValue2 { get; set; }
        /// <summary>
        /// 分级报警3的值
        /// </summary>
        public float SeniorGradeAlarmValue3 { get; set; }
        /// <summary>
        /// 分级报警4的值
        /// </summary>
        public float SeniorGradeAlarmValue4 { get; set; }
        /// <summary>
        /// 分级报警1时长值
        /// </summary>
        public float SeniorGradeTimeValue1 { get; set; }
        /// <summary>
        /// 分级报警2时长值
        /// </summary>
        public float SeniorGradeTimeValue2 { get; set; }
        /// <summary>
        /// 分级报警3时长值
        /// </summary>
        public float SeniorGradeTimeValue3 { get; set; }
        /// <summary>
        /// 分级报警4时长值
        /// </summary>
        public float SeniorGradeTimeValue4 { get; set; }
    }

}
