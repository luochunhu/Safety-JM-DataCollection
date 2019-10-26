using Basic.Framework.Logging;
using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Utils;
using Sys.DataCollection.Driver.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Driver.Net_8962
{
    public class DeviceDataConrtol
    {
        public DeviceInfo def;
        /// <summary>
        /// 处理回发的数据体，以分站为对象
        /// </summary>
        /// <param name="data"></param>
        /// <param name="protocolData"></param>
        public void HandleDeviceData(byte[] data, MasProtocol protocolData)
        {
            DeviceTypeInfo dev = null;
            dev = Cache.CacheManager.QueryFrist<DeviceTypeInfo>(p => p.Devid == def.Devid, true);
            if (dev != null)
            {
                DataControlByMonitor(data, protocolData, (byte)dev.LC2);
            }
        }
        /// <summary>
        /// 数据包命令判断---监控系统
        /// </summary>
        /// <param name="data">数据包体</param>
        /// <param name="protocol">应回发的对象</param>
        /// <param name="newsType">信息类别=1表示网口，=2表示串口</param>
        /// <param name="orderVersion">分类的驱动类型</param>
        private void DataControlByMonitor(byte[] data, MasProtocol protocol, byte orderVersion)
        {
            ushort startindex = 32760;//数据开始位置
            int receivelength = 0;//下标|接收数据长度
            ushort crcvalue = 0, receivevalue;//crc累加和 回发累加和
            byte commandtype;//接受命令
            if (data[0] == def.Fzh)
            {
                startindex = 0;
            } 
            if (startindex == 32760)
            {
                RealDataCreateByState(protocol, ItemState.EquipmentInterrupted);
                LogHelper.Error("【DataControlByMonitor】" + "没有长到分站地址引导符【" + def.Fzh + "】");
                return;
            }
            receivelength = CommandUtil.ConvertByteToInt16(data, startindex + 2,false);
            if (receivelength> data.Length)
            {
                RealDataCreateByState(protocol, ItemState.EquipmentInterrupted);
                LogHelper.Error("【DataControlByMonitor】" + "回发长度不足【" + startindex + receivelength + 3 + "】" + "【" + data.Length + "】");
                return;
            }
            receivevalue = CommandUtil.ConvertByteToInt16(data, startindex + receivelength - 2,false);
            crcvalue = CommandUtil.AddSumToBytes(data, startindex, startindex + receivelength);
            if (crcvalue != receivevalue)
            {
                RealDataCreateByState(protocol, ItemState.EquipmentBiterror);
                LogHelper.Error("【DataControlByMonitor】" + "通讯误码【" + crcvalue + "】" + "【" + receivevalue + "】");
                return;
            }
            commandtype = data[startindex + 1];//StartIndex当前为分站的下标索引
            switch (commandtype)
            {
                case CommandCodes.InitializeCommand://I初始化命令
                    InitializeResponseCommand InitCommandObject = new Commands.InitializeResponseCommand();
                    crcvalue = CommandUtil.ConvertByteToInt16(data, startindex + 5, false);
                    InitCommandObject.SendInitializeAffirmToCenter(protocol, def.Point, crcvalue);
                    break;
                case 0x46://F取数命令
                    RealDataResponseCommand RealCommandObject = new Commands.RealDataResponseCommand();
                    RealCommandObject.def = def;
                    RealCommandObject.HandleRealData(data, protocol, startindex, orderVersion);
                    break;
                case 0x41://A请求初始化命令
                    InitializeResponseCommand InitRequstCommandObject = new Commands.InitializeResponseCommand();
                    InitRequstCommandObject.SendInitializeRequestToCenter(protocol, def.Point);
                    break;
                case 0x55:
                    BatteryRealDataResponseCommand batterycommand = new BatteryRealDataResponseCommand();
                    batterycommand.HandleBatteryRealData(data, protocol, startindex, 0x00, def.Point);
                    break;
                case 0x58://X命令
                    ControlExtendCommand(data[startindex + 5], data, protocol, startindex, data[startindex + 5]);
                    break;
                case 0x45://分站回发接收错误，后续修改为，错误不回发
                case 0x50://分站回发接收错误，后续修改为，错误不回发
                    break;
                case 0x52://复位
                    ResetDeviceResponseCommand ResetDeviceCommandObject = new Commands.ResetDeviceResponseCommand();
                    ResetDeviceCommandObject.SendResetDeviceAffirmToCenter(protocol, def.Point);
                    break;
                case 0x5A://分站中断
                    LogHelper.Info("【DataControlByMonitor】" + "监控分站数据回发5A分站中断【" + def.Point + "】");
                    break;
                default:
                    LogHelper.Info("【DataControlByMonitor】" + "非法命令【" + commandtype + "】");
                    break;
            }

        }
        /// <summary>
        /// 处理D命令主函数
        /// </summary>
        /// <param name="data">输入的数据体</param>
        /// <param name="deviceCommunicationType">D命令类别</param>
        /// <param name="startIndex">分站号的开始位置</param>
        /// <param name="protocol">传输对象</param>
        /// <param name="extendCommandType">分站类型：即=0x26最新分站，还是老分站</param>
        private void ControlExtendCommand(byte extendCommandType, byte[] data, MasProtocol protocol, ushort startIndex, byte deviceCommunicationType)
        {
            extendCommandType &= 0x7F;//去掉最高位；
            switch (extendCommandType)
            {
                case 0://获取历史控制记录
                    QueryHistoryControlResponseCommand queryhistorycontrol = new QueryHistoryControlResponseCommand();
                    queryhistorycontrol.HandleQueryHistoryControl(data, protocol, startIndex, deviceCommunicationType, def.Point);
                    break;
                case 1://获取电源箱的状态(D1)
                    BatteryRealDataResponseCommand batterycommand = new BatteryRealDataResponseCommand();
                    batterycommand.HandleBatteryRealData(data, protocol, startIndex, deviceCommunicationType, def.Point);
                    break;
                case 2://修改传感器地址号(D2)
                    ModificationDeviceAdressResponseCommand modideviceaddress = new ModificationDeviceAdressResponseCommand();
                    modideviceaddress.HandleModificationDeviceAdressData(data, protocol, startIndex, deviceCommunicationType, def.Point);
                    break;
                case 11://
                    int exFlag = (data[startIndex + 7] >> 4);//分站响应标记 Bit7~bit4：数据交互标志
                    UpdateStationDataProc(exFlag, data, protocol, startIndex, def.Point);
                    break;
                case 3://获取分站4小时历史数据(D5)
                    QueryHistoryRealDataResponseCommand queryhistoryrealdata = new QueryHistoryRealDataResponseCommand();
                    queryhistoryrealdata.HandleHistoryRealData(data, protocol, startIndex, deviceCommunicationType, def.Point);
                    break;
                case 4://下发传感器的分级报警控制(D7)
                    SetSensorGradingAlarmResponseCommand sensorgradingalarm = new SetSensorGradingAlarmResponseCommand();
                    sensorgradingalarm.HandleSetSensorGradingAlarm(data, protocol, startIndex, deviceCommunicationType, def.Point);
                    break;
            }
        }
        private void RealDataCreateByState(MasProtocol protocol, ItemState state)
        {
            QueryRealDataResponse realData = new QueryRealDataResponse();
            realData.RealDataItems = new List<RealDataItem>();
            protocol.ProtocolType = ProtocolType.QueryRealDataResponse;
            RealDataItem item = new RealDataItem();
            item = new RealDataItem();
            item.Channel = "0";
            item.Address = "0";
            item.State = state;
            realData.RealDataItems.Add(item);
            realData.DeviceCode = def.Point;
            protocol.Protocol = realData;
        }


        private void UpdateStationDataProc(int exFlag, byte[] data,MasProtocol protocol, ushort startIndex,string point)
        {
            switch (exFlag)
            {
                case 1://中心站请求远程升级
                    StationUpdateResponseCommand stationUpdate = new StationUpdateResponseCommand();
                    stationUpdate.Handle(data, protocol, startIndex, point);
                    break;
                case 4://中心站巡检分站的文件接收情况
                    InspectionResponseCommand inspection = new InspectionResponseCommand();
                    inspection.Handle(data, protocol, startIndex, point);
                    break;
                case 5://中心站告知分站重启并升级
                    RestartResponseCommand restart = new RestartResponseCommand();
                    restart.Handle(data, protocol, startIndex, point);
                    break;
                case 6://中心站告知分站该次升级过程中止
                    UpdateCancleResponseCommand updateCancle = new UpdateCancleResponseCommand();
                    updateCancle.Handle(data, protocol, startIndex, point);
                    break;
                case 7://中心站告知分站恢复最近一次备份
                    ReductionResponseCommand reduction = new ReductionResponseCommand();
                    reduction.Handle(data, protocol, startIndex, point);
                    break;
                case 8://中心站查询分站信息
                    GetStationUpdateStateResponseCommand getStationUpdateState = new GetStationUpdateStateResponseCommand();
                    getStationUpdateState.Handle(data, protocol, startIndex, point);
                    break;
            }
        }
    }
}
