using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    public class ResetDeviceRequestCommand: ResetDeviceCommandRequest
    {
        public DeviceInfo def;
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[13];
            bytes[0] = 0x3E;
            bytes[1] = 0xE3;
            bytes[2] = 0x80;
            bytes[3] = 0x08;
            bytes[4] = (byte)def.Fzh;
            bytes[5] = 0x52;
            CommandUtil.ConvertInt16ToByte((ushort)(13 - 4), bytes, 6,false);//长度,6,7  
            bytes[8] = (byte)(this.LastAcceptFlag );
            CommandUtil.AddSumToBytes(bytes, 4, bytes.Length);//累加和
            return bytes;
        }
    }
}
