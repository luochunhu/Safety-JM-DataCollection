using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 设备类型信息
    /// </summary>
    public partial class DeviceTypeInfo : CacheInfo
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
        /// 设备类型ID(即驱动编号)0-20：用于分站类型 21-76：用于传感器类型
        /// </summary>
        public string Devid
        {
            get;
            set;
        }
        /// <summary>
        /// 设备对应系统ID（用于上位机解析处理，如果同一系统存在多个解析逻辑的，此处当成不同系统进行处理）
        /// </summary>
        public int Sysid
        {
            get;
            set;
        }
        /// <summary>
        /// 类型:0-分站1-模拟量2-开关量3-控制量4-累计量5-导出量 6-其他
        /// </summary>
        public int Type
        {
            get;
            set;
        }
        /// <summary>
        /// 传感器类型名
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：0x01监控/0x02抽累/0x04智开/0x08人员
        ///模拟量：全量程。

        /// </summary>
        public short LC
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：通讯协议类型 
        ///模拟量：分段量程

        /// </summary>
        public short LC2
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：模开口数
        ///模拟量：频率低值 
        ///开关量：0态是否报警 0-不报 1-报警
        ///控制量：0态是否报警 0-不报 1-报警

        /// </summary>
        public short Pl1
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：控制口数
        ///模拟量：频率高值
        ///开关量：1态是否报警 0-不报 1-报警
        ///控制量：1态是否报警 0-不报 1-报警

        /// </summary>
        public short Pl2
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：智能口数
        ///模拟量：分段频率
        ///开关量：2态是否报警 0-不报 1-报警

        /// </summary>
        public short Pl3
        {
            get;
            set;
        }
        /// <summary>
        /// 分站：人员口数
        ///模拟量：频率修正偏差

        /// </summary>
        public short Pl4
        {
            get;
            set;
        }
        /// <summary>
        /// 数据修正系数
        /// </summary>
        public float Xzxs
        {
            get;
            set;
        }
        /// <summary>
        /// Z1~Z8模拟量：上限预警/报警/断电/恢复/下限预警/报警/断电/恢复
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
        /// 开关量：0态/1态/2态显示颜色
        ///控制量：0态/1态显示颜色

        /// </summary>
        public int Color1
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Color2
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Color3
        {
            get;
            set;
        }
        /// <summary>
        /// Xs1~Xs3 模拟量：单位
        ///开关量：0态/1态/2态显示信息
        ///控制量：0态/1态显示信息

        /// </summary>
        public string Xs1
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Xs2
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Xs3
        {
            get;
            set;
        }
        /// <summary>
        /// 备用1
        /// </summary>
        public int Bz1
        {
            get;
            set;
        }
        /// <summary>
        /// 备用2
        /// </summary>
        public int Bz2
        {
            get;
            set;
        }
        /// <summary>
        /// 设备种类(枚举表中枚举数据)
        /// </summary>
        public int Bz3
        {
            get;
            set;
        }
        /// <summary>
        /// 设备型号 (枚举表中枚举数据)
        /// </summary>
        public int Bz4
        {
            get;
            set;
        }
        /// <summary>
        /// 标效期
        /// </summary>
        public int Bz5
        {
            get;
            set;
        }
        /// <summary>
        /// 有效期
        /// </summary>
        public string Bz6
        {
            get;
            set;
        }
        /// <summary>
        /// 库存数量
        /// </summary>
        public string Bz7
        {
            get;
            set;
        }
        /// <summary>
        /// 参数个数
        /// </summary>
        public string Bz8
        {
            get;
            set;
        }
        /// <summary>
        /// 扩展通道devid列表，多个之间用”|”分隔
        /// </summary>
        public string Bz9
        {
            get;
            set;
        }
        /// <summary>
        /// 备用10
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
