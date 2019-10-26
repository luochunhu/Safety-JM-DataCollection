using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 异常中止升级流程(设备->上位机)
    /// </summary>
    public class UpdateCancleResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /*
         * Bit7~bit4：数据交互标志
         * =6中心站告知分站该次升级过程中止；Bit3~bit0：故障码
         * =0：正常（分站已中止该次升级）
         * =1：设备类型不匹配（未能中止）
         * =2：升级文件版本号不匹配（未能中止）
         * =3：硬件版本号不匹配（未能中止）
         */

        /// <summary>
        /// 故障码
        /// </summary>
        public int ResponseCode;
    }

}
