using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Driver.Commands;
using Sys.DataCollection.Common.Driver;
using Basic.Framework.Logging;
using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Protocols.Devices;
using Sys.DataCollection.Common.Utils;

namespace Sys.DataCollection.Driver.Driver
{
    /// <summary>
    /// 用于KJ73N系统与核心业务逻辑的业务处理
    /// </summary>
    public class NetCoreBllRealizer : ICoreBllDriver
    {
        /// <summary>
        /// 表示当前通讯的分站号
        /// </summary>
        public string Point = "";
        private bool GetDeviceObject(ref DeviceInfo deviceInfoObject, ref DeviceTypeInfo deviceTypeObject, ref NetworkDeviceInfo netMacObject, ProtocolType pType, string deviceCode)
        {
            string ipstr = "";
            List<NetworkDeviceInfo> lstNet = new List<NetworkDeviceInfo>();
            deviceInfoObject = Cache.CacheManager.GetItemByKey<DeviceInfo>(deviceCode, true);//得到对应的分站对象
            if (deviceInfoObject == null)
            {
                LogHelper.Info("未找到分站【" + deviceCode + "】:" + pType.ToString());
                return false;
            }
            Point = deviceInfoObject.Point;//用于外部显示分站号
            ipstr = deviceInfoObject.Jckz2;//获取IP
            lstNet = Cache.CacheManager.Query<NetworkDeviceInfo>(p => (p.IP == ipstr) && (p.Upflag=="0"), true);
            if (lstNet.Count > 0)
                netMacObject = lstNet[0];
            else
            {
                LogHelper.Info("未找到分站【" + deviceCode + "】对应的网络地址【" + deviceInfoObject.Jckz2 + "】:" + pType.ToString());
                return false;
            }
            lock (Cache.LockWorkNet)//用缓存的连接号进行更新
            {
                CacheNetWork curnet = Cache.LstWorkNet.Find(delegate(CacheNetWork p) { return p.IP == ipstr; });
                if (curnet != null)
                {
                    netMacObject.NetID = curnet.NetID;
                }
            }
            deviceTypeObject = Cache.CacheManager.GetItemByKey<DeviceTypeInfo>(deviceInfoObject.Devid, true);//按照devid搜索对象
            if (deviceTypeObject == null)
            {
                LogHelper.Info("未找到分站【" + deviceCode + "】对应的设备类型【" + deviceInfoObject.Devid + "】:" + pType.ToString());
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取交换机的基础信息
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <param name="deviceControl"></param>
        /// <param name="deviceCode"></param>
        /// <param name="commPort"></param>
        /// <returns></returns>
        public byte[] HandGetSwitchInfo(MasProtocol masProtocol, GetSwitchboardParamCommRequest getswicthInfo, ref string deviceCode)
        {
            NetworkDeviceInfo netmacobject = null;
            GetSwitchboardParamCommRequestCommand commandobject = new GetSwitchboardParamCommRequestCommand();
            byte[] sendbuff = null;
            if (getswicthInfo == null)
            {
                return sendbuff;
            }
            netmacobject = Cache.CacheManager.GetItemByKey<NetworkDeviceInfo>(getswicthInfo.DeviceCode, true);//按照IP搜索对象                       
            if (netmacobject == null)
            {
                LogHelper.Error("获取交换机的基础信息，未找到对应的IP地址【" + getswicthInfo.DeviceCode + "】");
                return sendbuff;
            }
            lock (Cache.LockWorkNet)//用缓存的连接号和接收时间进行更新
            {
                CacheNetWork curnet = Cache.LstWorkNet.Find(delegate (CacheNetWork p) { return p.IP == netmacobject.IP; });
                if (curnet != null)
                {
                    netmacobject.NetID = curnet.NetID;
                }
            }
            if (netmacobject.NetID > 0)
            {
                deviceCode = netmacobject.IP;
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<GetSwitchboardParamCommRequest, GetSwitchboardParamCommRequestCommand>(getswicthInfo);//得到基础数据
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
            }
            return sendbuff;
        }
        public byte[] HandDeviceControl(MasProtocol masProtocol, DeviceControlRequest deviceControl, ref string deviceCode, ref int commPort)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            DeviceControlRequestCommand commandobject = new DeviceControlRequestCommand();
            byte[] sendbuff = null;
            if (masProtocol == null || deviceControl == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.DeviceControlRequest, deviceControl.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                commPort = deviceinfoobject.K3;
                deviceCode = netmacobject.IP;
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<DeviceControlRequest, DeviceControlRequestCommand>(deviceControl);//得到基础数据
                commandobject.OrderVersion = (byte)devicetypeobject.LC2;//设备设备类型
                commandobject.def = deviceinfoobject;
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
            }
            return sendbuff;
        }
        public byte[] HandInitialize(MasProtocol masProtocol, InitializeRequest Initialize, ref string deviceCode, ref int commPort)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            InitializeRequestCommand commandobject = new InitializeRequestCommand();
            byte[] sendbuff = null;
            if (Initialize == null)
                return sendbuff;
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.InitializeRequest, Initialize.DeviceCode))
                return sendbuff;
            if (netmacobject.NetID > 0)
            {
                commPort = deviceinfoobject.K3;
                deviceCode = netmacobject.IP;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<InitializeRequest, InitializeRequestCommand>(Initialize);//得到基础数据
                commandobject.OrderVersion = (byte)devicetypeobject.LC2;//设备设备类型
                commandobject.def = deviceinfoobject;
                commandobject.DeviceListItem = Cache.CacheManager.Query<DeviceInfo>(p => p.Fzh == deviceinfoobject.Fzh, true);
                commandobject.DeviceTypeListItem = Cache.CacheManager.GetAll<DeviceTypeInfo>(true); 
                if (commandobject.DeviceListItem == null)
                    commandobject.DeviceListItem = new List<DeviceInfo>();
                if (commandobject.DeviceTypeListItem == null)
                    commandobject.DeviceTypeListItem = new List<DeviceTypeInfo>();
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
            }
         
            return sendbuff;
        }
        public byte[] HandQueryBatteryRealData(MasProtocol masProtocol, QueryBatteryRealDataRequest batteryRealData, ref string deviceCode, ref int commPort)
        {
            List<NetworkDeviceInfo> lstNet = new List<NetworkDeviceInfo>();
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            BatteryRealDataRequestCommand commandobject = new BatteryRealDataRequestCommand();
            byte[] sendbuff = null;
            if (batteryRealData == null)
            {
                return sendbuff;
            }
            if (batteryRealData.DevProperty == ItemDevProperty.Switches)
            {//如果是交换机的操作，仅获取对应的网络地址即可。
                lstNet = Cache.CacheManager.Query<NetworkDeviceInfo>(p => (p.MAC == batteryRealData.DeviceCode) && (p.Upflag == "1"), true);
                if (lstNet.Count > 0)
                    netmacobject = lstNet[0];
                //netmacobject = Cache.CacheManager.GetItemByKey<NetworkDeviceInfo>(batteryRealData.DeviceCode, true);//按照MAC搜索对象                       
                if (netmacobject == null)
                {
                    LogHelper.Error("查询交换机的电源，未找到对应的IP地址【" + batteryRealData.DeviceCode + "】");
                    return sendbuff;
                }
                lock (Cache.LockWorkNet)//用缓存的连接号和接收时间进行更新
                {
                    CacheNetWork curnet = Cache.LstWorkNet.Find(delegate (CacheNetWork p) { return p.IP == netmacobject.Bz6; });
                    if (curnet != null)
                    {
                        netmacobject.NetID = curnet.NetID;
                    }
                }
            }
            else
            {
                if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.QueryBatteryRealDataRequest, batteryRealData.DeviceCode))
                {
                    return sendbuff;
                }
            }
            if (netmacobject.NetID > 0)
            {
                if (batteryRealData.DevProperty == ItemDevProperty.Switches)
                {
                    deviceCode = netmacobject.Bz6;//向下的发送的MAC地址
                    commPort = 1;
                }
                else
                {
                    commPort = deviceinfoobject.K3;

                    deviceCode = netmacobject.IP;//向下的发送的MAC地址
                }
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<QueryBatteryRealDataRequest, BatteryRealDataRequestCommand>(batteryRealData);//得到基础数据
                if (devicetypeobject != null)
                    commandobject.OrderVersion = (byte)devicetypeobject.LC2;
                commandobject.def = deviceinfoobject;// Framework.Common.ObjectConverter.DeepCopy<DeviceInfo>(deviceinfoobject);//得到分站的对象必须是深度拷贝
                commandobject.NetMacObject = netmacobject;
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
            }
            return sendbuff;
        }

        public byte[] HandResetDeviceCommand(MasProtocol masProtocol, ResetDeviceCommandRequest resetDevice, ref string deviceCode, ref int commPort)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            ResetDeviceRequestCommand commandobject = new ResetDeviceRequestCommand();
            byte[] sendbuff = null;
            if (resetDevice == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.ResetDeviceCommandRequest, resetDevice.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                commPort = deviceinfoobject.K3;
                deviceCode = netmacobject.IP;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<ResetDeviceCommandRequest, ResetDeviceRequestCommand>(resetDevice);//得到基础数据
                commandobject.def = deviceinfoobject;//得到分站的对象必须是深度拷贝
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
            }
            return sendbuff;
        }
        public byte[] HandTimeSynchronization(MasProtocol masProtocol, TimeSynchronizationRequest timeSynchronization, ref string deviceCode, ref int commPort)
        {
            DeviceTypeInfo devicetypeobject = null;

            DeviceInfo deviceinfoobject = null;
            NetworkDeviceInfo netmacobject = null;
            TimeSynchronizationRequestCommand commandobject = new TimeSynchronizationRequestCommand();
            byte[] sendbuff = null;
            if (timeSynchronization == null)
            {
                return sendbuff;
            }
            if(timeSynchronization.DeviceCode.Length<4)
            {//服务端只发下了分站号，自动生成分站的测点号
                timeSynchronization.DeviceCode = timeSynchronization.DeviceCode.PadLeft(3, '0') + "0000";
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.TimeSynchronizationRequest, timeSynchronization.DeviceCode))
            {
                return sendbuff;
            }
            lock (Cache.LockWorkNet)//用缓存的连接号和接收时间进行更新
            {
                CacheNetWork curnet = Cache.LstWorkNet.Find(delegate(CacheNetWork p) { return p.IP == netmacobject.IP; });
                if (curnet != null)
                {
                    netmacobject.NetID = curnet.NetID;
                }
            }
            if (netmacobject.NetID > 0)
            {
                commPort = timeSynchronization.CommPort;
                deviceCode = netmacobject.IP;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<TimeSynchronizationRequest, TimeSynchronizationRequestCommand>(timeSynchronization);//得到基础数据
                commandobject.def = deviceinfoobject;//得到分站的对象必须是深度拷贝
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
            }
            return sendbuff;
        }
        public byte[] HandCallPersonCommand(MasProtocol masProtocol, CallPersonRequest callPerson, ref string deviceCode)
        {
            DeviceInfo deviceinfoobject = null;
            NetworkDeviceInfo netmacobject = null;
            CallPersonRequestCommand commandobject = new CallPersonRequestCommand();
            byte[] sendbuff = null;
            if (callPerson == null)
            {
                return sendbuff;
            }
            netmacobject = Cache.CacheManager.GetItemByKey<NetworkDeviceInfo>(callPerson.DeviceCode, true);//按照MAC搜索对象                       
            if (netmacobject == null)
            {
                LogHelper.Error("呼叫命令，未找到对应的MAC地址【" + callPerson.DeviceCode + "】");
                return sendbuff;
            }
            lock (Cache.LockWorkNet)//用缓存的连接号和接收时间进行更新
            {
                CacheNetWork curnet = Cache.LstWorkNet.Find(delegate(CacheNetWork p) { return p.IP == netmacobject.IP; });
                if (curnet != null)
                {
                    netmacobject.NetID = curnet.NetID;
                }
            }
            if (netmacobject.NetID > 0)
            {
                deviceCode = netmacobject.IP;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<CallPersonRequest, CallPersonRequestCommand>(callPerson);//得到基础数据
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.CallPersonCommand, 0);//通过当前网络模块进行打包 
                //}
            }
            return sendbuff;
        }
        public byte[] HandModificationDeviceAdress(MasProtocol masProtocol, ModificationDeviceAdressRequest modificationDeviceAdress, ref string deviceCode, ref int commPort)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            ModificationDeviceAdressRequestCommand commandobject = new ModificationDeviceAdressRequestCommand();
            byte[] sendbuff = null;
            if (masProtocol == null || modificationDeviceAdress == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.ModificationDeviceAdressRequest, modificationDeviceAdress.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                commPort = deviceinfoobject.K3;
                deviceCode = netmacobject.IP;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<ModificationDeviceAdressRequest, ModificationDeviceAdressRequestCommand>(modificationDeviceAdress);//得到基础数据
                commandobject.def = deviceinfoobject;//得到分站的对象必须是深度拷贝
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
            }
            return sendbuff;
        }
        public byte[] HandQueryHistoryControl(MasProtocol masProtocol, QueryHistoryControlRequest queryHistoryControl, ref string deviceCode, ref int commPort)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            QueryHistoryControlRequestCommand commandobject = new QueryHistoryControlRequestCommand();
            byte[] sendbuff = null;
            if (queryHistoryControl == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.QueryHistoryControlRequest, queryHistoryControl.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                commPort = deviceinfoobject.K3;
                deviceCode = netmacobject.IP;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<QueryHistoryControlRequest, QueryHistoryControlRequestCommand>(queryHistoryControl);//得到基础数据
                commandobject.def = deviceinfoobject;//得到分站的对象必须是深度拷贝
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.ExtendCommand, (byte)devicetypeobject.LC2);//通过当前网络模块进行打包 
                //}
            }
            return sendbuff;
        }
        public byte[] HandQueryHistoryRealData(MasProtocol masProtocol, QueryHistoryRealDataRequest queryHistoryRealData, ref string deviceCode, ref int commPort)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            QueryHistoryRealDataRequestCommand commandobject = new QueryHistoryRealDataRequestCommand();
            byte[] sendbuff = null;
            if (queryHistoryRealData == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.QueryHistoryRealDataRequest, queryHistoryRealData.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                commPort = deviceinfoobject.K3;
                deviceCode = netmacobject.IP;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<QueryHistoryRealDataRequest, QueryHistoryRealDataRequestCommand>(queryHistoryRealData);//得到基础数据
                commandobject.def = deviceinfoobject;//得到分站的对象必须是深度拷贝
                commandobject.SerialNumber = queryHistoryRealData.SerialNumber; 
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.ExtendCommand, (byte)devicetypeobject.LC2);//通过当前网络模块进行打包 
                //}
            }
            return sendbuff;
        }
        public byte[] HandSensorGradingAlarm(MasProtocol masProtocol, SetSensorGradingAlarmRequest sensorGradingAlarm, ref string deviceCode, ref int commPort)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            SetSensorGradingAlarmRequestCommand commandobject = new SetSensorGradingAlarmRequestCommand();
            byte[] sendbuff = null;
            if (sensorGradingAlarm == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.SetSensorGradingAlarmRequest, sensorGradingAlarm.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                commPort = deviceinfoobject.K3;
                deviceCode = netmacobject.IP;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<SetSensorGradingAlarmRequest, SetSensorGradingAlarmRequestCommand>(sensorGradingAlarm);//得到基础数据
                commandobject.def = deviceinfoobject;//得到分站的对象必须是深度拷贝
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.ExtendCommand, (byte)devicetypeobject.LC2);//通过当前网络模块进行打包 
                //}
            }
            return sendbuff;
        }

        #region ----远程升级相关----

        /// <summary>
        /// 获取分站的工作状态
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <param name="getStationUpdateStateRequest"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public byte[] HandGetStationUpdateState(MasProtocol masProtocol, GetStationUpdateStateRequest getStationUpdateStateRequest, ref string deviceCode)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            GetStationUpdateStateRequestCommand commandobject = new GetStationUpdateStateRequestCommand();
            byte[] sendbuff = null;
            if (masProtocol == null || getStationUpdateStateRequest == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.DeviceControlRequest, getStationUpdateStateRequest.DeviceCode))
            {
                return sendbuff;
            } 
            if (netmacobject.NetID > 0)
            {
                deviceCode = netmacobject.MAC;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<GetStationUpdateStateRequest, GetStationUpdateStateRequestCommand>(getStationUpdateStateRequest);//得到基础数据

                commandobject.GetSoftVersion = getStationUpdateStateRequest.GetSoftVersion;
                commandobject.GetUpdateState = getStationUpdateStateRequest.GetUpdateState;
                commandobject.GetDevType = getStationUpdateStateRequest.GetDevType;
                commandobject.GetHardVersion = getStationUpdateStateRequest.GetHardVersion;

                commandobject.def = deviceinfoobject;// Framework.Common.ObjectConverter.DeepCopy<DeviceInfo>(deviceinfoobject);//得到分站的对象必须是深度拷贝;
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.ExtendCommand, (byte)devicetypeobject.LC2);//通过当前网络模块进行打包
                //}
            }
            return sendbuff;
        }

        /// <summary>
        /// 巡检单台分站的文件接收情况
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <param name="InspectionRequest"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public byte[] HandInspection(MasProtocol masProtocol, InspectionRequest inspectionRequest, ref string deviceCode)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            InspectionRequestCommand commandobject = new InspectionRequestCommand();
            byte[] sendbuff = null;
            if (masProtocol == null || inspectionRequest == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.DeviceControlRequest, inspectionRequest.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                deviceCode = netmacobject.MAC;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<InspectionRequest, InspectionRequestCommand>(inspectionRequest);//得到基础数据

                commandobject.DeviceId = inspectionRequest.DeviceId;
                commandobject.HardVersion = inspectionRequest.HardVersion;
                commandobject.FileVersion = inspectionRequest.FileVersion;
                commandobject.LostFileNum = inspectionRequest.LostFileNum;
                commandobject.FileBuffer = inspectionRequest.FileBuffer;

                commandobject.def = deviceinfoobject;// Framework.Common.ObjectConverter.DeepCopy<DeviceInfo>(deviceinfoobject);//得到分站的对象必须是深度拷贝;
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.ExtendCommand, (byte)devicetypeobject.LC2);//通过当前网络模块进行打包
                //}
            }
            return sendbuff;
        }
        /// <summary>
        /// 远程还原最近一次备份程序
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <param name="reductionRequest"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public byte[] HandReduction(MasProtocol masProtocol, ReductionRequest reductionRequest, ref string deviceCode)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            ReductionRequestCommand commandobject = new ReductionRequestCommand();
            byte[] sendbuff = null;
            if (masProtocol == null || reductionRequest == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.DeviceControlRequest, reductionRequest.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                deviceCode = netmacobject.MAC;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<ReductionRequest, ReductionRequestCommand>(reductionRequest);//得到基础数据

                commandobject.DeviceId = reductionRequest.DeviceId;
                commandobject.HardVersion = reductionRequest.HardVersion;
                commandobject.SoftVersion = reductionRequest.SoftVersion;

                commandobject.def = deviceinfoobject;// Framework.Common.ObjectConverter.DeepCopy<DeviceInfo>(deviceinfoobject);//得到分站的对象必须是深度拷贝;
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.ExtendCommand, (byte)devicetypeobject.LC2);//通过当前网络模块进行打包
                //}
            }
            return sendbuff;
        }
        /// <summary>
        /// 通知分站进行重启升级
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <param name="restartRequest"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public byte[] HandRestart(MasProtocol masProtocol, RestartRequest restartRequest, ref string deviceCode)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            RestartRequestCommand commandobject = new RestartRequestCommand();
            byte[] sendbuff = null;
            if (masProtocol == null || restartRequest == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.DeviceControlRequest, restartRequest.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                deviceCode = netmacobject.MAC;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<RestartRequest, RestartRequestCommand>(restartRequest);//得到基础数据

                commandobject.DeviceId = restartRequest.DeviceId;
                commandobject.HardVersion = restartRequest.HardVersion;
                commandobject.FileVersion = restartRequest.FileVersion;

                commandobject.def = deviceinfoobject;// Framework.Common.ObjectConverter.DeepCopy<DeviceInfo>(deviceinfoobject);//得到分站的对象必须是深度拷贝;
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.ExtendCommand, (byte)devicetypeobject.LC2);//通过当前网络模块进行打包
                //}
            }
            return sendbuff;
        }
        /// <summary>
        /// 广播升级文件片段
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <param name="sendUpdateBufferRequest"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public byte[] HandSendUpdateBuffer(MasProtocol masProtocol, SendUpdateBufferRequest sendUpdateBufferRequest, ref string deviceCode)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            SendUpdateBufferRequestCommand commandobject = new SendUpdateBufferRequestCommand();
            byte[] sendbuff = null;
            if (masProtocol == null || sendUpdateBufferRequest == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.DeviceControlRequest, sendUpdateBufferRequest.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                deviceCode = netmacobject.MAC;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<SendUpdateBufferRequest, SendUpdateBufferRequestCommand>(sendUpdateBufferRequest);//得到基础数据

                commandobject.DeviceId = sendUpdateBufferRequest.DeviceId;
                commandobject.HardVersion = sendUpdateBufferRequest.HardVersion;
                commandobject.FileVersion = sendUpdateBufferRequest.FileVersion;
                commandobject.NowBufferIndex = sendUpdateBufferRequest.NowBufferIndex;
                commandobject.Buffer = sendUpdateBufferRequest.Buffer;

                commandobject.def = deviceinfoobject;// Framework.Common.ObjectConverter.DeepCopy<DeviceInfo>(deviceinfoobject);//得到分站的对象必须是深度拷贝;
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.BroadCommand, (byte)devicetypeobject.LC2);//通过当前网络模块进行打包
                //}
            }
            return sendbuff;
        }
        /// <summary>
        /// 请求分站远程升级
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <param name="stationUpdateRequest"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public byte[] HandStationUpdate(MasProtocol masProtocol, StationUpdateRequest stationUpdateRequest, ref string deviceCode)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            StationUpdateRequestCommand commandobject = new StationUpdateRequestCommand();
            byte[] sendbuff = null;
            if (masProtocol == null || stationUpdateRequest == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.DeviceControlRequest, stationUpdateRequest.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                deviceCode = netmacobject.MAC;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<StationUpdateRequest, StationUpdateRequestCommand>(stationUpdateRequest);//得到基础数据
                commandobject.Crc = stationUpdateRequest.Crc;
                commandobject.DeviceId = stationUpdateRequest.DeviceId;
                commandobject.FileCount = stationUpdateRequest.FileCount;
                commandobject.FileVersion = stationUpdateRequest.FileVersion;
                commandobject.HardVersion = stationUpdateRequest.HardVersion;
                commandobject.maxVersion = stationUpdateRequest.maxVersion;
                commandobject.minVersion = stationUpdateRequest.minVersion;

                commandobject.def = deviceinfoobject;// Framework.Common.ObjectConverter.DeepCopy<DeviceInfo>(deviceinfoobject);//得到分站的对象必须是深度拷贝;
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.ExtendCommand, (byte)devicetypeobject.LC2);//通过当前网络模块进行打包
                //}
            }
            return sendbuff;
        }

        public byte[] HandUpdateCancle(MasProtocol masProtocol, UpdateCancleRequest updateCancleRequest, ref string deviceCode)
        {
            DeviceInfo deviceinfoobject = null;
            DeviceTypeInfo devicetypeobject = null;
            NetworkDeviceInfo netmacobject = null;
            UpdateCancleRequestCommand commandobject = new UpdateCancleRequestCommand();
            byte[] sendbuff = null;
            if (masProtocol == null || updateCancleRequest == null)
            {
                return sendbuff;
            }
            if (!GetDeviceObject(ref deviceinfoobject, ref devicetypeobject, ref netmacobject, ProtocolType.DeviceControlRequest, updateCancleRequest.DeviceCode))
            {
                return sendbuff;
            }
            if (netmacobject.NetID > 0)
            {
                deviceCode = netmacobject.MAC;//向下的发送的MAC地址
                commandobject = Basic.Framework.Common.ObjectConverter.Copy<UpdateCancleRequest, UpdateCancleRequestCommand>(updateCancleRequest);//得到基础数据

                commandobject.DeviceId = updateCancleRequest.DeviceId;
                commandobject.HardVersion = updateCancleRequest.HardVersion;
                commandobject.FileVersion = updateCancleRequest.FileVersion;

                commandobject.def = deviceinfoobject;// Framework.Common.ObjectConverter.DeepCopy<DeviceInfo>(deviceinfoobject);//得到分站的对象必须是深度拷贝;
                sendbuff = commandobject.ToBytes();//得到通讯命令的返回Buffer数组
                //if (sendbuff != null)
                //{
                //    sendbuff = Cache.GetPackages(sendbuff, netmacobject, deviceinfoobject, 2, CommandCodes.ExtendCommand, (byte)devicetypeobject.LC2);//通过当前网络模块进行打包
                //}
            }
            return sendbuff;
        }

        #endregion
    }
}
