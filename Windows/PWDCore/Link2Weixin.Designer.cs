namespace PWDCore
{
    partial class Link2Weixin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Link2Weixin));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel_Main = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_PWD = new System.Windows.Forms.TextBox();
            this.panel_Bind = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel_Main.SuspendLayout();
            this.panel_Bind.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(19, 50);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(141, 131);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panel_Main
            // 
            this.panel_Main.Controls.Add(this.label2);
            this.panel_Main.Controls.Add(this.label3);
            this.panel_Main.Controls.Add(this.textBox_PWD);
            this.panel_Main.Location = new System.Drawing.Point(12, 12);
            this.panel_Main.Name = "panel_Main";
            this.panel_Main.Size = new System.Drawing.Size(308, 200);
            this.panel_Main.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(26, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(241, 46);
            this.label2.TabIndex = 8;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Window;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label3.Location = new System.Drawing.Point(136, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 21);
            this.label3.TabIndex = 7;
            this.label3.Text = "验证PSS登录密码";
            // 
            // textBox_PWD
            // 
            this.textBox_PWD.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_PWD.ForeColor = System.Drawing.Color.DarkGreen;
            this.textBox_PWD.Location = new System.Drawing.Point(26, 71);
            this.textBox_PWD.Name = "textBox_PWD";
            this.textBox_PWD.PasswordChar = '●';
            this.textBox_PWD.Size = new System.Drawing.Size(247, 31);
            this.textBox_PWD.TabIndex = 6;
            this.textBox_PWD.TextChanged += new System.EventHandler(this.textBox_PWD_TextChanged);
            // 
            // panel_Bind
            // 
            this.panel_Bind.Controls.Add(this.textBox1);
            this.panel_Bind.Controls.Add(this.label1);
            this.panel_Bind.Controls.Add(this.pictureBox1);
            this.panel_Bind.Location = new System.Drawing.Point(12, 12);
            this.panel_Bind.Name = "panel_Bind";
            this.panel_Bind.Size = new System.Drawing.Size(308, 201);
            this.panel_Bind.TabIndex = 10;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.DarkGreen;
            this.textBox1.Location = new System.Drawing.Point(176, 100);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(115, 31);
            this.textBox1.TabIndex = 11;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "扫描二维码添加我并输入以下验证信息";
            // 
            // Link2Weixin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 225);
            this.Controls.Add(this.panel_Main);
            this.Controls.Add(this.panel_Bind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Link2Weixin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "绑定/解绑微信";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel_Main.ResumeLayout(false);
            this.panel_Main.PerformLayout();
            this.panel_Bind.ResumeLayout(false);
            this.panel_Bind.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel_Main;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_PWD;
        private System.Windows.Forms.Panel panel_Bind;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}