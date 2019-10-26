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
    /// 分站历史控制记录应答
    /// </summary>
    public class QueryHistoryControlResponseCommand: QueryHistoryControlResponse
    {
        public void HandleQueryHistoryControl(byte[] data, MasProtocol protocol, ushort startIndex, byte deviceCommunicationType, string point)
        {
            QueryHistoryControlResponse ResponseObject = new QueryHistoryControlResponse();
            DeviceHistoryControlItem item;
            protocol.ProtocolType = ProtocolType.QueryHistoryControlResponse;
            ResponseObject.DeviceCode = point;
            int totoalalarm = 0, curentalarm = 0;
            uint time;
            ushort tempshort = 0;
            byte tempcur;
            float fltcur;
            byte DecimalDigits = 0, minus, SensorState;//小数点位数
            totoalalarm = CommandUtil.ConvertByteToInt16(data, startIndex + 6);//总报警数
            startIndex += 8;
            curentalarm = data[startIndex++];//当前传输的报警数
            if (totoalalarm >= curentalarm)
                ResponseObject.AlarmTotal = (ushort)(totoalalarm - curentalarm);
            ResponseObject.HistoryControlItems = new List<DeviceHistoryControlItem>();
            for (int i = 0; i < curentalarm; i++)
            {//每个记录10个字节
                if (startIndex + 9 < data.Length)
                {
                    item = new DeviceHistoryControlItem();
                    time = CommandUtil.ConvertByteToInt(data, startIndex);//时间
                    startIndex += 4;
                    item.SaveTime = Cache.TurnTimeFromInt(time);
                    tempcur = data[startIndex++];
                    item.Channel = ((tempcur & 0x1F) + 1).ToString();//todo 无法区分多参传感器
                    item.Address = ((tempcur & 0x7F) >> 5).ToString();
                    if ((tempcur & 0x80) == 0x80)
                    {
                        item.DeviceProperty = ItemDevProperty.Analog;
                    }
                    else
                    {
                        item.DeviceProperty = ItemDevProperty.Derail;
                    }
                    tempcur = data[startIndex++]; 
                    DecimalDigits = (byte)(tempcur & 0x03);
                    tempcur >>= 2;
                    minus = (byte)(tempcur & 0x01);
                    tempcur >>= 1;
                    SensorState = (byte)(tempcur & 0x0F);
                    ItemState SensorTempState = ItemState.EquipmentCommOK;//设备状态
                    SensorTempState = Cache.GetSensorState(SensorState, 0x26);//6为类型有误
                    item.State = SensorTempState;
                    tempshort = CommandUtil.ConvertByteToInt16(data, startIndex);
                    startIndex += 2;
                    if (item.DeviceProperty == ItemDevProperty.Analog)
                    {
                        fltcur = (float)(tempshort / Math.Pow(10, DecimalDigits));
                        if (minus == 1)
                        {
                            item.RealData = (-fltcur).ToString();
                        }
                        else
                        {
                            item.RealData = fltcur.ToString();
                        }
                    }
                    else
                    {
                        item.RealData = tempshort.ToString();
                    }
                    item.ControlDevice = CommandUtil.ConvertByteToInt16(data, startIndex);
                    startIndex += 2;
                    ResponseObject.HistoryControlItems.Add(item);
                }
            }
            protocol.Protocol = ResponseObject;
        }
    }
    
}
