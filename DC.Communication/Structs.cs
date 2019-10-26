using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace DC.Communication.Components
{
    /// <summary>
    /// 获取设备返回对象
    /// </summary>
    public class NetDeviceInfo
    {
        /// <summary>
        /// 设备类型 1.c2000设备 2.新网桥8962设备 3.其它设备
        /// </summary>
        public ushort DeviceType;
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip;
        /// <summary>
        /// 网络模块MAC地址
        /// </summary>
        public string Mac;        
        /// <summary>
        /// A.表示模块在交换机中的地址号（值为1-6）  B.表示设备类型号（值>6）
        /// </summary>
        public int AddressNumber;
        /// <summary>
        /// 所属交换机的MAC地址（只有devType=2时）
        /// </summary>
        public string SwitchMac;

        /// <summary>
        /// 分站地址号(只能DevType=3时，此字段才有效)
        /// </summary>
        public int StationAddress { get; set; }   
        /// <summary>
        /// 设备唯一编号(只能DevType=3时，此字段才有效)
        /// </summary>
        public int DeviceSN { get; set; }
        /// <summary>
        /// 子网掩码
        /// </summary>
        public string SubMask;
        /// <summary>
        /// 网关IP
        /// </summary>
        public string GatewayIp;
    }

    /// <summary>
    /// 设备参数对象
    /// </summary>
    public class NetDeviceSetting
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
    {
        /// <summary>
        /// 原始数据包---用于回填的时候使用
        /// </summary>
        public byte[] srcPacket=new byte[67];
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
        public byte[] srcPacket=new byte[63];
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
        /// 使用控件或虚拟串口 0.使用 1.不使用
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
