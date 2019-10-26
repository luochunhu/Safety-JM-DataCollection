using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    public class GetSwitchboardParamCommRequestCommand : GetSwitchboardParamCommRequest
    {
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[11];
            bytes[0] = 0x5A;
            bytes[1] = 0xA5;
            bytes[2] = 0x7E;
            bytes[3] = 0xE7;
            bytes[4] = 252;
            bytes[5] = 0x46;
            CommandUtil.ConvertInt16ToByte((ushort)(bytes.Length - 4), bytes, 6);//长度,6,7
            bytes[8] = 0;
            CommandUtil.CRC16_CCITT(bytes, 4, bytes.Length - 2);//累加和
            return bytes;
        }
    }
}
