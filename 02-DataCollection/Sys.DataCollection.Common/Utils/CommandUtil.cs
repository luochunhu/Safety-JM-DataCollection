using Sys.DataCollection.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Utils
{

    /// <summary>
    /// 协议相关协助类
    /// </summary>
    public static class CommandUtil
    {

        /// <summary>
        ///         CRC检验采用CCITT-1021的方式。其算法程序如下：
        //输入：	* puchMsg为待计算CRC的数据缓存。
        //        start 为* puchMsg中需要计算CRC的数据的开始地址。
        //        end   为* puchMsg中需要计算CRC的数据的结束地址加一。
        //输出：	返回数据为unsigned short型数据。
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static ushort CRC16_CCITT(byte[] data, int start, int end)
        {
            ushort wCRCin = 0x0000;
            ushort wCPoly = 0x1021;
            ushort wChar = 0;
            int i, j;
            for (i = start; i < end; i++)
            {
                wChar = (ushort)data[i];
                wCRCin ^= (ushort)(wChar << 8);
                for (j = 0; j < 8; j++)
                {
                    if ((wCRCin & 0x8000) == 0x8000)
                        wCRCin = (ushort)((wCRCin << 1) ^ wCPoly);
                    else
                        wCRCin = (ushort)(wCRCin << 1);
                }
            }
            data[end] = (byte)(wCRCin >> 8);
            data[end + 1] = (byte)wCRCin;
            return wCRCin;
        }
        /// <summary>
        /// 传入数组计算累加和
        /// </summary>
        /// <param name="data"></param>
        public static ushort AddSumToBytes(byte[] data, int offset, int length, bool isReverse = true)
        {
            ushort SumVal = data[offset++];
            ushort rlst = 0;
            for (; offset < length - 2; offset++)
                SumVal += data[offset];
            rlst = SumVal;
            if (isReverse)
            {
                data[data.Length - 2] = (byte)(SumVal & 0xff);
                SumVal >>= 8;
                data[data.Length - 1] = (byte)(SumVal & 0xff);
            }
            else
            {
                data[data.Length - 1] = (byte)(SumVal & 0xff);
                SumVal >>= 8;
                data[data.Length - 2] = (byte)(SumVal & 0xff);
            }
            return rlst;
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
        /// 设置最后两位为效验数据
        /// </summary>
        /// <param name="data"></param>
        public static void AddCrcToBytes(byte[] data, int offset, int length)
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

        public static ushort CRC16(byte[] buf, UInt16 startindex, int len)
        {
            if (len > 65535) return 0;
            UInt16 wCrc = 0xFFFF;
            UInt16 k, j = 0;
            UInt16 i;

            for (i = startindex; i < len; i++)
            {
                j = (UInt16)buf[i];
                j &= 255;
                wCrc ^= j;
                for (k = 0; k < 8; k++)
                {
                    if ((wCrc & 1) == 1)
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

        /// <summary>
        /// 获取指定IP下分站队列的挂接情况
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        /// 
        //public static byte[] GetNetDeviceNumber(NetworkDeviceInfo mac)
        //{
        //    string[] strfzh = null;
        //    string str = mac.Bz1;
        //    byte[] deviceNumber = new byte[8];
        //    for (int i = 0; i < deviceNumber.Length; i++)
        //    {
        //        deviceNumber[i] = 0;
        //    }
        //    if (!string.IsNullOrEmpty(str))
        //    {
        //        strfzh = str.Split('|');
        //        for (int i = 0; i < strfzh.Length; i++)
        //        {
        //            if (i > 7)
        //            {
        //                break;
        //            }
        //            deviceNumber[i] = byte.Parse(strfzh[i]);
        //        }
        //    }
        //    return deviceNumber;
        //}

        /// <summary>
        /// 判断网络模块是否有分站
        /// </summary>
        /// <param name="net">传入的网络模块</param>
        /// <returns></returns>
        public static bool CheckNetHaveFzh(NetworkDeviceInfo net)
        {
            bool result = false;
            if (net.Bz1 != null && net.Bz1.Trim() != "")
            {
                string[] strValue = net.Bz1.Split(';');
                string[] strFzh = null;
                for (int i = 0; i < strValue.Length; i++)
                {
                    strFzh = strValue[i].Split('|');
                    for (int j = 0; j < strFzh.Length; j++)
                    {
                        if (Convert.ToInt32(strFzh[j]) > 0)
                        {
                            result = true;
                            break;
                        }
                    }
                    if (result) break;
                }
            }
            return result;
        }

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
        public static string ConvertByteToIP(byte[] data, int offSet)
        {
            byte[] ipBytes = new byte[4];
            Buffer.BlockCopy(data, offSet, ipBytes, 0, ipBytes.Length);
            IPAddress address = new IPAddress(ipBytes);
            return address.ToString();
        }

        /// <summary>
        /// IP转换为byte[]数组
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        public static byte[] ConvertIPToByte(string ip, byte[] data, int offSet)
        {
            IPAddress addr = IPAddress.Parse(ip);
            byte[] ipBytes = addr.GetAddressBytes();

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
        /// <returns></returns>
        public static UInt32 ConvertByteToInt(byte[] data, int offSet, bool isReverse = true)
        {
            byte[] tempbuffer = new byte[4];
            Buffer.BlockCopy(data, offSet, tempbuffer, 0, 4);
            if (isReverse)
                Array.Reverse(tempbuffer, 0, 4);//先做反转，再做转换, 高在前，低在后
            return BitConverter.ToUInt32(tempbuffer, 0);
        }

        public static DateTime ConvertByteToDate(byte[] data, int offset)
        {
            DateTime rdatetime = new DateTime(1900,01,01);
            try
            {
                UInt32 bdata = ConvertByteToInt(data, offset, false);
                /*D31--D26	D25--D22	D21--D17	D16--D12	D11--D6 	D5--D0
                     年        月          日         小时         分         秒
                    */
                rdatetime = new DateTime((int)(bdata >> 26) + 2000, (int)((bdata >> 22) & 0x0F), (int)((bdata >> 17) & 0x1F), (int)((bdata >> 12) & 0x1F), (int)((bdata >> 6) & 0x3F), (int)(bdata & 0x3F));
            }
            catch { }
            return rdatetime;
        }
        public static ushort ConvertByteToInt16(byte[] data, int offSet, bool isReverse = true)
        {
            byte[] tempbuffer = new byte[2];
            Buffer.BlockCopy(data, offSet, tempbuffer, 0, 2);
            if (isReverse)
            {
                Array.Reverse(tempbuffer, 0, 2);//先做反转，再做转换, 高在前，低在后 0x01 0x20  =0x0120  0x2001
            }
            return BitConverter.ToUInt16(tempbuffer, 0);
        }


        /// <summary>
        /// int32转换为byte[]数组
        /// </summary>
        /// <param name="number"></param>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        /// <param name="isReverse">True:高在前，低在后；False：低在前，高在后</param> 
        public static byte[] ConvertInt32ToByte(UInt32 number, byte[] data, int offSet, bool isReverse = true)
        {
            byte[] numberBytes = BitConverter.GetBytes(number);
            if (isReverse)
            {
                Array.Reverse(numberBytes, 0, numberBytes.Length);
            }

            if (data != null && data.Length >= numberBytes.Length && (data.Length - offSet) >= numberBytes.Length)
            {
                Buffer.BlockCopy(numberBytes, 0, data, offSet, numberBytes.Length);
            }

            return numberBytes;
        }

        /// <summary>
        /// int16转换为byte[]数组,高在前，低在后
        /// </summary>
        /// <param name="number"></param>
        /// <param name="data"></param>
        /// <param name="offSet"></param>
        /// <param name="isReverse">True:高在前，低在后；False：低在前，高在后</param>
        public static byte[] ConvertInt16ToByte(UInt16 number, byte[] data, int offSet, bool isReverse = true)
        {
            byte[] numberBytes = BitConverter.GetBytes(number);
            if (isReverse)
            {
                Array.Reverse(numberBytes, 0, numberBytes.Length);
            }

            if (data != null && data.Length >= numberBytes.Length && (data.Length - offSet) >= numberBytes.Length)
            {
                Buffer.BlockCopy(numberBytes, 0, data, offSet, numberBytes.Length);
            }

            return numberBytes;
        }

        #endregion

    
    }
    public class CRCCheck
    {
        private const ushort GENP = 0xA001;

        private static ushort Crc16;
        public CRCCheck()
        { }
        private static void CRC16(byte value)
        {
            byte i = 0;
            byte temp = 0;
            Crc16 ^= value;
            for (i = 0; i < 8; i++)
            {
                temp = Convert.ToByte((Crc16 & 0x0001));
                Crc16 >>= 1;
                Crc16 &= 0x7fff;
                if (temp == 0x0001)
                    Crc16 ^= GENP;
            }
        }
        /// <summary>
        /// 计算CRC数据
        /// </summary>
        /// <param name="Msg">字节数组</param>
        /// <param name="len">数据长度</param>
        /// <returns>返回16进制无符号整数</returns>
        public static ushort CalCrc16(byte[] Msg, int start, int len)
        {
            Crc16 = 0xffff;
            for (int i = start; i < start + len; i++)
                CRC16(Msg[i]);
            return Crc16;
        }
        /// <summary>
        /// CRC16校验字节数组
        /// </summary>
        /// <param name="msg">字节数组</param>
        /// <param name="len">字节数据长度</param>
        /// <returns>校验成功返回true</returns>
        public static bool CRC16Check(byte[] msg, int start, int len)
        {
            if (msg.Length < 3)
                return false;
            Crc16 = 0xffff;
            Crc16 = CalCrc16(msg, start, (ushort)(len - 2));
            if (((Crc16 & 0x00ff) == msg[start + len - 2]) && ((Crc16 >> 8) == msg[start + len - 1]))
                return true;
            else
                return false;
        }

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
            }

            return crc;
        }
    }
}
