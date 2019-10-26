using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 设备性质枚举
    /// </summary>
    public enum ItemDevProperty
    {
        /// <summary>
        /// 分站/基站
        /// </summary>
        [EnumMember]
        Substation = 0,
        /// <summary>
        /// 模拟量
        /// </summary>
        [EnumMember]
        Analog = 1,
        /// <summary>
        /// 开关量
        /// </summary>
        [EnumMember]
        Derail = 2,
        /// <summary>
        /// 控制量
        /// </summary>
        [EnumMember]
        Control = 3,
        /// <summary>
        /// 累积量
        /// </summary>
        [EnumMember]
        Accumulation = 4,
        /// <summary>
        /// 导出量
        /// </summary>
        [EnumMember]
        Export = 5,
        /// <summary>
        /// 其他
        /// </summary>
        [EnumMember]
        Other = 6,
        /// <summary>
        /// 人员识别器
        /// </summary>
        [EnumMember]
        Recognizer = 7,
        /// <summary>
        /// 区域
        /// </summary>
        [EnumMember]
        Area = 9,
        /// <summary>
        /// 字符串
        /// </summary>
        [EnumMember]
        Strings = 12,
        /// <summary>
        /// 统计量
        /// </summary>
        [EnumMember]
        Statistics = 13,
        /// <summary>
        /// 馈电量
        /// </summary>
        [EnumMember]
        Statiskd = 14,
        /// <summary>
        /// 电源箱
        /// </summary>
        [EnumMember]
        PowerStation = 15,
        /// <summary>
        /// 交换机
        /// </summary>
        [EnumMember]
        Switches = 16,
        /// <summary>
        /// 智能量
        /// </summary>
        [EnumMember]
        Intelligence = 17,

        /// <summary>
        /// 唯一编码枚举
        /// </summary>
        [EnumMember]
        SoleCoding = 18,

        /// <summary>
        /// 卡号信息
        /// </summary>
        [EnumMember]
        CardInfo = 19
    }
}
