using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic.Framework.Web;
using Basic.Framework.Web.WebApi.Proxy;
using Basic.Framework.Common;
using Sys.DataCollection.Common.Protocols;

namespace Sys.DataCollection.WebApiAgent
{
    /// <summary>
    /// 广播系统调用远程服务器接口代理类
    /// </summary>
    public class BroadCastControllerProxy : BroadCastBaseProxy
    {
        /// <summary>
        /// 单例锁
        /// </summary>
        protected static readonly object obj = new object();

        private static BroadCastControllerProxy broadCastControllerProxyInstance;
        /// <summary>
        /// 配置单例
        /// </summary>
        public static BroadCastControllerProxy broadCastControllerProxy
        {
            get
            {
                if (broadCastControllerProxyInstance == null)
                {
                    lock (obj)
                    {
                        if (broadCastControllerProxyInstance == null)
                        {
                            broadCastControllerProxyInstance = new BroadCastControllerProxy();
                        }
                    }
                }
                return broadCastControllerProxyInstance;
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LoginResponse Login(LoginRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/login.do", JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<LoginResponse>(responseStr);
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RefreshResponse Refresh(RefreshRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/refresh.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<RefreshResponse>(responseStr);
        }
        /// <summary>
        /// 查询终端
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TerminalQueryResponse queryTerm(TerminalQueryRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/queryTerm.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<TerminalQueryResponse>(responseStr);
        }
        /// <summary>
        /// 添加分区
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PartitionControlResponse addZone(PartitionControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/addZone.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<PartitionControlResponse>(responseStr);
        }
        /// <summary>
        /// 修改分区
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PartitionControlResponse setZone(PartitionControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/setZone.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<PartitionControlResponse>(responseStr);
        }
        /// <summary>
        /// 删除分区
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PartitionControlResponse delZone(PartitionControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/delZone.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<PartitionControlResponse>(responseStr);
        }
        /// <summary>
        /// 添加终端
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TerminalControlResponse addTerm(TerminalControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/addTerm.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<TerminalControlResponse>(responseStr);
        }
        /// <summary>
        /// 修改终端
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TerminalControlResponse setTerm(TerminalControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/setTerm.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<TerminalControlResponse>(responseStr);
        }
        /// <summary>
        /// 删除终端
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TerminalControlResponse delTerm(TerminalControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/delTerm.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<TerminalControlResponse>(responseStr);
        }
        /// <summary>
        /// 发起呼叫
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MakeCallResponse makeCall(MakeCallRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/makeCall.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<MakeCallResponse>(responseStr);
        }
        /// <summary>
        /// 挂断呼叫
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public HangupCallResponse hangupCall(HangupCallRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/hangupCall.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<HangupCallResponse>(responseStr);
        }
        /// <summary>
        /// 监听呼叫
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MonitorCallResponse monitorCall(MonitorCallRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/monitorCall.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<MonitorCallResponse>(responseStr);
        }
        /// <summary>
        /// 添加音乐
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MusicControlResponse addMusic(MusicControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/addMusic.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<MusicControlResponse>(responseStr);
        }
        /// <summary>
        /// 删除音乐
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MusicControlResponse delMusic(MusicControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/delMusic.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<MusicControlResponse>(responseStr);
        }
        /// <summary>
        /// 创建播放列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PlayListControlResponse createPlayList(PlayListControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/createPlayList.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<PlayListControlResponse>(responseStr);
        }
        /// <summary>
        /// 修改播放列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PlayListControlResponse setPlayList(PlayListControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/setPlayList.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<PlayListControlResponse>(responseStr);
        }
        /// <summary>
        /// 删除播放列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PlayListControlResponse delPlayList(PlayListControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/delPlayList.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<PlayListControlResponse>(responseStr);
        }
        /// <summary>
        /// 添加播放列表音乐
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PlayListMusicControlResponse addPlayListMusic(PlayListMusicControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/addPlayListMusic.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<PlayListMusicControlResponse>(responseStr);
        }
        /// <summary>
        /// 删除播放列表音乐
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PlayListMusicControlResponse delPlayListMusic(PlayListMusicControlRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/delPlayListMusic.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<PlayListMusicControlResponse>(responseStr);
        }
        /// <summary>
        /// 启动喊话广播任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public StartPaVoiceTaskResponse startPaVoiceTask(StartPaVoiceTaskRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/startPaVoiceTask.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<StartPaVoiceTaskResponse>(responseStr);
        }
        /// <summary>
        /// 启动音乐广播任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public StartPaMusicTaskResponse startPaMusicTask(StartPaMusicTaskRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/startPaMusicTask.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<StartPaMusicTaskResponse>(responseStr);
        }
        /// <summary>
        /// 启动文字广播任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public StartPaTtsTaskResponse startPaTtsTask(StartPaTtsTaskRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/startPaTtsTask.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<StartPaTtsTaskResponse>(responseStr);
        }
        /// <summary>
        /// 结束广播任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EndPaTaskResponse endPaTask(EndPaTaskRequest request)
        {
            var responseStr = HttpClientHelper.Post(Webapi + "/api/REST-API/endPaTask.do?accessToken=" + Token, JSONHelper.ToJSONString(request));
            return JSONHelper.ParseJSONString<EndPaTaskResponse>(responseStr);
        }
    }
}
