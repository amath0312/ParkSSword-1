using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace PWDCore
{
    partial class FrmForgetPWD : Form
    {
        public FrmForgetPWD()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当窗体结束时提示邮件发送成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DoSomethingWhenFormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show("用于找回密码的链接已发送至" + textBox1.Text.Trim() + Environment.NewLine + "链接仅24小时有效", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 启动验证
        /// </summary>
        void DoCheck()
        {
            if (textBox1.Text != "")
            {
                if (Functions.IsEmail(textBox1.Text))
                {
                    Thread oThread = new Thread(new ThreadStart(CheckThread));
                    oThread.IsBackground = true;
                    oThread.Start();
                }
                else
                {
                    label2.Text = "Email格式错误";
                }
            }
            else
            {
                label2.Text = "邮箱不能为空";
            }
        }

        /// <summary>
        /// 验证线程
        /// </summary>
        void CheckThread()
        {
            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;

                label2.Text = "正在发送请求...";

                textBox1.Enabled = false;
                button2.Enabled = false;

                string Src = Functions.GetSource("http://parkssword.sinaapp.com/GetbackAccount.php?Email=" + textBox1.Text.Trim());

                //更换
                if (Src.Trim() == "SUCC!")
                {
                    this.FormClosed += DoSomethingWhenFormClosed;
                    this.Close();
                }
                else if (Src.Trim().Contains("NOT Email!"))
                {
                    throw new Exception("错误的邮箱格式");
                }
                else if (Src.Trim().Contains("IP Limited!"))
                {
                    throw new Exception("您的IP操作太频繁,请稍后再试");
                }
                else if (Src.Trim().Contains("Not SignUp"))
                {
                    //帐号不存在
                    throw new Exception("帐号不存在");
                }
                else if (Src.Trim().Contains("Failed!"))
                {
                    throw new Exception("邮件发送失败，内部错误");
                }
                else
                {
                    throw new Exception("未知错误");
                }
            }
            catch (Exception E)
            {
                label2.Text = E.Message;
            }
            finally
            {
                textBox1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DoCheck();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Visible = textBox1.Text.Trim() != string.Empty ? false : true;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DoCheck();
            }
        }
    }
}
