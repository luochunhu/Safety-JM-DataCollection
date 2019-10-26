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
    /// 用于修改传感器地址号命令处理
    /// </summary>
    public class ModificationDeviceAdressRequestCommand : ModificationDeviceAdressRequest
    {
        /// <summary>
        /// 传入的分站设备定义信息
        /// </summary>
        public DeviceInfo def = null;
        public byte[] ToBytes()
        {
            byte[] bytes = null;
            int SoleCoding = 0;
            bytes = new byte[18];
            bytes[0] = 0x5A;
            bytes[1] = 0xA5;
            bytes[2] = 0x7E;
            bytes[3] = 0xE7;
            bytes[4] = (byte)def.Fzh;
            bytes[5] = 0x58;
            CommandUtil.ConvertInt16ToByte((ushort)(bytes.Length - 4), bytes, 6);//长度,6,7        
            bytes[8] = (byte)(this.LastAcceptFlag + 0x04);
            bytes[9] = RandomCode;//随机数
            if (ModificationItems.Count > 0)
            {
                int tempInt = ModificationItems[0].DeviceType;//todo:服务端下发时填大类
                SoleCoding += (tempInt << 24); //设备大类
                tempInt = Convert.ToInt32(ModificationItems[0].SoleCoding.Substring(0, 4));
                SoleCoding += ((tempInt - 2018) << 19); //年
                tempInt = Convert.ToInt32(ModificationItems[0].SoleCoding.Substring(4, 2));
                SoleCoding += (tempInt << 15);  //月
                tempInt = Convert.ToInt32(ModificationItems[0].SoleCoding.Substring(6, 2));
                SoleCoding += (tempInt << 10);  //日
                tempInt = Convert.ToInt32(ModificationItems[0].SoleCoding.Substring(8, 4));
                SoleCoding += tempInt;  //序列号
                CommandUtil.ConvertInt32ToByte(Convert.ToUInt32(SoleCoding), bytes, 10);//610  11  12  13 
                bytes[14] = ModificationItems[0].BeforeModification;
                bytes[15] = ModificationItems[0].AfterModification;
            }
            CommandUtil.CRC16_CCITT(bytes, 4, bytes.Length - 2);//累加和
            return bytes;
        }
    }
}
