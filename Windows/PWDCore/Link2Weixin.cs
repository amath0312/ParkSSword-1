using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PWDCore
{
    public partial class Link2Weixin : Form
    {
        public Link2Weixin()
        {
            InitializeComponent();

            if (!Functions.AccountStatu.Weixin)
            {
                //绑定
                textBox_PWD.KeyDown += KeyDown_Event_Bind;
                label2.Text = "输入密码以绑定微信帐号";
            }
            else
            { 
                //解绑
                textBox_PWD.KeyDown += KeyDown_Event_Cancel;
                label2.Text = "输入密码以解绑帐号";
                label2.ForeColor = Color.DarkOrange;
            }
        }

        void KeyDown_Event_Bind(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox_PWD.Text != "")
                {
                    Thread oThread = new Thread(new ParameterizedThreadStart(CheckThread));
                    oThread.IsBackground = true;
                    oThread.Start(true);
                }
                else
                {
                    label2.Text = "请完整输入用户名和密码";
                }
            }
        }

        void KeyDown_Event_Cancel(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox_PWD.Text != "")
                {
                    Thread oThread = new Thread(new ParameterizedThreadStart(CheckThread));
                    oThread.IsBackground = true;
                    oThread.Start(false);
                }
                else
                {
                    label2.Text = "请完整输入用户名和密码";
                }
            }
        }

        /// <summary>
        /// 当窗体结束时提示邮件发送成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DoSomethingWhenFormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show("成功解绑微信帐号", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 验证线程
        /// </summary>
        /// <param name="Para">执行参数[True:绑定;False:取消绑定]</param>
        void CheckThread(object Para)
        {
            //识别执行参数
            bool DoBind = Convert.ToBoolean(Para);

            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;

                textBox_PWD.Enabled = false;
                label2.Text = "验证中";

                string Src = "";

                if (!Functions.AccountStatu.Weixin)
                {
                    //绑定
                    Src = Functions.GetSource("http://parkssword.sinaapp.com/SendVerify.php?Binding_Type=Weixin&Email=" + Functions.EmailID + "&PassWord=" + MD5.PasswordFormat(textBox_PWD.Text));

                    if (Src.Trim().Length == 4)
                    {
                        //识别成功
                        textBox1.Text = "bd=" + Src.Trim();
                        panel_Main.Visible = false;
                    }
                    else if (Src.Trim().Contains("Wrong Password!"))
                    {
                        throw new Exception("密码错误");
                    }
                    else
                    {
                        //登录失败
                        throw new Exception("未知错误");
                    }
                }
                else
                { 
                    //解绑
                    Src = Functions.GetSource("http://parkssword.sinaapp.com/Unbundling.php?Type=Weixin&Email=" + Functions.EmailID + "&PassWord=" + MD5.PasswordFormat(textBox_PWD.Text));

                    if (Src.Trim().Contains("SUCC!"))
                    {
                        this.FormClosed += DoSomethingWhenFormClosed;
                        this.Close();
                    }
                    else if (Src.Trim().Contains("Wrong Email or PWD!"))
                    {
                        throw new Exception("密码错误");
                    }
                    else if (Src.Trim().Contains("Wrong Para!"))
                    {
                        throw new Exception("解绑参数错误");
                    }
                    else
                    {
                        throw new Exception("未知错误");
                    }
                }
            }
            catch (Exception E)
            {
                label2.Text = E.Message;
                label2.ForeColor = SystemColors.ControlText;
                panel_Main.Visible = true;
            }
            finally
            {
                textBox_PWD.Text = "";
                textBox_PWD.Enabled = true;
            }
        }

        private void textBox_PWD_TextChanged(object sender, EventArgs e)
        {
            label3.Visible = textBox_PWD.Text == "" ? true : false;
        }
    }
}
