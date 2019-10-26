using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 用于2000搜索时候的结构体对象
    /// </summary>
    //[DataContract]
    //public class C2000BaseStruct
    //{
    //    #region 结构原型
    //    [DataContract]
    //    [Serializable]
    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    public struct _COMSETTING
    //    {
    //        [DataMember]
    //        public uint baudrate;
    //        [DataMember]
    //        public uint databit;
    //        [DataMember]
    //        public uint checkmode;
    //        [DataMember]
    //        public uint stopbit;
    //        [DataMember]
    //        public uint flowmode;
    //        [DataMember]
    //        public uint minsendtime;
    //        [DataMember]
    //        public uint minsendbyte;
    //    }
    //    [DataContract]
    //    [Serializable]
    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    public struct _COMSTATUS
    //    {
    //        [DataMember]
    //        public uint baudrate;
    //        [DataMember]
    //        public uint databit;
    //        [DataMember]
    //        public uint checkmode;
    //        [DataMember]
    //        public uint stopbit;
    //        [DataMember]
    //        public uint flowmode;
    //        [DataMember]
    //        public uint flowstatus;
    //        [DataMember]
    //        public uint minsendtime;
    //        [DataMember]
    //        public uint minsendbyte;
    //        [DataMember]
    //        public uint nBytesSend;
    //        [DataMember]
    //        public uint nBytesRecv;
    //    }
    //    [DataContract]
    //    [Serializable]
    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    public struct _CONVINFO
    //    {
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        [DataMember]
    //        public string strMAC;
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        [DataMember]
    //        public string strIP;
    //        [DataMember]
    //        public ushort devType;
    //    }

    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    [DataContract]
    //    [Serializable]
    //    public struct _CONVSETTING
    //    {
    //        [DataMember]
    //        public _NETSETTING netSetting;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    //        public _SOCKSETTING[] sockSetting;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    //        public _COMSETTING[] comSetting;
    //    }
    //    [DataContract]
    //    [Serializable]
    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    public struct _CONVSTATUS
    //    {
    //        [DataMember]
    //        public _NETSTATUS netStatus;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    //        public _SOCKSTATUS[] sockStatus;
    //        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    //        [DataMember]
    //        public _COMSTATUS[] comStatus;
    //    }
    //    [DataContract]
    //    [Serializable]
    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    public struct _NETSETTING
    //    {
    //        [DataMember]
    //        public int bStaticIP;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipAddr;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipMask;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipGateway;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipDns;
    //    }
    //    [DataContract]
    //    [Serializable]
    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    public struct _NETSTATUS
    //    {
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipAddr;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipMask;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipGateway;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipDns;
    //    }
    //    [DataContract]
    //    [Serializable]
    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    public struct _SOCKSETTING
    //    {
    //        [DataMember]
    //        public int iMode;
    //        [DataMember]
    //        public int port;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipServer;
    //        [DataMember]
    //        public int ServerPort;
    //        [DataMember]
    //        public int bUseOcx;
    //    }
    //    [DataContract]
    //    [Serializable]
    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    public struct _SOCKSTATUS
    //    {
    //        [DataMember]
    //        public int iMode;
    //        [DataMember]
    //        public int port;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipServer;
    //        [DataMember]
    //        public int ServerPort;
    //        [DataMember]
    //        public int bUseOcx;
    //        [DataMember]
    //        public int iConnStatus;
    //        [DataMember]
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    //        public string ipPeer;
    //        [DataMember]
    //        public int PeerPort;
    //        [DataMember]
    //        public uint nBytesSend;
    //        [DataMember]
    //        public uint nBytesRecv;
    //    }
    //    #endregion
    //}

    /// <summary>
    /// 设备参数对象
    /// </summary>
    public class NetDeviceSettingInfo
    {
        /// <summary>
        /// 设备网络设置基本信息
        /// </summary>
        public Netsetting NetSetting { get; set; }

        /// <summary>
        /// 服务端套接字信息
        /// </summary>
        public SocketSetting[] SockSetting { get; set; }

        /// <summary>
        /// 串口设备信息
        /// </summary>
        public ComSetting[] ComSetting { get; set; }
    }

    /// <summary>
    /// 设备网络基本信息
    /// </summary>
    public class Netsetting
    {  /// <summary>
        /// 原始数据包---用于回填的时候使用
        /// </summary>
        public byte[] srcPacket = new byte[67];
        /// <summary>
        /// 是否使用静态IP
        /// 0：使用DHCP动态获取 1:使用静态IP
        /// </summary>
        public int IsUseStaticIP;
        /// <summary>
        /// IP地址 
        /// </summary>
        public string IpAddr;
        /// <summary>
        /// 子网掩码
        /// </summary>
        public string SubMask;
        /// <summary>
        /// 网关IP
        /// </summary>
        public string GatewayIp;
        /// <summary>
        /// DNS IP地址
        /// </summary>
        public string DnsIp;
        /// <summary>
        /// 用户名---Add
        /// </summary>
        public string User;
        /// <summary>
        /// 密码---Add
        /// </summary>
        public string PassWord;
        /// <summary>
        /// 模块名称---Add
        /// </summary>
        public string NetWorkName;
        /// <summary>
        /// 设备分站对应的地址号
        /// </summary>
        public byte SetFzh;
        ///ADD
        /// <summary>
        /// 千兆光口状态，数组长度为2
        /// 【0】=(千兆光口1状态):
        ///0：断开；1：正常
        /// 【1】=(千兆光口2状态):
        ///0：断开；1：正常
        /// </summary>
        public byte[] Switch1000JkState;
        /// <summary>
        /// 百兆光口状态，数组长度为5
        /// 【0】=(百兆接口1状态):
        ///0：断开；1：正常
        /// 【1】=(百兆接口2状态):
        ///0：断开；1：正常
        /// 【2】=(百兆接口3状态):
        ///0：断开；1：正常
        /// 【3】=(百兆接口4状态):
        ///0：断开；1：正常
        /// 【4】=(百兆接口5状态):
        ///0：断开；1：正常
        /// </summary>
        public byte[] Switch100JkState;
        /// <summary>
        ///  百兆电口状态，数组长度为3
        /// 【0】=(百兆接口1状态):
        ///0：断开；1：正常
        /// 【1】=(百兆接口2状态):
        ///0：断开；1：正常
        /// 【2】=(百兆接口3状态):
        ///0：断开；1：正常
        /// </summary>
        public byte[] Switch100RJ45State;
    }
    /// <summary>
    /// 服务端网络设置
    /// </summary>
    public class SocketSetting
    {
        /// <summary>
        /// 原始数据包---用于回填的时候使用
        /// </summary>
        public byte[] srcPacket = new byte[63];
        /// <summary>
        /// 工作模式
        /// 0：UDP,1：TCPClient,2：UDPServer, 3：TCP Server, 4：HTTPD Client 
        /// </summary>
        public int Mode;
        /// <summary>
        /// 设备端口号（估计只有 工作模式iMode=1时（设备作为服务端模式时），此端口号才有用处）
        /// </summary>
        public int Port;
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string IpServer;
        /// <summary>
        /// 服务器端口号
        /// </summary>
        public int ServerPort;
        /// <summary>
        /// 使用控件或虚拟串口 0.不使用 1.使用
        /// </summary>
        public int IsUseOcx;
    }

    /// <summary>
    /// 串口设置
    /// </summary>
    public class ComSetting
    {
        /// <summary>
        /// 波特率  1200
        ///         2400
        ///         4800
        ///         9600
        ///         19200
        ///         28800
        ///         38400
        ///         57600
        ///         115200
        /// </summary>
        public uint Baudrate;
        /// <summary>
        /// 数据位 6、7、8
        /// </summary>
        public uint Databit;
        /// <summary>
        /// 校验位
        /// 0.无; 1.奇效验; 2.偶效验; 3.标记; 4.空格
        /// </summary>
        public uint CheckMode;
        /// <summary>
        /// 停止位 1、2
        /// </summary>
        public uint StopBit;
        /// <summary>
        /// 流量控制 1. 无流量控制；3.硬件流制(CTS/RTS)；5.RS485
        /// </summary>
        public uint FlowMode;
        /// <summary>
        /// 最小发送时间(0-65535毫秒)
        /// </summary>
        public uint MinSendTime;
        /// <summary>
        /// 最小发送字节(0-1000)
        /// </summary>
        public uint MinSendByte;
    }


}
