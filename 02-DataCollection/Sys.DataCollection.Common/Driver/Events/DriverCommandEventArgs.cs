using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Driver
{
    /// <summary>
    /// 驱动命令事件参数
    /// </summary>
    public class DriverCommandEventArgs : DriverEventArgs
    {
        //public DriverCommandEventArgs()
        //{

        //}

        /// <summary>
        /// 命令类型
        /// 1.执行设备复位命令
        /// </summary>
        public int CommandType { get; set; }

        /// <summary>
        /// 命令的参数，以json格式
        /// </summary>
        public string JsonData { get; set; }
    }
}
