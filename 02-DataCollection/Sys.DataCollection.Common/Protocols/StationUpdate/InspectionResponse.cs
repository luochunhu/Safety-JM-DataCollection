using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 巡检单台分站的文件接收情况回复(设备->上位机)
    /// </summary>
    public class InspectionResponse : Sys.DataCollection.Common.Protocols.DeviceProtocol
    {
        /*
         * Bit3~bit0：故障码
         * =0：正常（升级文件已接收完毕）
         * =1：升级文件不属于该类设备（不能升级）
         * =2：请求的升级文件版本号与本地的升级文件版本号不匹配；（不能升级）
         * =3：该设备不处于升级模式（不能升级）
         * =4：升级文件缺失（不能升级，后续有缺失文件编号字段）
         * =5：文件编号异常；
         * =6：硬件版本号不匹配（不能升级） 
         */

        /// <summary>
        /// 故障码
        /// </summary>
        public int ResponseCode;
        /// <summary>
        /// 缺失文件编号
        /// </summary>
        public int LostFileNum;
    }

}
