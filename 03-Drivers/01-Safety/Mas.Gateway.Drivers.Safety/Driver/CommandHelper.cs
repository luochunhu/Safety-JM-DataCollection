using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Driver.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver
{
    /// <summary>
    /// 命令辅助类
    /// </summary>
    public class CommandHelper 
    {
        /// <summary>
        /// 获取完整打包后的协议
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static byte[] ConvertCommandToBytes(DownlinkCommand command)
        {
            byte[] bodyData = command.ToBytes();

            return GetPackages(bodyData);           
        }
       

        /// <summary>
        /// 获取完整协议包（会组装协议头+体+累加和，组成一个完整协议包）
        /// </summary>        
        /// <param name="bodyData">业务数据体</param>
        /// <returns></returns>
        private static byte[] GetPackages( byte[] bodyData)
        {
            //总长度 =外包协议头(4位) + 数据体长度 + 累加和(2位)
            int bufferLength = 4 + bodyData.Length + 2;

            byte[] buffer = new byte[bufferLength];

            //0`3 起启符标识
            buffer[0] = 0xAD;
            buffer[1] = 0xDA;
            buffer[2] = 0xAD;
            buffer[3] = 0xDA;

            //10~11 长度 ,命令参数到累加和的长度值;高在前低在后
            ushort length = (ushort)(bodyData.Length + 1 + 2);
            byte[] bLength = BitConverter.GetBytes(length);
            buffer[10] = bLength[1];//高在前，低在后
            buffer[11] = bLength[0];
            //高低位 CRC效验等常用转换 辅助操作见 Sys.DataCollection.Common.Utils.CommandUtil 类

            ////12 协议标识，0x55为升级协议
            //buffer[12] = 0x55;


            //数据体
            Buffer.BlockCopy(bodyData, 0, buffer, 5, bodyData.Length);

            
            //计算累加和
            buffer = SetCUSUM(buffer);

            return buffer;
        }

        /// <summary>
        /// 设置累加和
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static byte[] SetCUSUM(byte[] buffer)
        {
            ushort sum = 0;
            //根据协议，前4位头信息、最后2位不参与累加和计算
            for (int i = 4; i < buffer.Length - 2; i++)
            {
                sum += buffer[i];
            }

            buffer[buffer.Length - 1] = (byte)(sum & 0xff);
            sum >>= 8;
            buffer[buffer.Length - 2] = (byte)(sum & 0xff);
            return buffer;
        }

    }
}
