using Sys.DataCollection.Common.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.WebApi
{
    /// <summary>
    /// 广播系统上行数据事件
    /// </summary>
    public class BroadCastUpEvent
    {
        /// <summary>
        /// 生成广播上行对象数据委托
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">上行数据生成事件参数</param>
        public delegate void BroadCastProtocolDataEventHandler(object sender, ProtocolDataCreatedEventArgs args);
        /// <summary>
        /// 广播系统服务接口产生上行对象事件
        /// </summary>
        public static event BroadCastProtocolDataEventHandler OnBraodCastProtocolDataCreated;
        /// <summary>
        /// 接收上行数据处理的接口
        /// </summary>
        /// <param name="pdcEvents"></param>
        public static void OnBroadCastProtocolData(ProtocolDataCreatedEventArgs pdcEvents)
        {
            if (OnBraodCastProtocolDataCreated != null)
            {
                //触发事件
                OnBraodCastProtocolDataCreated(null, pdcEvents);
            }
        }
    }
}
