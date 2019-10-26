using Sys.DataCollection.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    public class ResetDeviceResponseCommand
    {
        public void SendResetDeviceAffirmToCenter(MasProtocol Protocol, string Fzh)
        {
            ResetDeviceCommandResponse ResponseObject = new ResetDeviceCommandResponse();
            Protocol.ProtocolType = ProtocolType.ResetDeviceCommandResponse;
            ResponseObject.DeviceCode = Fzh;
            ResponseObject.ReturnCode = 1;
            Protocol.Protocol = ResponseObject;
        }
    }
}
