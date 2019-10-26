using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 终端对象
    /// </summary>
    public class TermListInfo
    {
        /// <summary>
        /// 终端归属分区标识
        /// </summary>
        public string zoneDN { get; set; }
        /// <summary>
        /// 终端号码
        /// </summary>
        public string termDN { get; set; }
    }
}
