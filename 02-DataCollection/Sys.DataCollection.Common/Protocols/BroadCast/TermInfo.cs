using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 终端对象
    /// </summary>
    public class TermInfo
    {
        /// <summary>
        /// 终端号码
        /// </summary>
        public string termDN { get; set; }
        /// <summary>
        /// 终端名称
        /// </summary>
        public string termName { get; set; }
        /// <summary>
        /// 终端注册状态
        /// “online”  - 在线
        /// “offline” - 离线
        /// </summary>
        public string regState { get; set; }
        /// <summary>
        /// 终端呼叫状态
        ///“idle”    - 空闲
        ///“offhook” - 摘机
        ///“calling” - 呼叫
        ///“ring”    - 振铃 
        ///“alert”   - 回铃
        ///“talk”    - 通话
        ///“hold”    - 保持
        /// </summary>
        public string callState { get; set; }
    }
}
