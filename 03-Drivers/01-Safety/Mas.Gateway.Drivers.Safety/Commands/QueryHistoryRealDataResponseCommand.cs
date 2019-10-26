using Basic.Framework.Logging;
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
    /// 分站历史4小时数据应签
    /// </summary>
    public class QueryHistoryRealDataResponseCommand : QueryHistoryRealDataResponse
    {
        public void HandleHistoryRealData(byte[] data, MasProtocol protocol, ushort startIndex, byte deviceCommunicationType, string point)
        {          
            string startIndexItems = "";
            try
            {
                uint time;
                byte tempcur = 0, mincount, parametercount;
                byte DecimalDigits = 0, minus;//小数点位数
                ushort ushortcur = 0;
                float fltcur;
                DateTime SaveTime;
                string Channel;
                ItemState State;
                QueryHistoryRealDataResponse ResponseObject = new QueryHistoryRealDataResponse();
                protocol.ProtocolType = ProtocolType.QueryHistoryRealDataResponse;
                ResponseObject.DeviceCode = point;
                ResponseObject.HistoryRealDataItems = new List<Common.Protocols.Devices.DeviceHistoryRealDataItem>();
                DeviceHistoryRealDataItem item;
                ItemDevProperty deviceProperty = ItemDevProperty.Analog;
                ResponseObject.MinuteDataTotal = 0;

                if (startIndex + 9 < data.Length)
                {
                    ResponseObject.MinuteDataTotal = (ushort)((data[startIndex + 8] << 8) + (data[startIndex + 9])); 
                    startIndex += 10;    
                    mincount = data[startIndex];   
                    startIndex++;   
                    for (int index = 0; index < mincount; index++)
                    {
                        startIndexItems += startIndex + "-";
                        time = CommandUtil.ConvertByteToInt(data, startIndex);//时间
                        startIndex += 4;
                        SaveTime = Cache.TurnTimeFromInt(time); 
                        tempcur = data[startIndex++];//通道号
                        Channel = ((tempcur & 0x1F) + 1).ToString();
                        tempcur >>= 5;
                        if (tempcur == 6)
                        {
                            State = ItemState.EquipmentTypeError;
                        }
                        else if (tempcur == 5)
                        {
                            State = ItemState.EquipmentDown;
                        }
                        else if (tempcur == 4)
                        {
                            State = ItemState.EquipmentBiterror;
                        }
                        else
                        {
                            #region ----正常有数据需要处理----
                            State = ItemState.EquipmentCommOK;
                            parametercount = data[startIndex++];//参数个数
                            for (int i = 0; i < parametercount; i++)
                            {
                                item = new DeviceHistoryRealDataItem();
                                item.HistoryDate = SaveTime;
                                item.Channel = Channel;
                                item.State = State;
                                tempcur = data[startIndex++];
                                #region ----解析参数属性----

                                if ((tempcur & 0x0F) == 2)
                                {
                                    item.State = ItemState.EquipmentChange;
                                }
                                else if ((tempcur & 0x0F) == 1)
                                {
                                    item.State = ItemState.EquipmentHeadDown;
                                }
                                tempcur >>= 4;
                                if ((tempcur & 0x03) == 1)
                                {
                                    deviceProperty = ItemDevProperty.Control;
                                }
                                else if ((tempcur & 0x03) == 2)
                                {
                                    deviceProperty = ItemDevProperty.Analog;
                                }
                                else if ((tempcur & 0x03) == 3)
                                {
                                    deviceProperty = ItemDevProperty.Derail;
                                }
                                item.DeviceProperty = deviceProperty;

                                #endregion
                                item.Address = (parametercount == 1 ? "0" : (i + 1).ToString());                          
                                ushortcur = CommandUtil.ConvertByteToInt16(data, startIndex);//平均值
                                startIndex += 2;
                                if (deviceProperty == ItemDevProperty.Analog)
                                {
                                    DecimalDigits = (byte)((ushortcur >> 13) & 0x03);
                                    fltcur = (float)((ushortcur & 0x1FFF) / Math.Pow(10, DecimalDigits));
                                    minus = (byte)(ushortcur >> 15);
                                    if (minus == 1) fltcur = -fltcur;
                                    //item.FiveAvgData = fltcur;
                                }
                                else
                                {
                                    //item.FiveAvgData = ushortcur;
                                }
                                ushortcur = CommandUtil.ConvertByteToInt16(data, startIndex);//最大值 
                                startIndex += 2;
                                if (deviceProperty == ItemDevProperty.Analog)
                                {
                                    DecimalDigits = (byte)((ushortcur >> 13) & 0x03);
                                    fltcur = (float)((ushortcur & 0x1FFF) / Math.Pow(10, DecimalDigits));
                                    minus = (byte)(ushortcur >> 15);
                                    if (minus == 1) fltcur = -fltcur;
                                    //item.FiveMaxData = fltcur;
                                }
                                else
                                {
                                    //item.FiveMaxData = ushortcur;
                                }
                                ushortcur = CommandUtil.ConvertByteToInt16(data, startIndex);//最小值 
                                startIndex += 2;
                                if (deviceProperty == ItemDevProperty.Analog)
                                {
                                    DecimalDigits = (byte)((ushortcur >> 13) & 0x03);
                                    fltcur = (float)((ushortcur & 0x1FFF) / Math.Pow(10, DecimalDigits));
                                    minus = (byte)(ushortcur >> 15);
                                    if (minus == 1) fltcur = -fltcur;
                                    //item.FiveMinData = fltcur;
                                }
                                else
                                {
                                    //item.FiveMinData = ushortcur;
                                }
                                if (deviceProperty == ItemDevProperty.Analog)
                                {
                                    //处理最大值时间和最小值时间
                                    ushortcur = CommandUtil.ConvertByteToInt16(data, startIndex);//最大值时间 
                                    startIndex += 2;
                                    //item.FiveMaxDateTime = item.SaveTime.AddSeconds(ushortcur);

                                    ushortcur = CommandUtil.ConvertByteToInt16(data, startIndex);// 
                                    startIndex += 2;
                                    //item.FiveMinDataTime = item.SaveTime.AddSeconds(ushortcur);//最小值时间
                                }
                                ResponseObject.HistoryRealDataItems.Add(item); 
                            }
                            #endregion
                        }
                    }
                }
                protocol.Protocol = ResponseObject;
            }
            catch (Exception ex)
            {
                LogHelper.Error("QueryHistoryRealDataResponseCommand HandleHistoryRealData Error:" + ex.Message);
            }
        }
    }
}
