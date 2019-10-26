using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic.Framework.Web;
using Basic.Framework.Service;
using System.Web.Http;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Driver;

namespace Sys.DataCollection.WebApi
{
    /// <summary>
    /// 广播系统回调方法
    /// </summary>
    public class BroadCastController : Basic.Framework.Web.WebApi.BasicApiController
    {
        /// <summary>
        /// 终端状态注册事件
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("REST-API/termRegStateReport.do")]
        public void termRegStateReport(TermRegResponse request)
        {
            //通过事件，触发并将数据发送到服务端 
            if (request != null)
            {
                ProtocolDataCreatedEventArgs upData = new ProtocolDataCreatedEventArgs();
                upData.DriverCode = "";
                upData.DeviceCode = request.termDN;//终端号码
                upData.MasProtocol = new MasProtocol(SystemType.Broadcast, DirectionType.Up, ProtocolType.TermRegResponse);
                upData.MasProtocol.CreatedTime = DateTime.Now;
                upData.MasProtocol.Protocol = request;
                BroadCastUpEvent.OnBroadCastProtocolData(upData);
            }
        }        
        /// <summary>
        /// 终端状态注册事件
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("REST-API/termCallStateReport.do")]
        public void termCallStateReport(TermCallResponse request)
        {
            //通过事件，触发并将数据发送到服务端 
            if (request != null)
            {
                ProtocolDataCreatedEventArgs upData = new ProtocolDataCreatedEventArgs();
                upData.DriverCode = "";
                upData.DeviceCode = request.termDN;//终端号码
                upData.MasProtocol = new MasProtocol(SystemType.Broadcast, DirectionType.Up, ProtocolType.TermCallResponse);
                upData.MasProtocol.CreatedTime = DateTime.Now;
                //将英文转换成枚举值并重新创建对象 （统一在服务端去转换）
                //var requestNew = new TermCallResponse();
                //requestNew.zoneId = request.zoneId;
                //requestNew.termDN = request.termDN;
                //requestNew.callState = (ItemCallState)Enum.Parse(typeof(ItemCallState), request.callState);
                upData.MasProtocol.Protocol = request;
                BroadCastUpEvent.OnBroadCastProtocolData(upData);
            }
        }
        /// <summary>
        /// 呼叫开始事件
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("REST-API/callStartReport.do")]
        public void callStartReport(CallStartResponse request)
        {
            //通过事件，触发并将数据发送到服务端 
            if (request != null)
            {
                ProtocolDataCreatedEventArgs upData = new ProtocolDataCreatedEventArgs();
                upData.DriverCode = "";
                upData.DeviceCode = request.callerDN;//主叫终端号码
                upData.MasProtocol = new MasProtocol(SystemType.Broadcast, DirectionType.Up, ProtocolType.CallStartResponse);
                upData.MasProtocol.CreatedTime = DateTime.Now;
                upData.MasProtocol.Protocol = request;
                BroadCastUpEvent.OnBroadCastProtocolData(upData);
            }
        }
        /// <summary>
        /// 呼叫结束事件
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("REST-API/callEndReport.do")]
        public void callEndReport(CallEndResponse request)
        {
            //通过事件，触发并将数据发送到服务端 
            if (request != null)
            {
                ProtocolDataCreatedEventArgs upData = new ProtocolDataCreatedEventArgs();
                upData.DriverCode = "";
                upData.DeviceCode = request.callId;//呼叫唯一标识
                upData.MasProtocol = new MasProtocol(SystemType.Broadcast, DirectionType.Up, ProtocolType.CallEndResponse);
                upData.MasProtocol.CreatedTime = DateTime.Now;
                upData.MasProtocol.Protocol = request;
                BroadCastUpEvent.OnBroadCastProtocolData(upData);
            }
        }
    }
}
