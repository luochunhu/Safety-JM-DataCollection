using Basic.Framework.Configuration;
using Basic.Framework.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sys.DataCollection.ServiceHost
{
    public partial class MainService : ServiceBase
    {
        bool _isRun = false;
        string _serviceName = "DataCollectionService";

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (_isRun)
            {
                return;
            }
            _isRun = true;
             
            Task.Run(() =>
            {
                //启动网关服务
                Sys.DataCollection.Services.GatewayService.Start();
                //启动双机热备状态检测
                StartHAService();
            });
        }      

        protected override void OnStop()
        {
            if (!_isRun)
            {
                return;
            }

            _isRun = false;
            //停止服务
            Sys.DataCollection.Services.GatewayService.Stop();
        }

        /// <summary>
        /// 开始启动HA检测
        /// </summary>
        private void StartHAService()
        {
            Thread haThread = new Thread(new ThreadStart(MonitorHAService));
            //haThread.IsBackground = true;
            haThread.Start();

            LogHelper.SystemInfo("热备热切状态检测服务启动");
        }

        /// <summary>
        /// 监测双机热备退出
        /// </summary>
        private void MonitorHAService()
        {
            LogHelper.Debug("热备热切状态检测服务DoWork()开始执行");
            string haConfigPath = "";
            try
            {
                 haConfigPath = ConfigurationManager.FileConfiguration.GetString("HAPath", @"HA\BackConfig.ini");
                if (!haConfigPath.StartsWith(@"\"))
                {
                    haConfigPath = @"\" + haConfigPath;
                }

                var dr = new System.IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                if (dr != null && dr.Parent != null)
                {
                    //获取双机热备的完整目录
                    haConfigPath = dr.Parent.FullName + haConfigPath;
                }
               // LogHelper.SystemInfo("haConfigPath：" + haConfigPath);

                if (!File.Exists(haConfigPath))
                {
                    LogHelper.Debug("HA热备配置文件路径不存，haConfigPath：" + haConfigPath);
                    return;
                }

                LogHelper.Debug("热备热切状态检测服务开始执行循环检测状态");
            }
            catch (Exception ex)
            {
                LogHelper.SystemInfo("Exception:" + ex.ToString());
                return;
            }
          
            while (_isRun)
            {
                try
                {
                    #region 监测双机热备是否已置退出标记，如果已置，则退出程序
                    string flagIndex = "";

                    string[] tempArrStr = Basic.Framework.Common.IniConfigHelper.INIGetStringValue(haConfigPath, "Backupdb", "PFilePath", "").Split('|');

                    for (int i = 0; i < tempArrStr.Length; i++)
                    {
                        if (tempArrStr[i].ToString().ToLower().Contains(_serviceName.ToLower()))
                        {
                            flagIndex = (i + 1).ToString();
                            break;
                        }
                    }

                    if (Basic.Framework.Common.IniConfigHelper.INIGetStringValue(haConfigPath, "Backupdb", "ProgCloseFlag" + flagIndex, "") == "1")
                    {                       
                        LogHelper.Debug("检测到HA ProgCloseFlag=1，开始停止DataCollectionService 服务");
                        
                        //停止windows服务
                        this.Stop();
                       
                        //重新回写标识
                        Basic.Framework.Common.IniConfigHelper.INIWriteValue(haConfigPath, "Backupdb", "ProgCloseFlag" + flagIndex, "2");

                        LogHelper.Debug("重写 HA ProgCloseFlag=2");                      
                        break;
                    }
                    #endregion
                }
                catch (Exception err)
                {
                    LogHelper.Error("监测双机热备退出异常:", err);
                }
                finally
                {
                    if (_isRun)
                    {
                        Thread.Sleep(1000);
                    }                   
                }
            }

            LogHelper.SystemInfo("热备热切状态检测服务停止");
        }


    }
}
