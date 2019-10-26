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
    public class DeviceControlRequestCommand : DeviceControlRequest
    {
        /// <summary>
        /// 传入的分站设备定义信息
        /// </summary>
        public DeviceInfo def = null;
        /// <summary>
        /// 命令下发的字节数
        /// </summary>
        private int BufferLength = 12;
        /// <summary>
        /// 表示命令版本，扩展用
        /// </summary>
        public byte OrderVersion;
        public byte[] ToBytes()
        {
            byte[] bytes = null;
            bytes = GetControlBufferCapacity();
            CommandUtil.AddSumToBytes(bytes, 4, bytes.Length);//累加和
            return bytes;
        }
        /// <summary>
        /// 智能分站的F命令获取
        /// </summary>
        /// <returns></returns>
        private byte[] GetControlBufferCapacity()
        {
            byte[] bytes = null;
            BufferLength = 15;
      
            bytes = new byte[BufferLength];
            bytes[0] = 0x3E;
            bytes[1] = 0xE3;
            bytes[2] = 0x80;
            bytes[3] = 0x08;
            bytes[4] = (byte)def.Fzh;
            bytes[5] = 0x46;
            CommandUtil.ConvertInt16ToByte((ushort)(BufferLength - 4), bytes, 6,false);//长度,6,7
            Buffer.BlockCopy(BitConverter.GetBytes(GetSignByte()), 0, bytes, 8, 1);//标志  8
            CommandUtil.ConvertInt16ToByte(GetIntelligentByte(1), bytes, 9,false);//控制字9,10
            CommandUtil.ConvertInt16ToByte(GetIntelligentByte(2), bytes, 11, false);//强制断电11 12

            return bytes;
        }
        /// <summary>
        /// 得到下发的标志控制字节
        /// </summary>      
        /// <returns></returns>
        private byte GetSignByte()
        {
            byte Sign = 0;
            //ClearHistoryData = 1;//todo:要带起。通过判断F命令里面有没有历史数据来弄
            Sign += (byte)(LastAcceptFlag & 0x01);//接收标记,未进行标记的赋值[由服务端进行控制，以下发置为false,以接收到数据为true]
            Sign += (byte)(ClearHistoryData << 1);//表示清除历史数据
            Sign += (byte)(GasThreeUnlockContro << 2);//表示3分强制解锁标记
            Sign += (byte)(GetHistoryData << 3);//表示获取历史数据  

            return Sign;
        }
        /// <summary>
        /// 获取智断电器的控制字节
        /// =1表示断电，=2表示强制复电
        /// </summary>
        /// <returns></returns>
        private ushort GetIntelligentByte(byte conType)
        {
            ushort Intelligent = 0;
            if (ControlChanels != null)
            {
                for (int i = 0; i < ControlChanels.Count; i++)
                {
                    if (ControlChanels[i].ControlType == conType)
                        Intelligent |= (ushort)(1<< (ControlChanels[i].Channel - 1));
                }
            }
            return Intelligent;
        }
        /// <summary>
        /// 得到唯一标识编码确认字节
        /// </summary>
        /// <returns></returns>
        private UInt32 GetSoleCoding()
        {
            UInt32 SoleCoding = 0;
            if (SoleCodingChanels != null)
            {
                for (int i = 0; i < SoleCodingChanels.Count; i++)
                {
                    SoleCoding |= (UInt32)(SoleCodingChanels[i].ControlType << (SoleCodingChanels[i].Channel - 1));
                }
            }
            return SoleCoding;
        }
    }
}
