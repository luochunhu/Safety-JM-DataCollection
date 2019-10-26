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
    /// 播放列表管理
    /// </summary>
    public class PlayListControlRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        ///// <summary>
        ///// 分区操作类型（0新增、1修改、2删除）
        ///// </summary>
        //public int controlType;
        public InfoState InfoState { get; set; }
        /// <summary>
        /// 播放列表名称(新增\修改)
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 音乐musicId列表(新增)
        /// </summary>
        public List<MusicInfo> musicList { get; set; }

        /// <summary>
        /// 播放列表标识(修改、删除)
        /// </summary>
        public string plstId { get; set; }

    }
}
