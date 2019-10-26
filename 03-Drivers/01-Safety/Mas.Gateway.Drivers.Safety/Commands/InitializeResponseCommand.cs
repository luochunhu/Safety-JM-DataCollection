using Sys.DataCollection.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    public class InitializeResponseCommand: InitializeResponse
    {
        public void SendInitializeAffirmToCenter(MasProtocol Protocol,  string point,ushort crc)
        {
            InitializeResponse ResponseObject = new InitializeResponse();
            Protocol.ProtocolType = ProtocolType.InitializeResponse;
            ResponseObject.StationCrc = crc;
            ResponseObject.DeviceCode = point;
            Protocol.Protocol = ResponseObject;
        }
        public void SendInitializeRequestToCenter(MasProtocol Protocol, string point)
        {
            DeviceInitializeRequest ResponseObject = new DeviceInitializeRequest();
            Protocol.ProtocolType = ProtocolType.DeviceInitializeRequest;
            ResponseObject.DeviceCode = point;
            Protocol.Protocol = ResponseObject;
        }
    }
}
