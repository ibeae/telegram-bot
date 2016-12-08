namespace telegram_app
{
    partial class mini_aktivasi
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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btn_gettoken = new System.Windows.Forms.Button();
            this.group_token = new System.Windows.Forms.GroupBox();
            this.txt_hp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_pin = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_agenid = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.txt_token = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_status = new System.Windows.Forms.Label();
            this.group_token.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(50, 125);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(117, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "sudah punya token";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // btn_gettoken
            // 
            this.btn_gettoken.Location = new System.Drawing.Point(50, 92);
            this.btn_gettoken.Name = "btn_gettoken";
            this.btn_gettoken.Size = new System.Drawing.Size(83, 27);
            this.btn_gettoken.TabIndex = 7;
            this.btn_gettoken.Text = "Get Token";
            this.btn_gettoken.UseVisualStyleBackColor = true;
            this.btn_gettoken.Click += new System.EventHandler(this.btn_gettoken_Click);
            // 
            // group_token
            // 
            this.group_token.Controls.Add(this.txt_hp);
            this.group_token.Controls.Add(this.btn_gettoken);
            this.group_token.Controls.Add(this.checkBox1);
            this.group_token.Controls.Add(this.label4);
            this.group_token.Controls.Add(this.txt_pin);
            this.group_token.Controls.Add(this.label3);
            this.group_token.Controls.Add(this.txt_agenid);
            this.group_token.Controls.Add(this.label2);
            this.group_token.Controls.Add(this.button2);
            this.group_token.Controls.Add(this.txt_token);
            this.group_token.Controls.Add(this.label1);
            this.group_token.Location = new System.Drawing.Point(9, 5);
            this.group_token.Name = "group_token";
            this.group_token.Size = new System.Drawing.Size(199, 225);
            this.group_token.TabIndex = 3;
            this.group_token.TabStop = false;
            // 
            // txt_hp
            // 
            this.txt_hp.Location = new System.Drawing.Point(50, 40);
            this.txt_hp.Name = "txt_hp";
            this.txt_hp.Size = new System.Drawing.Size(143, 20);
            this.txt_hp.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "HP";
            // 
            // txt_pin
            // 
            this.txt_pin.Location = new System.Drawing.Point(50, 66);
            this.txt_pin.Name = "txt_pin";
            this.txt_pin.Size = new System.Drawing.Size(143, 20);
            this.txt_pin.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Pin";
            // 
            // txt_agenid
            // 
            this.txt_agenid.Location = new System.Drawing.Point(50, 14);
            this.txt_agenid.Name = "txt_agenid";
            this.txt_agenid.Size = new System.Drawing.Size(143, 20);
            this.txt_agenid.TabIndex = 4;
            this.txt_agenid.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_agenid_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "AgenID";
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(50, 180);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 27);
            this.button2.TabIndex = 9;
            this.button2.Text = "Confirm";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txt_token
            // 
            this.txt_token.Enabled = false;
            this.txt_token.Location = new System.Drawing.Point(50, 154);
            this.txt_token.Name = "txt_token";
            this.txt_token.Size = new System.Drawing.Size(143, 20);
            this.txt_token.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Token";
            // 
            // lbl_status
            // 
            this.lbl_status.ForeColor = System.Drawing.Color.Maroon;
            this.lbl_status.Location = new System.Drawing.Point(6, 233);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(214, 51);
            this.lbl_status.TabIndex = 4;
            this.lbl_status.Text = "-";
            // 
            // mini_aktivasi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 236);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.group_token);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "mini_aktivasi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Aktivasi";
            this.Load += new System.EventHandler(this.mini_aktivasi_Load);
            this.group_token.ResumeLayout(false);
            this.group_token.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btn_gettoken;
        private System.Windows.Forms.GroupBox group_token;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txt_token;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_hp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_pin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_agenid;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_status;
    }
}