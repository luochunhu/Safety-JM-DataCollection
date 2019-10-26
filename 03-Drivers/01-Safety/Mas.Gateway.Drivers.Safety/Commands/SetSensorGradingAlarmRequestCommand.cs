using Sys.DataCollection.Common.Protocols.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Utils;
using Sys.DataCollection.Common.Commands;

namespace Sys.DataCollection.Driver.Commands
{
    /// <summary>
    /// 设备传感器分级响应功能
    /// </summary>
    public class SetSensorGradingAlarmRequestCommand: SetSensorGradingAlarmRequest
    {
        /// <summary>
        /// 设备分站信息
        /// </summary>
        public DeviceInfo def;
        public byte[] ToBytes()
        {
            byte[] bytes = null;
            byte startindex;
            bytes = new byte[14 + GradingAlarmItems.Count];
            bytes[0] = 0x3E;
            bytes[1] = 0xE3;
            bytes[2] = 0x80;
            bytes[3] = 0x08;
            bytes[4] = (byte)def.Fzh;
            bytes[5] = 0x56;
            CommandUtil.ConvertInt16ToByte((ushort)(bytes.Length - 4), bytes, 6,false);//长度,6,7        
            bytes[8] = (byte)(this.LastAcceptFlag);
            bytes[9] = (byte)GradingAlarmItems.Count;
            startindex = 10;
            if (GradingAlarmItems != null)
            {
                for (int i = 0; i < GradingAlarmItems.Count; i++)
                {
                    bytes[startindex++] = (byte)(byte.Parse(GradingAlarmItems[i].Channel) + (GradingAlarmItems[i].AlarmStep << 5));
                }
            }
            CommandUtil.AddSumToBytes(bytes, 4, bytes.Length);//累加和
            return bytes;
        }
    }
}
