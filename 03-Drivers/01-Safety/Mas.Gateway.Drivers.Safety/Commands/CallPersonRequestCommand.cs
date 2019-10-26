using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Protocols.Devices;
using Sys.DataCollection.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    public class CallPersonRequestCommand: CallPersonRequest
    {
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[31];
            int i = 0;
            ushort ljh = 0, kh = 0,Cur;
            bytes[0] = 0xFE;//广播用254站号
            bytes[1] = CommandCodes.CallPersonCommand;//H命令
            bytes[2] = 0;
            bytes[3] = 0;
            if (HJ_State == 0)
            {
                bytes[4] = 1;//解除呼叫 0->1
                for (i = 0; i < 24; i++) bytes[5 + i] = 0x00;
            }
            else
            {
                if (HJ_Type == 0)
                {
                    bytes[4] = 3;//全员呼叫
                    for (i = 0; i < 24; i++) bytes[5 + i] = 0x00;
                }
                else if (HJ_Type == 1)
                {//区段
                    bytes[4] = 1;//指定人员呼叫
                    for (i = 0; i < 24; i++) bytes[5 + i] = 0x0;
                    kh = HJ_KH[0];//开始卡号
                    ljh = HJ_KH[1];//结束卡号
                    for (i = 0; i < 12; i++)
                    {
                        if (kh > 0 && kh <= ljh)
                        {
                            Cur = kh;
                            bytes[5 + i * 2 + 1] = (Byte)Cur;
                            Cur >>= 8;
                            bytes[5 + i * 2] = (Byte)Cur;
                            kh++;
                        }
                    }
                }
                else if (HJ_Type == 2)
                {
                    bytes[4] = 1;//指定人员呼叫
                    for (i = 0; i < 24; i++) bytes[5 + i] = 0x0;
                    for (i = 0; i < 12; i++)
                    {
                        if (HJ_KH[i] > 0)
                        {
                            Cur = HJ_KH[i];
                            bytes[5 + i * 2 + 1] = (Byte)Cur;
                            Cur >>= 8;
                            bytes[5 + i * 2] = (Byte)Cur;
                        }
                    }
                }
                else if (HJ_Type == 4)
                {//呼叫的是井下设备
                    bytes[4] = 4;//指定井下设备
                    for (i = 0; i < 24; i++) bytes[5 + i] = 0x0;
                    for (i = 0; i < 12; i++)
                    {
                        if (HJ_KH[i] > 0)
                        {
                            Cur = HJ_KH[i];
                            bytes[5 + i * 2 + 1] = (Byte)Cur;//要控制的识别器号
                            Cur >>= 8;
                            bytes[5 + i * 2] = (Byte)Cur;//要控制的分站号
                        }
                    }
                }
            }
            CommandUtil.AddSumToBytes(bytes, 0, bytes.Length);//累加和
            return bytes;
        }
    }
}
