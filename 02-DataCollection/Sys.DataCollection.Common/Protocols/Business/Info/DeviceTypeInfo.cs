using Basic.Framework.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Protocols
{
    public partial class DeviceTypeInfo
    {
        public InfoState InfoState { get; set; }

        protected string _DevProperty;

        protected string _DevClass;

        protected string _DevModel;

        /// <summary>
        /// 设备性质名称
        /// </summary>
        public string DevProperty
        {
            get { return _DevProperty; }
            set
            {
                if (value != null && value.Length > 50)
                    throw new ArgumentOutOfRangeException("此属性的值长度过长 DevProperty", value, value.ToString());
                _DevProperty = value;
            }
        }

        /// <summary>
        /// 设备种类名称
        /// </summary>
        public string DevClass
        {
            get { return _DevClass; }
            set
            {
                if (value != null && value.Length > 50)
                    throw new ArgumentOutOfRangeException("此属性的值长度过长 DevClass", value, value.ToString());
                _DevClass = value;
            }
        }

        /// <summary>
        /// 设备型号名称
        /// </summary>
        public string DevModel
        {
            get { return _DevModel; }
            set
            {
                if (value != null && value.Length > 50)
                    throw new ArgumentOutOfRangeException("此属性的值长度过长 DevModel", value, value.ToString());
                _DevModel = value;
            }
        }
    }
}
