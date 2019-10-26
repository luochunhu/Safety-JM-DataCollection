using Basic.Framework.Logging;
using Sys.DataCollection.Common.Driver;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
   public  class GetSwitchboardParamCommResponseCommand: GetSwitchboardParamCommResponse
    {
        public static void HandleSwitchInfo(byte[] data, ProtocolDataCreatedEventArgs upData, NetworkDeviceInfo net)
        {
            GetSwitchboardParamCommResponse cData = new GetSwitchboardParamCommResponse();//交换机基本信息；
            byte[] buffer = new byte[data.Length - 4];
            ushort startindex = 32760;//数据开始位置
            int receivelength = 0;//下标|接收数据长度
            ushort crcvalue = 0, receivevalue;//crc累加和 回发累加和
            byte curByte = 0, SignCount = 0;
            cData.DeviceCode = net.MAC;
            cData.RealDataItems = new List<RealDataItem>();
            for (int i = 4; i < data.Length; i++)
            {
                buffer[i - 4] = data[i];
            }
            if (buffer[0] == 252)
            {
                startindex = 0;
            }
            if (startindex == 32760)
            {
                LogHelper.Error("【HandleSwitchInfo】" + "没有长到分站地址引导符【252】");
                return;
            }
            receivelength = CommandUtil.ConvertByteToInt16(buffer, startindex + 2);
            if (receivelength > buffer.Length)
            {
                LogHelper.Error("【HandleSwitchInfo】" + "回发长度不足【" + receivelength + "】" + "【" + data.Length + "】");
                return;
            }
            receivevalue = CommandUtil.ConvertByteToInt16(buffer, startindex + receivelength - 2);
            crcvalue = CommandUtil.CRC16_CCITT(buffer, startindex, startindex + receivelength - 2);
            if (crcvalue != receivevalue)
            {
                LogHelper.Error("【HandleSwitchInfo】" + "通讯误码【" + crcvalue + "】" + "【" + receivevalue + "】");
                return;
            }

            #region 协议解析
            //5A A5 3C C3 FC 46 00 12 C1 04 64 FF FF 00 F8 C1 C1 00 00 00 0B 84
            curByte = buffer[startindex + 5];
            SignCount =(byte)( curByte >> 2);
            cData.BatteryControlState = (byte)(curByte & 0x01);
            cData.BatteryState = (byte)((curByte>>1) & 0x01);
            startindex = (ushort)(startindex + 6);
            cData.BatteryCapacity = buffer[startindex++];
            curByte= buffer[startindex++];
            cData.SerialPortBatteryState = (byte)((curByte >> 6) & 0x01);//（串口服务器-供电电源）
            cData.SerialPortRunState = (byte)((curByte >> 5) & 0x01);//（串口服务器-运行状态）
            cData.SwitchRunState = (byte)((curByte >> 1) & 0x01);//（交换机-运行状态）
            cData.SwitchBatteryState = (byte)(curByte & 0x01);//（交换机-供电电源）
            cData.Switch1000State = new byte[3];
            cData.Switch1000State[0]= (byte)((curByte >> 2) & 0x01);//Bit2位（千兆光口1）
            cData.Switch1000State[1] = (byte)((curByte >> 3) & 0x01);//Bit2位（千兆光口2）
            cData.Switch1000State[2] = (byte)((curByte >> 4) & 0x01);//Bit2位（千兆光口3）
            curByte = buffer[startindex++];
            cData.Switch100State = new byte[7];
            for(int i=0;i<7;i++)
            {
                cData.Switch100State[i]= (byte)((curByte >> i) & 0x01);//Bit2位（百兆接口1~7）
            }
            startindex++;//预留字节
            if (SignCount > 0)
                HandleSoleCoding(SignCount, buffer, startindex, cData);
            #endregion
            upData.MasProtocol.Protocol = cData;
        }
        /// <summary>
        /// 处理F命令上去的唯一性编码
        /// </summary>
        private static void HandleSoleCoding(int soleCodingCount, byte[] data, ushort startIndex, GetSwitchboardParamCommResponse cdata)
        {
            int index = 0;
            byte branch;//分支号
            byte address;//地址号
            RealDataItem item;
            UInt32 soleCoding;//唯一性编码；
            ushort SensorDevID = 0;//设备的类型
            ItemState SensorTempState = ItemState.EquipmentCommOK;//设备状态
            for (; index < soleCodingCount; index++)//有多少个唯一性编码
            {
                SensorTempState = ItemState.EquipmentCommOK;//设备状态
                branch = data[startIndex++];//分支号及地址号
                SensorDevID = data[startIndex++];//设备型号
                soleCoding = CommandUtil.ConvertByteToInt(data, startIndex);//唯一性编码
                address = (byte)(branch >> 3);//地址号
                branch = (byte)(branch & 0x07);//分支号
                branch += 1;
                item = new RealDataItem();
                item.DeviceProperty = ItemDevProperty.SoleCoding;
                item.DeviceTypeCode = SensorDevID;//设备型号
                item.BranchNumber = branch;//分支号
                if ((soleCoding & 0xFFFFFF) != 0x00)
                {
                    item.SoleCoding = (((soleCoding >> 19) & 0x1F) + 2018).ToString().PadLeft(4, '0')
                        + ((soleCoding >> 15) & 0x0F).ToString().PadLeft(2, '0')
                        + ((soleCoding >> 10) & 0x1F).ToString().PadLeft(2, '0')
                        + (soleCoding & 0x3FF).ToString().PadLeft(4, '0');
                }
                else
                {
                    item.SoleCoding = "0";
                }
                item.Address = "0";
                item.Channel = address.ToString();//地址号
                item.RealData = "0";
                item.State = SensorTempState;
                cdata.RealDataItems.Add(item);
                startIndex += 4;
            }
        }
    }
}
