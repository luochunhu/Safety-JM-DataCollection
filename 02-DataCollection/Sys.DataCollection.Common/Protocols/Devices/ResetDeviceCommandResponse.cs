using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 复位命令回复（设备->上位机）
    /// </summary>
    public class ResetDeviceCommandResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 回复命令内容 （1.复位成功；2.复位失败；）
        /// </summary>
        public int ReturnCode { get; set; }
    }
}
