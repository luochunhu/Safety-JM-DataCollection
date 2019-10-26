using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Protocols.Devices;
using Sys.DataCollection.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    /// <summary>
    /// 响应分站4小时数据处理
    /// </summary>
    public class QueryHistoryRealDataRequestCommand: QueryHistoryRealDataRequest
    {
        /// <summary>
        /// 设备分站信息
        /// </summary>
        public DeviceInfo def;
        public byte[] ToBytes()
        {
            byte[] bytes = null;
            bytes = new byte[13];
            bytes[0] = 0x5A;
            bytes[1] = 0xA5;
            bytes[2] = 0x7E;
            bytes[3] = 0xE7;
            bytes[4] = (byte)def.Fzh;
            bytes[5] = 0x58;
            CommandUtil.ConvertInt16ToByte((ushort)(bytes.Length - 4), bytes, 6);//长度,6,7        
            bytes[8] = (byte)(this.LastAcceptFlag + (3 << 1));

            bytes[9] = (byte)(QueryMinute == 0 ? 48 : QueryMinute);
            bytes[10] = SerialNumber;
            CommandUtil.CRC16_CCITT(bytes, 4, bytes.Length - 2);//累加和
            return bytes;
        }
    }
}
