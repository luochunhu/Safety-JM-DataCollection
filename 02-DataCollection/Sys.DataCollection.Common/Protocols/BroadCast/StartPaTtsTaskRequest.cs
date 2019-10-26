using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 启动文字广播任务
    /// </summary>
    public class StartPaTtsTaskRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
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
        /// 播放文字内容
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 重复次数（取值范围：1 - 16）
        /// </summary>
        public string repeatTimes { get; set; }
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
