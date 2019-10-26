using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 设备请求升级回复(设备->上位机)
    /// </summary>
    public class StationUpdateResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /*
         * Bit3~bit0：故障码  
         * =0：正常（已准备好接收升级文件）
         * =1：升级文件不属于该类设备（不能升级）
         * =2：升级条件不满足（不能升级）
         * =3：本地代码存储空间过小（不能升级）
         * =4：分站已处于远程升级模式且升级软件版本号与本次请求一致（分站将不更新已接收数据）；
         * =5：分站已处于升级模式且升级软件版本号与本次请求不匹配（中心站若想升级该分站，应下发强制中止当前升级流程再重新下发本次升级请求。）
         * =6：本地存储器不可靠，最近一次升级失败（不能升级）
         * =7：分站还未做好升级准备，稍后再试；（不能升级）
         * =8：硬件版本号与本地不匹配（不能升级）
         */

        /// <summary>
        /// 故障码
        /// </summary>
        public int ResponseCode;
    }

}
