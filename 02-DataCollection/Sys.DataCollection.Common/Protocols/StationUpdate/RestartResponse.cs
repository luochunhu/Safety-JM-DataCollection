using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 通知分站进行重启升级回复(设备->上位机)
    /// </summary>
    public class RestartResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /*
         * Bit3~bit0：故障码
         * =0：正常（分站准备重启并升级）
         * =1：升级文件不属于该类设备（不能升级）
         * =2：请求的升级文件版本号与本地的升级文件版本号不匹配；（不能升级）
         * =3：该分站不处于升级模式（不能升级）
         * =4：该分站升级文件缺失；（不能升级）
         * =5：文件校验失败（不能升级）
         * =6：硬件版本号不匹配（不能升级） 
         */

        /// <summary>
        /// 故障码
        /// </summary>
        public int ResponseCode;
    }

}
