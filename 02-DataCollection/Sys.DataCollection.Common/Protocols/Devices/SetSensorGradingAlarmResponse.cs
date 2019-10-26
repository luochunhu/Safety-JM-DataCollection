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
    public class SetSensorGradingAlarmResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    { 
        /// <summary>
      /// 表示当前下发的随机码，设备收到回发时，也按此码进行应答
      /// </summary>
        public byte RandomCode { get; set; }
    }
}
