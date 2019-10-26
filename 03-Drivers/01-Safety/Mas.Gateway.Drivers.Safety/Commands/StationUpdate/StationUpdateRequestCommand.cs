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
    /// 请求分站远程升级
    /// </summary>
    public class StationUpdateRequestCommand : Sys.DataCollection.Common.Protocols.StationUpdateRequest
    {
        public DeviceInfo def;
        public byte[] ToBytes()
        {
            byte[] buffer = new byte[19];
            int index = 0;

            buffer[index] = (byte)def.Fzh;  //分站号
            index++;
            buffer[index] = (byte)(((buffer.Length - 3) >> 8) & 0xFF);//长度高
            index++;
            buffer[index] = (byte)((buffer.Length - 3) & 0xFF);   //长度低
            index++;
            buffer[index] = 0x44;   //D命令
            index++;
            buffer[index] = 0x07;   //标志位4、3、2、1位 =3表示分站远程升级相关命令;第0位＝1表示接收正确
            index++;
            buffer[index] = 0x01;  //状态标志位
            index++;
            buffer[index] = DeviceId;   //设备编码	1 Byte
            index++;
            buffer[index] = HardVersion;    //硬件版本号	1 Byte
            index++;
            buffer[index] = FileVersion; //升级文件版本号	1 Byte
            //升级版本控制	2 Byte  
            index++;
            buffer[index] = maxVersion;
            index++;
            buffer[index] = minVersion;
            //升级文件总片数	2 Byte
            index++;
            buffer[index] = (byte)((FileCount >> 8) & 0xFF);
            index++;
            buffer[index] = (byte)(FileCount & 0xFF);
            //文件校验	4 Byte
            index++;
            buffer[index] = (byte)((Crc >> 24) & 0xFF);
            index++;
            buffer[index] = (byte)((Crc >> 16) & 0xFF);
            index++;
            buffer[index] = (byte)((Crc >> 8) & 0xFF);
            index++;
            buffer[index] = (byte)((Crc >> 0) & 0xFF);

            CommandUtil.AddSumToBytes(buffer, 0, buffer.Length);//累加和

            return buffer;
        }
    }
}
