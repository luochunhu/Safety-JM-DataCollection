using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    /// <summary>
    /// 获取分站的工作状态(设备->上位机)
    /// </summary>
    public class GetStationUpdateStateResponseCommand : Sys.DataCollection.Common.Protocols.GetStationUpdateStateResponse
    {
        public void Handle(byte[] data, MasProtocol protocol, ushort startIndex,string point)
        {
            GetStationUpdateStateResponse StateResponse = new GetStationUpdateStateResponse();
            StateResponse.DeviceCode = point;
            startIndex += 8;
            int tempInt1 = data[startIndex];//数据域标记字1
            int tempInt2 = 0;//数据域标记字2
            int tempInt3 = 0;//数据域标记字3
            int tempInt4 = 0;//数据域标记字4
            startIndex++;
            if (((tempInt1 >> 7) & 0x01) == 0x01)
            {
                startIndex++;
                tempInt2 = data[startIndex];
                if (((tempInt2 >> 7) & 0x01) == 0x01)
                {
                    startIndex++;
                    tempInt3 = data[startIndex];
                    if (((tempInt3 >> 7) & 0x01) == 0x01)
                    {
                        
                        tempInt4 = data[startIndex];
                        startIndex++;
                        if (((tempInt4 >> 7) & 0x01) == 0x01)
                        {
                            
                        }
                    }
                }
            }

            if ((tempInt1 & 0x01) == 0x01)  //后续有软件版本号字段
            {
                startIndex++;
                StateResponse.GetSoftVersion = data[startIndex] * 0.1;
                startIndex++;
            }
            if (((tempInt1 >> 1) & 0x01) == 0x01)  //后续有远程升级状态字段
            {
                startIndex++;//字节数  无用
                StateResponse.GetUpdateState = data[startIndex];  //升级状态
                startIndex++;
                StateResponse.UpdateVersion = data[startIndex]; //升级版本号
                startIndex++;
            }

            if (((tempInt1 >> 2) & 0x01) == 0x01)  //后续有设备类型字段
            {
                startIndex++;//字节数  无用
                StateResponse.GetDevType = data[startIndex];  //设备类型
                startIndex++;
            }

            if (((tempInt1 >> 3) & 0x01) == 0x01)  //后续有设备硬件版本号
            {
                startIndex++;//字节数  无用
                StateResponse.GetHardVersion = data[startIndex]*0.1;  //设备硬件版本号
            }

            protocol.ProtocolType = ProtocolType.GetStationUpdateStateResponse;
            protocol.Protocol = StateResponse;
        }
    }

}
    