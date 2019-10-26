using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    public enum ItemCallState
    {
        //49	广播电话-通话中
        //50	广播电话-广播中
        //51	广播电话-音乐播放中

        //52	广播电话-空闲
        //53	广播电话-摘机
        //54	广播电话-呼叫
        //55	广播电话-振铃
        //56	广播电话-回铃
        //57	广播电话-通话保持

        [EnumMember]
        [Description("广播中")]
        radio = 50,
        [EnumMember]
        [Description("音乐播放中")]
        musicPlay = 51,
        [EnumMember]
        [Description("空闲")]
        idle = 52,
        [EnumMember]
        [Description("摘机")]
        offhook = 53,
        [EnumMember]
        [Description("呼叫")]
        calling = 54,
        [EnumMember]
        [Description("振铃")]
        ring = 55,
        [EnumMember]
        [Description("回铃")]
        alert = 56,
        [EnumMember]
        [Description("通话")]
        talk = 49,
        [EnumMember]
        [Description("保持")]
        hold = 57
    }
}
