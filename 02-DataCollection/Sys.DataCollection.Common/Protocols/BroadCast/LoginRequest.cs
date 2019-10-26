using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 登录iNBS服务器
    /// </summary>
    public class LoginRequest 
    {
        /// <summary>
        /// 控制台标识
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 控制台登录密码(服务器后台设置)
        /// </summary>
        public string secret { get; set; }
        /// <summary>
        /// 回调URL地址，即服务器发送通知到第三方应用
        /// </summary>
        public string callbackUrl { get; set; }        
    }
}
