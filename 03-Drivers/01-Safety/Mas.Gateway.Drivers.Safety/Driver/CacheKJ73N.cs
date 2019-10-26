using Sys.DataCollection.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Driver
{
    public class CacheNetWork
    {
        private bool m_resetcontrol;
        protected DateTime dttBridgeReceiveTime;
        public virtual DateTime DttBridgeReceiveTime
        {
            get { return dttBridgeReceiveTime; }
            set { dttBridgeReceiveTime = value; }
        }
        /// <summary>
        /// 上一帧接收标志 1为正确
        /// </summary>
        private bool m_bBridgeRevMark;
        /// <summary>
        /// 上一帧接收标志 1为正确
        /// </summary>
        public bool BBridgeRevMark
        {
            get { return m_bBridgeRevMark; }
            set { m_bBridgeRevMark = value; }
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
        /// MAC地址
        /// </summary>
        public string MAC
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
        /// 状态        /// 状态0	通讯中断，3	交流正常，4	直流正常
        /// </summary>
        public short State
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 用于源码输出的类
    /// </summary>
    public class CommSendData
    {
        /// <summary>
        /// 网络地址
        /// </summary>
        public string Mac;
        /// <summary>
        /// =1表示上行数据，=2表示下行数据
        /// </summary>
        public byte Flag;
        /// <summary>
        /// 表示通讯的数据体
        /// </summary>
        public byte[] data;
    }
    /// <summary>
    /// 脉冲量计数类
    /// </summary>
    public class McAddTotal
    {
         /// <summary>
         /// 测点编号
         /// </summary>
        public string point = "";
        /// <summary>
        /// 上一次计数个数
        /// </summary>
        public int Count = 0;
        /// <summary>
        /// 最后一次更新时间，超过好久未更新直接删除掉。
        /// </summary>
        public DateTime Time;
    }
    //public class CacheDevice 
    //{
    //    private int _DevPropertyID;
    //    /// <summary>
    //    /// 设备性质ID =0分站，1=模拟量，=2开关量，=3控制量
    //    /// </summary>
    //    public int DevPropertyID
    //    {
    //        get { return _DevPropertyID; }
    //        set
    //        {
    //            _DevPropertyID = value;
    //        }
    //    }
    //    /// <summary>
    //    /// 表示设备通讯时的，分站上传的设备类型
    //    /// </summary>
    //    protected byte _DeviceCommunicationType = 0x00;
    //    /// <summary>
    //    /// 0x26为最新智能分站，0x02 0x16为老的智能分站
    //    /// </summary>
    //    public byte DeviceCommunicationType
    //    {
    //        get { return _DeviceCommunicationType; }
    //        set
    //        {
    //            _DeviceCommunicationType = value;
    //        }
    //    }
    //    /// <summary>
    //    /// 当前初始化的类型，即I或者J
    //    /// </summary>
    //    public byte InitializeType { set; get; }
    //    /// <summary>
    //    /// 测点号
    //    /// </summary>
    //    public string Point
    //    {
    //        get;
    //        set;
    //    }

    //}

    /// <summary>
    /// 表示断电失败和复电失败的容错处理类。一旦正常后，对应记录删除掉
    /// </summary>
    public class ControlInfo
    {
        /// <summary>
        /// 主键，其生成规则为分站号*50+控制口号
        /// </summary>
        public int Pid;
        /// <summary>
        /// 第一次失败时的时间
        /// </summary>
        public DateTime FailTime;
        /// <summary>
        /// //1断电成功 2 断电失败 3 复电成功 4 复电失败）
        /// </summary>
        public byte FeedState = 0;
    }
    /// <summary>
    /// 处理重复卡号
    /// </summary>
    public class RyMore
    {
        public ushort bh;
        public DateTime dt;
        public string point;
        public RyMore(DateTime _t, ushort _k, string _point)
        {
            bh = _k;
            dt = _t;
            point = _point;
        }
    }
}
