using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 上传网络设备实时数据（设备->上位机）
    /// </summary>
    public class NetworkDeviceDataRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
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
        /// 实时值（表示监测设备的实时值，如果此分站为人员定位系统下的识别器就表示为卡号；）MAC地址
        /// </summary>
        public string RealData        { get; set; }
        /// <summary>
        /// 数据状态描述（见枚举ItermState说明）
        /// </summary>
        public ItemState State { get; set; }
    }
}
