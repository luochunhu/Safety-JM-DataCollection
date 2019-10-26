using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols.Devices
{
    /// <summary>
    /// 修改传感器地址号信息（上位机->设备）
    /// </summary>
    public class ModificationDeviceAdressRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 表示当前下发的随机码，设备收到回发时，也按此码进行应答
        /// </summary>
        public byte RandomCode { get; set; }
        /// <summary>
        /// 修改传感器地址列表，一次性仅能修改1个设备，链表是便于后续扩展
        /// </summary>
        public List<EditDeviceAdressItem> ModificationItems { get; set; }
    }
    public class EditDeviceAdressItem
    {
        /// <summary>
        /// 表示设备的唯一编码；
        /// </summary>
        public string SoleCoding { get; set; }
        /// <summary>
        /// 修改前的地址号
        /// </summary>
        public byte BeforeModification { get; set; }
        /// <summary>
        /// 修改后的地址号
        /// </summary>
        public byte AfterModification { get; set; }
        /// <summary>
        /// 设备型号 2018.4.2 by AI 新唯一编码带设备型号
        /// </summary>
        public byte DeviceType { get; set; }
    }

}
