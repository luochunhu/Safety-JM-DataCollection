using Sys.DataCollection.Communications.Provider;
using Sys.DataCollection.Services.Communications.Provider.C8962Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Communications
{
    /// <summary>
    /// 通讯工厂类
    /// </summary>
    public class CommunicationFactory
    {
        /// <summary>
        /// 创建通讯模块
        /// </summary>
        /// <param name="communicationType">通讯方式</param>
        /// <returns></returns>
        public static ICommunication CreateCommunication(CommunicationType communicationType)
        {
            ICommunication communication = null;

            switch (communicationType)
            {
                case CommunicationType.C8962:
                    communication = new C8962Communication();
                    break;
                case CommunicationType.socketClient:
                    communication = new SocketTcpClientCommunication();
                    break;
                case CommunicationType.Http:
                    communication = new HttpCommunication();
                    break;
                case CommunicationType.HongDian:
                    throw new NotImplementedException("CommunicationType.HongDian 通讯方式暂时未实现");
                    //break;
                case CommunicationType.MAS:
                    throw new NotImplementedException("CommunicationType.MAS 通讯方式暂时未实现");
                    //break;

                default:
                    throw new NotImplementedException("未知通讯方式");
                    //break;
            }
            return communication;
        }
    }
}
