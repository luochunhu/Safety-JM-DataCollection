using Sys.DataCollection.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Driver
{
    /// <summary>
    /// 生成上行对象数据委托
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">上行数据生成事件参数</param>
    public delegate void ProtocolDataEventHandler(object sender, ProtocolDataCreatedEventArgs args);

    /// <summary>
    /// 生成下行的数据包
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">下行数据生成事件参数</param>
    public delegate void NetDataEventHandler(object sender, NetDataEventCreatedArgs args);
    
    /// <summary>
    /// 驱动需要网关执行特殊命令委托
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">命令参数</param>
    /// <returns>成功或者失败</returns>
    public delegate bool DriverCommandEventHandler(object sender, DriverCommandEventArgs args);
}
