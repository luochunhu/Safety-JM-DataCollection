using Sys.DataCollection.Common.Protocols;
using Sys.DataCollection.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace Sys.DataCollection.ConsoleHost
{
    public partial class simulationFrm : Form
    {
        public simulationFrm()
        {
            InitializeComponent();
        }
        Thread thdd;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte fzh = 0;
            List<DeviceInfo> items = null;
            if(comboBox1.SelectedIndex>=0)
            {
                fzh = Convert.ToByte(comboBox1.Text.Substring(0, comboBox1.Text.IndexOf('.')));
                items= GatewayManager.CacheManager.Query<DeviceInfo>(p => p.Fzh == fzh && (p.DevPropertyID == 2 || p.DevPropertyID == 0 || p.DevPropertyID == 1 || p.DevPropertyID == 3));
                dataGridView1.Rows.Clear();
                for (int i = 0; i < items.Count; i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = items[i].Point;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = items[i].Wz;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Value = items[i].DevName;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[3].Value = items[i].Ssz;
                }
            }
        }

        private void simulationFrm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(500);
            }
            List<DeviceInfo> items = null;
            items = GatewayManager.CacheManager.Query<DeviceInfo>(p =>  p.DevPropertyID == 0);
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    comboBox1.Items.Add(items[i].Fzh.ToString() + "." + items[i].Point + "【" + items[i].Wz + "-" + items[i].DevName + "】");
                }
            }
            Column5.Items.Add("通讯中断"); 
            Column5.Items.Add("通讯误码");
            Column5.Items.Add("初始化中");
            Column5.Items.Add("交流正常");
            Column5.Items.Add("直流正常");

            Column5.Items.Add("红外遥控");
            Column5.Items.Add("设备正常");
            Column5.Items.Add("设备标校");
            Column5.Items.Add("开机中");
            Column5.Items.Add("头子断线");
            Column5.Items.Add("类型有误");
            thdd = new Thread(new ThreadStart(SendMessage));
            thdd.Start();
        }
        private void SendMessage()
        {
            for(;;)
            {

                Thread.Sleep(100);
            }
        }
    }
}
