using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 启动喊话广播任务
    /// </summary>
    public class StartPaVoiceTaskRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 控制话机号码
        /// </summary>
        public string agentDN { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 广播音量（取值范围：1 - 9）
        /// </summary>
        public string outVol { get; set; }
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
