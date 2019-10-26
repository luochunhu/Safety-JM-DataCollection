using Sys.DataCollection.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 下发初始化（上位机->设备）
    /// </summary>
    public class InitializeRequest : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /// <summary>
        /// 设置分站主通讯故障闭锁输出延时，单位：秒
        /// </summary>
        public byte FaultBlockTime { get; set; }

        /// <summary>
        /// 设置分站的馈电识别阈值，秒
        /// </summary>
        public byte FeedThreshold { get; set; }
        /// <summary>
        /// 瓦电3分强制解锁标记=1表示解除，=0表示不解除
        /// </summary>
        public byte GasThreeUnlockContro { get; set; }
        /// <summary>
        /// 表示是否允许接入第三方的传感器，=1表示允许，=0表示不允许
        /// </summary>
        public byte ImpowerJoinUp { get; set; }
        /// <summary>
        /// 表示控制链表todo
        /// </summary>
        public List<DeviceControlItem> ControlChanels { get; set; }
        //抽放修正数据
        public List<DrainageInfo> LstDrainageInfo { get; set; }
    }
    //抽放下发的修正数据20180921
    public class DrainageInfo
    {
        //1表示小时标混是否有编差值，=2表示小时标纯是否有编差值  =3表示小时工混是否有编差值 =4表示小时工纯是否有编差值
        //5表示小时标混是否有编差值，=6表示小时标纯是否有编差值  =7表示小时工混是否有编差值 =8表示小时工纯是否有编差值
        //9表示小时标混是否有编差值，=10表示小时标纯是否有编差值  =11表示小时工混是否有编差值 =12表示小时工纯是否有编差值
        //13表示小时标混是否有编差值，=14表示小时标纯是否有编差值  =15表示小时工混是否有编差值 =16表示小时工纯是否有编差值
        public byte Address { get; set; }
        //表示修正值
        public float XzValue { get; set; }
    }
}
