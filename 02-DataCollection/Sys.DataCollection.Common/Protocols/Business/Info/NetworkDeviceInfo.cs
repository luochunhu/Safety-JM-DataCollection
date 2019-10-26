using Basic.Framework.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    public partial class NetworkDeviceInfo
    {
        public InfoState InfoState { get; set; }

        #region Member Variables

        protected string _Wz;

        //protected SpecialOrder _Order = SpecialOrder.None;

        protected byte _PowerPackState;

        protected byte _PowerPackVOL;

        protected float _PowerPackMA;

        protected float[] _BatteryVOL;

        protected DateTime _PowerDateTime = new DateTime(1900, 1, 1, 0, 0, 0);

        protected DateTime dttBridgeReceiveTime;

        protected bool _IsMemoryData;

        /// 
        #endregion

        #region Public Properties

        public virtual DateTime DttBridgeReceiveTime
        {
            get { return dttBridgeReceiveTime; }
            set { dttBridgeReceiveTime = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Wz
        {
            get { return _Wz; }
            set
            {
                if (value != null && value.Length > 50)
                    throw new ArgumentOutOfRangeException("此属性的值长度过长 Wz", value, value.ToString());
                _Wz = value;
            }
        }


        ///// <summary>
        ///// 分站特殊命令
        ///// </summary>
        //public virtual SpecialOrder Order
        //{
        //    get { return _Order; }
        //    set
        //    {
        //        _Order = value;
        //    }
        //}

        /// <summary>
        /// 电源箱状态
        /// </summary>
        public virtual byte PowerPackState
        {
            get { return _PowerPackState; }
            set
            {
                _PowerPackState = value;
            }
        }

        /// <summary>
        /// 电源箱电量
        /// </summary>
        public virtual byte PowerPackVOL
        {
            get { return _PowerPackVOL; }
            set
            {
                _PowerPackVOL = value;
            }
        }

        /// <summary>
        /// 电源箱负载电流
        /// </summary>
        public virtual float PowerPackMA
        {
            get { return _PowerPackMA; }
            set
            {
                _PowerPackMA = value;
            }
        }

        /// <summary>
        /// 电源箱电池电压
        /// </summary>
        public virtual float[] BatteryVOL
        {
            get { return _BatteryVOL; }
            set
            {
                _BatteryVOL = value;
            }
        }

        /// <summary>
        /// 获取电源箱电压时间
        /// </summary>
        public virtual DateTime PowerDateTime
        {
            get { return _PowerDateTime; }
            set
            {
                _PowerDateTime = value;
            }
        }

        private byte fdstate;

        /// <summary>
        /// 放电状态
        /// </summary>
        public virtual byte Fdstate
        {
            get { return fdstate; }
            set { fdstate = value; }
        }
        /// <summary>
        /// 内存数据标记
        /// </summary>
        public virtual bool IsMemoryData
        {
            get { return _IsMemoryData; }
            set { _IsMemoryData = value; }
        }
        #endregion

        #region ----原NetBridge内容----

        /// <summary>
        /// 时间同步命令是否需要下发
        /// </summary>
        private bool m_timeSynchronization = false;
        /// <summary>
        /// 时间同步命令是否需要下发
        /// </summary>
        public bool TimeSynchronization
        {
            get { return m_timeSynchronization; }
            set { m_timeSynchronization = value; }
        }
        /// <summary>
        /// 时间同步命令下发次数
        /// </summary>
        private uint m_timeSynchronizationcount = 3;
        /// <summary>
        /// 时间同步命令下发次数
        /// </summary>
        public uint TimeSynchronizationcount
        {
            get { return m_timeSynchronizationcount; }
            set { m_timeSynchronizationcount = value; }
        }

        /// <summary>
        /// 是否要下发分站序列 
        /// True-是 
        /// False-否
        /// </summary>
        private bool m_bBridgeInitStationQueen;
        /// <summary>
        /// 是否要下发分站序列 
        /// True-是 
        /// False-否
        /// </summary>
        public bool BBridgeInitStationQueen
        {
            get { return m_bBridgeInitStationQueen; }
            set { m_bBridgeInitStationQueen = value; }
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
        /// 连接是否关闭
        /// </summary>
        private bool m_bBridgeClosed;
        /// <summary>
        /// 连接是否关闭
        /// </summary>
        public bool BBridgeClosed
        {
            get { return m_bBridgeClosed; }
            set { m_bBridgeClosed = value; }
        }
        /// <summary>
        /// 连接关闭时间
        /// </summary>
        private DateTime m_bBridgeClosedTime;
        /// <summary>
        /// 连接关闭时间
        /// </summary>
        public DateTime BBridgeClosedTime
        {
            get { return m_bBridgeClosedTime; }
            set { m_bBridgeClosedTime = value; }
        }


        private bool sendU;
        /// <summary>
        /// 发送u命令  广播 tanxingyan 20161124
        /// </summary>
        public bool SendU
        {
            get { return sendU; }
            set { sendU = value; }
        }
        #endregion
    }
}
