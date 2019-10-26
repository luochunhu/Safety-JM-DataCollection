using Basic.Framework.Common;
using Basic.Framework.Logging;
using Basic.Framework.Rpc;
using Sys.DataCollection.Common.Protocols;
using DC.Communication.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sys.DataCollection.Communications.Provider
{
    /// <summary>
    /// 8962Udp处理模块
    /// </summary>
    public class C8962UdpCommunication
    {
        static string _netServerIp = "";

        static C8962UdpCommunication()
        {
            //modified by  20170713
            //服务端的IP（此IP为网络设置中设置的服务器IP，可能为虚拟IP）
            _netServerIp = System.Configuration.ConfigurationManager.AppSettings["NetServerIp"];
        }

        /// <summary>
        /// 处理Udp模块请求
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse HandleDeviceUdpRequest(MasProtocol masProtocol)
        {                      
            if (masProtocol.ProtocolType == ProtocolType.ResetDeviceCommandRequest)
            {
                var request = masProtocol.Deserialize<ResetDeviceCommandRequest>();
                return ResetDevice(request);
            }
            else if (masProtocol.ProtocolType == ProtocolType.SearchNetworkDeviceRequest)
            {
                if (masProtocol.StationFind == 1)
                    return SearchNetworkDevice(1);
                else
                    return SearchNetworkDevice();

            }
            else if (masProtocol.ProtocolType == ProtocolType.QuerytNetworkDeviceParamRequest)
            {
                var request = masProtocol.Deserialize<QuerytNetworkDeviceParamRequest>();
                return GetNetworkDeviceParam(request);
            }
            else if (masProtocol.ProtocolType == ProtocolType.SetNetworkDeviceParamRequest)
            {
                var request = masProtocol.Deserialize<SetNetworkDeviceParamRequest>();
                return SetNetworkDeviceParam(request);
            }
            else if (masProtocol.ProtocolType == ProtocolType.SetNetworkDeviceParamCommRequest)
            {
                var request = masProtocol.Deserialize<SetNetworkDeviceParamCommRequest>();
                return SetNetworkDeviceParamComm(request);
            }
            return null;
        }
        /// <summary>
        /// 设备复位
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static RpcResponse ResetDevice(ResetDeviceCommandRequest request)
        {
            bool result = SocketUDPServer.ResetConverter(request.Mac, 5000, _netServerIp);           

            ResetDeviceCommandResponse response = new Sys.DataCollection.Common.Protocols.ResetDeviceCommandResponse();
            if (result)
            {
                response.ReturnCode = 1;
            }
            else
            {
                response.ReturnCode = 2;
            }
            LogHelper.Info("ResetDevice:设备复位");
            return RpcResponse.Response<ResetDeviceCommandResponse>(response);
        }

        /// <summary>
        /// 复位网络模块
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        internal static bool ResetDevice(string mac,int timeout)
        {
            LogHelper.Info("ResetDevice:复位网络模块");
            return SocketUDPServer.ResetConverter(mac, timeout , _netServerIp);
        }

        /// <summary>
        /// 搜索网络模块
        /// </summary>
        /// <returns></returns>
        private static RpcResponse SearchNetworkDevice(int stationfind=0)
        {
            SearchNetworkDeviceResponse response = new SearchNetworkDeviceResponse();

            try
            {
                List<NetDeviceInfo> info = SocketUDPServer.FindConverter(2000, _netServerIp, stationfind.ToString());

                List<NetworkDeviceItem> networkDeviceList = new List<NetworkDeviceItem>();
                if (info != null && info.Count > 0)
                {
                    for (int i = 0; i < info.Count; i++)
                    {
                        NetworkDeviceItem item = new NetworkDeviceItem();
                        item.Mac = info[i].Mac;//网络模块MAC
                        item.Ip = info[i].Ip;//网络模块IP
                        item.SwitchMac = info[i].SwitchMac;//网络模块所属交换机MAC
                        item.AddressNumber = info[i].AddressNumber;//网络模块在交换机中的地址编码
                        item.DeviceType = info[i].DeviceType;
                        item.StationAddress = info[i].StationAddress;
                        item.DeviceSN = info[i].DeviceSN;
                        item.GatewayIp = info[i].GatewayIp;
                        item.SubMask = info[i].SubMask;
                        networkDeviceList.Add(item);
                    }
                }

                response.NetworkDeviceItems = networkDeviceList;
            }
            catch (Exception ex)
            {
                LogHelper.Error("搜索网络模块", ex);
            }

            return RpcResponse.Response<SearchNetworkDeviceResponse>(response);
        }

        /// <summary>
        /// 获取网络设置参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static RpcResponse GetNetworkDeviceParam(QuerytNetworkDeviceParamRequest request)
        {
            QuerytNetworkDeviceParamResponse response = new QuerytNetworkDeviceParamResponse();

            try
            {
                NetDeviceSetting setting = new NetDeviceSetting();
                bool result = SocketUDPServer.GetConvSetting(request.Mac, out setting, 2000, _netServerIp);

                if (result)
                {
                    response.NetworkDeviceParam = JSONHelper.ParseJSONString<NetDeviceSettingInfo>(JSONHelper.ToJSONString(setting));
                    //response.NetworkDeviceParam.ComSetting

                    //response.NetworkDeviceParam = ObjectConverter.Copy<NetDeviceSetting, NetDeviceSettingInfo>(setting);

                  
                }

              //  LogHelper.Info("GetNetworkDeviceParam: Result:" + result.ToString() + " Setting" + Basic.Framework.Common.JSONHelper.ToJSONString(setting));
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetNetworkDeviceParam() error:" + ex.ToString());
            }

            return RpcResponse.Response<QuerytNetworkDeviceParamResponse>(response);
        }

        /// <summary>
        /// 设置网络设备参数---基础参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static RpcResponse SetNetworkDeviceParam(SetNetworkDeviceParamRequest request)
        {
            SetNetworkDeviceParamResponse response = new SetNetworkDeviceParamResponse();

            try
            {

                NetDeviceSetting setting = JSONHelper.ParseJSONString<NetDeviceSetting>(JSONHelper.ToJSONString(request.NetworkDeviceParam));
                if (string.IsNullOrEmpty(request.StationFind))
                    request.StationFind = "0";
                bool result = SocketUDPServer.SetConvSetting(request.Mac, setting, 6000, _netServerIp, request.StationFind);
                if (result)
                {
                    response.ExeRtn = 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SetNetworkDeviceParam() error:" + ex.ToString());
            }

            return RpcResponse.Response<SetNetworkDeviceParamResponse>(response);
        }
        /// <summary>
        /// 设置网络设备参数---串口参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static RpcResponse SetNetworkDeviceParamComm(SetNetworkDeviceParamCommRequest request)
        {
            SetNetworkDeviceParamCommResponse response = new SetNetworkDeviceParamCommResponse();

            try
            {

                NetDeviceSetting setting = JSONHelper.ParseJSONString<NetDeviceSetting>(JSONHelper.ToJSONString(request.NetworkDeviceParam));
                bool result = SocketUDPServer.SetConvCommSetting(request.Mac, setting, 6000, _netServerIp, request.CommPort);
                if (result)
                {
                    response.ExeRtn = 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SetNetworkDeviceParam() error:" + ex.ToString());
            }

            return RpcResponse.Response<SetNetworkDeviceParamCommResponse>(response);
        }
    }
}
