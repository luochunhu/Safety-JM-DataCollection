using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{   
    /// <summary>
    /// 设备信息（测点）
    /// </summary>
    public partial class DeviceAcrossControlInfo : CacheInfo
    {
        /// <summary>
        /// ID编号
        /// </summary>
        public string ID
        {
            get;
            set;
        }
        /// <summary>
        /// 主控测点号
        /// </summary>
        public string ZkPoint
        {
            get;
            set;
        }
        /// <summary>
        /// 控制类型,见数据库文档描述
        /// </summary>
        public short Type
        {
            get;
            set;
        }
        /// <summary>
        /// 被控测点号
        /// </summary>
        public string Bkpoint
        {
            get;
            set;
        }
        /// <summary>
        /// 上传标志0-未传1-已传
        /// </summary>
        public string Upflag
        {
            get;
            set;
        }
    }

}
