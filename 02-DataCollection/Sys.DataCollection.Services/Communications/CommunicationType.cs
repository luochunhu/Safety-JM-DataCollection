using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Communications
{
    /// <summary>
    /// 通讯组件类型
    /// </summary>
    public enum CommunicationType
    {
        /// <summary>
        /// 标准通讯组件（待实现）
        /// </summary>
        MAS = 0,
        /// <summary>
        /// 宏电通讯组件（待实现）
        /// </summary>
        HongDian = 1,
        /// <summary>
        /// 8962通讯组件 TCP服务端模式
        /// </summary>
        C8962=2,
        /// <summary>
        /// Http通讯
        /// </summary>
        Http = 3,
        /// <summary>
        /// 客户端通讯模式
        /// </summary>
        socketClient=4
    }
}
