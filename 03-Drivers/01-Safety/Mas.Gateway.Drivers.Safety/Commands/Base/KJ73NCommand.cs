using Basic.Framework.Logging;
using Sys.DataCollection.Common.Commands;
using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Common.Utils;
using Sys.DataCollection.Driver.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    public class KJ73NCommand : MasCommand
    {
        public byte PackageType { get; set; }
        public bool IsSuccess { get; set; }
        public string errorMessage { get; set; }
        public byte[] InnerPackage { get; set; }
        /// <summary>
        /// 外包解包处理
        /// </summary>
        /// <param name="data">收到的网络数据包</param>
        /// <returns></returns>
        public static KJ73NCommand ToCommand(byte[] data, NetworkDeviceInfo net)
        {
            KJ73NCommand command = new Commands.KJ73NCommand();
            try
            {
                if (data.Length > 999 || data.Length < 4)
                {
                    command.IsSuccess = false;
                    command.errorMessage = "数据包总长度错误：【" + data.Length + "】";
                    return command;
                }
                if (data[0] == 0x7F && (data[1] == 0x03 || data[1] == 0x10))
                {
                    if (data[1] == 0x03 && data.Length >= 40)
                    {//数据回发
                        command.IsSuccess = true;
                        command.PackageType = 2;//表示包类型---data[4]
                        lock (Cache.LockWorkNet)//用缓存的网络设备进行操作
                        {
                            CacheNetWork curnet = Cache.LstWorkNet.Find(delegate (CacheNetWork p) { return p.IP == net.IP; });
                            if (curnet != null)
                            {
                                curnet.State = 3;
                            }
                        }
                    }
                    else if (data[1] == 0x10 && data.Length >= 7)
                    {//控制回发
                        command.IsSuccess = true;
                        command.PackageType = 2;//表示包类型---data[4]
                        lock (Cache.LockWorkNet)//用缓存的网络设备进行操作
                        {
                            CacheNetWork curnet = Cache.LstWorkNet.Find(delegate (CacheNetWork p) { return p.IP == net.IP; });
                            if (curnet != null)
                            {
                                curnet.State = 3;
                            }
                        }
                    }
                    else
                    {
                        command.IsSuccess = false;
                        command.errorMessage = string.Format("交换机回发错误：{0}-{1}-{2}-{3}", data[0], data[1], data[2], data.Length);
                        return command;
                    }
                }
                else
                {
                    if ((data[0] != 0x3E) && (data[1] != 0xE3) && (data[2] != 0x90) && (data[3] != 0x09))
                    {
                        command.IsSuccess = false;
                        command.errorMessage = string.Format("引导符错误：{0}-{1}-{2}-{3}", data[0], data[1], data[2], data[3]);
                        return command;
                    }
                    command.IsSuccess = true;
                    command.PackageType = 1;//表示包类型---data[4]
                    if (data[4] == 252) command.PackageType = 2;//表示为交换机的状态信息数据包
                    lock (Cache.LockWorkNet)//用缓存的网络设备进行操作
                    {
                        CacheNetWork curnet = Cache.LstWorkNet.Find(delegate (CacheNetWork p) { return p.IP == net.IP; });
                        if (curnet != null)
                        {
                            curnet.State = 3;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                command.IsSuccess = false;
                command.errorMessage = ex.Message;
            }
            return command;
        }
    }
}
