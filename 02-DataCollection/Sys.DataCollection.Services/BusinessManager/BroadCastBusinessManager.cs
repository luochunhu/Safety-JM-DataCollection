using Basic.Framework.Rpc;
using Basic.Framework.Web;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.WebApiAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.BusinessManager
{
    /// <summary>
    /// 广播系统业务处理类
    /// </summary>
    public class BroadCastBusinessManager
    {
        /// <summary>
        /// 查询终端
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse queryTerm(MasProtocol masProtocol)
        {
            TerminalQueryRequest request = masProtocol.Deserialize<TerminalQueryRequest>();
            if (request == null)
            {
                return null;
            }
            TerminalQueryResponse response = BroadCastControllerProxy.broadCastControllerProxy.queryTerm(request);
            return RpcResponse.Response<TerminalQueryResponse>(response);
        }
        /// <summary>
        /// 分区管理
        /// </summary>
        public static RpcResponse zoneManage(MasProtocol masProtocol)
        {
            PartitionControlRequest request = masProtocol.Deserialize<PartitionControlRequest>();
            if (request == null)
            {
                return null;
            }
            if (request.InfoState == InfoState.AddNew)
            {
                addZone(request);
            }
            if (request.InfoState == InfoState.Modified)
            {
                setZone(request);
            }
            if (request.InfoState == InfoState.Delete)
            {
                delZone(request);
            }
            return null;
        }
        /// <summary>
        /// 添加分区
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse addZone(PartitionControlRequest request)
        {
            PartitionControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.addZone(request);
            return RpcResponse.Response<PartitionControlResponse>(response);
        }
        /// <summary>
        /// 修改分区
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse setZone(PartitionControlRequest request)
        {
            PartitionControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.setZone(request);
            return RpcResponse.Response<PartitionControlResponse>(response);
        }
        /// <summary>
        /// 删除分区
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse delZone(PartitionControlRequest request)
        {
            PartitionControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.delZone(request);
            return RpcResponse.Response<PartitionControlResponse>(response);
        }
        /// <summary>
        /// 设备管理
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse termManage(MasProtocol masProtocol)
        {
            TerminalControlRequest request = masProtocol.Deserialize<TerminalControlRequest>();
            if (request == null)
            {
                return null;
            }
            if (request.InfoState == InfoState.AddNew)
            {
                addTerm(request);
            }
            if (request.InfoState == InfoState.Modified)
            {
                setTerm(request);
            }
            if (request.InfoState == InfoState.Delete)
            {
                delTerm(request);
            }
            return null;
        }
        /// <summary>
        /// 添加终端
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse addTerm(TerminalControlRequest request)
        {           
            TerminalControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.addTerm(request);
            return RpcResponse.Response<TerminalControlResponse>(response);
        }
        /// <summary>
        /// 修改终端
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse setTerm(TerminalControlRequest request)
        {           
            TerminalControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.setTerm(request);
            return RpcResponse.Response<TerminalControlResponse>(response);
        }
        /// <summary>
        /// 删除终端
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse delTerm(TerminalControlRequest request)
        {           
            TerminalControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.delTerm(request);
            return RpcResponse.Response<TerminalControlResponse>(response);
        }
        /// <summary>
        /// 发起呼叫
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse makeCall(MasProtocol masProtocol)
        {
            MakeCallRequest request = masProtocol.Deserialize<MakeCallRequest>();
            if (request == null)
            {
                return null;
            }
            MakeCallResponse response = BroadCastControllerProxy.broadCastControllerProxy.makeCall(request);
            return RpcResponse.Response<MakeCallResponse>(response);
        }
        /// <summary>
        /// 挂断呼叫
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse hangupCall(MasProtocol masProtocol)
        {
            HangupCallRequest request = masProtocol.Deserialize<HangupCallRequest>();
            if (request == null)
            {
                return null;
            }
            HangupCallResponse response = BroadCastControllerProxy.broadCastControllerProxy.hangupCall(request);
            return RpcResponse.Response<HangupCallResponse>(response);
        }
        /// <summary>
        /// 监听呼叫
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse monitorCall(MasProtocol masProtocol)
        {
            MonitorCallRequest request = masProtocol.Deserialize<MonitorCallRequest>();
            if (request == null)
            {
                return null;
            }
            MonitorCallResponse response = BroadCastControllerProxy.broadCastControllerProxy.monitorCall(request);
            return RpcResponse.Response<MonitorCallResponse>(response);
        }
        /// <summary>
        /// 音乐管理
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse musicManage(MasProtocol masProtocol)
        {
            MusicControlRequest request = masProtocol.Deserialize<MusicControlRequest>();
            if (request == null)
            {
                return null;
            }
            if (request.InfoState == InfoState.AddNew)
            {
                addMusic(request);
            }           
            if (request.InfoState == InfoState.Delete)
            {
                delMusic(request);
            }
            return null;
        }
        /// <summary>
        /// 添加音乐
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse addMusic(MusicControlRequest request)
        {          
            MusicControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.addMusic(request);
            return RpcResponse.Response<MusicControlResponse>(response);
        }
        /// <summary>
        /// 删除音乐
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse delMusic(MusicControlRequest request)
        {           
            MusicControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.delMusic(request);
            return RpcResponse.Response<MusicControlResponse>(response);
        }
        /// <summary>
        /// 音乐播放列表管理
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse playListManage(MasProtocol masProtocol)
        {
            PlayListControlRequest request = masProtocol.Deserialize<PlayListControlRequest>();
            if (request == null)
            {
                return null;
            }
            if (request.InfoState == InfoState.AddNew)
            {
                createPlayList(request);
            }
            if (request.InfoState == InfoState.Modified)
            {
                setPlayList(request);
            }
            if (request.InfoState == InfoState.Delete)
            {
                delPlayList(request);
            }
            return null;
        }
        /// <summary>
        /// 创建播放列表
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse createPlayList(PlayListControlRequest request)
        {            
            PlayListControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.createPlayList(request);
            return RpcResponse.Response<PlayListControlResponse>(response);
        }
        /// <summary>
        /// 修改播放列表
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse setPlayList(PlayListControlRequest request)
        {
            PlayListControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.setPlayList(request);
            return RpcResponse.Response<PlayListControlResponse>(response);
        }
        /// <summary>
        /// 删除播放列表
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse delPlayList(PlayListControlRequest request)
        {
            PlayListControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.delPlayList(request);
            return RpcResponse.Response<PlayListControlResponse>(response);
        }
        /// <summary>
        /// 音乐播放列表下音乐管理
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse playListMusicManage(MasProtocol masProtocol)
        {
            PlayListControlRequest request = masProtocol.Deserialize<PlayListControlRequest>();
            if (request == null)
            {
                return null;
            }
            if (request.InfoState == InfoState.AddNew)
            {
                createPlayList(request);
            }
            if (request.InfoState == InfoState.Modified)
            {
                setPlayList(request);
            }
            if (request.InfoState == InfoState.Delete)
            {
                delPlayList(request);
            }
            return null;
        }
        /// <summary>
        /// 添加播放列表音乐
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse addPlayListMusic(MasProtocol masProtocol)
        {
            PlayListMusicControlRequest request = masProtocol.Deserialize<PlayListMusicControlRequest>();
            if (request == null)
            {
                return null;
            }
            PlayListMusicControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.addPlayListMusic(request);
            return RpcResponse.Response<PlayListMusicControlResponse>(response);
        }
        /// <summary>
        /// 删除播放列表音乐
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse delPlayListMusic(MasProtocol masProtocol)
        {
            PlayListMusicControlRequest request = masProtocol.Deserialize<PlayListMusicControlRequest>();
            if (request == null)
            {
                return null;
            }
            PlayListMusicControlResponse response = BroadCastControllerProxy.broadCastControllerProxy.delPlayListMusic(request);
            return RpcResponse.Response<PlayListMusicControlResponse>(response);
        }
        /// <summary>
        /// 启动喊话广播任务
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse startPaVoiceTask(MasProtocol masProtocol)
        {
            StartPaVoiceTaskRequest request = masProtocol.Deserialize<StartPaVoiceTaskRequest>();
            if (request == null)
            {
                return null;
            }
            StartPaVoiceTaskResponse response = BroadCastControllerProxy.broadCastControllerProxy.startPaVoiceTask(request);
            return RpcResponse.Response<StartPaVoiceTaskResponse>(response);
        }
        /// <summary>
        /// 启动音乐广播任务
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse startPaMusicTask(MasProtocol masProtocol)
        {
            StartPaMusicTaskRequest request = masProtocol.Deserialize<StartPaMusicTaskRequest>();
            if (request == null)
            {
                return null;
            }
            StartPaMusicTaskResponse response = BroadCastControllerProxy.broadCastControllerProxy.startPaMusicTask(request);
            return RpcResponse.Response<StartPaMusicTaskResponse>(response);
        }
        /// <summary>
        /// 启动文字广播任务
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse startPaTtsTask(MasProtocol masProtocol)
        {
            StartPaTtsTaskRequest request = masProtocol.Deserialize<StartPaTtsTaskRequest>();
            if (request == null)
            {
                return null;
            }
            StartPaTtsTaskResponse response = BroadCastControllerProxy.broadCastControllerProxy.startPaTtsTask(request);
            return RpcResponse.Response<StartPaTtsTaskResponse>(response);
        }
        /// <summary>
        /// 结束广播任务
        /// </summary>
        /// <param name="masProtocol"></param>
        /// <returns></returns>
        public static RpcResponse endPaTask(MasProtocol masProtocol)
        {
            EndPaTaskRequest request = masProtocol.Deserialize<EndPaTaskRequest>();
            if (request == null)
            {
                return null;
            }
            EndPaTaskResponse response = BroadCastControllerProxy.broadCastControllerProxy.endPaTask(request);
            return RpcResponse.Response<EndPaTaskResponse>(response);
        }
    }
}
