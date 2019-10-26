using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 远程还原最近一次备份程序(设备->上位机)
    /// </summary>
    public class ReductionResponseCommand : Sys.DataCollection.Common.Protocols.ReductionResponse
    {
        public void Handle(byte[] data, MasProtocol protocol, ushort startIndex,string point)
        {
            ReductionResponse reductionResponse = new ReductionResponse();
            reductionResponse.DeviceCode = point;

            reductionResponse.ResponseCode = (data[startIndex + 7] & 0x0F);

            protocol.ProtocolType = ProtocolType.ReductionResponse  ;
            protocol.Protocol = reductionResponse;
        }
    }

}
