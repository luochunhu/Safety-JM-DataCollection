using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    public class GetSwitchboardParamCommResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 电源箱电池控制状态（0不放电，1放电）
        /// </summary>
        public byte BatteryControlState { get; set; }
        /// <summary>
        ///电源箱状态  1表示直流供电，=0表示交流供电
        /// </summary>
        public byte BatteryState { get; set; }
        /// <summary>
        /// 电源箱电池容量，取值[0,100]；单位：%，如有电池箱是汇总电池箱后的容量百分比
        /// </summary>
        public byte BatteryCapacity { get; set; }
        /// <summary>
        /// （串口服务器-供电电源）：
        ///    0：供电故障；1：供电正常
        /// </summary>
        public byte SerialPortBatteryState { get; set; }
        /// <summary>
        /// （串口服务器-运行状态）：
        ///    0：运行故障；1：运行正常
        /// </summary>
        public byte SerialPortRunState { get; set; }
        /// <summary>
        /// （交换机-供电电源）：
        ///    0：供电故障；1：供电正常
        /// </summary>
        public byte SwitchBatteryState { get; set; }
        /// <summary>
        /// （交换机-运行状态）：
        ///    0：运行故障；1：运行正常
        /// </summary>
        public byte SwitchRunState { get; set; }
        /// <summary>
        /// 数组长度为3
        /// 【0】=(千兆光口1状态):
        ///0：通信故障；1：通信正常
        /// 【1】=(千兆光口2状态):
        ///0：通信故障；1：通信正常
        /// 【2】=(千兆光口3状态):
        ///0：通信故障；1：通信正常
        /// </summary>
        public byte[] Switch1000State { get; set; }
        /// <summary>
        /// 数组长度为7
        /// 【0】=(百兆接口1状态):
        ///0：通信故障；1：通信正常
        /// 【1】=(百兆接口2状态):
        ///0：通信故障；1：通信正常
        /// 【2】=(百兆接口3状态):
        ///0：通信故障；1：通信正常
        /// 【3】=(百兆接口4状态):
        ///0：通信故障；1：通信正常
        /// 【4】=(百兆接口5状态):
        ///0：通信故障；1：通信正常
        /// 【5】=(百兆接口6状态):
        ///0：通信故障；1：通信正常
        /// 【6】=(百兆接口7状态):
        ///0：通信故障；1：通信正常
        /// </summary>
        public byte[] Switch100State { get; set; }
        /// <summary>
        /// 唯 一编码信息
        /// </summary>
        public List<RealDataItem> RealDataItems { get; set; }
    }

}
