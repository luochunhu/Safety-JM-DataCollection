using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 网络设备信息（MAC）
    /// </summary>
    public partial class NetworkDeviceInfo : CacheInfo
    {
        /// <summary>
        /// ID编号
        /// </summary>
        public string ID
        {
            get;
            set;
        }
        /// <summary>
        /// MAC地址
        /// </summary>
        public string MAC
        {
            get;
            set;
        }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP
        {
            get;
            set;
        }
        /// <summary>
        /// 安装位置编号
        /// </summary>
        public string Wzid
        {
            get;
            set;
        }
        /// <summary>
        /// 网络IP模块的连接号
        /// </summary>
        public int NetID
        {
            get;
            set;
        }
        /// <summary>
        /// 状态0	通讯中断，3	交流正常，4	直流正常
        /// </summary>
        public short State
        {
            get;
            set;
        }
        /// <summary>
        /// 是否透明传输标记
        /// </summary>
        public short Istmcs
        {
            get;
            set;
        }
        /// <summary>
        /// 0：网口
        ///1：串口

        /// </summary>
        public short Type
        {
            get;
            set;
        }
        /// <summary>
        /// 串口 波特率 网口 挂接分站设备信息，多个用“|”分隔
        /// </summary>
        public string Bz1
        {
            get;
            set;
        }
        /// <summary>
        /// 串口 通讯制式 网口 交换机IP地址
        /// </summary>
        public string Bz2
        {
            get;
            set;
        }
        /// <summary>
        /// 串口 数据位 网口 空
        /// </summary>
        public string Bz3
        {
            get;
            set;
        }
        /// <summary>
        /// 串口 校验位 网口 空
        /// </summary>
        public string Bz4
        {
            get;
            set;
        }
        /// <summary>
        /// 串口 停止位 网口 空
        /// </summary>
        public string Bz5
        {
            get;
            set;
        }
        /// <summary>
        /// 交换机对应串口服务器的Ip地址
        /// </summary>
        public string Bz6
        {
            get;
            set;
        }
        /// <summary>
        /// 标志
        /// </summary>
        public string Upflag
        {
            get;
            set;
        }
    }
}
