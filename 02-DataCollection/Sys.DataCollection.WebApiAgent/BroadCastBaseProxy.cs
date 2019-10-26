using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.WebApiAgent
{
    /// <summary>
    /// 代理基类
    /// </summary>
    public class BroadCastBaseProxy
    {
        public string Token = "token";
        /// <summary>
        /// 远程服务器IP
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 远程服务器端口
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 获取API URL
        /// </summary>
        public string Webapi
        {
            get
            {
                return GetApiUrl();
            }
        }

        private string GetApiUrl()
        {
            string url = "";            
            if (string.IsNullOrEmpty(Ip))
            {
                //Ip = System.Configuration.ConfigurationManager.AppSettings["BroadCastServerIp"];
                //Port = System.Configuration.ConfigurationManager.AppSettings["BroadCastServerPort"];
            }
            url = string.Format("http://{0}:{1}", Ip, Port);
            return url;
        }
    }
}
