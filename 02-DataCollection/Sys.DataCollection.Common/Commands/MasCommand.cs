using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Commands
{
    public abstract class MasCommand
    {
        /// <summary>
        /// 通讯命令字
        /// </summary>
        public byte CommandType { get; set; }
        /// <summary>
        /// 表示上一次是否正常接受标记
        /// </summary>
        public byte LastAcceptFlag { get; set; }
        /// <summary>
        /// 表示设备唯一地址
        /// </summary>
        public string DeviceCode { get; set; }
    }
}
