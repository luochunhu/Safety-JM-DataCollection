using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    public class DeviceControlResponseCommand: UplinkCommand
    {
        public static DeviceControlResponseCommand ToCommand(byte[] data)
        {
            //效验CRC等
            //高低位 CRC效验等常用转换 辅助操作见  Sys.DataCollection.Common.Utils.CommandUtil 类

            DeviceControlResponseCommand command = new Commands.DeviceControlResponseCommand();

            return command;
        }
    }
}
