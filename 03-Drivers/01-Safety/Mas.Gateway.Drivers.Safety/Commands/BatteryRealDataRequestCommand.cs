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
    public class BatteryRealDataRequestCommand : QueryBatteryRealDataRequest
    {
        public NetworkDeviceInfo NetMacObject = null;
        public DeviceInfo def;
        /// <summary>
        /// 表示命令版本
        /// </summary>
        public byte OrderVersion;
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[1];
            DeviceControlRequestCommand commandobject = new DeviceControlRequestCommand();
            if (DevProperty == ItemDevProperty.Switches)//表示对交换机电源箱的查找
            {
                bytes = GetBatteryBufferForNet();
            }
            else if (DevProperty == ItemDevProperty.Substation)
            {
                bytes = GetBatteryBuffer();
            }
            return bytes;
        }
        private byte[] GetBatteryBuffer()
        {
            byte[] bytes = new byte[14];
            bytes[0] = 0x3E;
            bytes[1] = 0xE3;
            bytes[2] = 0x80;
            bytes[3] = 0x08;
            bytes[4] = (byte)def.Fzh;
            bytes[5] = 0x55;
            CommandUtil.ConvertInt16ToByte((ushort)(bytes.Length - 4), bytes, 6,false);//长度,6,7        
            bytes[8] = (byte)(this.LastAcceptFlag);
            bytes[9] = (byte)(BatteryControl );
            bytes[9] += (byte)((PowerPercentum / 2) << 2);
            CommandUtil.AddSumToBytes(bytes, 4, bytes.Length);//累加和
            return bytes;
        }
        /// <summary>
        /// 交换机的电源获取
        /// </summary>
        /// <returns></returns>
        private byte[] GetBatteryBufferForNet()
        {
            //包含三部分，0是数据获取，2是放电操作，1是取消放电操作
            byte[] bytes;
            if(BatteryControl==1)//取消放电
            {//7F 10 01 01 00 02 1B EA 
                bytes = new byte[8] { 0x7F, 0x10, 0x01, 0x01, 0x00, 0x02, 0x1B, 0xEA };
            }
            else if (BatteryControl == 2)//放电
            {//7F 10 01 01 00 01 5B EB 
                bytes = new byte[8] { 0x7F, 0x10, 0x01, 0x01, 0x00, 0x01, 0x5B, 0xEB };
            }
            else//不动作，就直接 查询操作
            {//7F 03 01 15 29 AF 
                bytes = new byte[6] { 0x7F, 0x03, 0x01, 0x15, 0x29, 0xAF };
            }
            return bytes;
        }
    }
}
