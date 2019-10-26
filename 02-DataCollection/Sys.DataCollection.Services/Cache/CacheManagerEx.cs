using Basic.Framework.Logging;
using Basic.Framework.Web;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.DataCollection.Common.Rpc;

namespace Sys.DataCollection.Cache
{
    /// <summary>
    /// 缓存扩展类
    /// </summary>
    public partial class CacheManager
    {
        /// <summary>
        /// 启动初始化缓存
        /// </summary>
        public void Start()
        {
            MasProtocol masProtocol = new MasProtocol(SystemType.Security, DirectionType.Up, ProtocolType.QueryCacheDataRequest);
            masProtocol.Protocol = new QueryCacheDataRequest();

            var result = GatewayManager.RpcManager.Send<QueryCacheDataResponse>(masProtocol, RequestType.BusinessRequest);

            if (result == null)
            {
                return;
            }

            if (result.DeviceList != null && result.DeviceList.Count > 0)
            {
                GatewayManager.CacheManager.AddItems<DeviceInfo>(result.DeviceList);

                LogHelper.Debug("获取测点缓存成功，数量：" + result.DeviceList.Count);
            }
            else
            {
                LogHelper.Debug("获取测点缓存成功，数量：0");
            }

            if (result.DeviceTypeList != null && result.DeviceTypeList.Count > 0)
            {
                GatewayManager.CacheManager.AddItems<DeviceTypeInfo>(result.DeviceTypeList);

                LogHelper.Debug("获取测点类型缓存成功，数量：" + result.DeviceTypeList.Count);
            }
            else
            {
                LogHelper.Debug("获取测点类型缓存成功，数量：0");
            }

            if (result.NetworkDeviceList != null && result.NetworkDeviceList.Count > 0)
            {
                GatewayManager.CacheManager.AddItems<NetworkDeviceInfo>(result.NetworkDeviceList);

                LogHelper.Debug("获取网络模块缓存成功，数量：" + result.NetworkDeviceList.Count);
            }
            else
            {
                LogHelper.Debug("获取网络模块缓存成功，数量：0");
            }

            if (result.DeviceAcrossControlList != null && result.DeviceAcrossControlList.Count > 0)
            {
                GatewayManager.CacheManager.AddItems<DeviceAcrossControlInfo>(result.DeviceAcrossControlList);

                LogHelper.Debug("获取交叉控制缓存成功，数量：" + result.DeviceAcrossControlList.Count);
            }
            else
            {
                LogHelper.Debug("获取交叉控制缓存成功，数量：0");
            }
        }

        /// <summary>
        /// 停止缓存模块
        /// </summary>
        public void Stop()
        {
            ClearCache();
            LogHelper.SystemInfo("停止（清空）缓存模块完成");
        }

        /// <summary>
        /// 核心服务层同步缓存到网关的处理
        /// </summary>
        /// <param name="masProtocol"></param>
        public void HandleUpdateCacheDataRequest(MasProtocol masProtocol)
        {
            UpdateCacheDataRequest request = masProtocol.Deserialize<UpdateCacheDataRequest>();
            if (request == null)
            {
                return;
            }

            if (request.DeviceList != null)
            {
                HandleDeviceCache(request.DeviceList);//测点同步
            }
            if (request.DeviceTypeList != null)
            {
                HandleDeviceTypeCache(request.DeviceTypeList);//设备类型同步
            }
            if (request.NetworkDeviceList != null)
            {
                HandleNetworkDeviceCache(request.NetworkDeviceList);//网络模块同步
            }
            if (request.DeviceAcrossControlList != null)
            {
                HandleDeviceAcrossControlCache(request.DeviceAcrossControlList);//交叉控制同步
            }
        }

        /// <summary>
        /// 测点同步
        /// </summary>
        /// <param name="list"></param>
        private void HandleDeviceCache(List<DeviceInfo> list)
        {
            foreach (var item in list)
            {
                if (item.InfoState == InfoState.AddNew)
                {
                    this.AddItem<DeviceInfo>(item);
                }
                else if (item.InfoState == InfoState.Modified)
                {
                    this.UpdateItem<DeviceInfo>(item);
                }
                else if (item.InfoState == InfoState.Delete)
                {
                    this.DeleteItem<DeviceInfo>(item);
                }
            }
        }

        /// <summary>
        /// 设备类型同步
        /// </summary>
        /// <param name="list"></param>
        private void HandleDeviceTypeCache(List<DeviceTypeInfo> list)
        {
            foreach (var item in list)
            {
                if (item.InfoState == InfoState.AddNew)
                {
                    this.AddItem<DeviceTypeInfo>(item);
                }
                else if (item.InfoState == InfoState.Modified)
                {
                    this.UpdateItem<DeviceTypeInfo>(item);
                }
                else if (item.InfoState == InfoState.Delete)
                {
                    this.DeleteItem<DeviceTypeInfo>(item);
                }
            }
        }

        /// <summary>
        /// 网络模块同步
        /// </summary>
        /// <param name="list"></param>
        private void HandleNetworkDeviceCache(List<NetworkDeviceInfo> list)
        {
            foreach (var item in list)
            {
                if (item.InfoState == InfoState.AddNew)
                {
                    this.AddItem<NetworkDeviceInfo>(item);
                }
                else if (item.InfoState == InfoState.Modified)
                {
                    this.UpdateItem<NetworkDeviceInfo>(item);
                }
                else if (item.InfoState == InfoState.Delete)
                {
                    this.DeleteItem<NetworkDeviceInfo>(item);
                }
            }
        }

        /// <summary>
        /// 交叉控制同步
        /// </summary>
        /// <param name="list"></param>
        private void HandleDeviceAcrossControlCache(List<DeviceAcrossControlInfo> list)
        {
            foreach (var item in list)
            {
                if (item.InfoState == InfoState.AddNew)
                {
                    this.AddItem<DeviceAcrossControlInfo>(item);
                }
                else if (item.InfoState == InfoState.Modified)
                {
                    this.UpdateItem<DeviceAcrossControlInfo>(item);
                }
                else if (item.InfoState == InfoState.Delete)
                {
                    this.DeleteItem<DeviceAcrossControlInfo>(item);
                }
            }
        }


    }
}
