using Basic.Framework.Web;
using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 添加分区
    /// </summary>
    public class PartitionControlRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {

        public InfoState InfoState { get; set; }
        ///// <summary>
        ///// 分区操作类型（0新增、1修改、2删除）
        ///// </summary>
        //public int controlType;

        /// <summary>
        /// 分区名称(新增、修改)
        /// </summary>
        public string zoneName { get; set; }
        /// <summary>
        /// 分区报警联动用户号码列表(新增、修改)
        /// </summary>
        public string almLinkUdn1 { get; set; }
        /// <summary>
        /// 分区报警联动用户号码列表(新增、修改)
        /// </summary>
        public string almLinkUdn2 { get; set; }
        /// <summary>
        /// 分区报警联动用户号码列表(新增、修改)
        /// </summary>
        public string almLinkUdn3 { get; set; }

        /// <summary>
        /// 分区标识(修改、删除)
        /// </summary>
        public string zoneId { get; set; }
        /// <summary>
        /// 分区广播接入号码（修改）
        /// </summary>
        public string paTaskDN { get; set; }
    }
}
