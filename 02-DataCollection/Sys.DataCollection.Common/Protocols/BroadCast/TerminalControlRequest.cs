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
    /// 终端管理
    /// </summary>
    public class TerminalControlRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        ///// <summary>
        ///// 端操作类型（0新增、1修改、2删除）
        ///// </summary>
        //public int controlType;
        public InfoState InfoState { get; set; }

        /// <summary>
        /// 归属分区标识(新增、修改、删除)
        /// </summary>
        public string zoneId { get; set; }
        /// <summary>
        /// 终端号码(新增、修改、删除)
        /// </summary>
        public string termDN { get; set; }
        /// <summary>
        /// 0:normal-普通终端 1:operator-对讲主机 2:mst-融合终端 3:pai-扩音终端  (新增、修改)
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 终端名称(新增、修改)
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 录音使能是否启用(新增、修改)
        /// </summary>
        public string record { get; set; }
        /// <summary>
        /// 注册鉴权是否启用(新增、修改)
        /// </summary>
        public string auth { get; set; }
        /// <summary>
        /// 注册鉴权启用则必选(新增、修改)
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 广播使能是否启用(新增、修改)
        /// </summary>
        public string pa { get; set; }
        /// <summary>
        /// 始终呼叫转移号码(新增、修改)
        /// </summary>
        public string cfuDN { get; set; }
        /// <summary>
        /// 条件呼叫转移号码(新增、修改)
        /// </summary>
        public string cfxDN { get; set; }

    }
}
