using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 远程还原最近一次备份程序(设备->上位机)
    /// </summary>
    public class ReductionResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /*
         * Bit3~bit0：故障码
         * =0：正常（分站已准备恢复操作）
         * =1：设备类型不匹配（未能恢复）
         * =2：版本号不匹配（未能恢复）
         * =3：该分站未进行备份，不能恢复；
         * =4：分站最近因存储器不可靠而升级失败，不能恢复
         * =5：硬件版本号不匹配（未能恢复）
         */

        /// <summary>
        /// 故障码
        /// </summary>
        public int ResponseCode;
    }

}
