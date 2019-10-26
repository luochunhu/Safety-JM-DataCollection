using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols.Devices
{
    /// <summary>
    /// 下发呼叫命令（上位机->设备）
    /// </summary>
    public class CallPersonRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 呼叫范围类型0-全员呼叫 1-卡号段呼叫 2-指定卡号呼叫  =4表示呼叫井下设备，此时后面的HJ-KH传输的时高字节表示分站号，低字节表示口号
        /// </summary>
        public  byte HJ_Type{ get; set; }
        /// <summary>
        /// 呼叫状态0-解除 1-一般呼叫 2-紧急呼叫
        /// </summary>
        public  Byte HJ_State{ get; set; }
        /// <summary>
        /// 呼叫的12个卡号和号段开始结束
        /// </summary>
        public ushort[] HJ_KH { get; set; }
    }
}
