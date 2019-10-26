using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 设备请求升级回复(设备->上位机)
    /// </summary>
    public class StationUpdateResponseCommand : Sys.DataCollection.Common.Protocols.StationUpdateResponse
    {
        public void Handle(byte[] data, MasProtocol protocol, ushort startIndex,string point)
        {
            StationUpdateResponse restartResponse = new StationUpdateResponse();
            restartResponse.DeviceCode = point;
            restartResponse.ResponseCode = (data[startIndex + 7] & 0x0F);

            protocol.ProtocolType = ProtocolType.StationUpdateResponse;
            protocol.Protocol = restartResponse;
        }
    }
}