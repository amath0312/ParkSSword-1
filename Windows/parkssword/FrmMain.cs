using PWDCore;
using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace parkssword
{
    public partial class FrmMain : Form
    {
        public FrmMain(string[] args)
        {
            /**
             * 调试的命令行参数传递
             * [debug]cpuid=11111FFFFF00000A&hardid=AAABBCCD
             */

            //调试模式
            if (args.Length == 1 && args[0].IndexOf("[debug]") != -1)
            {
                Functions.PickArgsStr(args[0].Substring(args[0].IndexOf("cpuid="), args[0].IndexOf("&") - args[0].IndexOf("cpuid=")), ref Functions.PreDefine_CPUCodeStr, "cpuid=");
                Functions.PickArgsStr(args[0].Substring(args[0].IndexOf("hardid="), args[0].Length - args[0].IndexOf("hardid=")), ref Functions.PreDefine_HardCodeStr, "hardid=");
            }
            else
            {
                //提取参数ID并赋值
                foreach (string Str in args)
                {
                    Functions.PickArgsStr(Str, ref Functions.PreDefine_CPUCodeStr, "CPUID=");
                    Functions.PickArgsStr(Str, ref Functions.PreDefine_HardCodeStr, "HARDID=");
                }
            }

            //预定义错误参数规避
            if (Functions.PreDefine_CPUCodeStr != null && Functions.PreDefine_CPUCodeStr.Length != 16) Functions.PreDefine_CPUCodeStr = null;
            if (Functions.PreDefine_HardCodeStr != null && Functions.PreDefine_HardCodeStr.Length != 8) Functions.PreDefine_HardCodeStr = null;

            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //设置停靠屏幕左下角
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 10;
            this.Left = 10;

            try
            {
                //获取CPU密钥数据
                Thread oThread = new Thread(new ThreadStart(Functions.GetCPUKeysAndLoadXML));
                oThread.Start();

                //获取Hard密钥数据
                Functions.GetHardKeys();

                if (Functions.HardCodeStr == "") throw new Exception("硬件编码获取失败");
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
            }
        }

        private void MixStringCheck_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Functions.CPUStrDone)
                {
                    MixStringCheck.Enabled = false;
                    this.Text = "ParkSSword";
                    button1.Enabled = true;
                    if (Functions.CPUCodeStr == "")
                    {
                        throw new Exception("CPU数据获取失败，程序即将终止");
                    }
                    //textBox_Input.Focus();
                }
                else
                {
                    this.Text += ".";
                    if (this.Text.Length > 10)
                    {
                        this.Text = ".";
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
            }
        }

        private void textBox_StarInput_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)//按回车键copy结果
                {
                    textBox_Input.Enabled = false;

                    while (!Functions.CPUStrDone)
                    {
                        Functions.Delay(1);
                    }

                    string SourceInput = textBox_Input.Text.ToLower().Trim();//原始输入数据

                    //过滤非域名信息
                    string Source_Temp = SourceInput;
                    string[] Domin_Name = { ".com.cn", ".com", ".cn", ".cc", ".me", ".tv", ".tf", ".edu", ".gov", ".net", ".org" };
                    int Domin_Check_Path = -1;
                    string Domin_Check_Name = "";
                    foreach (string S in Domin_Name)
                    {
                        Domin_Check_Path = SourceInput.IndexOf(S);

                        if (Domin_Check_Path != -1)
                        {
                            Domin_Check_Name = S;
                            Source_Temp = Source_Temp.Substring(0, Domin_Check_Path);
                            break;
                        }
                    }
                    if (Domin_Check_Path != -1)
                    {
                        //指定域名前用于识别的部分
                        string[] Domin_CheckStop = { ".", "//" };
                        int Domin_CheckStop_Path = -1;
                        string Domin_CheckStop_Name = "";
                        foreach (string B in Domin_CheckStop)
                        {
                            Domin_CheckStop_Path = Source_Temp.LastIndexOf(B);

                            if (Domin_CheckStop_Path != -1)
                            {
                                Domin_CheckStop_Name = B;
                                break;
                            }
                        }

                        if (Domin_CheckStop_Path != -1)
                        {
                            Source_Temp = Source_Temp.Substring(Domin_CheckStop_Path + Domin_CheckStop_Name.Length, Domin_Check_Path - Domin_CheckStop_Path - Domin_CheckStop_Name.Length) + Domin_Check_Name;
                            SourceInput = Source_Temp;
                        }
                    }

                    string PWD = "";
                    Clipboard.Clear();

                    PWD = Functions.CreatCore(SourceInput);
                    Clipboard.SetText(PWD);
                    Console.WriteLine(Functions.CPUCodeStr);
                    this.Close();
                }
                else if ((Control.ModifierKeys & Keys.Shift) != 0)//shift 显示原文
                {
                    if (textBox_Input.PasswordChar == '*')
                    {
                        textBox_Input.PasswordChar = new char();
                    }
                    else
                    {
                        textBox_Input.PasswordChar = '*';
                    }
                }
            }
            catch (Exception E)
            {
                textBox_Input.Enabled = true;

                switch (E.Message)
                {
                    case "读取USB序列号失败": textBox_Input.Clear(); textBox_Input.Focus(); break;
                    default: MessageBox.Show(E.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); break;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!textBox_Input.Focused)
            {
                button1.Text = button1.Text == "." ? "" : ".";
            }
            else
            {
                button1.Text = ".";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //设置停靠屏幕左下角
            FrmSetting SettingForm = new FrmSetting();
            SettingForm.Top = Screen.PrimaryScreen.WorkingArea.Height - SettingForm.Height;
            SettingForm.Left = 0;
            SettingForm.ShowDialog();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                System.Environment.Exit(0);
            }
            catch
            {
            }
        }
    }
}
