using Microsoft.Win32;
using PWDCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

            ReloadSettingsAndUnits();

            //加载Change事件
            numericUpDown1.ValueChanged += ChangedChanged;
            numericUpDown3.ValueChanged += ChangedChanged;
            checkBox1.CheckedChanged += ChangedChanged;
            checkBox2.CheckedChanged += ChangedChanged;
            checkBox3.CheckedChanged += ChangedChanged;
            checkBox6.CheckedChanged += ChangedChanged;
            checkBox8.CheckedChanged += ChangedChanged;
            checkBox9.CheckedChanged += ChangedChanged;

            button_CheckUpdate.Click += delegate(object sender, EventArgs e)
            {
                CheckUpdateAndLoginS SS = new CheckUpdateAndLoginS(ProductVersion.ToString());
                SS.LoginS();
                SS.CheckUpdate(button_CheckUpdate);
            };
            textBox_Email.TextChanged += delegate(object sender, EventArgs e) { label_Email.Visible = textBox_Email.Text == "" ? true : false; };
            textBox_Pass.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (textBox_Email.Text != "" && textBox_Pass.Text != "")
                    {
                        if (Functions.IsEmail(textBox_Email.Text))
                        {
                            DoSynchronizationThread();
                        }
                        else
                        {
                            label_BBS.Text = "Email格式错误";
                        }
                    }
                    else
                    {
                        label_BBS.Text = "请完整输入用户名和密码";
                    }
                }
            };
            textBox_Pass.TextChanged += delegate(object sender, EventArgs e) { label_Pass.Visible = textBox_Pass.Text == "" ? true : false; };

            button_Extend.Click += delegate(object sender, EventArgs e)
            {
                splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
                panel2.Enabled = !panel2.Enabled;
                button_Extend.Text = button_Extend.Text == "<" ? ">" : "<";
            };

            tabControl1.SelectedIndexChanged += delegate(object sender, EventArgs e)
            {
                if (tabControl1.SelectedIndex == 2)
                {
                    if (textBox_Email.Text == "")
                    {
                        textBox_Email.Focus();
                    }
                    else
                    {
                        textBox_Pass.Focus();
                    }
                }
            };

            button_SignUp.Click += delegate(object sender, EventArgs e) { FrmSignUp Fsu = new FrmSignUp(); Fsu.ShowDialog(); };
            button_ForgetPWD.Click += delegate(object sender, EventArgs e) { FrmForgetPWD FfPWD = new FrmForgetPWD(); FfPWD.ShowDialog(); };

            textBox_Email.Text = Functions.Setting_Default.LoginEmail;

            label_Hard.Text = "[CPUID:" + Functions.CPUCodeStr + " HardID:" + Functions.HardCodeStr + " USBID:" + Functions.USBCodeStr + "]";
        }

        private void ReloadSettingsAndUnits()
        {
            //加载设定
            FreshDefaultSettingButtons();

            //加载手动列表
            FreshListview();
        }

        private void FrmSetting_Load(object sender, EventArgs e)
        {
            if (textBox1.Text.IndexOf(this.ProductVersion) == -1)
            {
                textBox1.AppendText(Environment.NewLine + "Version:" + this.ProductVersion);
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
                Lv.SubItems.Add(Functions.ManualItemsLst[i].TimeStamp);
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
                FM.TimeStamp = listView1.Items[i].SubItems[7].Text;
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

            Functions.Setting_Default.LoginEmail = textBox_Email.Text;

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
        /// 执行同步线程
        /// </summary>
        void DoSynchronizationThread()
        {
            if (textBox_Email.Text.Trim() == "") throw new Exception("请输入帐号");
            if (textBox_Pass.Text == "") throw new Exception("请输入密码");

            Functions.Setting_Default.LoginEmail = textBox_Email.Text;
            Thread oThread = new Thread(new ThreadStart(SynchronizationThread));
            oThread.IsBackground = true;
            oThread.Start();
        }

        //转换列表为String格式用于Post
        public static string LoadManualItems2StrData()
        {
            StringBuilder SB = new StringBuilder("#Setting#@" + Functions.Setting_Default.TimeStamp
                    + "#@#" + Functions.Setting_Default.Length + "#@#" + Functions.Setting_Default.MD5Times
                    + "#@#" + Functions.Setting_Default.LockCPU
                    + "#@#" + Functions.Setting_Default.LockHard
                    + "#@#" + Functions.Setting_Default.LockUSB
                    + "@#Setting#");

            List<Functions.ManualItems> Lst2Str = Functions.ManualItemsLst;
            foreach (Functions.ManualItems Item in Lst2Str)
            {
                SB.Append("#Unit#@" + Item.Domin + "#@#" + Item.TimeStamp + "#@#" + Item.PWDPool + "#@#" + Item.Length + "#@#" + Item.MD5Times + "#@#" + Item.LockCPU + "#@#" + Item.LockHard + "#@#" + Item.LockUSB + "@#Unit#");
            }

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

                label_BBS.Text = "正在同步...";

                textBox_Email.Enabled = false;
                textBox_Pass.Enabled = false;

                #region 提交POST请求
                string Src = string.Empty;
                string PostData = LoadManualItems2StrData();

                byte[] byteArray = Encoding.GetEncoding("gb2312").GetBytes(PostData);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri("http://pss.codeeer.com/StrFresh.php?Email=" + textBox_Email.Text + "&PassWord=" + MD5.PasswordFormat(textBox_Pass.Text) + "&DeviceID=Windows:" + Functions.CPUCodeStr));

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

                //处理从服务器下载的数据
                if (!Src.Contains("#Head#") || !Src.Contains("#Tail#")) throw new Exception("数据校验失败，请重试！");

                if (Src.Contains("SUCC!"))
                {
                    Regex pattern_Unit = new Regex("#Unit#@(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)@#Unit#");
                    MatchCollection matcher_Unit = pattern_Unit.Matches(Src);

                    Regex pattern_Setting = new Regex("#Setting#@(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)@#Setting#");
                    MatchCollection matcher_Setting = pattern_Setting.Matches(Src);

                    if (matcher_Setting.Count == 1)
                    {
                        Functions.Setting_Default.TimeStamp = matcher_Setting[0].Groups[1].Value;
                        Functions.Setting_Default.Length = Convert.ToInt16(matcher_Setting[0].Groups[2].Value);
                        Functions.Setting_Default.MD5Times = Convert.ToInt16(matcher_Setting[0].Groups[3].Value);
                        Functions.Setting_Default.LockCPU = Convert.ToBoolean(matcher_Setting[0].Groups[4].Value);
                        Functions.Setting_Default.LockHard = Convert.ToBoolean(matcher_Setting[0].Groups[5].Value);
                        Functions.Setting_Default.LockUSB = Convert.ToBoolean(matcher_Setting[0].Groups[6].Value);
                    }
                    else
                    {
                        throw new Exception("配置参数获取失败，请重试！");
                    }

                    Functions.ManualItemsLst.Clear();

                    foreach(Match Mc in matcher_Unit)
                    {
                        //写入Functions.ManualItemsLst列表
                        Functions.ManualItems FM = new Functions.ManualItems();
                        FM.Domin = Mc.Groups[1].Value;
                        FM.TimeStamp = Mc.Groups[2].Value;
                        FM.PWDPool = Mc.Groups[3].Value;
                        FM.Length = Convert.ToInt16(Mc.Groups[4].Value);
                        FM.MD5Times = Convert.ToInt16(Mc.Groups[5].Value);
                        FM.LockCPU = Convert.ToBoolean(Mc.Groups[6].Value);
                        FM.LockHard = Convert.ToBoolean(Mc.Groups[7].Value);
                        FM.LockUSB = Convert.ToBoolean(Mc.Groups[8].Value);

                        Functions.ManualItemsLst.Add(FM);
                    }

                    label_BBS.Text = "同步成功 " + DateTime.Now.ToString("HH:mm:ss");
                }
                else if (Src.IndexOf("Wrong UserName or Password!") == 0)
                {
                    throw new Exception("用户名或密码错误");
                }
                else
                {
                    throw new Exception("数据同步失败（" + Src.Length + "）");
                }

                SaveSettingS(false);

                ReloadSettingsAndUnits();
            }
            catch (Exception E)
            {
                label_BBS.Text = E.Message;
            }
            finally
            {
                textBox_Email.Enabled = true;
                textBox_Pass.Enabled = true;
                textBox_Pass.Text = "";
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
