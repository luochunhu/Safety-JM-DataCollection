using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols.Devices
{
    /// <summary>
    /// 20180921---获取设备的信息详细
    /// </summary>
    public class GetDeviceInformationRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        //表示需要获取设备的地址列表=0表示1号地址的传感器，分站默认都需要回发
        public List<int> GetAddressLst = new List<int>();
    }
}
