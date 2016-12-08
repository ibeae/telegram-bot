using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace telegram_app
{
    public partial class login : Form
    {
        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public login()
        {
            InitializeComponent();
        }
        private void tg_OnLoginFailed(string data)
        {
          
            button1.Enabled = true;
            MessageBox.Show(data.ToString(), "Error...login", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }        
       
        private void Form1_Load(object sender, EventArgs e)
        {
           // reg.LoadAllSettings(Application.ProductName, this);
        }
               
        private async void button1_Click(object sender, EventArgs e)
        {            
            this.button1.Enabled = false;
            if (await CheckLogin(txt_api.Text))
            {
                using (var frm = new Form1(this.txt_api.Text,Name))
                {                   
                    reg.SaveSetting(Application.ProductName, "txt_api", txt_api.Text);
                    this.Visible = false;
                    frm.ShowDialog();

                    this.Visible = true;
                    this.BringToFront();
                }
                button1.Enabled = true;
            }else
            {
                MessageBox.Show(this, "Login failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button1.Enabled = true;
                return;
            }
        }
        private async Task<bool> CheckLogin(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(token))
                    return false;

                WhatSocket.Create(token);
                var me = await WhatSocket.Instance.GetMe();                
                if (!string.IsNullOrEmpty(me.ToString()))
                {
                    this.name = me.Username;
                    return true;
                }
            }
            catch (Exception)
            { }
            return false;
        }

        private void login_Load(object sender, EventArgs e)
        {           
            txt_api.Text = reg.GetSetting(Application.ProductName, "txt_api", "").ToString();
            

           // //string s = "/sendini 5 fhtytyu";
           // //String s = "johndoe@tempuri.org";
           //// Match m = Regex.Match(s, @"^/(.+)(\d+)*$");
           // //Regex reg = new Regex("(?<user>[^@]+)(?<host>.+)");
           // //Match m = reg.Match(s);
           // if (!m.Success)
           // {
           //     MessageBox.Show("gagal");
           // }
           // else
           // {
           //     MessageBox.Show("sukses"+m.Groups[0].ToString());
           // }
        }
    }
}
