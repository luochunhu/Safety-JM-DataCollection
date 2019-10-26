using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 刷新iNBS服务器
    /// </summary>
    public class RefreshRequest 
    {
        /// <summary>
        /// 控制台标识
        /// </summary>
        public string id { get; set; }            
    }
}
