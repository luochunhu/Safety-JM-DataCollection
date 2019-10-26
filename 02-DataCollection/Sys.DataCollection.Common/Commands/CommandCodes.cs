using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Commands
{
    /// <summary>
    /// 命令列表
    /// </summary>
    public class CommandCodes
    {
        //I(0x49) 初始化指令
        //F(0x46) 取数命令
        //T(0x54) 通讯测试命令
        //S(0x53) 时间同步命令
        //R(0x52) 复位命令
        //L(0x4C) 远程校正命令
        //D(0x44) 取历史报警数据记录，通过手动触发
        //D(0x44) 读取电源箱的状态，通过手动触发，通过复用D命令标记来实现。


        public const byte ControlCommand = 0x43;  //举例     
        public const byte DataQueryCommand = 0x46;//F 我给看吧令
        public const byte InitializeCommand = 0x49;
        public const byte CommunicationTestCommand = 0x54;
        public const byte ExtendCommand = 0x44;//
        public const byte ResetCommand = 0x52;//
        public const byte TimeSynCommand = 0x53;//
        public const byte CallPersonCommand = 0x48;//
        public const byte BroadCommand = 0x55;//
    }
}
