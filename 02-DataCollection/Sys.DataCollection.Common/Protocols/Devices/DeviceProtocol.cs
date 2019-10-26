using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 设备协议基类
    /// </summary>
    public class DeviceProtocol
    {
        /// <summary>
        /// 唯一标识符，用于标识此条命令针对的具体设备或者名称(用于表示一个设备的唯一编码，监控以POINT表示)
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 表示上一次是否正常接受标记,网络通讯时，常置的1，toDo:需要分析标记的使用
        /// </summary>
        public byte LastAcceptFlag  { get ; set; }

    }
}
