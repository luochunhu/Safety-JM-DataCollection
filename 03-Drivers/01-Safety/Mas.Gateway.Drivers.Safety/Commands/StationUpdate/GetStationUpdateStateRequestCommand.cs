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
    /// 获取分站的工作状态
    /// </summary>
    public class GetStationUpdateStateRequestCommand : Sys.DataCollection.Common.Protocols.GetStationUpdateStateRequest
    {
        public DeviceInfo def;
        public byte[] ToBytes()
        {
            byte[] buffer = new byte[9];
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
            buffer[index] = 0x08;  //状态标志位
            index++;
            buffer[index] = (byte)(GetSoftVersion + (GetUpdateState << 1) + (GetDevType << 2) + (GetHardVersion << 3));   //数据域标记字1

            CommandUtil.AddSumToBytes(buffer, 0, buffer.Length);//累加和

            return buffer;
        }
    }
}
