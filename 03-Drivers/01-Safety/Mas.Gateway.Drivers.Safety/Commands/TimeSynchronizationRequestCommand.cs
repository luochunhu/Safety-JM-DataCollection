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
    public class TimeSynchronizationRequestCommand: TimeSynchronizationRequest
    {        /// <summary>
             /// 设备分站信息
             /// </summary>
        public DeviceInfo def;
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[19];
            int j = 0;
            bytes[0] = 0x3E;
            bytes[1] = 0xE3;
            bytes[2] = 0x80;
            bytes[3] = 0x08;
            bytes[4] = (byte)def.Fzh;
            bytes[5] = 0x50;
            CommandUtil.ConvertInt16ToByte(15, bytes, 6,false);//长度,6,7    
            bytes[8] = LastAcceptFlag;

            j = (ushort)(this.SyncTime.Year - 2000);
            bytes[9] = (byte)j;
            bytes[10] = (byte)(this.SyncTime.Month);
            bytes[10] += (Byte)(((byte)this.SyncTime.DayOfWeek) << 5);
            bytes[11] = (byte)this.SyncTime.Day;
            bytes[12] = (byte)this.SyncTime.Hour;
            bytes[13] = (byte)this.SyncTime.Minute;
            bytes[14] = (byte)this.SyncTime.Second;
            CommandUtil.AddSumToBytes(bytes, 4, bytes.Length);//累加和
            return bytes;
        }
    }
}
