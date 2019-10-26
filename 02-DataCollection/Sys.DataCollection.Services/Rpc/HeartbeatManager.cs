using Basic.Framework.Logging;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Rpc;
using Sys.DataCollection.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sys.DataCollection.Rpc
{   
    /// <summary>
    /// 心跳定时任务模块
    /// </summary>
    public class HeartbeatManager: Basic.Framework.JobSchedule.BasicTask
    {
        public HeartbeatManager(int interval)
            : base("心跳检测定时任务", interval * 1000)
        { }

        /// <summary>
        /// 循环执行定时任务
        /// </summary>
        protected override void DoWork()
        {
            SendHeartbeatRequest();
            base.DoWork();
        }

        /// <summary>
        /// 启动心跳模块
        /// </summary>
        public override void Start()
        {
            //启动之前做服务端连接检测，如果未连接服务端成功，则一直连接服务端
            bool isConnected = false;
            isConnected = SendHeartbeatRequest();
            while (!isConnected)
            {
                LogHelper.Debug("正在连接核心服务器，请等待");
                isConnected = SendHeartbeatRequest();
                Thread.Sleep(1000 * 3);
            }                

            base.Start();
        }

        /// <summary>
        /// 发送心跳请求
        /// </summary>
        /// <returns></returns>
        private bool SendHeartbeatRequest()
        {
            HeartbeatRequest request = new HeartbeatRequest()
            {
                RequestTime = DateTime.Now,
                Status = 1
            };

            MasProtocol masProtocol = new MasProtocol(SystemType.Security, DirectionType.Up, ProtocolType.HeartbeatRequest);
            masProtocol.Protocol = request;

            var response = GatewayManager.RpcManager.Send<GatewayRpcResponse>(masProtocol, RequestType.DeviceRequest);
            if (response != null && response.IsSuccess)
            {
                return true;
            }
            return false;
        }
    }
}
