using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Driver
{
    /// <summary>
    /// 驱动事件参数基类
    /// </summary>
    public abstract class DriverEventArgs : EventArgs
    {
        /// <summary>
        /// 驱动编号
        /// </summary>
        public string DriverCode { get; set; }
    }
}
