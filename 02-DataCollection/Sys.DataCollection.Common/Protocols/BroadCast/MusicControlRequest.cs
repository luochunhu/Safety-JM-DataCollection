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
    /// 音乐管理
    /// </summary>
    public class MusicControlRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        ///// <summary>
        ///// 分区操作类型（0新增、1修改、2删除）
        ///// </summary>
        //public int controlType;
        public InfoState InfoState { get; set; }
        /// <summary>
        /// 音乐名称(新增)
        /// </summary>
        public string musicName { get; set; }
        /// <summary>
        /// 音乐文件资源路径(由上传接口返回)(新增)
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 音乐标识(删除)
        /// </summary>
        public string musicId { get; set; }
    }
}
