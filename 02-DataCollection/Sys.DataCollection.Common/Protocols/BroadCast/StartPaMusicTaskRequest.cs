using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    ///  启动音乐广播任务
    /// </summary>
    public class StartPaMusicTaskRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 广播音量（取值范围：1 - 9）
        /// </summary>
        public string outVol { get; set; }
        /// <summary>
        /// 播放列表标识
        /// </summary>
        public string plstId { get; set; }
        /// <summary>
        /// 播放模式( loop: 循环 , random: 顺序)
        /// </summary>
        public string playMode { get; set; }
        /// <summary>
        /// 持续时长(分钟)
        /// </summary>
        public string duration { get; set; }
        /// <summary>
        /// 接收终端列表
        /// </summary>
        public List<TermListInfo> termList { get; set; }
        /// <summary>
        /// 终端归属分区标识
        /// </summary>
        public string zoneDN { get; set; }
        /// <summary>
        /// 接收终端号码
        /// </summary>
        public string termDN { get; set; }

    }
}
