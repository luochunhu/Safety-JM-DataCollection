using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace DC.Communication.Components
{
    /// <summary>
    /// UDP服务处理类
    /// </summary>
    public static class SocketUDPServer
    {
        static SocketUDPHandler _udpHandler;

        //搜索的设备回复列表
        static Dictionary<string, NetDeviceInfo> _searchList;

        static NetDeviceSetting _setting;//获取参数的回复对象

        static bool _result = false;//其它设置返回的结果 

        static ManualResetEvent _resetEvent;//信号通知

        /// <summary>
        /// 当前处理的MAC地址
        /// </summary>
        static string _currentMac = "";

        static SocketUDPServer()
        {
            _searchList = new Dictionary<string, NetDeviceInfo>();
            _resetEvent = new ManualResetEvent(false);

            _udpHandler = new SocketUDPHandler();

            _udpHandler.OnDataArrive += new DataArriveEventHandler(_udpHandler_OnDataArrive);

            //现在改为模块打开时，初始化端口（原来是每次调用发送命令时打开和关闭端口）
            string netServerIp = System.Configuration.ConfigurationManager.AppSettings["NetServerIp"];
            _udpHandler.OpenSocket(netServerIp);
        }

        /// <summary>
        /// 收到UDP数据的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void _udpHandler_OnDataArrive(object sender, DataArriveEventArgs e)
        {
            if (e == null || e.Data == null || e.Data.Length < 4)//收到的数据包长度判断
            {
                return;
            }
            HandleReceiveData(e.Data);//路由数据包
        }

        /// <summary>
        /// 收到的数据包处理
        /// </summary>
        /// <param name="data"></param>
        private static void HandleReceiveData(byte[] data)
        {
            ushort uType = 0;
            if ((data[0] == 0x3E) && (data[1] == 0xE3) && (data[2] == 0x90) && (data[3] == 0x09) && (data[5] == 0x4C))
            {
                HandleSearchFzhData(data);
            }
            else if ((data[0] == 0x3E) && (data[1] == 0xE3) && (data[2] == 0x90) && (data[3] == 0x09) && (data[5] == 0x4D))
            {
                HandleSetFzhData(data);
            }
            else
            {//52 53 57 52 41 4c 4c 50 4f 52 54 4c 49 4e 4b 53 54 41 54 45 00 22 6f 0c e7 07 00 00 01 00 00 00 00 00 01 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
                if (data.Length < 15) return;
                if (((data[0] == 0x7F) && (data[data.Length - 1] == 0x0D)) ||
                    ((data[0] == 0x52) && (data[1] == 0x53) && (data[2] == 0x57) && (data[3] == 0x52) && (data[4] == 0x41) && (data[5] == 0x4c)))
                {
                    uType = (ushort)((data[1] << 8) + data[2]);
                    switch (uType)//效验第19位
                    {
                        case 0x1004:
                            HandleSearchData(data);//OK
                            break;
                        case 0x1006://设置交换机IP信息参数向上回复
                            HandleSetBaseParameter(data);//已写待测试
                            break;
                        case 0x5357://交换机状态获取回发
                            GetNetBaseinfo(data);
                            break;
                    }
                }
            }
        }

        #region 搜索及回复

        /// <summary>
        /// 搜索网络设备
        /// </summary>
        /// <param name="timeout">等待超时时间，单位毫秒</param>
        /// <param name="bindIp">指定固定的绑定IP发送数据</param>
        /// <param name="stationfind">搜索分站为1，搜索交换机为0</param>
        /// <returns>网络设备列表</returns>
        public static List<NetDeviceInfo> FindConverter(int timeout, string bindIp = "", string stationfind = "0")
        {
            Console.WriteLine("开始搜索设备 01");

            _searchList.Clear();

            if (stationfind == "1")
                _udpHandler.Send(Util.GetDataByType("01", ""), 1);
            else
                _udpHandler.Send(Util.GetDataByType("10", ""), 1);

            Thread.Sleep(timeout);

            return new List<NetDeviceInfo>(_searchList.Values);
        }
        /// <summary>
        /// 接收到 分站搜索应答处理
        /// </summary>
        /// <param name="data"></param>
        private static void HandleSearchFzhData(byte[] data)
        {
            ushort ljl = 0;

            Console.WriteLine("FZH:" + data[4] + " MAC:" + Util.ConvertByteToMac(data, 13) + " IP:" + Util.ConvertByteToIP(data, 9) + " 收到设备回复 02 " + data.Length);
            if (data.Length < 29)
                return;
            for (int i = 4; i < data.Length - 2; i++)
                ljl += data[i];
            if (ljl == ((data[data.Length - 1] << 8) + data[data.Length - 2]))
            {
                NetDeviceInfo info = new NetDeviceInfo();
                info.DeviceType = 3;
                info.Mac = BitConverter.ToString(data, 13, 6).Replace('-', '.');
                info.Ip = Util.ConvertByteToIP(data, 9);
                info.SubMask = Util.ConvertByteToIP(data, 19);
                info.GatewayIp = Util.ConvertByteToIP(data, 23);
                info.StationAddress = data[4];
                if (!_searchList.ContainsKey(info.Ip))
                {
                    _searchList.Add(info.Ip, info);
                }
            }
        }

        /// <summary>
        /// 接收到 交换机设备搜索应答处理
        /// </summary>
        /// <param name="data"></param>
        private static void HandleSearchData(byte[] data)
        {
            if (data.Length != 208)
            {
                return;
            }
            if (data[5] != 0x11 && data[5] != 0x12)
            {
                return;
            }
            Console.WriteLine("MAC:" + Util.ConvertByteToMac(data, 72) + " 收到设备回复 02 ");

            NetDeviceInfo info = new NetDeviceInfo();
            info.DeviceType = 1;
            info.Mac = BitConverter.ToString(data, 72, 6).Replace('-', '.');
            info.Ip = Util.ConvertByteToIP(data, 78);
            info.SubMask= Util.ConvertByteToIP(data, 82);
            info.GatewayIp = Util.ConvertByteToIP(data, 86);

            if (!_searchList.ContainsKey(info.Ip))
            {
                _searchList.Add(info.Ip, info);
            }

        }

        #endregion 搜索及回复

        #region 获取设备配置参数相关
        private static void GetNetBaseinfo(byte[] data)
        {//52 53 57 52 41 4c 4c 50 4f 52 54 4c 49 4e 4b 53 54 41 54 45 00 22 6f 0c e7 07 00 00 01 00 00 00 00 00 01 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
            if (data.Length > 37)
            {
                int i, index = 0;
                _getConvSettingResult = true;
                _setting.NetSetting = new Netsetting();
                _setting.NetSetting.srcPacket = data;
                _setting.NetSetting.IpAddr = Util.ConvertByteToMac(data, 20).Replace('-', '.');//20 21 22 23 24 25 ---MAC地址
                _setting.NetSetting.Switch100RJ45State = new byte[3];
                index = 26;
                for (i = 0; i < 3; i++)
                {
                    _setting.NetSetting.Switch100RJ45State[i] = data[index++];//百兆电口
                }
                _setting.NetSetting.Switch100JkState = new byte[4];
                for (i = 0; i < 4; i++)
                {
                    _setting.NetSetting.Switch100JkState[i] = data[index++];//百兆光口
                }
                _setting.NetSetting.Switch1000JkState = new byte[3];
                for (i = 0; i < 3; i++)
                {
                    _setting.NetSetting.Switch1000JkState[i] = data[index++];//千兆光口
                }
            }
        }
        static bool _getConvSettingResult = false;
        /// <summary>
        /// 获取设备配置参数
        /// </summary>
        /// <param name="strMAC">mac地址</param>
        /// <param name="pConvSetting">配置参数结构体</param>
        /// <param name="timeout">等待超时时间，单位毫秒。</param>
        /// <param name="bindIp">指定固定的绑定IP发送数据</param>
        /// <param name="stationfind">搜索分站为1，搜索交换机为0</param>
        /// <returns>true:成功；false:失败。备注：如果在超时时间内收到设备回复，则会立即返回结果 </returns>
        public static bool GetConvSetting(string strMAC, out NetDeviceSetting pConvSetting, int timeout, string bindIp = "", string stationfind = "0")
        {
            Console.WriteLine(DateTime.Now.ToString("yyyyMMdd HH:mm:ss ffff") + "  MAC:" + strMAC + " 获取交换机的基础信息 03");

            _getConvSettingResult = false;

            _setting = new NetDeviceSetting();

            if (stationfind == "0")
                _udpHandler.Send(Util.GetDataByType("11", strMAC), 1);

            _resetEvent.WaitOne(timeout);//信号量等待

            pConvSetting = _setting;

            _resetEvent.Reset();
            
            return _getConvSettingResult;
        }
        

        #endregion 获取设备配置参数相关

        #region 设置设备基础配置参数相关
        static bool _setConvSettingResult = false;
        /// <summary>
        /// 设置设备配置参数
        /// </summary>
        /// <param name="strMAC">mac地址</param>
        /// <param name="pConvSetting">配置参数结构体</param>
        /// <param name="timeout">等待超时时间，单位毫秒。</param>
        /// <param name="bindIp">指定固定的绑定IP发送数据</param>
        /// <param name="stationfind">搜索分站为1，搜索交换机为0</param>
        /// <returns>true:成功；false:失败。备注：如果在超时时间内收到设备回复，则会立即返回结果</returns>
        public static bool SetConvSetting(string strMAC, NetDeviceSetting pConvSetting, int timeout, string bindIp = "", string stationfind = "0")
        {
            Console.WriteLine("MAC:" + strMAC + " 开始设置网络设备基础配置参数 05");
            byte[] data = null;
            ushort ljl = 0;

            _resetEvent = new ManualResetEvent(false);

            _currentMac = strMAC;

            _setConvSettingResult = false;

            if (stationfind == "1")//表示设置的是分站的IP信息
            {
                data = new byte[37];
                data[0] = 0x3E;
                data[1] = 0xE3;
                data[2] = 0x80;
                data[3] = 0x08;
                data[4] = 0;
                data[5] = 0x4D;
                data[6] = 27;
                data[7] = 0x00;
                Util.ConvertMacToByte(strMAC, data, 8);//8,9,10,11,12,13
                Util.ConvertIPToByte(pConvSetting.NetSetting.IpAddr, data, 14);//IP ---77m78 79 80 
                Util.ConvertIPToByte(pConvSetting.NetSetting.SubMask, data, 22);//子网掩码 ---81 82 83 84 
                Util.ConvertIPToByte(pConvSetting.NetSetting.GatewayIp, data, 18);//网关 ---85 86 87 88
                data[26] = pConvSetting.NetSetting.SetFzh;

                for (int i = 4; i <= 26; i++)
                    ljl += data[i];
                data[35] = Convert.ToByte(ljl & 0xFF);
                data[36] = Convert.ToByte((ljl >> 8) & 0xFF);
            }
            else if (stationfind == "0")//表示设置的是交换机的IP信息
            {
                data = new byte[208];
                data[0] = 0x7F;
                data[1] = 0x10;
                data[2] = 0x05;
                data[3] = 0x00;
                data[4] = 0xD0;
                data[5] = 0x11;
                data[6] = 0x06;
                Util.ConvertMacToByte(strMAC, data, 71);//3,4,5,6,7,8---MAC---71 72 73 74 75 76 
                Util.ConvertIPToByte(pConvSetting.NetSetting.IpAddr, data, 77);//IP ---77m78 79 80 
                Util.ConvertIPToByte(pConvSetting.NetSetting.SubMask, data, 81);//子网掩码 ---81 82 83 84 
                Util.ConvertIPToByte(pConvSetting.NetSetting.GatewayIp, data, 85);//网关 ---85 86 87 88
                data[207] = 0x0D;
                ljl = CRC_16(data, data.Length);
                data[205] = Convert.ToByte(ljl >> 8);
                data[206] =Convert.ToByte(ljl & 0xFF);
            }
        
            if (data != null)
            {

                _udpHandler.Send(data, 1);

                _resetEvent.WaitOne(timeout);//信号量等待

                _resetEvent.Reset();
            }

            return _setConvSettingResult;
        }
        /// <summary>
        /// 交换机收到设备返回的基础配置参数的回发处理
        /// </summary>
        /// <param name="data"></param>
        private static void HandleSetBaseParameter(byte[] data)
        {
            if (data.Length <208)
            {
                return;
            }
            ushort temp1, temp2;

            temp1 = Convert.ToUInt16((data[205] << 8) + data[206]);

            data[205] = 0x00;
            data[206] = 0x00;

            temp2 = CRC_16(data, data.Length);

            string strMac = Util.ConvertByteToMac(data, 71).Replace('-','.');//71 72 73 74 75 76 

            Console.WriteLine("MAC:" + _currentMac + " 收到交换机配置基础参数回发 06");

            if ((strMac == _currentMac) && (temp2 == temp1) && (data[77] == 0x00))//设置成功标识
            {
                _setConvSettingResult = true;
            }

            _resetEvent.Set();
        }
        private static void HandleSetFzhData(byte[] data)
        {
            ushort ljl = 0;

            Console.WriteLine("FZH:" + data[4] + " 设置IP相关信息应答 07 ");

            for (int i = 4; i < data.Length - 2; i++)
                ljl += data[i];
            if (ljl == ((data[data.Length - 1] << 8) + data[data.Length - 2]))
            {
                _setConvSettingResult = true;
            }
            _resetEvent.Set();

        }
        #endregion 设置设备配置参数相关

        #region 设置设备串口配置参数相关
        static bool _setConvCommSettingResult = false;
        /// <summary>
        /// 设置设备配置参数
        /// </summary>
        /// <param name="strMAC">mac地址</param>
        /// <param name="pConvSetting">配置参数结构体</param>
        /// <param name="timeout">等待超时时间，单位毫秒。</param>
        /// <param name="bindIp">指定固定的绑定IP发送数据</param>
        /// <returns>true:成功；false:失败。备注：如果在超时时间内收到设备回复，则会立即返回结果</returns>
        public static bool SetConvCommSetting(string strMAC, NetDeviceSetting pConvSetting, int timeout, string bindIp = "", int CommPort = 1)
        {
            Console.WriteLine("MAC:" + strMAC + " 开始设置串口配置参数【" + CommPort + "】 " + pConvSetting.SockSetting[CommPort - 1].srcPacket.Length);

            byte[] data = null;

            _currentMac = strMAC;

            _setConvCommSettingResult = false;

            data = GetCommConvData(strMAC, pConvSetting, CommPort);

            if (data != null)
            {
                //data为数据体，现在要加22个字节的包头和检验
                byte[] sendbuff = new byte[data.Length + 22];

                sendbuff[0] = 0xFF;

                sendbuff[1] = (byte)(sendbuff.Length - 3);

                sendbuff[2] = (byte)(0x05 + CommPort);
                if (CommPort == 4) sendbuff[2] = 0x0F;

                Util.ConvertMacToByte(strMAC, sendbuff, 3);//3,4,5,6,7,8---MAC---

                Buffer.BlockCopy(Util.User, 0, sendbuff, 9, Util.User.Length);//9,10,11,12,13,14---用户名---

                Buffer.BlockCopy(Util.PassWord, 0, sendbuff, 15, Util.User.Length);//15,16,17,18,19,20---密码---

                Buffer.BlockCopy(data, 0, sendbuff, 21, data.Length);

                sendbuff[sendbuff.Length - 1] = Util.NetNetModelLjl(sendbuff);

                _udpHandler.Send(sendbuff, 1);

                _resetEvent.WaitOne(timeout);//信号量等待

                _resetEvent.Reset();

            }

            if (_setConvCommSettingResult)
            {//设置成功后，再保存一下串口的参数
                _udpHandler.Send(Util.GetDataByType("04", strMAC), 1);

                _resetEvent.WaitOne(300);//信号量等待

                _resetEvent.Reset();
            }
            _currentMac = "";

            return _setConvCommSettingResult;
        }
        /// <summary>
        /// 串口参数下发生成
        /// </summary>
        /// <param name="strMAC"></param>
        /// <param name="pConvSetting"></param>
        /// <param name="PortComm"></param>
        /// <returns></returns>
        private static byte[] GetCommConvData(string strMAC, NetDeviceSetting pConvSetting, int PortComm)
        {
            if (pConvSetting.SockSetting == null || PortComm > pConvSetting.SockSetting.Length || pConvSetting.ComSetting == null || PortComm > pConvSetting.ComSetting.Length)
                return null;

            PortComm -= 1;

            byte[] data = new byte[pConvSetting.SockSetting[PortComm].srcPacket.Length];

            int i;

            Buffer.BlockCopy(pConvSetting.SockSetting[PortComm].srcPacket, 0, data, 0, data.Length);//先把存储的设置信息读出来

            if (data.Length == 63)
            {
                SocketSetting socketSet = pConvSetting.SockSetting[PortComm];
                ComSetting comSet = pConvSetting.ComSetting[PortComm];
                //需要修改的内容有：串口波特率，停止位，校验位，起如位，串口流控制
                Util.ConvertIntToByte(comSet.Baudrate, data, 0);//0,1,2,3,串口波特率
                data[4] = (byte)comSet.Databit;//数据位
                data[5] = (byte)comSet.CheckMode;//校验位
                data[6] = (byte)comSet.StopBit;//停止位
                data[7] = (byte)comSet.FlowMode;//串口控制流
                Util.ConvertInt16ToByte((ushort)socketSet.Port, data, 12);//本地端口
                Util.ConvertInt16ToByte((ushort)socketSet.ServerPort, data, 14);//服务器端口
                for (i = 16; i < 46; i++)
                {
                    data[i] = 0;
                }
                Util.ConvertIpToAscllByte(socketSet.IpServer, data, 16);//ip地址
                data[51] = (byte)socketSet.Mode;//通讯模式
                Util.ConvertIntToByte(comSet.MinSendByte, data, 52);//串口打包长度
                data[56] = (byte)comSet.MinSendTime;//串口打包时间
            }
            else data = null;

            return data;
        }
        /// <summary>
        /// 收到设备返回的串口配置参数的回发处理
        /// </summary>
        /// <param name="data"></param>
        private static void HandleSetCommParameter(byte[] data)
        {
            if (data.Length < 4)
            {
                return;
            }
            Console.WriteLine("MAC:" + _currentMac + " 收到配置串口参数回发 08");

            if (data[3] == 0x4B)//设置成功标识
            {
                _setConvCommSettingResult = true;
                //如果设置确认成功               
            }

            _resetEvent.Set();
        }

        #endregion 设置设备配置参数相关

        #region 设备复位相关

        static bool _resetConverterResult = false;
        /// <summary>
        /// 设备复位
        /// </summary>
        /// <param name="strMAC">mac地址</param>
        /// <param name="timeout">等待超时时间，单位毫秒。</param>
        /// <param name="bindIp">指定固定的绑定IP发送数据</param>
        /// <returns>true:成功；false:失败。备注：如果在超时时间内收到设备回复，则会立即返回结果</returns>
        public static bool ResetConverter(string strMAC, int timeout, string bindIp = "")
        {
            Console.WriteLine("MAC:" + strMAC + " 开始复位 0b");

            _resetConverterResult = false;

            _udpHandler.Send(Util.GetDataByType("02", strMAC), 1);

            _resetEvent.WaitOne(timeout);//信号量等待

            _resetEvent.Reset();


            return _resetConverterResult;
        }

        /// <summary>
        /// 收到设备复位回复
        /// </summary>
        /// <param name="data"></param>
        private static void HandleReset(byte[] data)
        {
            if (data.Length < 4)
            {
                return;
            }
            if (data[3] == 0X4B)//复位成功标识
            {
                _resetConverterResult = true;
                //如果复位成功               
            }
            _resetEvent.Set();
        }

        #endregion 设备复位相关

        #region 三旺交换机  CRC算法

        private static ushort cnCRC_16 = 0x8005;

        private static UInt32[] Table_CRC = new UInt32[256]; // CRC 表 unsigned long  // 构造 16 位 CRC 表

        private static void BuildTable16()
        {
            ushort i, j;
            ushort nData;
            ushort nAccum;
            for (i = 0; i < 256; i++)
            {
                nData = (ushort)(i << 8);
                nAccum = 0;
                for (j = 0; j < 8; j++)
                {
                    if (((nData ^ nAccum) & 0x8000) == 0x8000)
                    {
                        nAccum <<= 1;
                        nAccum ^= cnCRC_16;
                    }
                    else
                        nAccum <<= 1;
                    nData <<= 1;
                }
                Table_CRC[i] = (UInt32)nAccum;
            }
        }
        // 计算 16 位 CRC 值， CRC-16 或 CRC-CC99vT
        private static ushort CRC_16(byte[] aData, int aSize)
        {
            UInt32 i;
            ushort nAccum = 0;
            ushort temp = 0;
            BuildTable16();
            for (i = 0; i < aSize; i++)
            {
                temp = nAccum;
                nAccum <<= 8;
                nAccum ^= Convert.ToUInt16(Table_CRC[((temp >> 8) ^ aData[i])]);
            }
            return nAccum;
            //demo
            /*str = str.Replace(' ', ',');
            string[] strHx = str.Split(',');
            byte[] buff = new byte[strHx.Length];
            for (int i = 0; i < buff.Length; i++)
                buff[i] = Convert.ToByte(strHx[i], 16);
            buff[205] = 0x00;
            buff[206] = 0x00;
            ushort ljl = CRC_16(buff, (buff.Length));
        */
        }
        #endregion
    }
    
}
