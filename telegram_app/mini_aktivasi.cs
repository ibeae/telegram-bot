using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace telegram_app
{
    public partial class mini_aktivasi : Form
    {
        string agenid, token = string.Empty;
        static string mac = string.Empty;
        public mini_aktivasi(string agenid=null, string pin=null)
        {
            InitializeComponent();
            //this.txt_hp.Text = Form1.trx_hp;
            if (agenid != null)
            {
                txt_agenid.Text = agenid;
                txt_pin.Text = pin;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {              
                txt_token.Enabled = true;
                btn_gettoken.Enabled = false;
                button2.Enabled = true;
            }
            else
            {                
                txt_token.Enabled = false;
                btn_gettoken.Enabled = true;
                button2.Enabled = false;
            }
        }

        private void btn_gettoken_Click(object sender, EventArgs e)
        {
            string url_server = reg.GetSetting(Application.ProductName, "txt_url", "").ToString();
            if (string.IsNullOrEmpty(txt_agenid.Text) || string.IsNullOrEmpty(txt_hp.Text) || string.IsNullOrEmpty(txt_pin.Text))
            {
                MessageBox.Show("Lengkapi Data...");
            }
            else if (string.IsNullOrEmpty(url_server))
            {
                MessageBox.Show("URL Server belum disetting", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    lbl_status.Text = "loading...";
                    btn_gettoken.Enabled = false;
                    group_token.Enabled = true;
                    string jsondata = "{\"agenid\":\"" + txt_agenid.Text + "\"," +
                                   "\"hp\":\"" + txt_hp.Text + "\"," +
                                   "\"pin\":\"" + txt_pin.Text + "\"," +
                                   "\"deviceid\":\"" + mac + "\"}";

                    JObject joResponse = mydata.send_request(url_server, "getactivationkey", jsondata);
                    lbl_status.Text = joResponse["response"][0]["message"].ToString();
                    if (joResponse["response"][0]["status"].ToString() == "1")
                    {
                        txt_token.Enabled = true;
                        button2.Enabled = true;
                        btn_gettoken.Enabled = false;
                        reg.SaveChildSettings(Application.ProductName, group_token);
                        this.ActiveControl = txt_token;
                    }
                    else
                    {
                        btn_gettoken.Enabled = true;
                    }
                    for (int i = 259; i <= 269 + lbl_status.Height; i++)
                    {
                        this.Size = new Size(236, i);
                        Thread.Sleep(5);
                    }
                }
                catch (Exception x) { MessageBox.Show(x.Message.ToString()); }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {            
            button2.Enabled = false;
            lbl_status.Text = "loading...";
            string url_server = reg.GetSetting(Application.ProductName, "txt_url", "").ToString();
            //foreach (Control ctrl in group_token.Controls)
            //{
            //    if (ctrl is TextBox)
            //    {
            //        TextBox textBox = (TextBox)ctrl;
            //        if (string.IsNullOrEmpty(textBox.Text))
            //            return;
            //    }
            //} string url_server = reg.GetSetting(Application.ProductName, "txt_url", "").ToString();
            if (string.IsNullOrEmpty(url_server))
            {
                MessageBox.Show("URL Server belum disetting", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else {
                try
                {
                    string jsondata = "{\"agenid\":\"" + txt_agenid.Text + "\"," +
                                  "\"hp\":\"" + txt_hp.Text + "\"," +
                                  "\"pin\":\"" + txt_pin.Text + "\"," +
                                   "\"deviceid\":\"" + mac + "\"," +
                                  "\"keyactivation\":\"" + txt_token.Text + "\"}";

                    JObject joResponse = mydata.send_request(url_server,"activated", jsondata);
                    if (joResponse["response"][0]["status"].ToString() == "1")
                    {
                        group_token.Enabled = false;
                        reg.SaveChildSettings(Application.ProductName, group_token);
                        Form1.get_data_server();
                    }
                    else
                    {
                        button2.Enabled = true;
                    }
                    lbl_status.Text = joResponse["response"][0]["message"].ToString();
                    for (int i = 259; i <= 269 + lbl_status.Height; i++)
                    {
                        this.Size = new Size(236, i);
                        Thread.Sleep(5);
                    }
                }
                catch (Exception x) { MessageBox.Show(x.Message.ToString()); }
            }
        }
        
        private void txt_agenid_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void mini_aktivasi_Load(object sender, EventArgs e)
        {  
            reg.LoadAllSettings(Application.ProductName, this);
            mac = mydata.GetMACAddress();
            agenid = txt_agenid.Text;
            token = txt_token.Text;
            if (!string.IsNullOrEmpty(agenid) && !string.IsNullOrEmpty(token))
            {
                group_token.Enabled = false;
            }

        }
    }
}
