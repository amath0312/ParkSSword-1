namespace parkssword
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.MixStringCheck = new System.Windows.Forms.Timer(this.components);
            this.textBox_Input = new System.Windows.Forms.TextBox();
            this.CheckFocus = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MixStringCheck
            // 
            this.MixStringCheck.Enabled = true;
            this.MixStringCheck.Tick += new System.EventHandler(this.MixStringCheck_Tick);
            // 
            // textBox_Input
            // 
            this.textBox_Input.Location = new System.Drawing.Point(3, 3);
            this.textBox_Input.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox_Input.Name = "textBox_Input";
            this.textBox_Input.PasswordChar = '*';
            this.textBox_Input.Size = new System.Drawing.Size(127, 22);
            this.textBox_Input.TabIndex = 2;
            this.textBox_Input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_StarInput_KeyDown);
            // 
            // CheckFocus
            // 
            this.CheckFocus.Enabled = true;
            this.CheckFocus.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(134, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(21, 21);
            this.button1.TabIndex = 3;
            this.button1.Text = ".";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(159, 28);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_Input);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ParkSSword";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer MixStringCheck;
        private System.Windows.Forms.TextBox textBox_Input;
        private System.Windows.Forms.Timer CheckFocus;
        private System.Windows.Forms.Button button1;
    }
}

