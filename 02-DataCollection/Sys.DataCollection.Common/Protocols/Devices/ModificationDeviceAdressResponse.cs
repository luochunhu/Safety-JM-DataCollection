using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols.Devices
{
    /// <summary>
    /// 修改传感器地址号信息（设备->上位机）
    /// </summary>
    public class ModificationDeviceAdressResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 表示分站是对那一次的设备地址修改进行接收正确，回发
        /// </summary>
        public byte RandomCode { get; set; }
    }
}
