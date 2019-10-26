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
    /// 巡检单台分站的文件接收情况
    /// </summary>
    public class InspectionRequestCommand : Sys.DataCollection.Common.Protocols.InspectionRequest
    {
        public DeviceInfo def;
        public byte[] ToBytes()
        {
            byte[] buffer = new byte[13 + 256];
            if (LostFileNum == 0)
            {
                buffer = new byte[13];
            }
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
            buffer[index] = 0x04;  //状态标志位4中心站巡检分站的文件接收情况
            index++;
            buffer[index] = DeviceId;    //设备编码
            index++;
            buffer[index] = HardVersion;   //硬件版本号
            index++;
            buffer[index] = FileVersion;     //升级文件版本号
            index++;
            buffer[index] = (byte)((LostFileNum >> 8) & 0xFF);      //缺失文件编号 高
            index++;
            buffer[index] = (byte)(LostFileNum & 0xFF);   //缺失文件编号 低
            if (LostFileNum > 0)
            {
                for (int i = 0; i < FileBuffer.Length; i++) //文件数据体
                {
                    index++;
                    buffer[index] = FileBuffer[i];
                }
            }

            CommandUtil.AddSumToBytes(buffer, 0, buffer.Length);//累加和

            return buffer;
        }
    }
}
