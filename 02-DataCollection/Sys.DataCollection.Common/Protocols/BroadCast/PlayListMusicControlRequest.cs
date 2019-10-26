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
    /// 播放列表音乐管理
    /// </summary>
    public class PlayListMusicControlRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        ///// <summary>
        ///// 分区操作类型（0新增、2删除）
        ///// </summary>
        //public int controlType;
        public InfoState InfoState { get; set; }
        /// <summary>
        /// 播放列表标识(新增、删除)
        /// </summary>
        public string plstId { get; set; }
        /// <summary>
        /// 音乐musicId列表(新增、删除)
        /// </summary>
        public List<MusicInfo> musicList { get; set; }

    }
}
