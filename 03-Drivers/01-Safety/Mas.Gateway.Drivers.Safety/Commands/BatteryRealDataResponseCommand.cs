using Basic.Framework.Logging;
using Sys.DataCollection.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    public class BatteryRealDataResponseCommand: QueryBatteryRealDataResponse
    {
        /// <summary>
        /// 仅适用于分站的电源箱解码
        /// <param name="data">传入回发的Buffer</param>
        /// <param name="protocol">回发的对象</param>
        /// <param name="startIndex">分站号的索引位置</param>
        /// <param name="deviceCommunicationType">分站的类型</param>
        /// <param name="point">分站的测点号</param>
        /// </summary>
        public void HandleBatteryRealData(byte[] data, MasProtocol protocol, ushort startIndex, byte deviceCommunicationType, string point)
        {
            QueryBatteryRealDataResponse realData = new QueryBatteryRealDataResponse();
            BatteryRealDataItem BatteryItem = new BatteryRealDataItem();
            protocol.ProtocolType = ProtocolType.QueryBatteryRealDataResponse;
            realData.BatteryDateTime = DateTime.Now;
            realData.DeviceCode = point;
            realData.BatteryRealDataItems = new List<BatteryRealDataItem>();

            BatteryItem.DeviceProperty = ItemDevProperty.Substation;
           
            Cache.HandleDeviceBattery(data, (byte)(startIndex + 5), BatteryItem);//解析电源箱的数据  111
            
            realData.BatteryRealDataItems.Add(BatteryItem);
            protocol.Protocol = realData;
        }
    }
}
