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
    public class LoginResponse 
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public string retCode { get; set; }
        /// <summary>
        /// 令牌
        /// </summary>
        public string accessToken { get; set; }
        /// <summary>
        /// 超时时间(单位:秒)
        /// </summary>
        public string expires { get; set; }        
    }
}
