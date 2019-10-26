
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Driver.Commands
{
    /// <summary>
    /// 向下（对设备）命令基类
    /// </summary>
    public abstract class DownlinkCommand : Sys.DataCollection.Common.Commands.MasCommand
    {
        /// <summary>
        /// 表示命令版本，即=13表示kj306-f(16)h智能分站；=1表示大分站；=14表示抽放分站；
        /// </summary>
        public short OrderVersion;
        public abstract byte[] ToBytes();
    }
}
