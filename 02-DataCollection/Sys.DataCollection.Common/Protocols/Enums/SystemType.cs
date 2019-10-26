using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 系统编码对照表
    /// </summary>
    [Serializable]
    public enum SystemType
    {
        /// <summary>
        /// 燃气系统
        /// </summary>
        
        Gas = 1,
        /// <summary>
        /// 下水道系统
        /// </summary>
        
        Sewer = 2,
        /// <summary>
        /// 井盖系统
        /// </summary>
        
        ManholeCover = 3,
        /// <summary>
        /// 公安视频箱系统
        /// </summary>
        
        PoliceVideo = 4,
        /// <summary>
        /// 水质公共卫生系统
        /// </summary>
        
        WaterQuality = 5,
        /// <summary>
        /// 路灯监控系统
        /// </summary>
        
        RoadLamp = 6,
        /// <summary>
        /// 电力管网系统
        /// </summary>
        
        ElectricPower = 7,
        /// <summary>
        /// 电梯系统
        /// </summary>
        
        Elevator = 8,
        /// <summary>
        /// 安全监控系统
        /// </summary>
        
        Security = 9,
        /// <summary>
        /// 瓦斯抽采系统
        /// </summary>
        
        GasExtraction = 10,
        /// <summary>
        /// 人员定位系统
        /// </summary>
        
        Personnel = 11,
        /// <summary>
        /// 语音扩播系统
        /// </summary>
        
        Broadcast = 12,
        /// <summary>
        /// 降尘系统
        /// </summary>
        
        Dust = 13,
        /// <summary>
        /// 空气质量监测
        /// </summary>
        
        AirQuality = 22,
        /// <summary>
        /// 区域环境噪声监测
        /// </summary>
        
        Noise = 23,
        /// <summary>
        /// 大气环境污染源
        /// </summary>
        
        Atmospheric = 31,
        /// <summary>
        /// 地表水体环境污染源
        /// </summary>
        
        SurfaceWater = 32,
        /// <summary>
        /// 地下水体环境污染源
        /// </summary>
        
        UndergroundWater = 33,
        /// <summary>
        /// 海洋环境污染源
        /// </summary>
        
        MarineEnvironment = 34,
        /// <summary>
        /// 土壤环境污染源
        /// </summary>
        
        SoilEnvironment = 35,
        /// <summary>
        /// 声环境污染源
        /// </summary>
        
        SoundEnvironment = 36,
        /// <summary>
        /// 振动环境污染源
        /// </summary>
        
        VibrationEnvironment = 37,
        /// <summary>
        /// 放射性环境污染源
        /// </summary>
        
        RadioactiveEnvironment = 38,
        /// <summary>
        /// 电磁环境污染源
        /// </summary>
        
        ElectromagneticEnvironment = 41,
        /// <summary>
        /// 结构监测系统
        /// </summary>
        
        Structure = 71,
        /// <summary>
        /// 环境监测系统
        /// </summary>
        
        Environmental = 72,
        /// <summary>
        /// 附属设施监测系统
        /// </summary>
        
        AffiliatedFacilities = 73,
        /// <summary>
        /// 视频监控系统
        /// </summary>
        
        Video = 74,
        /// <summary>
        /// 入侵报警系统
        /// </summary>
        
        IntrusionAlarm = 75,
        /// <summary>
        /// 通信系统
        /// </summary>
        
        Communication = 76,
        /// <summary>
        /// 巡更系统
        /// </summary>
        
        Patrolling = 77,
        /// <summary>
        /// 火灾报警系统
        /// </summary>
        
        FireAlarm = 78,
        /// <summary>
        /// 可燃气体报警系统
        /// </summary>
        
        GasAlarm = 79,
        /// <summary>
        /// 综合管廊监管平台
        /// </summary>
        
        ComprehensiveUtility = 80,
        /// <summary>
        /// 水务运营管理平台
        /// </summary>
        
        WaterOperationManagement = 81, 
    }
}
