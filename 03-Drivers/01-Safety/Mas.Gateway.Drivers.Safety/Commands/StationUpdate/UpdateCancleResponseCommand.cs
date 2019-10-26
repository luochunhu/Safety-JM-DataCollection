using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 异常中止升级流程(设备->上位机)
    /// </summary>
    public class UpdateCancleResponseCommand : Sys.DataCollection.Common.Protocols.UpdateCancleResponse
    {
        public void Handle(byte[] data, MasProtocol protocol, ushort startIndex,string point)
        {
            UpdateCancleResponse updateCancleResponse = new UpdateCancleResponse();
            updateCancleResponse.DeviceCode = point;

            updateCancleResponse.ResponseCode = (data[startIndex + 7] & 0x0F);

            protocol.ProtocolType = ProtocolType.UpdateCancleResponse;
            protocol.Protocol = updateCancleResponse;
        }
    }

}
