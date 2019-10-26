using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 搜索网络设备回复（设备->上位机）
    /// </summary>
    public class SearchNetworkDeviceResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        public List<NetworkDeviceItem> NetworkDeviceItems { get; set; }
    }

    public class NetworkDeviceItem
    {
        /// <summary>
        /// 设备类型 1.c2000设备 2.新网桥8962设备 3.其它设备
        /// </summary>
        public int DeviceType { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 网络模块MAC地址
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// A.表示模块在交换机中的地址号（值为1-6）  B.表示设备类型号（值>6）保留不使用
        /// </summary>
        public int AddressNumber { get; set; }
        /// <summary>
        /// 所属交换机的MAC地址（只有devType=2时）  保留不使用
        /// </summary>
        public string SwitchMac { get; set; }

        /// <summary>
        /// 分站地址号(只能DevType=3时，此字段才有效)   分站处于自带网络设备时，采用这个方式进行处理。
        /// </summary>
        public int StationAddress { get; set; }

        /// <summary>
        /// 设备唯一编号(只能DevType=3时，此字段才有效)  保留
        /// </summary>
        public int DeviceSN { get; set; }
        /// <summary>
        /// 子网掩码
        /// </summary>
        public string SubMask;
        /// <summary>
        /// 网关IP
        /// </summary>
        public string GatewayIp;
    }
}
