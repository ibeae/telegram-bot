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
    public partial class frm_reg : Form
    {
        string chat_id = string.Empty;
        string upline = string.Empty;
        public frm_reg(string get_chatid, string get_upline)
        {
            InitializeComponent();
            chat_id = get_chatid;
            upline = get_upline;
        }

        private void frm_reg_Load(object sender, EventArgs e)
        {

        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);
        }

        private void frm_reg_Paint(object sender, PaintEventArgs e)
        {
            Form frm = (Form)sender;
            ControlPaint.DrawBorder(e.Graphics, frm.ClientRectangle,
            Color.LightBlue, 5, ButtonBorderStyle.Solid,
            Color.LightBlue, 5, ButtonBorderStyle.Solid,
            Color.LightBlue, 5, ButtonBorderStyle.Solid,
            Color.LightBlue, 5, ButtonBorderStyle.Solid);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url_server = reg.GetSetting(Application.ProductName, "txt_url", "").ToString();
            if (string.IsNullOrEmpty(url_server))
            {
                MessageBox.Show("URL Server belum disetting", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    string jsondata = "{\"telegramid\":\"" + "TGR-" + chat_id + "\"," +
                                    "\"hp\":\"" + txt_hp.Text + "\"," +
                                   "\"nama\":\"" + txt_nama.Text + "\"," +
                                   "\"kota\":\"" + txt_kota.Text + "\"," +
                                   "\"bonus\":\"" + txt_bonus.Text + "\"," +
                                    "\"saldoawal\":\"" + txt_saldo.Text + "\"," +
                                    "\"app_id\":\"" + Form1.me_username + "\"," +
                                   "\"upagenid\":\"" + upline + "\"}";

                    JObject joResponse = mydata.send_request(url_server, "reg_downline", jsondata, "telegram.php");
                    lbl_status.MaximumSize = new Size(227, 0);
                    lbl_status.AutoSize = true;
                    lbl_status.Text = joResponse["message"].ToString();
                    MessageBox.Show(joResponse["message"].ToString());
                    for (int i = 259; i <= 275 + lbl_status.Height; i++)
                    {
                        this.Size = new Size(244, i);
                        Thread.Sleep(5);
                    }
                }
                catch (Exception x) { MessageBox.Show(x.Message.ToString()); }
                Cursor.Current = Cursors.Default;
            }
        }

        private void txt_hp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }      

        private void txt_bonus_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txt_saldo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
