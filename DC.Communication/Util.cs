using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Net;

namespace DC.Communication.Components
{
    /// <summary>
    /// 协议相关协助类
    /// </summary>
    public static class Util
    {
        #region 协议封装
        public static byte NetNetModelLjl(byte[] data)
        {
            byte ljh = 0;
            for (int i = 1; i < data.Length; i++)
            {
                ljh += data[i];
            }
            return ljh;
        }
        public static byte[] User = { 0x61, 0x64, 0x6D, 0x69, 0x6E, 0x00 };
        public static byte[] PassWord = { 0x61, 0x64, 0x6D, 0x69, 0x6E, 0x00 };
   
        /// <summary>
        /// 根据协议类型获取byte[]数组 通用协议转换 
        /// </summary>
        /// <param name="protocolType"></param>
        /// <param name="mac"></param>
        /// <returns></returns>
        public static byte[] GetDataByType(string protocolType, string mac)
        {
            byte[] data = new byte[1];
            byte[] macBytes = new byte[1];
            switch (protocolType)
            {
                case "10"://搜索交换机命令
                    data = new byte[208];
                    data[0] = 0x7F;
                    data[1] = 0x10;
                    data[2] = 0x03;
                    data[3] = 0x00;
                    data[4] = 0xD0;
                    data[5] = 0x11;
                    data[6] = 0x06;
                    data[205] = 0x97;
                    data[206] = 0x45;
                    data[207] = 0x0D;
                    break;
                case "11"://得到交换机的运行状态SWRALLPORTLINKSTATE                          
                    data = new byte[25];//53 57 52 41 4C 4C 50 4F 52 54 4C 49 4E 4B 53 54 41 54 45 00 22 6F 0C E7 07 
                    macBytes = System.Text.Encoding.Default.GetBytes("SWRALLPORTLINKSTATE");
                    Buffer.BlockCopy(macBytes, 0, data, 0, macBytes.Length);
                    Util.ConvertMacToByte(mac, data, 19);//3,4,5,6,7,8
                    break;
                case "01"://搜索分站命令
                    data = new byte[12];
                    data[0] = 0x3E;
                    data[1] = 0xE3;
                    data[2] = 0x80;
                    data[3] = 0x08;
                    data[4] = 0;
                    data[5] = 0x4C;
                    data[6] = 0x08;
                    data[7] = 0x00;
                    data[8] = 0;
                    data[9] = 0;
                    data[10] = 0x54;
                    data[11] = 0x00;
                    break;
                case "02"://复位命令
                    data = new byte[22];
                    data[0] = 0xFF;
                    data[1] = 0x13;
                    data[2] = 0x02;
                    Util.ConvertMacToByte(mac, data, 3);//3,4,5,6,7,8
                    Buffer.BlockCopy(Util.User, 0, data, 9, Util.User.Length);//9,10,11,12,13,14
                    Buffer.BlockCopy(Util.PassWord, 0, data, 15, Util.User.Length);//15,16,17,18,19,20
                    data[21] = Util.NetNetModelLjl(data);
                    break;
                
            }

            return data;
        }

        /// <summary>
        /// 设置最后两位为效验数据
        /// </summary>
        /// <param name="data"></param>
        public static void AddCrcToBytes(byte[] data)
        {
            ushort xx = CrcCheck(data, 0, (ushort)(data.Length - 2)); // udp_data       
            data[data.Length - 1] = (byte)(xx & 0xff);
            xx >>= 8;
            data[data.Length - 2] = (byte)(xx & 0xff);
        }

        /// <summary>
        /// 设置最后两位为效验数据,
        /// </summary>
        /// <param name="data"></param>
        public static void AddCrcToBytes(byte[] data,int offset,int length)
        {
            ushort xx = CrcCheck(data, (ushort)offset, (ushort)(length)); // udp_data       
            data[data.Length - 1] = (byte)(xx & 0xff);
            xx >>= 8;
            data[data.Length - 2] = (byte)(xx & 0xff);
        }


        /// <summary>
        /// CRC效验算法
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="xx"></param>
        /// <param name="gs"></param>
        /// <returns></returns>
        public static ushort CrcCheck(byte[] buf, ushort xx, ushort gs)
        {
            byte bl, dl;
            ushort i;
            ushort eax = 0;
            for (i = 0; i < gs; i++)
            {
                bl = buf[xx + i];
                dl = 0x80;
                while (dl > 0)
                {
                    if ((eax & 0x8000) > 0)
                    {
                        eax += eax;
                        eax ^= 0x1021;
                    }
                    else
                    {
                        eax += eax;
                    }
                    if ((bl & dl) > 0)
                    {
                        eax ^= 0x1021;
                    }
                    dl >>= 1;
                }
            }
            return eax;
        }

        /// <summary>
        /// 备用使用。
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="startindex"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static ushort CRC16(byte[] buf, UInt16 startindex, int len)
        {
            if (len > 65535) return 0;
            UInt16 wCrc = 0xFFFF;
            UInt16 k, j = 0;
            UInt16 i;

            for (i = startindex; i < len; i++)
            {
                j = (UInt16)buf[i];
                wCrc ^= j;
                for (k = 0; k < 8; k++)
                {
                    if ((wCrc & 1) > 0)
                    {
                        wCrc >>= 1;
                        wCrc ^= 0xA001;
                    }
                    else
                    {
                        wCrc >>= 1;
                    }
                }

            }
            return wCrc;
        }


        #endregion

        #region 相关转换方法

        /// <summary>
        /// 转换MAC地址为byte[]数组
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public static byte[] ConvertMacToByte(string mac, byte[] data, int offSet)
        {
            mac = mac.Replace(":", "");
            mac = mac.Replace("-", "");
            mac = mac.Replace(".", "");

            byte[] macBytes = new byte[mac.Length / 2];
            for (int i = 0; i < macBytes.Length; i++)
            {
                macBytes[i] = Convert.ToByte(mac.Substring(i * 2, 2), 16);
            }

            if (data != null && data.Length >= macBytes.Length && (data.Length - offSet) >= macBytes.Length)
            {
                Buffer.BlockCopy(macBytes, 0, data, offSet, macBytes.Length);
            }

            return macBytes;
        }

        /// <summary>
        /// 转换byte[]数组为MAC地址
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public static string ConvertByteToMac(byte[] data, int offSet)
        {
            return BitConverter.ToString(data, offSet, 6);
        }

        /// <summary>
        /// byte[]数组 转换为IP
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public static string ConvertByteToIP(byte[] data, int offSet,bool reset=false)
        {
            byte[] ipBytes = new byte[4];
            Buffer.BlockCopy(data, offSet, ipBytes, 0, ipBytes.Length);
            if(reset)
                Array.Reverse(ipBytes, 0, ipBytes.Length);
            IPAddress address = new IPAddress(ipBytes);
            return address.ToString();
        }
        /// <summary>
        /// Ascll码转IP--串口配置
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public static string ConvertByteToIpForAscll(byte[] data, int offSet)
        {
            byte countByte = 0;
            for (int i = 0; i < data.Length;i++ )
            {
                if (data[offSet + i] != 0)
                    countByte++;
                else
                    break;
                if (countByte >= 30) break;//最多30个字节
            }
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            string address = asciiEncoding.GetString(data, offSet, countByte);
            return address;
        }
        /// <summary>
        /// IP码转ASCLL码字节数组--串口配置
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public static byte[] ConvertIpToAscllByte(string IP, byte[] data, int offSet)
        {
            byte[] ipBytes = new byte[IP.Length];
            for (int i = 0; i < ipBytes.Length; i++)
            {
                ipBytes[i] = Convert.ToByte(IP[i]);
            }
            Buffer.BlockCopy(ipBytes, 0, data, offSet, ipBytes.Length);

            return data;
        }
        /// <summary>
        /// IP转换为byte[]数组
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        public static byte[] ConvertIPToByte(string ip, byte[] data, int offSet,bool reset=false)
        {
            IPAddress addr = IPAddress.Parse(ip);
            byte[] ipBytes = addr.GetAddressBytes();
            if(reset)
                Array.Reverse(ipBytes, 0, ipBytes.Length);
            if (data != null && data.Length >= ipBytes.Length && (data.Length - offSet) >= ipBytes.Length)
            {
                Buffer.BlockCopy(ipBytes, 0, data, offSet, ipBytes.Length);
            }

            return ipBytes;
        }

        /// <summary>
        /// byte[]数组转换int32  高在前，低在后
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        /// <param name="Reset">True默认是高在前低在后</param>
        /// <returns></returns>
        public static int ConvertByteToInt(byte[] data, int offSet)
        {          
           return BitConverter.ToInt32(data, offSet);
        }

        public static Int16 ConvertByteToInt16(byte[] data, int offSet)
        { 
            return BitConverter.ToInt16(data, offSet);
        }


        /// <summary>
        /// int32转换为byte[]数组
        /// </summary>
        /// <param name="number"></param>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        public static byte[] ConvertIntToByte(uint number, byte[] data, int offSet,bool reset=false)
        {
            byte[] numberBytes = BitConverter.GetBytes(number);
            if (reset)//高在前，低在后
                Array.Reverse(numberBytes, 0, numberBytes.Length);

            if (data != null && data.Length >= numberBytes.Length && (data.Length - offSet) >= numberBytes.Length)
            {
                Buffer.BlockCopy(numberBytes, 0, data, offSet, numberBytes.Length);
            }

            return numberBytes;
        }

        /// <summary>
        /// int32转换为byte[]数组
        /// </summary>
        /// <param name="number"></param>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        public static byte[] ConvertInt16ToByte(ushort number, byte[] data, int offSet,bool reset=false)
        {
            byte[] numberBytes = BitConverter.GetBytes(number);
            if (reset)
                Array.Reverse(numberBytes, 0, numberBytes.Length);
            // byte[] numberBytes = BitConverter.GetBytes(number).Reverse().ToArray();//先做反转，再做复制

            if (data != null && data.Length >= numberBytes.Length && (data.Length - offSet) >= numberBytes.Length)
            {
                Buffer.BlockCopy(numberBytes, 0, data, offSet, numberBytes.Length);
            }

            return numberBytes;
        }


        #endregion
       
    }

    public class CRC32
    {
        static UInt32[] crcTable =  
        {  
          0x00000000, 0x04c11db7, 0x09823b6e, 0x0d4326d9, 0x130476dc, 0x17c56b6b, 0x1a864db2, 0x1e475005,  
          0x2608edb8, 0x22c9f00f, 0x2f8ad6d6, 0x2b4bcb61, 0x350c9b64, 0x31cd86d3, 0x3c8ea00a, 0x384fbdbd,  
          0x4c11db70, 0x48d0c6c7, 0x4593e01e, 0x4152fda9, 0x5f15adac, 0x5bd4b01b, 0x569796c2, 0x52568b75,  
          0x6a1936c8, 0x6ed82b7f, 0x639b0da6, 0x675a1011, 0x791d4014, 0x7ddc5da3, 0x709f7b7a, 0x745e66cd,  
          0x9823b6e0, 0x9ce2ab57, 0x91a18d8e, 0x95609039, 0x8b27c03c, 0x8fe6dd8b, 0x82a5fb52, 0x8664e6e5,  
          0xbe2b5b58, 0xbaea46ef, 0xb7a96036, 0xb3687d81, 0xad2f2d84, 0xa9ee3033, 0xa4ad16ea, 0xa06c0b5d,  
          0xd4326d90, 0xd0f37027, 0xddb056fe, 0xd9714b49, 0xc7361b4c, 0xc3f706fb, 0xceb42022, 0xca753d95,  
          0xf23a8028, 0xf6fb9d9f, 0xfbb8bb46, 0xff79a6f1, 0xe13ef6f4, 0xe5ffeb43, 0xe8bccd9a, 0xec7dd02d,  
          0x34867077, 0x30476dc0, 0x3d044b19, 0x39c556ae, 0x278206ab, 0x23431b1c, 0x2e003dc5, 0x2ac12072,  
          0x128e9dcf, 0x164f8078, 0x1b0ca6a1, 0x1fcdbb16, 0x018aeb13, 0x054bf6a4, 0x0808d07d, 0x0cc9cdca,  
          0x7897ab07, 0x7c56b6b0, 0x71159069, 0x75d48dde, 0x6b93dddb, 0x6f52c06c, 0x6211e6b5, 0x66d0fb02,  
          0x5e9f46bf, 0x5a5e5b08, 0x571d7dd1, 0x53dc6066, 0x4d9b3063, 0x495a2dd4, 0x44190b0d, 0x40d816ba,  
          0xaca5c697, 0xa864db20, 0xa527fdf9, 0xa1e6e04e, 0xbfa1b04b, 0xbb60adfc, 0xb6238b25, 0xb2e29692,  
          0x8aad2b2f, 0x8e6c3698, 0x832f1041, 0x87ee0df6, 0x99a95df3, 0x9d684044, 0x902b669d, 0x94ea7b2a,  
          0xe0b41de7, 0xe4750050, 0xe9362689, 0xedf73b3e, 0xf3b06b3b, 0xf771768c, 0xfa325055, 0xfef34de2,  
          0xc6bcf05f, 0xc27dede8, 0xcf3ecb31, 0xcbffd686, 0xd5b88683, 0xd1799b34, 0xdc3abded, 0xd8fba05a,  
          0x690ce0ee, 0x6dcdfd59, 0x608edb80, 0x644fc637, 0x7a089632, 0x7ec98b85, 0x738aad5c, 0x774bb0eb,  
          0x4f040d56, 0x4bc510e1, 0x46863638, 0x42472b8f, 0x5c007b8a, 0x58c1663d, 0x558240e4, 0x51435d53,  
          0x251d3b9e, 0x21dc2629, 0x2c9f00f0, 0x285e1d47, 0x36194d42, 0x32d850f5, 0x3f9b762c, 0x3b5a6b9b,  
          0x0315d626, 0x07d4cb91, 0x0a97ed48, 0x0e56f0ff, 0x1011a0fa, 0x14d0bd4d, 0x19939b94, 0x1d528623,  
          0xf12f560e, 0xf5ee4bb9, 0xf8ad6d60, 0xfc6c70d7, 0xe22b20d2, 0xe6ea3d65, 0xeba91bbc, 0xef68060b,  
          0xd727bbb6, 0xd3e6a601, 0xdea580d8, 0xda649d6f, 0xc423cd6a, 0xc0e2d0dd, 0xcda1f604, 0xc960ebb3,  
          0xbd3e8d7e, 0xb9ff90c9, 0xb4bcb610, 0xb07daba7, 0xae3afba2, 0xaafbe615, 0xa7b8c0cc, 0xa379dd7b,  
          0x9b3660c6, 0x9ff77d71, 0x92b45ba8, 0x9675461f, 0x8832161a, 0x8cf30bad, 0x81b02d74, 0x857130c3,  
          0x5d8a9099, 0x594b8d2e, 0x5408abf7, 0x50c9b640, 0x4e8ee645, 0x4a4ffbf2, 0x470cdd2b, 0x43cdc09c,  
          0x7b827d21, 0x7f436096, 0x7200464f, 0x76c15bf8, 0x68860bfd, 0x6c47164a, 0x61043093, 0x65c52d24,  
          0x119b4be9, 0x155a565e, 0x18197087, 0x1cd86d30, 0x029f3d35, 0x065e2082, 0x0b1d065b, 0x0fdc1bec,  
          0x3793a651, 0x3352bbe6, 0x3e119d3f, 0x3ad08088, 0x2497d08d, 0x2056cd3a, 0x2d15ebe3, 0x29d4f654,  
          0xc5a92679, 0xc1683bce, 0xcc2b1d17, 0xc8ea00a0, 0xd6ad50a5, 0xd26c4d12, 0xdf2f6bcb, 0xdbee767c,  
          0xe3a1cbc1, 0xe760d676, 0xea23f0af, 0xeee2ed18, 0xf0a5bd1d, 0xf464a0aa, 0xf9278673, 0xfde69bc4,  
          0x89b8fd09, 0x8d79e0be, 0x803ac667, 0x84fbdbd0, 0x9abc8bd5, 0x9e7d9662, 0x933eb0bb, 0x97ffad0c,  
          0xafb010b1, 0xab710d06, 0xa6322bdf, 0xa2f33668, 0xbcb4666d, 0xb8757bda, 0xb5365d03, 0xb1f740b4  
        };

        public static uint GetCRC32(byte[] bytes)
        {
            uint iCount = (uint)bytes.Length;
            uint crc = 0xFFFFFFFF;

            for (uint i = 0; i < iCount; i++)
            {
                crc = (crc << 8) ^ crcTable[(crc >> 24) ^ bytes[i]];
               // crc = (crc << 8) ^ crcTable[((crc >> 24) ^ bytes[i]) & 0xFF];        
            }

            return crc;
        }
    }
}
