using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols.Devices
{
    /// <summary>
    ///  分级报警的控制命令
    /// </summary>
    public class SetSensorGradingAlarmRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 表示当前下发的随机码，设备收到回发时，也按此码进行应答
        /// </summary>
        public byte RandomCode { get; set; }
        /// <summary>
        /// 需要分级控制的设备清单
        /// </summary>
        public List<SensorGradingAlarmItem> GradingAlarmItems { set; get; }
    }
    public class SensorGradingAlarmItem
    {
        /// <summary>
        /// 表示设备的地址号
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 表示分级报警等级=0表示不报警，正常响度，根据自身的报警值判断  = 1表示一级报警;= 2表示二级报警; = 3表示三级报警;=4表示四级报警
        /// </summary>
        public byte AlarmStep { get; set; }
    }
}
