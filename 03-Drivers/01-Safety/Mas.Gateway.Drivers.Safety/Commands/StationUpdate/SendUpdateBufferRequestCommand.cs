using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 广播升级文件片段
    /// </summary>
    public class SendUpdateBufferRequestCommand : Sys.DataCollection.Common.Protocols.SendUpdateBufferRequest
    {
        public DeviceInfo def;
        public byte[] ToBytes()
        {
            byte[] buffer = new byte[269];
            int index = 0;

            buffer[index] = 0x7E;  //引导符
            index++;
            buffer[index] = 0xFC;//带地址传输
            index++;
            buffer[index] = (byte)(((buffer.Length - 4) >> 8) & 0xFF);   //长度高
            index++;
            buffer[index] = (byte)((buffer.Length - 4) & 0xFF);   //长度低
            index++;
            buffer[index] = 0x55;   //U命令；通用广播命令
            index++;
            buffer[index] = 0x00;   //标志位
            index++;
            buffer[index] = DeviceId;  //设备编码
            //硬件版本号	1 Byte
            index++;
            buffer[index] = HardVersion;
            //升级文件版本号	1 Byte
            index++;
            buffer[index] = FileVersion;
            //分片序号	2 Byte 高在前，低在后
            index++;
            buffer[index] = (byte)((NowBufferIndex >> 8) & 0xFF);
            index++;
            buffer[index] = (byte)(NowBufferIndex & 0xFF);
            //数据体	256Byte
            for (int i = 0; i < Buffer.Length; i++)
            {
                index++;
                buffer[index] = Buffer[i];
            }

            CommandUtil.AddSumToBytes(buffer, 1, buffer.Length-1 ,false);//累加和

            return buffer;
        }
    }
}
