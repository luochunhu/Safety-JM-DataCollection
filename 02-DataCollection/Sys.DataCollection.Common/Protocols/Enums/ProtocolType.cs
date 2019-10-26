using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 协议类型列表
    /// 枚举值说明
    /// 1-99     为上位机到设备
    /// 100-199  为设备到上位机
    /// 200-299  为上位机到网关
    /// 300-399  为网关到上位机
    /// 400-499  为上位机到设备（Udp广播的特殊处理，主要应用在安全监控系统）
    /// 500-599  为设备到上位机（Udp广播的特殊处理，主要应用在安全监控系统）
    /// </summary>
    [Serializable]
    public enum ProtocolType
    {
        /// <summary>
        /// 下发通讯测试命令（上位机->设备）
        /// </summary>
        [EnumMember]
        CommunicationTestRequest = 1,
        /// <summary>
        /// 通讯测试命令回发（设备->上位机）
        /// </summary>
        [EnumMember]
        CommunicationTestResponse = 101,
        /// <summary>
        /// 下发控制命令（上位机->设备）
        /// </summary>
        [EnumMember]
        DeviceControlRequest = 2,
        /// <summary>
        /// 控制命令回复（设备->上位机）
        /// </summary>
        [EnumMember]
        DeviceControlResponse = 102,
        /// <summary>
        /// 设备请求初始化（设备->上位机）
        /// </summary>
        [EnumMember]
        DeviceInitializeRequest = 104,
        /// <summary>
        /// 下发初始化（上位机->设备）
        /// </summary>
        InitializeRequest = 3,
        /// <summary>
        /// 设备初始化应答（设备->上位机）
        /// </summary>
        [EnumMember]
        InitializeResponse = 103,
        /// <summary>
        /// 上传网络设备实时数据（设备->上位机）
        /// </summary>
        [EnumMember]
        NetworkDeviceDataRequest = 105,
        /// <summary>
        /// 获取电源箱实时数据回复（上位机->设备）
        /// </summary>
        [EnumMember]
        QueryBatteryRealDataRequest = 6,
        /// <summary>
        /// 电源箱实时数据回复（设备->上位机）
        /// </summary>
        [EnumMember]
        QueryBatteryRealDataResponse = 106,
        /// <summary>
        /// 获取设备唯一编码（上位机->设备）
        /// </summary>
        [EnumMember]
        QueryDeviceInfoRequest = 7,
        /// <summary>
        /// 设备出厂编码信息回复（设备->上位机）
        /// </summary>
        [EnumMember]
        QueryDeviceInfoResponse = 107,
        /// <summary>
        /// 请求设备回发实时数据(上位机->设备)
        /// </summary>
        [EnumMember]
        QueryRealDataRequest = 8,
        /// <summary>
        /// 设备回发实时数据(设备->上位机)
        /// </summary>
        [EnumMember]
        QueryRealDataResponse = 108,
        /// <summary>
        /// 下发时间同步命令（上位机->设备）
        /// </summary>
        [EnumMember]
        TimeSynchronizationRequest = 9,
        /// <summary>
        /// 修改传感器地址号(上位机->设备)
        /// </summary>
        [EnumMember]
        ModificationDeviceAdressRequest=10,
        /// <summary>
        /// 修改传感器地址号(设备->上位机)
        /// </summary>
        [EnumMember]
        ModificationDeviceAdressResponse = 109,
        /// <summary>
        /// 获取分站的历史控制记录(上位机->设备)
        /// </summary>
        [EnumMember]
        QueryHistoryControlRequest =11,
        /// <summary>
        /// 获取分站的历史控制记录(设备->上位机)
        /// </summary>
        [EnumMember]
        QueryHistoryControlResponse =110,
        /// <summary>
        /// 获取分站的4小时五分钟数据(上位机->设备)
        /// </summary>
        [EnumMember]
        QueryHistoryRealDataRequest=12,
        /// <summary>
        /// 获取分站的4小时五分钟数据(设备->上位机)
        /// </summary>
        [EnumMember]
        QueryHistoryRealDataResponse = 111,
        /// <summary>
        /// 下发传感器分给报警命令(上位机->设备)
        /// </summary>
        [EnumMember]
        SetSensorGradingAlarmRequest =13,
        /// <summary>
        /// 下发传感器分给报警命令(设备->上位机)
        /// </summary>
        [EnumMember]
        SetSensorGradingAlarmResponse = 112,

        /// <summary>
        /// 网关心跳监测数据包(网关->上位机)
        /// </summary>
        [EnumMember]
        HeartbeatRequest = 201,
        /// <summary>
        /// 查询缓存请求（初始化时使用，网关->上位机）
        /// </summary>
        [EnumMember]
        QueryCacheDataRequest = 202,
        /// <summary>
        /// 查询缓存应答（上位机->网关）
        /// </summary>
        [EnumMember]
        QueryCacheDataResponse = 302,
        /// <summary>
        /// 更新缓存操作（新增、修改、删除，上位机->网关）
        /// </summary>
        [EnumMember]
        UpdateCacheDataRequest = 303,
       

        /// <summary>
        /// 下发复位命令（上位机->设备）
        /// </summary>
        [EnumMember]
        ResetDeviceCommandRequest = 401,
        /// <summary>
        /// 复位命令回复（设备->上位机）
        /// </summary>
        [EnumMember]
        ResetDeviceCommandResponse = 501,
        /// <summary>
        /// 下发搜索网络设备命令（上位机->设备）
        /// </summary>
        [EnumMember]
        SearchNetworkDeviceRequest = 402,
        /// <summary>
        /// 搜索网络设备回复（设备->上位机）
        /// </summary>
        [EnumMember]
        SearchNetworkDeviceResponse = 502,
        /// <summary>
        /// 获取指定网络设备参数（上位机->设备）
        /// </summary>
        [EnumMember]
        QuerytNetworkDeviceParamRequest = 403,
        /// <summary>
        /// 获取指定网络设备参数回复（设备->上位机）
        /// </summary>
        [EnumMember]
        QuerytNetworkDeviceParamResponse = 503,
        /// <summary>
        /// 设置指定网络设备参数（上位机->设备）
        /// </summary>
        [EnumMember]
        SetNetworkDeviceParamRequest = 404,
        /// <summary>
        /// 设置指定网络设备参数回复（设备->上位机）
        /// </summary>
        [EnumMember]
        SetNetworkDeviceParamResponse = 504,
        /// <summary>
        /// 复位指定的网络模块
        /// </summary>
        [EnumMember]
        ResetNetWorkDeviceRequest =405,
        /// <summary>
        /// 设置指定网络设备参数---串口参数（上位机->设备）
        /// </summary>
        [EnumMember]
        SetNetworkDeviceParamCommRequest = 407,
        /// <summary>
        /// 设置指定网络设备参数回复---串口参数（设备->上位机）
        /// </summary>
        [EnumMember]
        SetNetworkDeviceParamCommResponse = 507,
        /// <summary>
        /// 获取交换机基础状态信息---串口参数（上位机->设备）lb20180525
        /// </summary>
        [EnumMember]
        GetSwitchboardParamCommRequest = 408,
        /// <summary>
        /// 获取交换机基础状态信息回复---（设备->上位机）lb20180525
        /// </summary>
        [EnumMember]
        GetSwitchboardParamCommResponse = 508,


        /// <summary>
        /// 呼叫命令
        /// </summary>
        [EnumMember]
        CallPersonRequest = 406,

        #region ----分站升级相关----
        /// <summary>
        /// 获取分站的工作状态
        /// </summary>
        [EnumMember]
        GetStationUpdateStateRequest = 14,
        /// <summary>
        /// 获取分站的工作状态(设备->上位机)
        /// </summary>
        [EnumMember]
        GetStationUpdateStateResponse = 114,
        /// <summary>
        /// 请求分站远程升级
        /// </summary>
        [EnumMember]
        InspectionRequest = 15,
        /// <summary>
        /// 设备请求升级回复(设备->上位机)
        /// </summary>
        [EnumMember]
        InspectionResponse = 1150,
        /// <summary>
        /// 远程还原最近一次备份程序
        /// </summary>
        [EnumMember]
        ReductionRequest = 16,
        /// <summary>
        /// 远程还原最近一次备份程序(设备->上位机)
        /// </summary>
        [EnumMember]
        ReductionResponse = 17,
        /// <summary>
        /// 通知分站进行重启升级
        /// </summary>
        [EnumMember]
        RestartRequest = 117,
        /// <summary>
        /// 通知分站进行重启升级回复(设备->上位机)
        /// </summary>
        [EnumMember]
        RestartResponse = 18,
        /// <summary>
        /// 广播升级文件片段
        /// </summary>
        [EnumMember]
        SendUpdateBufferRequest = 118,
        /// <summary>
        /// 请求分站远程升级
        /// </summary>
        [EnumMember]
        StationUpdateRequest = 19,
        /// <summary>
        /// 设备请求升级回复(设备->上位机)
        /// </summary>
        [EnumMember]
        StationUpdateResponse = 119,
        /// <summary>
        /// 异常中止升级流程
        /// </summary>
        [EnumMember]
        UpdateCancleRequest = 20,
        /// <summary>
        /// 异常中止升级流程(设备->上位机)
        /// </summary>
        [EnumMember]
        UpdateCancleResponse = 120,
        #endregion

        #region ----广播系统----

        #region ----上位机->设备（25->36）----
        /// <summary>
        /// 结束广播任务
        /// </summary>
        [EnumMember]
        EndPaTaskRequest = 25,
        /// <summary>
        /// 挂断呼叫
        /// </summary>
        [EnumMember]
        HangupCallRequest = 26,
        /// <summary>
        /// 发起呼叫
        /// </summary>
        [EnumMember]
        MakeCallRequest = 27,
        /// <summary>
        /// 监听呼叫
        /// </summary>
        [EnumMember]
        MonitorCallRequest = 28,
        /// <summary>
        /// 音乐管理
        /// </summary>
        [EnumMember]
        MusicControlRequest = 29,
        /// <summary>
        /// 分区管理
        /// </summary>
        [EnumMember]
        PartitionControlRequest = 30,
        /// <summary>
        /// 播放列表管理
        /// </summary>
        [EnumMember]
        PlayListControlRequest = 31,
        /// <summary>
        /// 播放列表音乐管理
        /// </summary>
        [EnumMember]
        PlayListMusicControlRequest = 32,
        /// <summary>
        /// 启动音乐广播任务
        /// </summary>
        [EnumMember]
        StartPaMusicTaskRequest = 33,
        /// <summary>
        /// 启动文字广播任务
        /// </summary>
        [EnumMember]
        StartPaTtsTaskRequest = 34,
        /// <summary>
        /// 启动喊话广播任务
        /// </summary>
        [EnumMember]
        StartPaVoiceTaskRequest = 35,
        /// <summary>
        /// 终端管理
        /// </summary>
        [EnumMember]
        TerminalControlRequest = 36,
        /// <summary>
        /// 查询终端
        /// </summary>
        [EnumMember]
        QureyTerminalRequest = 37,
        #endregion

        #region ----设备->上位机(125->136)----

        /// <summary>
        /// 结束广播任务
        /// </summary>
        [EnumMember]
        EndPaTasklResponse = 125,
        /// <summary>
        /// 挂断呼叫
        /// </summary>
        [EnumMember]
        HangupCallResponse = 126,
        /// <summary>
        /// 发起呼叫
        /// </summary>
        [EnumMember]
        MakeCallResponse = 127,
        /// <summary>
        /// 监听呼叫
        /// </summary>
        [EnumMember]
        MonitorCallResponse = 128,
        /// <summary>
        /// 音乐管理
        /// </summary>
        [EnumMember]
        MusicControlResponse = 129,
        /// <summary>
        /// 分区管理
        /// </summary>
        [EnumMember]
        PartitionControlResponse = 130,
        /// <summary>
        /// 播放列表管理
        /// </summary>
        [EnumMember]
        PlayListControlResponse = 131,
        /// <summary>
        /// 播放列表音乐管理
        /// </summary>
        [EnumMember]
        PlayListMusicControlResponse = 132,
        /// <summary>
        ///  启动文字广播任务
        /// </summary>
        [EnumMember]
        StartPaTtsTaskResponse = 133,
        /// <summary>
        ///  启动音乐广播任务
        /// </summary>
        [EnumMember]
        StartPaMusicTaskResponse = 134,
        /// <summary>
        ///  启动喊话广播任务
        /// </summary>
        [EnumMember]
        StartPaVoiceTaskResponse = 135,
        /// <summary>
        ///  终端管理
        /// </summary>
        [EnumMember]
        TerminalControlResponse = 136,
        /// <summary>
        /// 查询终端
        /// </summary>
        [EnumMember]
        QureyTerminalResponse = 137,
        #endregion

        #region ----设备->上位机 Event(137->140)----
        /// <summary>
        /// 呼叫结束事件
        /// </summary>
        [EnumMember]
        CallEndResponse = 137,
        /// <summary>
        /// 呼叫开始事件
        /// </summary>
        [EnumMember]
        CallStartResponse = 138,
        /// <summary>
        /// 终端呼叫
        /// </summary>
        [EnumMember]
        TermCallResponse = 139,
        /// <summary>
        /// 终端注册
        /// </summary>
        [EnumMember]
        TermRegResponse = 140
        #endregion

        #endregion
    }
}
