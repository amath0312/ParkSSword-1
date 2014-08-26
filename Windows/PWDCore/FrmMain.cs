using Microsoft.Win32;
using PWDCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace parkssword
{
    public partial class FrmSetting : Form
    {
        public FrmSetting()
        {
            InitializeComponent();

            //加载设定
            FreshDefaultSettingButtons();

            //加载手动列表
            FreshListview();

            //加载Change事件
            numericUpDown1.ValueChanged += ChangedChanged;
            numericUpDown3.ValueChanged += ChangedChanged;
            checkBox1.CheckedChanged += ChangedChanged;
            checkBox2.CheckedChanged += ChangedChanged;
            checkBox3.CheckedChanged += ChangedChanged;
            checkBox6.CheckedChanged += ChangedChanged;
            checkBox8.CheckedChanged += ChangedChanged;
            checkBox9.CheckedChanged += ChangedChanged;
        }

        private void FrmSetting_Load(object sender, EventArgs e)
        {
            if (textBox1.Text.IndexOf(this.ProductVersion) == -1)
            {
                textBox1.AppendText(Environment.NewLine + "Version:" + this.ProductVersion);
                Functions.AccountStatu.RefreshDone = true;
            }
        }

        /// <summary>
        /// 根据ManualItemsLst刷新默认设定选项
        /// </summary>
        public void FreshDefaultSettingButtons()
        {
            numericUpDown1.Value = Functions.Setting_Manual.Length;
            numericUpDown2.Value = Functions.Setting_Manual.MD5Times;
            checkBox6.Checked = Functions.Setting_Manual.LockCPU;
            checkBox8.Checked = Functions.Setting_Manual.LockHard;
            checkBox9.Checked = Functions.Setting_Manual.LockUSB;

            numericUpDown3.Value = Functions.Setting_Default.Length;
            checkBox3.Checked = Functions.Setting_Default.LockCPU;
            checkBox2.Checked = Functions.Setting_Default.LockHard;
            checkBox1.Checked = Functions.Setting_Default.LockUSB;
        }

        /// <summary>
        /// 根据ManualItemsLst刷新Listview列表
        /// </summary>
        public void FreshListview()
        {
            listView1.Items.Clear();

            for (int i = 0; i < Functions.ManualItemsLst.Count; i++)
            {
                //写入Listview列表
                ListViewItem Lv = new ListViewItem();
                Lv.Text = Functions.ManualItemsLst[i].Domin;
                Lv.SubItems.Add(Functions.ManualItemsLst[i].PWDPool);
                Lv.SubItems.Add(Functions.ManualItemsLst[i].Length.ToString());
                Lv.SubItems.Add(Functions.ManualItemsLst[i].MD5Times.ToString());
                Lv.SubItems.Add(Functions.ManualItemsLst[i].LockCPU.ToString());
                Lv.SubItems.Add(Functions.ManualItemsLst[i].LockHard.ToString());
                Lv.SubItems.Add(Functions.ManualItemsLst[i].LockUSB.ToString());
                listView1.Items.Add(Lv);
            }
        }

        /// <summary>
        /// 根据Listview刷新ManualItemsLst
        /// </summary>
        public void FreshManualItemsLst()
        {
            Functions.ManualItemsLst.Clear();

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                //写入ManualItemsLst列表
                Functions.ManualItems FM = new Functions.ManualItems();
                FM.Domin = listView1.Items[i].Text;
                FM.PWDPool = listView1.Items[i].SubItems[1].Text;
                FM.Length = Convert.ToInt16(listView1.Items[i].SubItems[2].Text);
                FM.MD5Times = Convert.ToInt16(listView1.Items[i].SubItems[3].Text);
                FM.LockCPU = Convert.ToBoolean(listView1.Items[i].SubItems[4].Text);
                FM.LockHard = Convert.ToBoolean(listView1.Items[i].SubItems[5].Text);
                FM.LockUSB = Convert.ToBoolean(listView1.Items[i].SubItems[6].Text);
                Functions.ManualItemsLst.Add(FM);
            }
        }

        /// <summary>
        /// 更改条目
        /// </summary>
        /// <param name="ChangeLv">选中的Listview项</param>
        void ItemsChange(ListViewItem ChangeLv)
        {
            FrmManualPWD Frm = new FrmManualPWD(this, ChangeLv);
            Frm.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - Frm.Height;
            Frm.Left = 0;
            Frm.checkBox7.Checked = false;
            if (Frm.ShowDialog() == DialogResult.OK)
            {
                FreshManualItemsLst();
                Functions.SaveCodeBaseXML();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string DominString = iDLL.Forms.InputBox.ShowInputBox("添加", "请输入域名：", toolStripTextBox1.Text);
                if (DominString.Trim() != string.Empty)
                {
                    DominString = DominString.Trim();

                    //检测重复
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (DominString.ToLower() == listView1.Items[i].Text.ToLower())
                        {
                            throw new Exception("输入的域名已经存在");
                        }
                    }

                    FrmManualPWD Frm = new FrmManualPWD(this, DominString);
                    Frm.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - Frm.Height;
                    Frm.Left = 0;
                    Frm.checkBox7.Enabled = false;
                    if (Frm.ShowDialog() == DialogResult.OK)
                    {
                        FreshManualItemsLst();
                        Functions.SaveCodeBaseXML();
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ItemsChange(listView1.SelectedItems[0]);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    DialogResult R = MessageBox.Show("确认删除" + listView1.SelectedItems[i].Text + "?", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (R == DialogResult.Yes)
                    {
                        listView1.Items.RemoveAt(listView1.SelectedItems[i].Index);
                        i--;//删除之后Index少1，所有位置也减1达到平衡
                    }
                }
                FreshManualItemsLst();
                Functions.SaveCodeBaseXML();
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ItemsChange(listView1.SelectedItems[0]);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                //打开【删除】
                toolStripButton3.Enabled = true;

                //打开【修改】
                if (listView1.SelectedItems.Count == 1)
                {
                    toolStripButton2.Enabled = true;
                }
                else
                {
                    toolStripButton2.Enabled = false;
                }
            }
            else
            {
                toolStripButton3.Enabled = false;
                toolStripButton2.Enabled = false;
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string SO = toolStripTextBox1.Text.ToUpper();
                if (SO.Trim() == "") throw new Exception("Nothing to Search");
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i].Text.ToUpper().Contains(SO) || SO.Contains(listView1.Items[i].Text.ToUpper()))
                    {
                        listView1.Items[i].ForeColor = System.Drawing.Color.Blue;
                    }
                    else
                    {
                        if (listView1.Items[i].ForeColor == System.Drawing.Color.Blue)
                        {
                            listView1.Items[i].ForeColor = System.Drawing.Color.Black;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                if (E.Message == "Nothing to Search")
                {
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (listView1.Items[i].ForeColor == System.Drawing.Color.Blue)
                        {
                            listView1.Items[i].ForeColor = System.Drawing.Color.Black;
                        }
                    }
                }
            }
            finally
            {
                int Count = 0;
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i].ForeColor == System.Drawing.Color.Blue)
                    {
                        Count++;
                    }
                }
                toolStripLabel2.Text = Count.ToString();
            }
        }


        /// <summary>
        /// 保存设定
        /// </summary>
        void SaveSettingS(bool UpdateTimeStamp = true)
        {
            Functions.Setting_Manual.Length = Convert.ToInt16(numericUpDown1.Value);
            Functions.Setting_Manual.MD5Times = Convert.ToInt16(numericUpDown2.Value);
            Functions.Setting_Manual.LockCPU = Convert.ToBoolean(checkBox6.Checked);
            Functions.Setting_Manual.LockHard = Convert.ToBoolean(checkBox8.Checked);
            Functions.Setting_Manual.LockUSB = Convert.ToBoolean(checkBox9.Checked);

            Functions.Setting_Default.Length = Convert.ToInt16(numericUpDown3.Value);
            Functions.Setting_Default.LockCPU = Convert.ToBoolean(checkBox3.Checked);
            Functions.Setting_Default.LockHard = Convert.ToBoolean(checkBox2.Checked);
            Functions.Setting_Default.LockUSB = Convert.ToBoolean(checkBox1.Checked);

            Functions.Setting_Default.LoginEmail = Functions.EmailID;

            Functions.SaveCodeBaseXML(UpdateTimeStamp);
        }

        private void button_SaveSetting_Click(object sender, EventArgs e)
        {
            SaveSettingS();
        }

        void ChangedChanged(object sender, EventArgs e)
        {
            button_SaveSetting.Enabled = true;
        }

        private void FrmSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button_SaveSetting.Enabled)
            {
                if (DialogResult.Yes == MessageBox.Show("已经更改了设置，是否保存？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    SaveSettingS();
                }
            }
        }

        /// <summary>
        /// 刷新状态
        /// </summary>
        void RefreshStatu()
        {
            if (Functions.EmailID != "" && Functions.EmailID != null && Functions.Login)
            {
                //判定绑定情况
                bool IsSignUp = Functions.AccountStatu.SignUp;
                bool IsBDWeixin = Functions.AccountStatu.Weixin;

                //加载图片
                pictureBox3.Image = IsSignUp ? imageList1.Images[0] : imageList1.Images[1];
                pictureBox1.Image = IsBDWeixin ? imageList1.Images[0] : imageList1.Images[1];

                //填写按钮文字
                button_BD_Email.Enabled = true;
                if (IsSignUp)
                {
                    //恢复默认
                    button_BD_WeChat.Enabled = true;

                    label8.Text = "已经绑定：" + Functions.EmailID;
                    button_BD_Email.Text = "更换";

                    button_BD_WeChat.Text = IsBDWeixin ? "更换" : "绑定";
                    if (IsBDWeixin) label6.Text = "已经绑定微信帐号";
                }
                else
                {
                    label8.Text = "帐号：" + Functions.EmailID + "尚未激活";
                    button_BD_Email.Text = "激活";
                    button_BD_WeChat.Enabled = false;

                    button_BD_WeChat.Text = "请先激活Email";
                }
            }
            else
            {
                pictureBox3.Image = imageList1.Images[2];
                pictureBox1.Image = imageList1.Images[2];

                label8.Text = "绑定Email号，用以登录云端账号";

                button_BD_Email.Text = "绑定";
                button_BD_WeChat.Text = "绑定";

                button_BD_Email.Enabled = false;
                button_BD_WeChat.Enabled = false;
            }

            panel4.Visible = !Functions.Login;
        }

        /// <summary>
        /// 登录线程
        /// </summary>
        void LoginThread()
        {
            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;

                label9.Text = "正在登录...";

                string Email = textBox2.Text.Trim();
                string PassWord = textBox3.Text.Trim();

                textBox2.Enabled = false;
                textBox3.Enabled = false;

                string Src = Functions.GetSource("http://parkssword.sinaapp.com/Login.php?Email=" + Email + "&PassWord=" + MD5.PasswordFormat(PassWord) + "&CPUID=" + Functions.CPUCodeStr);
                //模拟返回值 -> 测试结束后删除！！
                //string Src = "SUCC![SignUp=1][Weixin=testID][Phone=]";

                //处理PHP返回信息
                if (Src.Contains("SUCC!"))
                {
                    Functions.EmailID = textBox2.Text.Trim();

                    int SignUp_Fst = Src.IndexOf("[SignUp=");
                    int SignUp_Lst = Src.IndexOf("]", SignUp_Fst);
                    string SignUp = "";
                    if (SignUp_Fst != -1 && SignUp_Lst != -1 && SignUp_Fst < SignUp_Lst)
                    {
                        SignUp = Src.Substring(SignUp_Fst + "[SignUp=".Length, SignUp_Lst - SignUp_Fst - "[SignUp=".Length);
                    }
                    Functions.AccountStatu.SignUp = SignUp == "0" ? false : true;

                    int Weixin_Fst = Src.IndexOf("[Weixin=");
                    int Weixin_Lst = Src.IndexOf("]", Weixin_Fst);
                    string Weixin = "";
                    if (Weixin_Fst != -1 && Weixin_Lst != -1 && Weixin_Fst < Weixin_Lst)
                    {
                        Weixin = Src.Substring(Weixin_Fst + "[Weixin=".Length, Weixin_Lst - Weixin_Fst - "[Weixin=".Length).Trim();
                    }
                    Functions.AccountStatu.Weixin = Weixin.Trim() == "" ? false : true;

                    label9.Text = "登录成功";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    Functions.Login = true;
                    button_Refresh.Enabled = true;
                    SaveSettingS(false);
                }
                else if (Src.Contains("Wrong UserName or Password"))
                {
                    //登录失败
                    throw new Exception("错误的用户名或密码");
                }
                else
                {
                    //登录失败
                    throw new Exception("未知错误");
                }
            }
            catch (Exception E)
            {
                Functions.Login = false;
                label9.Text = E.Message;
            }
            finally
            {
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                panel4.Visible = !Functions.Login;

                RefreshStatu();
            }
        }

        /// <summary>
        /// 执行同步线程
        /// </summary>
        void DoSynchronizationThread()
        {
            if (textBox4.Text != "")
            {
                Thread oThread = new Thread(new ThreadStart(SynchronizationThread));
                oThread.IsBackground = true;
                oThread.Start();
            }
            else
            {
                label9.Text = "请输入密码";
            }
        }

        public static String LoadManualItems2StrData()
        {
            StringBuilder SB = new StringBuilder("#Setting#@" + Functions.Setting_Default.TimeStamp
                    + "#@#" + Functions.Setting_Default.Length + "#@#" + Functions.Setting_Default.MD5Times
                    + "#@#" + Functions.Setting_Default.LockCPU
                    + "#@#" + Functions.Setting_Default.LockHard
                    + "#@#" + Functions.Setting_Default.LockUSB
                    + "#@#" + Functions.CPUCodeStr + "#@#" + Functions.HardCodeStr + "#@#" + Functions.USBCodeStr + "@#Setting#");


            return SB.ToString();
        }

        /// <summary>
        /// 同步线程
        /// </summary>
        void SynchronizationThread()
        {
            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;

                label9.Text = "正在同步...";

                button_Synchronization.Enabled = false;
                textBox4.Enabled = false;

                #region 提交POST请求
                string Src = string.Empty;
                string XMLData = string.Empty;
                using (StreamReader SR = new StreamReader(Application.StartupPath + Functions.UserPWDsPath))
                {
                    XMLData = SR.ReadToEnd();
                }

                byte[] byteArray = Encoding.GetEncoding("gb2312").GetBytes(XMLData);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri("http://parkssword.sinaapp.com/XmlFresh.php?Email=" + Functions.EmailID + "&PassWord=" + MD5.PasswordFormat(textBox4.Text) + "&CPUID=" + Functions.CPUCodeStr + "&HardID=" + Functions.HardCodeStr + "&USBID=" + Functions.USBCodeStr));

                webReq.Method = "POST";
                webReq.ContentType = "application/octet-stream";

                webReq.ContentLength = byteArray.Length;
                using (Stream newStream = webReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);
                    using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312")))
                        {
                            Src = sr.ReadToEnd();
                        }
                    }
                }
                #endregion

                //更换
                if (Src.Trim().Contains("SUCC!"))
                {
                    //成功
                    label9.Text = "同步成功 " + DateTime.Now.ToString("HH:mm:ss");
                }
                else if (Src.Trim().Contains("Wrong PassWord!"))
                {
                    throw new Exception("密码错误");
                }
                else if (Src.Trim().Contains("Missing Paras!"))
                {
                    throw new Exception("缺少参数");
                }
                else if (Src.Trim().Contains("Failed!"))
                {
                    throw new Exception("验证失败");
                }
                else
                {
                    throw new Exception("未知错误");
                }
            }
            catch (Exception E)
            {
                label9.Text = E.Message;
            }
            finally
            {
                button_Synchronization.Enabled = true;
                textBox4.Enabled = true;
                textBox4.Text = "";
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
        }

        private void label11_Click(object sender, EventArgs e)
        {
            textBox3.Focus();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label10.Visible = textBox2.Text == "" ? true : false;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label11.Visible = textBox3.Text == "" ? true : false;
        }

        private void button_Extend_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
            panel2.Enabled = !panel2.Enabled;
            button_Extend.Text = button_Extend.Text == "<" ? ">" : "<";
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox2.Text != "" && textBox3.Text != "")
                {
                    if (Functions.IsEmail(textBox2.Text))
                    {
                        Thread oThread = new Thread(new ThreadStart(LoginThread));
                        oThread.IsBackground = true;
                        oThread.Start();
                    }
                    else
                    {
                        label9.Text = "Email格式错误";
                    }
                }
                else
                {
                    label9.Text = "请完整输入用户名和密码";
                }
            }
        }

        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Functions.Login)
            {
                if (tabControl3.SelectedIndex != 0)
                {
                    MessageBox.Show("请先登录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tabControl3.SelectedIndex = 0;
                }
            }
        }

        private void button_CheckUpdate_Click_1(object sender, EventArgs e)
        {
            CheckUpdateAndLoginS SS = new CheckUpdateAndLoginS(ProductVersion.ToString());
            SS.LoginS();
            SS.CheckUpdate(button_CheckUpdate);
        }

        private void button_BD_Email_Click(object sender, EventArgs e)
        {
            Bind_Email BE = new Bind_Email();
            BE.ShowDialog();

            Functions.AccountStatu.Refresh();
            timer_CheckStatu.Enabled = true;
        }

        private void button_BD_WeChat_Click(object sender, EventArgs e)
        {
            Link2Weixin LW = new Link2Weixin();
            LW.ShowDialog();

            Functions.AccountStatu.Refresh();
            timer_CheckStatu.Enabled = true;
        }

        void LogOut(bool Silence = false, bool ClsUser = true)
        {
            Functions.Login = false;
            if (ClsUser) Functions.EmailID = null;
            RefreshStatu();
            button_Refresh.Enabled = false;
            SaveSettingS(false);
            if (!Silence)
            {
                label9.Text = "已经退出登录";
                MessageBox.Show("成功退出登录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button_LogOut_Click(object sender, EventArgs e)
        {
            LogOut();
        }

        private void button_SignUp_Click(object sender, EventArgs e)
        {
            FrmSignUp Fsu = new FrmSignUp();
            Fsu.ShowDialog();
        }

        private void button_ForgetPWD_Click(object sender, EventArgs e)
        {
            FrmForgetPWD FfPWD = new FrmForgetPWD();
            FfPWD.ShowDialog();
        }

        private void timer_CheckStatu_Tick(object sender, EventArgs e)
        {
            if (Functions.AccountStatu.RefreshDone)
            {
                label10.Visible = true;
                label11.Visible = true;
                label9.Text = "状态已经刷新";
                tabControl3.Enabled = true;
                button_Refresh.Enabled = true;
                Functions.Login = !Functions.AccountStatu.Error;
                if (Functions.AccountStatu.Error)
                {
                    label9.Text = Functions.AccountStatu.ErrorStr;
                    LogOut(true, false);
                }

                timer_CheckStatu.Enabled = false;
                RefreshStatu();
            }
            else
            {
                label10.Visible = false;
                label11.Visible = false;
                tabControl3.Enabled = false;
                button_Refresh.Enabled = false;
                label9.Text = "正在刷新状态...";
            }
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            Functions.AccountStatu.Refresh();
            timer_CheckStatu.Enabled = true;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label7.Visible = textBox4.Text.Trim() != string.Empty ? false : true;
        }

        private void button_Synchronization_Click(object sender, EventArgs e)
        {
            DoSynchronizationThread();
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DoSynchronizationThread();
            }
        }

        bool HasLoad = false;

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2 && !HasLoad && Functions.AccountStatu.RefreshDone)
            {
                HasLoad = true;

                //如果已经保留帐号，就直接登录
                if (Functions.Setting_Default.LoginEmail != null) Functions.EmailID = Functions.Setting_Default.LoginEmail;

                if (Functions.EmailID == "" || Functions.EmailID == null)
                {
                    pictureBox1.Image = imageList1.Images[2];
                    pictureBox3.Image = imageList1.Images[2];
                }
                else
                {
                    //如果已经登录，就直接加载细节
                    Functions.AccountStatu.Refresh();
                    button_Refresh.Enabled = true;
                    timer_CheckStatu.Enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// 软件基础信息
    /// 使用：
    /// CheckUpdateAndLoginS SS = new CheckUpdateAndLoginS(ProductVersion.ToString());
    /// SS.LoginS();SS.CheckUpdate();
    /// </summary>
    public class SoftwareInfo
    {
        /// <summary>
        /// 软件ID信息
        /// </summary>
        public string ID { get { return "ParkSSword"; } }

        /// <summary>
        /// 软件主页
        /// </summary>
        public string MainPage { get { return "http://pss.codeeer.com/"; } }

        /// <summary>
        /// 如果静默模式为真，在非发现新版本时不会提示
        /// </summary>
        public bool SilenceUntilFoundNew { get { return false; } }
    }

    /// <summary>
    /// 检查软件更新和发送使用信息
    /// </summary>
    sealed class CheckUpdateAndLoginS : SoftwareInfo
    {
        const string URL_CheckUpdate = "http://check4update.sinaapp.com/CheckVersion.php";
        const string URL_LoginS = "http://check4update.sinaapp.com/SoftLoginS.php";

        Button Button_CheckUpdate;

        /// <summary>
        /// 记录当前版本号
        /// </summary>
        string NowVersion;

        /// <summary>
        /// 检查软件更新和发送使用信息
        /// </summary>
        /// <param name="Version">当前软件版本信息</param>
        public CheckUpdateAndLoginS(string Version)
        {
            NowVersion = Version;
        }

        /// <summary>
        /// 检查软件更新
        /// </summary>
        public void CheckUpdate(Button DoButton = null)
        {
            Button_CheckUpdate = DoButton;
            Control.CheckForIllegalCrossThreadCalls = false;

            Thread oThread = new Thread(new ParameterizedThreadStart(CheckUpdate_Core));
            oThread.IsBackground = true;
            oThread.Start(ID);
        }

        /// <summary>
        /// 发送使用信息
        /// </summary>
        public void LoginS()
        {
            Thread oThread = new Thread(new ParameterizedThreadStart(LoginS_Core));
            oThread.IsBackground = true;
            oThread.SetApartmentState(System.Threading.ApartmentState.STA);
            oThread.Start(ID);
        }

        void CheckUpdate_Core(object Name)
        {
            try
            {
                if (Button_CheckUpdate != null) Button_CheckUpdate.Enabled = false;

                using (WebClient myWebClient = new WebClient())
                {
                    using (Stream myStream = myWebClient.OpenRead(URL_CheckUpdate + "?SoftwareID=" + (string)Name))
                    {
                        using (StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8")))
                        {
                            /*
                             * 返回值1：{最新版本}##{更新日期}##{备注}
                             * 返回值2：Not Exist!
                             */

                            //分隔符
                            string KEY = "##";

                            string Temp = sr.ReadToEnd();
                            int KEYPosition = Temp.IndexOf(KEY);
                            if (KEYPosition != -1)
                            {
                                int Notes_Pos = Temp.IndexOf(KEY, KEYPosition + 1);

                                string Version = Temp.Substring(0, KEYPosition);
                                string UpdateTime = Notes_Pos == -1 ? Temp.Substring(KEYPosition + KEY.Length, Temp.Length - KEYPosition - KEY.Length) : Temp.Substring(KEYPosition + KEY.Length, Notes_Pos - KEYPosition - KEY.Length);

                                string Notes = Notes_Pos == -1 ? "（暂无）" : Temp.Substring(Notes_Pos + KEY.Length, Temp.Length - Notes_Pos - KEY.Length);

                                //比较版本号
                                Version v1 = new Version(NowVersion), v2 = new Version(Version);
                                if (v1 < v2)
                                {
                                    if (DialogResult.OK == MessageBox.Show("当前版本：" + v1 + Environment.NewLine + "最新版本：" + v2 + Environment.NewLine + "更新日期：" + UpdateTime + Environment.NewLine + "更新说明：" + Notes + Environment.NewLine + "是否打开软件主页？", "发现新版本！", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
                                    {
                                        Process process = new Process();
                                        process.StartInfo.FileName = "cmd.exe";
                                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                        process.StartInfo.Arguments = "/c start " + MainPage;
                                        process.Start();
                                    }
                                }
                                else
                                {
                                    if (!SilenceUntilFoundNew) throw new Exception("已经是最新版本");
                                }
                            }
                            else
                            {
                                if (!SilenceUntilFoundNew) throw new Exception("检查更新失败");
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (Button_CheckUpdate != null) Button_CheckUpdate.Enabled = true;
            }
        }

        void LoginS_Core(object Name)
        {
            try
            {
                //获取系统信息
                RegistryKey Key1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                RegistryKey Key2 = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                RegistryKey Key3 = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");
                StringBuilder SB = new StringBuilder();
                SB.AppendLine(Key1.GetValue("ProductName").ToString());
                SB.AppendLine(" " + Key1.GetValue("CurrentVersion") + "." + Key1.GetValue("CurrentBuildNumber"));
                SB.AppendLine(" " + Key1.GetValue("ProductId"));
                SB.AppendLine(" " + Key1.GetValue("RegisteredOwner"));
                SB.AppendLine(" " + Key2.GetValue("ProcessorNameString"));
                SB.AppendLine(" " + Key2.GetValue("~MHz") + " MHz");
                SB.AppendLine(" " + Key3.GetValue("BaseBoardManufacturer") + "." + Key3.GetValue("BaseBoardProduct"));

                string Data = SB.ToString();
                if (Data.Length > 255) Data = Data.Substring(0, 255);

                //加密发送
                byte[] encData_byte = new byte[Data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(Data);
                string encodedData = Convert.ToBase64String(encData_byte);
                Console.WriteLine(encodedData);

                WebBrowser WB = new WebBrowser();
                WB.Navigate(URL_LoginS + "?SoftwareID=" + (string)Name + "&Info=" + encodedData.Replace('+', '-').Replace('/', '_').Replace('=', '*'));
            }
            catch
            {
            }
        }
    }
}
