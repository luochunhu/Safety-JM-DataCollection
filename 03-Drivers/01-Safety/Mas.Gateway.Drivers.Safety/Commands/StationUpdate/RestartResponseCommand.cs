using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 通知分站进行重启升级回复(设备->上位机)
    /// </summary>
    public class RestartResponseCommand : Sys.DataCollection.Common.Protocols.RestartResponse
    {
        public void Handle(byte[] data, MasProtocol protocol, ushort startIndex,string point)
        {
            RestartResponse restartResponse = new RestartResponse();
            restartResponse.DeviceCode = point;

            restartResponse.ResponseCode = (data[startIndex + 7] & 0x0F);

            protocol.ProtocolType = ProtocolType.RestartResponse;
            protocol.Protocol = restartResponse;
        }
    }

}
