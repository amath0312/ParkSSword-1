using System;
using System.IO;
using System.Windows.Forms;

namespace PWDCore
{
    public partial class USBSelect : Form
    {
        public USBSelect()
        {
            InitializeComponent();
            if (!LoadUSBLst(true))
            {
                this.ShowDialog();
            }
        }

        /// <summary>
        /// 启动移动设备加载序列
        /// 如果返回True，表明运行完毕
        /// </summary>
        /// <param name="AutoExitWhenChooseONE">如果检测USB数量为一个，则无需验证直接选中并退出</param>
        /// <returns></returns>
        bool LoadUSBLst(bool AutoExitWhenChooseONE)
        {
            //初始化设定
            listView1.Items.Clear();
            button1.Text = "请选择USB设备";
            button1.Enabled = false;

            //加载设备
            DriveInfo[] myAllDrives = DriveInfo.GetDrives();
            foreach (DriveInfo myDrive in myAllDrives)
            {
                if (myDrive.IsReady)
                {
                    if (myDrive.DriveType == DriveType.Removable)
                    {
                        ListViewItem Lv = new ListViewItem();
                        Lv.Text = myDrive.Name;
                        Lv.SubItems.Add(myDrive.VolumeLabel);
                        Lv.SubItems.Add((myDrive.TotalSize / 1024 / 1024 / 1024 + 1).ToString());

                        listView1.Items.Add(Lv);
                    }
                }
            }

            if (AutoExitWhenChooseONE)
            {
                if (listView1.Items.Count == 1)
                {
                    //如果为单一设备，则直接设定并退出
                    Functions.USBCodeStr = Functions.ReadUSBInfo(listView1.Items[0].SubItems[0].Text);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Functions.USBCodeStr = Functions.ReadUSBInfo(listView1.Items[0].SubItems[0].Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadUSBLst(false);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                button1.Text = "选择 -> " + listView1.SelectedItems[0].SubItems[1].Text;
                button1.Enabled = true;
            }
            else
            {
                button1.Text = "请选择USB设备";
                button1.Enabled = false;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            Functions.USBCodeStr = Functions.ReadUSBInfo(listView1.Items[0].SubItems[0].Text);
            this.Close();
        }
    }
}
