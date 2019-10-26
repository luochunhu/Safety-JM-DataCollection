using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// RPC通讯协议类
    /// 支持自定义扩展
    /// </summary>
    public class MasProtocol
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MasProtocol()
        {
            GenerateNewSeriesNumber();
            this.Version = 1;
            this.CreatedTime = DateTime.Now;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="systemType">系统类型</param>
        /// <param name="to">数据方向，向上或者向下</param>        
        /// <param name="protocolType">协议类型</param>
        /// <param name="version"></param>
        public MasProtocol(SystemType systemType, DirectionType Direction, ProtocolType protocolType, int version = 1)
        {
            GenerateNewSeriesNumber();
            this.CreatedTime = DateTime.Now;
            this.Direction = Direction;
            this.SystemType = systemType;
            this.ProtocolType = protocolType;
            this.Version = version;
            this.DeviceNumber = 0;
        }
        /// <summary>
        /// 数据包设备地址号，网络模块及其它数据包，此字节固定为0.放入第一个队列处理。
        /// </summary>
        public ushort DeviceNumber { get; set; }
        /// <summary>
        /// 数据方向
        /// 0.上行数据
        /// 1.下行数据
        /// </summary>
        public DirectionType Direction { get; set; }
        /// <summary>
        /// 系统类型编码
        /// </summary>
        public SystemType SystemType { get; set; }
        /// <summary>
        /// 协议命令
        /// </summary>
        public ProtocolType ProtocolType { get; set; }
        /// <summary>
        /// 命令版本号
        /// </summary>
        public int Version { get; set; }                
        ///// <summary>
        ///// 表示命令集中的参数集合(每个监测量中存在多个监测因子时以“，”分隔；)
        ///// </summary>
        //public string CP { get; set; }
        /// <summary>
        /// 表示此条命令是否需要对方给应答(Flag=0表示不需要应答，Flag=1表示需要应答,=2表示直接回传数据)
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 数据包唯一编码(采用GUID LONG型唯一标识)
        /// </summary>
        public string SeriesNumber { get; set; }
        /// <summary>
        /// 数据时间()
        /// </summary>
        public DateTime CreatedTime { get; set; }
       /// <summary>
       /// 用于是否指定获取分站的网络信息20181101  =1表示是的
       /// </summary>
        public int StationFind { get; set; }

        /// <summary>
        /// 协议对象
        /// </summary>
        public object Protocol { get; set; }
        /// <summary>
        /// 收到数据后反序列化CommandProperties对象
        /// </summary>
        /// <typeparam name="T">待序列化的实体对象</typeparam>
        /// <returns></returns>
        public T Deserialize<T>()
        {
            T result = default(T);
            try
            {
                if (this.Protocol != null)
                {
                    result = Basic.Framework.Common.JSONHelper.ParseJSONString<T>(this.Protocol.ToString());
                }
            }
            catch
            { }
            return result;
        }
        /// <summary>
        /// 生成默认应答实体对象
        /// </summary>
        /// <returns></returns>
        public MasProtocol GenerateResponse()
        {
            MasProtocol response = new MasProtocol();
            response.SeriesNumber = this.SeriesNumber;
            response.SystemType = this.SystemType;
            response.Version = this.Version;
            response.Protocol = this.Protocol;
            return response;
        }
        /// <summary>
        /// 生成新的QN
        /// </summary>
        public void GenerateNewSeriesNumber()
        {
            this.SeriesNumber = GenerateSeriesNumber().ToString();
        }
        /// <summary>
        /// 根据GUID 生成唯一长整型数字
        /// </summary>
        /// <returns></returns>
        private Int64 GenerateSeriesNumber()
        {
            Int64 sn = 0; 
            byte[] buffer = Guid.NewGuid().ToByteArray();
            sn = BitConverter.ToInt64(buffer, 0);
            return sn;
        }        
    }
}
