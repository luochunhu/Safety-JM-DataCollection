using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 缓存对象基类
    /// </summary>
    public class CacheInfo //: Basic.Framework.Web.MasInfo
    {
        /// <summary>
        /// 缓存对象唯一主键（可以是Id、Code）
        /// </summary>
        public string UniqueKey { get; set; }
    }
}
