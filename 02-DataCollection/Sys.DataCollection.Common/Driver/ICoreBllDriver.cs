using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Protocols.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Driver
{
    /// <summary>
    /// 定义核心业务逻辑层与网关之间关于设备的通讯接口（标准协议）
    /// </summary>
    public interface ICoreBllDriver
    {
        /// <summary>
        /// 控制命令业务处理
        /// </summary>
        /// <param name="deviceControl">控制业务对象</param>
        byte[] HandDeviceControl(MasProtocol masProtocol, DeviceControlRequest deviceControl, ref string DeviceCode, ref int CommPort);
        /// <summary>
        /// 初始化命令业务处理
        /// </summary>
        /// <param name="Initialize">初始化业务对象</param>
        byte[] HandInitialize(MasProtocol masProtocol, InitializeRequest Initialize, ref string DeviceCode, ref int CommPort);
        /// <summary>
        ///获取电源箱数据业务处理
        /// </summary>
        /// <param name="batteryRealData">获取电源箱业务对像</param>
        byte[] HandQueryBatteryRealData(MasProtocol masProtocol, QueryBatteryRealDataRequest batteryRealData, ref string DeviceCode, ref int CommPort);
        /// <summary>
        /// 设备复位命令业务处理
        /// </summary>
        /// <param name="resetDevice">设备复位对象</param>
        byte[] HandResetDeviceCommand(MasProtocol masProtocol, ResetDeviceCommandRequest resetDevice, ref string DeviceCode, ref int CommPort);

        /// <summary>
        /// 时钟同步命令业务处理
        /// </summary>
        /// <param name="timeSynchronization">时钟同步业务对象</param>
        byte[] HandTimeSynchronization(MasProtocol masProtocol, TimeSynchronizationRequest timeSynchronization, ref string DeviceCode, ref int CommPort);

        /// <summary>
        /// 传感器地址修改命令业务处理
        /// </summary>
        /// <param name="modificationdeviceadress">时钟同步业务对象</param>
        byte[] HandModificationDeviceAdress(MasProtocol masProtocol, ModificationDeviceAdressRequest modificationDeviceAdress, ref string DeviceCode, ref int CommPort);
        /// <summary>
        /// 处理分站历史控制记录
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <param name="modificationDeviceAdress"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        byte[] HandQueryHistoryControl(MasProtocol masProtocol, QueryHistoryControlRequest queryHistoryControl, ref string deviceCode, ref int CommPort);
        /// <summary>
        /// 处理分站历史控制记录
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <param name="modificationDeviceAdress"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        byte[] HandQueryHistoryRealData(MasProtocol masProtocol, QueryHistoryRealDataRequest queryHistoryRealData, ref string deviceCode, ref int CommPort);
        /// <summary>
        /// 下发传感器分级报警结果
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <param name="modificationDeviceAdress"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        byte[] HandSensorGradingAlarm(MasProtocol masProtocol, SetSensorGradingAlarmRequest sensorGradingAlarm, ref string deviceCode, ref int CommPort);
    }
}
