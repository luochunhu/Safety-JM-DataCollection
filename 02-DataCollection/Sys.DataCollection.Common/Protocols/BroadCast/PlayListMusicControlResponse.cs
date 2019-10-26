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
    public class PlayListMusicControlResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 响应代码, 0表示成功, 其他表示失败
        /// </summary>
        public string retCode { get; set; }
    }
}
