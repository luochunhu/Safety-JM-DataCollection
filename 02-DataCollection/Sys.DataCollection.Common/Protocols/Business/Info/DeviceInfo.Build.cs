using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{   
    /// <summary>
    /// 设备信息（测点）
    /// </summary>
    public partial class DeviceInfo : CacheInfo
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
        /// 测点ID编号[历史表关联字段]
        /// </summary>
        public string PointID
        {
            get;
            set;
        }
        /// <summary>
        /// 区域ID
        /// </summary>
        public string Areaid
        {
            get;
            set;
        }
        /// <summary>
        /// 系统标志/编码，关联CBF_SysInf
        /// </summary>
        public int Sysid
        {
            get;
            set;
        }
        /// <summary>
        /// 是否活动点（0：非活动，1：活动）
        /// </summary>
        public string Activity
        {
            get;
            set;
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateUpdateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 删除时间(将设备置为非活动的时间)
        /// </summary>
        public DateTime DeleteTime
        {
            get;
            set;
        }
        /// <summary>
        /// 分站号
        /// </summary>
        public short Fzh
        {
            get;
            set;
        }
        /// <summary>
        /// 口号
        /// </summary>
        public short Kh
        {
            get;
            set;
        }
        /// <summary>
        /// 地址号
        /// </summary>
        public short Dzh
        {
            get;
            set;
        }
        /// <summary>
        /// 设备类型编号
        /// </summary>
        public string Devid
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
        /// 处理措施编号
        /// </summary>
        public int Csid
        {
            get;
            set;
        }
        /// <summary>
        /// 测点号
        /// </summary>
        public string Point
        {
            get;
            set;
        }
        /// <summary>
        /// 实时值
        /// </summary>
        public string Ssz
        {
            get;
            set;
        }
        /// <summary>
        /// 数据状态
        /// </summary>
        public short DataState
        {
            get;
            set;
        }
        /// <summary>
        /// 状态值
        /// </summary>
        public short State
        {
            get;
            set;
        }
        /// <summary>
        /// 报警状态 大于零为报警
        /// </summary>
        public short Alarm
        {
            get;
            set;
        }
        /// <summary>
        /// 电压等级
        /// </summary>
        public float Voltage
        {
            get;
            set;
        }
        /// <summary>
        /// 状态变动时间
        /// </summary>
        public DateTime Zts
        {
            get;
            set;
        }
        /// <summary>
        /// 分站: mac地址/Ip地址/抽放绑定开停
        ///模拟量：上下限断电交叉控制测点号/断线交叉控制测点号/上溢负漂交叉控制测点号
        ///开关量：0态交叉控制测点号/1态交叉控制测点号/2态交叉控制测点号
        ///导出量：测点1/测点2/测点3
        ///识别器：指定人员编号（当为限制进入时启用）
        ///对应多个测点之间用‘|’分隔如:001C01|005C06

        /// </summary>
        public string Jckz1
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Jckz2
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Jckz3
        {
            get;
            set;
        }
        /// <summary>
        /// 模拟量：上限预警值/上限报警值/上限断电值/上限恢复值/下限预警值/下限报警值/下限断电值/下限恢复值
        ///累计量：偏差值
        ///导出量：参数1/参数2

        /// </summary>
        public float Z1
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public float Z2
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public float Z3
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public float Z4
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public float Z5
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public float Z6
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public float Z7
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public float Z8
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：大气压（101）/抽放正负压（0负1正）/串口端口号
        ///模拟量：上限报警控制口/上限断电控制口/下限报警控制口/下限断电控制口/上溢控制口/负漂控制口/断线控制口/备用控制口
        ///开关量:0态控制口/1态控制口/2态控制口/逻辑报警类型（1与 2或）/逻辑报警关联口号
        ///控制量：控制量所关联的馈电传感器的分站号/控制量所关联的馈电传感器的通道号/是否智能开停
        ///累计量：关联的分站号/关联的口号
        ///区域：额定人数
        ///识别器：报警类型/报警时间/离开时间/额定人数

        /// </summary>
        public int K1
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int K2
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int K3
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int K4
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int K5
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int K6
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int K7
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int K8
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：  运行记录标志
        ///模拟量：运行记录标志
        ///开关量：运行记录标志

        /// </summary>
        public int Bz1
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：响铃报警标
        ///模拟量：响铃报警标志
        ///开关量：响铃报警标志

        /// </summary>
        public int Bz2
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：风电闭锁标志 1；逻辑控制 2
        ///模拟量：突出预测标志 

        /// </summary>
        public int Bz3
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：设备休眠标志
        ///模拟量：密采记录标志
        ///开关量：设备休眠标志
        ///控制量：标记

        /// </summary>
        public int Bz4
        {
            get;
            set;
        }
        /// <summary>
        /// 联网上传标志
        /// </summary>
        public int Bz5
        {
            get;
            set;
        }
        /// <summary>
        /// 模拟量：传感器标校周期
        /// </summary>
        public string Bz6
        {
            get;
            set;
        }
        /// <summary>
        /// 最近一次标校时间
        /// </summary>
        public string Bz7
        {
            get;
            set;
        }
        /// <summary>
        /// 备用8
        /// </summary>
        public string Bz8
        {
            get;
            set;
        }
        /// <summary>
        /// 备用9
        /// </summary>
        public string Bz9
        {
            get;
            set;
        }
        /// <summary>
        /// 存储风电闭锁信息
        /// </summary>
        public string Bz10
        {
            get;
            set;
        }
        /// <summary>
        /// 备用11
        /// </summary>
        public string Bz11
        {
            get;
            set;
        }
        /// <summary>
        /// 备用12
        /// </summary>
        public string Bz12
        {
            get;
            set;
        }
        /// <summary>
        /// 备用13
        /// </summary>
        public string Bz13
        {
            get;
            set;
        }
        /// <summary>
        /// 备用14
        /// </summary>
        public string Bz14
        {
            get;
            set;
        }
        /// <summary>
        /// 备用15
        /// </summary>
        public string Bz15
        {
            get;
            set;
        }
        /// <summary>
        /// 备用16
        /// </summary>
        public string Bz16
        {
            get;
            set;
        }
        /// <summary>
        /// 备用17
        /// </summary>
        public string Bz17
        {
            get;
            set;
        }
        /// <summary>
        /// 备用18
        /// </summary>
        public string Bz18
        {
            get;
            set;
        }
        /// <summary>
        /// 备用19
        /// </summary>
        public string Bz19
        {
            get;
            set;
        }
        /// <summary>
        /// 备用20
        /// </summary>
        public string Bz20
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// 上传标志0-未传1-已传
        /// </summary>
        public string Upflag
        {
            get;
            set;
        }
    }
}
