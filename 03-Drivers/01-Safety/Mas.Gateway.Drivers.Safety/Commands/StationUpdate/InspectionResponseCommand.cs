using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 巡检单台分站的文件接收情况回复(设备->上位机)
    /// </summary>
    public class InspectionResponseCommand : Sys.DataCollection.Common.Protocols.InspectionResponse
    {
        public void Handle(byte[] data, MasProtocol protocol, ushort startIndex,string point)
        {
            InspectionResponse inspectionResponse = new InspectionResponse();
            inspectionResponse.DeviceCode = point;

            inspectionResponse.ResponseCode = data[startIndex + 7] & 0x0F;//分站响应标记
            inspectionResponse.LostFileNum = (data[startIndex + 8] << 8) + data[startIndex + 9]; //缺失文件编号

            protocol.ProtocolType = ProtocolType.InspectionResponse;
            protocol.Protocol = inspectionResponse;
        }
    }

}
