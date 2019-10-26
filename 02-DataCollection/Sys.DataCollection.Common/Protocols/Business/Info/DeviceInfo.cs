using Basic.Framework.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    public partial class DeviceInfo
    {
        public InfoState InfoState { get; set; }

        #region Public Properties
        /// <summary>
        /// 表示设备通讯时的，分站上传的设备类型
        /// </summary>
        protected byte _DeviceCommunicationType = 0x00;
        public byte DeviceCommunicationType
        {
            get { return _DeviceCommunicationType; }
            set
            {
                _DeviceCommunicationType = value;
            }
        }

        protected string _Wz;
        /// <summary>
        /// 安装位置
        /// </summary>
        public string Wz
        {
            get { return _Wz; }
            set
            {
                if (value != null && value.Length > 50)
                    throw new ArgumentOutOfRangeException("此属性的值长度过长 Wz", value, value.ToString());
                _Wz = value;
            }
        }

        protected string _DevName;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DevName
        {
            get { return _DevName; }
            set
            {
                if (value != null && value.Length > 40)
                    throw new ArgumentOutOfRangeException("此属性的值长度过长 DevName", value, value.ToString());
                _DevName = value;
            }
        }

        protected int _DevPropertyID;
        /// <summary>
        /// 设备性质ID
        /// </summary>
        public int DevPropertyID
        {
            get { return _DevPropertyID; }
            set
            {
                _DevPropertyID = value;
            }
        }

        protected int _DevClassID;
        /// <summary>
        /// 设备种类ID
        /// </summary>
        public int DevClassID
        {
            get { return _DevClassID; }
            set
            {
                _DevClassID = value;
            }
        }

        protected int _DevModelID;
        /// <summary>
        /// 设备型号ID
        /// </summary>
        public int DevModelID
        {
            get { return _DevModelID; }
            set
            {
                _DevModelID = value;
            }
        }

        protected string _DevProperty;
        /// <summary>
        /// 设备性质名称
        /// </summary>
        public string DevProperty
        {
            get { return _DevProperty; }
            set
            {
                if (value != null && value.Length > 50)
                    throw new ArgumentOutOfRangeException("此属性的值长度过长 DevProperty", value, value.ToString());
                _DevProperty = value;
            }
        }

        protected string _DevClass;
        /// <summary>
        /// 设备种类名称
        /// </summary>
        public string DevClass
        {
            get { return _DevClass; }
            set
            {
                if (value != null && value.Length > 50)
                    throw new ArgumentOutOfRangeException("此属性的值长度过长 DevClass", value, value.ToString());
                _DevClass = value;
            }
        }

        protected string _DevModel;
        /// <summary>
        /// 设备型号名称
        /// </summary>
        public string DevModel
        {
            get { return _DevModel; }
            set
            {
                if (value != null && value.Length > 50)
                    throw new ArgumentOutOfRangeException("此属性的值长度过长 DevModel", value, value.ToString());
                _DevModel = value;
            }
        }

        protected string _unit;
        /// <summary>
        /// 单位 （Xs1）
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set
            {
                if (value != null && value.Length > 50)
                    throw new ArgumentOutOfRangeException("此属性的值长度过长 Unit", value, value.ToString());
                _unit = value;
            }
        }

        protected byte _PowerPackState;
        /// <summary>
        /// 电源箱状态
        /// </summary>
        public byte PowerPackState
        {
            get { return _PowerPackState; }
            set
            {
                _PowerPackState = value;
            }
        }

        protected byte _PowerPackVOL;
        /// <summary>
        /// 电源箱电量
        /// </summary>
        public byte PowerPackVOL
        {
            get { return _PowerPackVOL; }
            set
            {
                _PowerPackVOL = value;
            }
        }

        protected float _PowerPackMA;
        /// <summary>
        /// 电源箱负载电流
        /// </summary>
        public float PowerPackMA
        {
            get { return _PowerPackMA; }
            set
            {
                _PowerPackMA = value;
            }
        }

        protected float[] _BatteryVOL;
        /// <summary>
        /// 电源箱电池电压
        /// </summary>
        public float[] BatteryVOL
        {
            get { return _BatteryVOL; }
            set { _BatteryVOL = value; }
        }

        protected DateTime _PowerDateTime = new DateTime(1900, 1, 1, 0, 0, 0);
        /// <summary>
        /// 获取电源箱电压时间
        /// </summary>
        public DateTime PowerDateTime
        {
            get { return _PowerDateTime; }
            set
            {
                _PowerDateTime = value;
            }
        }

        protected string _AreaName;
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName
        {
            get { return _AreaName; }
            set
            {
                _AreaName = value;
            }
        }

        protected string _areaLoc;
        /// <summary>
        /// 所属区域编码（Loc）
        /// </summary>
        public string AreaLoc
        {
            get { return _areaLoc; }
            set
            {
                _areaLoc = value;
            }
        }

        protected string _XCoordinate;
        /// <summary>
        /// 经度
        /// </summary>
        public string XCoordinate
        {
            get { return _XCoordinate; }
            set
            {
                _XCoordinate = value;
            }
        }

        protected string _YCoordinate;
        /// <summary>
        /// 纬度
        /// </summary>
        public string YCoordinate
        {
            get { return _YCoordinate; }
            set
            {
                _YCoordinate = value;
            }
        }

        protected bool _DefIsInit;
        /// <summary>
        /// 定义成功后是否需要下发初始化的标记
        /// </summary>
        public bool DefIsInit
        {
            get { return _DefIsInit; }
            set { _DefIsInit = value; }
        }

        private int _NErrCount;
        /// <summary>
        /// 当前通讯中断次数
        /// </summary>
        public int NErrCount
        {
            set { _NErrCount = value; }
            get { return _NErrCount; }
        }

        private byte _Fdstate;
        /// <summary>
        /// 放电状态
        /// </summary>
        public byte Fdstate
        {
            set { _Fdstate = value; }
            get { return _Fdstate; }
        }

        private bool _BDisCharge;
        /// <summary>
        /// 手动放电
        /// </summary>
        public bool BDisCharge
        {
            get { return _BDisCharge; }
            set { _BDisCharge = value; }
        }

        private byte _LastOrderNum;
        /// <summary>
        /// 上次下发的命令
        /// </summary>
        public byte LastOrderNum
        {
            get { return _LastOrderNum; }
            set { _LastOrderNum = value; }
        }

        private DateTime _lastOrderSendTime;
        /// <summary>
        /// 上次下发命令的时间
        /// </summary>
        public DateTime lastOrderSendTime
        {
            get { return _lastOrderSendTime; }
            set { _lastOrderSendTime = value; }
        }

        private int _realControlCount;
        /// <summary>
        /// 需要下发多少次F命令
        /// </summary>
        public int realControlCount
        {
            get { return _realControlCount; }
            set { _realControlCount = value; }
        }

        private int _sendIniCount;
        /// <summary>
        /// 需要下发多少次i命令
        /// </summary>
        public int sendIniCount
        {
            get { return _sendIniCount; }
            set { _sendIniCount = value; }
        }

        private DateTime _sendDTime;
        /// <summary>
        /// D命令下发时间
        /// </summary>
        public DateTime sendDTime
        {
            get { return _sendDTime; }
            set { _sendDTime = value; }
        }

        private bool _kzchangeflag;
        /// <summary>
        /// 控制口变化标记
        /// </summary>
        public bool kzchangeflag
        {
            get { return _kzchangeflag; }
            set { _kzchangeflag = value; }
        }

        private bool _endAlarmflag;
        /// <summary>
        /// 结束报警标记（xflag）
        /// </summary>
        public bool EndAlarmflag
        {
            get { return _endAlarmflag; }
            set { _endAlarmflag = value; }
        }

        /// <summary>
        /// 处理措施
        /// </summary>
        private string m_strMeasure;
        /// <summary>
        /// 处理措施
        /// </summary>
        public string StrMeasure
        {
            get { return m_strMeasure; }
            set { m_strMeasure = value; }
        }

        private List<DateTime> m_ddyistarttime;
        /// <summary>
        /// 断电开始时间
        /// </summary>
        public List<DateTime> Ddyistarttime
        {
            get { return m_ddyistarttime; }
            set { m_ddyistarttime = value; }
        }

        private List<DateTime> m_kzstarttime;
        /// <summary>
        /// 控制执行开始时间
        /// </summary>
        public List<DateTime> Kzstarttime
        {
            get { return m_kzstarttime; }
            set { m_kzstarttime = value; }
        }

        private int m_nCtrlSate;
        /// <summary>
        /// 控制返回状态 复电成功/失败 断电成功/失败
        /// </summary>
        public int NCtrlSate
        {
            get { return m_nCtrlSate; }
            set { m_nCtrlSate = value; }
        }

        private DateTime m_dttkdStrtime;
        /// <summary>
        /// 馈电异常开始时间
        /// </summary>
        public DateTime DttkdStrtime
        {
            get { return m_dttkdStrtime; }
            set { m_dttkdStrtime = value; }
        }

        private long m_sckdid;
        /// <summary>
        /// 上次馈电异常的id号
        /// </summary>
        public long Sckdid
        {
            get { return m_sckdid; }
            set { m_sckdid = value; }
        }

        private DateTime m_kdStrtime;
        /// <summary>
        /// 馈电异常开始时间 用于复电失败更新记录
        /// </summary>
        public DateTime DkdStrtime
        {
            get { return m_kdStrtime; }
            set { m_kdStrtime = value; }
        }

        private DateTime m_dttRunStateTime;
        /// <summary>
        /// 对象运行状态时间
        /// </summary>
        public DateTime DttRunStateTime
        {
            get { return m_dttRunStateTime; }
            set { m_dttRunStateTime = value; }
        }

        private bool m_bEdit;
        /// <summary>
        /// 对象修改标记
        /// </summary>
        public bool BEdit
        {
            get { return m_bEdit; }
            set { m_bEdit = value; }
        }

        //private CommProperty m_clsCommObj;
        ///// <summary>
        ///// 对象通讯类
        ///// </summary>
        //public CommProperty ClsCommObj
        //{
        //    get { return m_clsCommObj; }
        //    set { m_clsCommObj = value; }
        //}

        //private AlarmProperty m_clsAlarmObj;
        ///// <summary>
        ///// 对象报警类
        ///// </summary>
        //public AlarmProperty ClsAlarmObj
        //{
        //    get { return m_clsAlarmObj; }
        //    set { m_clsAlarmObj = value; }
        //}

        //private FiveMinData m_clsFiveMinObj;
        ///// <summary>
        ///// 5分钟数据处理类
        ///// </summary>
        //public FiveMinData ClsFiveMinObj
        //{
        //    get { return m_clsFiveMinObj; }
        //    set { m_clsFiveMinObj = value; }
        //}

        //private List<ControlRemote> m_ClsCtrlObj;
        ///// <summary>
        ///// 
        ///// </summary>
        //public List<ControlRemote> ClsCtrlObj
        //{
        //    get { return m_ClsCtrlObj; }
        //    set { m_ClsCtrlObj = value; }
        //}

        private bool m_bCommDevTypeMatching;
        /// <summary>
        /// 设备匹配标记
        /// </summary>
        public bool BCommDevTypeMatching
        {
            get { return m_bCommDevTypeMatching; }
            set { m_bCommDevTypeMatching = value; }
        }
        #endregion
    }
}
