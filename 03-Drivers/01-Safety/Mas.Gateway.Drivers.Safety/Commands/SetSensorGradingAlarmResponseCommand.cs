using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Protocols.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    /// <summary>
    /// 传感器分级报警响应
    /// </summary>
    public class SetSensorGradingAlarmResponseCommand: SetSensorGradingAlarmResponse
    {
        public void HandleSetSensorGradingAlarm(byte[] data, MasProtocol protocol, ushort startIndex, byte deviceCommunicationType, string point)
        {
            SetSensorGradingAlarmResponse ResponseObject = new SetSensorGradingAlarmResponse();
            protocol.ProtocolType = ProtocolType.SetSensorGradingAlarmResponse;
            ResponseObject.DeviceCode = point;
            ResponseObject.RandomCode = 0;
            protocol.Protocol = ResponseObject;
        }
    }
}
