using Sys.DataCollection.Common.Cache;
using Sys.DataCollection.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Driver
{   
    /// <summary>
    /// 分站级设备驱动接口
    /// </summary>
    public interface IDeviceDriver
    {
        /// <summary>
        /// 驱动编号（唯一，可重复）
        /// </summary>
        string DriverCode { get; }

        /// <summary>
        /// 网关通讯状态变化通知
        /// </summary>
        /// <param name="stateChangeArgs">通讯状态变化参数</param>
        void CommunicationStateChangeNotify(CommunicationStateChangeArgs stateChangeArgs);

        /// <summary>
        /// 缓存访问器
        /// </summary>
        ICacheManager CacheManager { get; set; }  
        /// <summary>
        /// 处理服务端下发的命令协议数据（对象转Buffer，下行）     
        /// </summary>
        /// <param name="masProtocol">下发的协议对象</param>
        void HandleProtocolData(MasProtocol masProtocol);
        /// <summary>
        /// 处理网络接收的数据（上行）
        /// </summary>
        /// <param name="data">收到的网络数据包</param>
        /// <param name="uniqueCode">数据来源唯一标识（可为mac、手机号、IP等）</param>
        void HandleNetData(byte[] data, string uniqueCode);
        /// <summary>
        /// 驱动产生下行数据事件        
        /// </summary>
        event NetDataEventHandler OnNetDataCreated;
        /// <summary>
        /// 驱动产生上行对象事件
        /// </summary>
        event ProtocolDataEventHandler OnProtocolDataCreated;
        /// <summary>
        /// 驱动在网关程序中执行特殊命令事件
        /// </summary>
        event DriverCommandEventHandler OnExcuteDriverCommand;
    }

}
