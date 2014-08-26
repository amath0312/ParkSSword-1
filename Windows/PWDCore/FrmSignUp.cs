using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace PWDCore
{
    partial class FrmSignUp : Form
    {
        public FrmSignUp()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Visible = textBox1.Text.Trim() != string.Empty ? false : true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label3.Visible = textBox2.Text.Trim() != string.Empty ? false : true;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label4.Visible = textBox3.Text.Trim() != string.Empty ? false : true;
        }

        /// <summary>
        /// 当窗体结束时提示邮件发送成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DoSomethingWhenFormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show("验证邮件已经发送至" + textBox1.Text.Trim() + Environment.NewLine + "请在24小时内完成激活", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 启动验证
        /// </summary>
        void DoCheck()
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                Thread oThread = new Thread(new ThreadStart(CheckThread));
                oThread.IsBackground = true;
                oThread.Start();
            }
            else
            {
                label2.Text = "请完整输入用户名和密码";
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
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                button1.Enabled = false;

                string Src = Functions.GetSource("http://parkssword.sinaapp.com/SignUp.php?Email=" + textBox1.Text.Trim() + "&PassWord=" + MD5.PasswordFormat(textBox2.Text));

                //更换
                if (Src.Trim().Contains("SUCC!"))
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
                else if (Src.Trim().Contains("Exist Email!"))
                {
                    throw new Exception("该Email已存在");
                }
                else if (Src.Trim().Contains("Failed!"))
                {
                    throw new Exception("内部错误");
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
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                button1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                label2.Text = "输入不完整";
            }
            else if (textBox2.Text != textBox3.Text)
            {
                label2.Text = "两次输入的密码不一致";
            }
            else if (!Functions.IsEmail(textBox1.Text))
            {
                label2.Text = "Email格式错误";
            }
            else
            {
                DoCheck();
            }
        }
    }
}
