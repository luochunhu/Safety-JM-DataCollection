using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols.Devices
{    /// <summary>
     /// 20180921---获取设备的信息详细
     /// </summary>
    public class GetDeviceInformationResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        public List<StationInfo> lstStation = new List<StationInfo>();
        public List<SensorInfo> lstSensor = new List<SensorInfo>();

    }
    //分站的基本信息20180921
    public class StationInfo
    {
        /// <summary>
        /// 表示设备的唯一编码
        /// </summary>
        public string SoleCoding { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime TimeNow { get; set; }
        /// <summary>
        /// 生产时间
        /// </summary>
        public DateTime ProductionTime { get; set; }
        /// <summary>
        /// 入口电压
        /// </summary>
        public string Voltage { get; set; }
        /// <summary>
        /// 重启次数
        /// </summary>
        public int RestartNum { get; set; }
        public string IP { get; set; }
        public string MAC { get; set; }
    }
    //传感器的基本信息20180921
    public class SensorInfo
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
        /// 表示设备的唯一编码
        /// </summary>
        public string SoleCoding { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime TimeNow { get; set; }
        /// <summary>
        /// 生产时间
        /// </summary>
        public DateTime ProductionTime { get; set; }
        /// <summary>
        /// 入口电压
        /// </summary>
        public string Voltage { get; set; }
        /// <summary>
        /// 重启次数
        /// </summary>
        public int RestartNum { get; set; }
        /// <summary>
        /// 报警次数
        /// </summary>
        public int AlarmNum { get; set; }
    }
}
