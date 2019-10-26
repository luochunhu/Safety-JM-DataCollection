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
    /// 用于处理分站修改设备地址号的回发
    /// </summary>
    public class ModificationDeviceAdressResponseCommand : ModificationDeviceAdressResponse
    {
        public void HandleModificationDeviceAdressData(byte[] data, MasProtocol protocol, ushort startIndex, byte deviceCommunicationType, string point)
        {
            ModificationDeviceAdressResponse ResponseObject = new ModificationDeviceAdressResponse();
            protocol.ProtocolType = ProtocolType.ModificationDeviceAdressResponse;
            ResponseObject.DeviceCode = point;
            if (startIndex + 6 < data.Length)
                ResponseObject.RandomCode = data[startIndex + 6];
            else
                ResponseObject.RandomCode = 0;
            protocol.Protocol = ResponseObject;
        }
    }
}
