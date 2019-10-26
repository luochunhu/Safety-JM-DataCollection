using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 监测对象状态
    /// </summary>
    [Serializable, DataContract]
    public enum ItemState
    {
        /// <summary>
        /// 通讯中断
        /// </summary>
        [EnumMember]
        EquipmentInterrupted = 0,
        /// <summary>
        /// 通讯误码
        /// </summary>
        [EnumMember]
        EquipmentBiterror = 1,
        /// <summary>
        /// 初始化中
        /// </summary>
        [EnumMember]
        EquipmentIniting = 2,
        /// <summary>
        /// 交流正常
        /// </summary>
        [EnumMember]
        EquipmentAC = 3,
        /// <summary>
        /// 直流正常
        /// </summary>
        [EnumMember]
        EquipmentDC = 4,
        /// <summary>
        /// 红外遥控
        /// </summary>
        [EnumMember]
        EquipmentInfrareding = 5,
        /// <summary>
        /// 休眠
        /// </summary>
        [EnumMember]
        EquipmentSleep = 6,
        /// <summary>
        /// 设备检修
        /// </summary>
        [EnumMember]
        EquipmentDebugging = 7,
        /// <summary>
        /// 断线
        /// </summary>
        [EnumMember]
        EquipmentDown = 20,
        /// <summary>
        /// 设备正常
        /// </summary>
        [EnumMember]
        EquipmentCommOK = 21,
        /// <summary>
        /// 上溢
        /// </summary>
        [EnumMember]
        EquipmentOverrange = 22,
        /// <summary>
        /// 负漂
        /// </summary>
        [EnumMember]
        EquipmentUnderrange = 23,
        /// <summary>
        /// 标校
        /// </summary>
        [EnumMember]
        EquipmentAdjusting = 24,
        /// <summary>
        /// 开关量0态
        /// </summary>
        [EnumMember]
        DataDerailState0 = 25,
        /// <summary>
        /// 开机
        /// </summary>
        [EnumMember]
        EquipmentStart = 28,
        /// <summary>
        /// 头子断线
        /// </summary>
        [EnumMember]
        EquipmentHeadDown = 33,
        /// <summary>
        /// 类型有误
        /// </summary>
        [EnumMember]
        EquipmentTypeError = 34,
        /// <summary>
        /// 系统退出
        /// </summary>
        [EnumMember]
        SystemExsit = 35,
        /// <summary>
        /// 系统启动
        /// </summary>
        [EnumMember]
        SystemStart = 36,
        /// <summary>
        /// 非法退出
        /// </summary>
        [EnumMember]
        SystemExsitFF = 37,
        /// <summary>
        /// 过滤数据
        /// </summary>
        [EnumMember]
        DataFilter = 38,
        /// <summary>
        /// 热备日志
        /// </summary>
        [EnumMember]
        DataCurLog = 39,
        /// <summary>
        /// 线性突变
        /// </summary>
        [EnumMember]
        EquipmentChange = 42,
        /// <summary>
        /// 控制量断线
        /// </summary>
        [EnumMember]
        EquipmentControlDown = 45,
        /// <summary>
        /// 设备状态未知(历史表中不会出现该状态，用于实时显示部分表示对应分站通讯中断后传感器的状态)
        /// </summary>
        [EnumMember]
        EquipmentStateUnknow = 46,

        /// <summary>
        ///传感器电量过低
        /// </summary>
        [EnumMember]       
        SensorPowerAlarm = 58,
        /// <summary>
        ///传感器电压过低
        /// </summary>
        [EnumMember]      
        UnderVoltageAlarm = 59,
        /// <summary>
        ///传感器更换中
        /// </summary>
        [EnumMember]
        SensorChangeing = 60,

        /// <summary>
        /// 删除  
        /// </summary>
        [EnumMember]
        Deleted = 50
    }
}
