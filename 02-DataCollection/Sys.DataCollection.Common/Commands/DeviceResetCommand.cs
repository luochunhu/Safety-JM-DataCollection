using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Commands
{
    /// <summary>
    /// 重启设备命令
    /// </summary>
    public class DeviceResetCommand
    {
        /// <summary>
        /// 执行重启设备的MAC
        /// </summary>
        public string Mac { get; set; }
        /// <summary>
        /// 等待超时时间（单位毫秒）
        /// </summary>
        public int Timeout { get; set; }

    }
}
