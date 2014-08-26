using parkssword;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PWDCore
{
    public partial class FrmManualPWD : Form
    {
        FrmSetting FrmS = new FrmSetting();

        string DominName = "";
        int Need2ChangeLv;

        /// <summary>
        /// 添加模式重载
        /// </summary>
        /// <param name="ThisForm">传递this</param>
        /// <param name="Domin">域名</param>
        public FrmManualPWD(FrmSetting ThisForm, string Domin)
        {
            InitializeComponent();
            FrmS = ThisForm;
            this.Text = "[" + Domin + "] 密码策略";
            button1.Text = "添加";

            DominName = Domin;
            FreshCheckButton();

            LoadDefaultSetting();
        }

        /// <summary>
        /// 修改模式重载
        /// </summary>
        /// <param name="ThisForm">传递this</param>
        /// <param name="ChangeLv">需要改变的项</param>
        public FrmManualPWD(FrmSetting ThisForm, ListViewItem ChangeLv)
        {
            InitializeComponent();
            FrmS = ThisForm;
            this.Text = "[" + ChangeLv.Text + "] 密码策略";
            textBox2.Text = ChangeLv.SubItems[1].Text;
            numericUpDown1.Value = Convert.ToInt16(ChangeLv.SubItems[2].Text);
            numericUpDown2.Value = Convert.ToInt16(ChangeLv.SubItems[3].Text);
            checkBox6.Checked = Convert.ToBoolean(ChangeLv.SubItems[4].Text);
            checkBox8.Checked = Convert.ToBoolean(ChangeLv.SubItems[5].Text);
            checkBox9.Checked = Convert.ToBoolean(ChangeLv.SubItems[6].Text);
            button1.Text = "保存";

            DominName = ChangeLv.Text;
            Need2ChangeLv = ChangeLv.Index;
            //FreshCheckButton(); //载入模式不执行刷新
        }

        /// <summary>
        /// 加载默认设定
        /// </summary>
        void LoadDefaultSetting()
        {
            numericUpDown1.Value = Functions.Setting_Manual.Length;
            numericUpDown2.Value = Functions.Setting_Manual.MD5Times;
            checkBox6.Checked = Functions.Setting_Manual.LockCPU;
            checkBox8.Checked = Functions.Setting_Manual.LockHard;
            checkBox9.Checked = Functions.Setting_Manual.LockUSB;
        }

        /// <summary>
        /// 刷新复选框生成结果
        /// </summary>
        void FreshCheckButton()
        {
            textBox2.Text = "";
            if (checkBox1.Checked)
            {
                textBox2.AppendText("0123456789");
            }
            if (checkBox2.Checked)
            {
                textBox2.AppendText("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            }
            if (checkBox3.Checked)
            {
                textBox2.AppendText(("ABCDEFGHIJKLMNOPQRSTUVWXYZ").ToLower());
            }
            if (checkBox4.Checked)
            {
                textBox2.AppendText(@"~!@#$%^&*()");
            }
            if (checkBox5.Checked)
            {
                textBox2.AppendText(@"-=_+[]\{}|;':"",./<>?");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Black;
            FreshCheckButton();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string Temp = textBox2.Text.Trim();

                //密码池不能为空
                if (Temp == string.Empty)
                {
                    throw new Exception("密码池不能为空");
                }

                //写入列表修正
                if (button1.Text == "添加")
                {
                    //写入Functions.ManualItemsLst列表
                    Functions.ManualItems FM = new Functions.ManualItems();
                    FM.Domin = DominName;
                    FM.PWDPool = Temp;
                    FM.Length = Convert.ToInt16(numericUpDown1.Value);
                    FM.MD5Times = Convert.ToInt16(numericUpDown2.Value);
                    FM.LockCPU = checkBox6.Checked;
                    FM.LockHard = checkBox8.Checked;
                    FM.LockUSB = checkBox9.Checked;

                    Functions.ManualItemsLst.Add(FM);

                    FrmS.FreshListview();
                }
                else if (button1.Text == "保存")
                {
                    FrmS.listView1.Items[Need2ChangeLv].Text = DominName;
                    FrmS.listView1.Items[Need2ChangeLv].SubItems[1].Text = Temp;
                    FrmS.listView1.Items[Need2ChangeLv].SubItems[2].Text = numericUpDown1.Value.ToString();
                    FrmS.listView1.Items[Need2ChangeLv].SubItems[3].Text = numericUpDown2.Value.ToString();
                    FrmS.listView1.Items[Need2ChangeLv].SubItems[4].Text = checkBox6.Checked.ToString();
                    FrmS.listView1.Items[Need2ChangeLv].SubItems[5].Text = checkBox8.Checked.ToString();
                    FrmS.listView1.Items[Need2ChangeLv].SubItems[6].Text = checkBox9.Checked.ToString();

                    FrmS.FreshManualItemsLst();
                }
                else
                {
                    throw new Exception("不可预知的错误");
                }

                this.Close();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            button3.ForeColor = Color.DarkGreen;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (textBox2.Text != string.Empty)
            {
                if (checkBox7.Checked) RandTheText();
            }
        }

        /// <summary>
        /// 打乱排列
        /// </summary>
        void RandTheText()
        {
            if (textBox2.Text != string.Empty)
            {
                //随机打乱置后
                string Temp = textBox2.Text;
                Random RD = new Random();
                int TempNum = RD.Next(0, Temp.Length);
                string TempChar = Temp[TempNum].ToString();
                Temp = Temp.Remove(TempNum, 1);
                Temp = Temp + TempChar;

                //半数打乱
                if (Temp.Length > 2)
                {
                    string T1 = Temp.Substring(2, Temp.Length / 2);
                    Temp = Temp.Replace(T1, "");
                    Temp = T1 + Temp;
                }

                //头置尾
                string C1 = Temp[0].ToString();
                Temp = Temp.Substring(1, Temp.Length - 1) + C1;

                textBox2.Text = Temp;
            }
        }

        private void textBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (checkBox7.Checked) RandTheText();
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            if (checkBox7.Checked) RandTheText();
        }
    }
}
