using Khendys.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot.Types;

namespace telegram_app
{
    public partial class frm_chat : Form
    {
        private User user;
        private string user_id, you, me;
        string agenid = string.Empty;
        string user_hp, user_nick;
        private bool isTyping;
        volatile bool mClosePending = false;
        volatile bool mCompleted = false;
        string selectedFile;

        public frm_chat(string get_me, string nm_depan, string get_you, string get_user_id, string get_agenid)
        {
            InitializeComponent();
            this.Text = get_you;
            this.user_id = get_user_id;
            this.Name = get_user_id;
            this.you = get_you;
            this.me = get_me;
            lbl_nama.Text = nm_depan;
            agenid = get_agenid;
        }
        private void txtinvoke(string pesan, Label lbl)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke(new MethodInvoker(delegate
                {
                    lbl.Text = pesan;
                }));
            }
            else
            {
                lbl.Text = pesan;
            }
        }
        public string Convert(string str)
        {
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(str);
            str = Encoding.UTF8.GetString(utf8Bytes);
            return str;
        }
        private void frm_chat_Load(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (rtb_tulis.Text.Length == 0)
                    return;

                //user_id = "92641151";
                string emoticons = "\ud83d\udc4e";
                //String s = "\uD83D\uDE4C" + "myKeyboardTest";
                //string x = rtb_tulis.Text + Convert("\x5C\x75\x64\x38\x33\x64\x5C\x75\x64\x63\x34\x65");
                //String s = new String(new byte[]{(byte) 0xF0, (byte) 0x9F, (byte) 0x98, (byte) 0x81}, "UTF-8");
                //MessageBox.Show(x);
                //ex_rtb.AppendRtf(x);

                string pesan = rtb_tulis.Text;
                rtb_tulis.Clear();
                //await WhatSocket.Instance.SendTextMessage(user_id, pesan + emoticons);
                await WhatSocket.Instance.SendTextMessage(user_id, pesan);
                mydata.tg_pesan(user_id, me, pesan);
                this.AddNewText(me, user_id, pesan, "text");
            }
            catch (Exception x)
            {
                MessageBox.Show("periksa koneksi internet...");
            }
        }
        
        public void AddNewText(string username, string user_id, string text, string type)
        {            
            string nama = (string.IsNullOrEmpty(username)) ? user_id : username;
            //string newMessage = nama +" "+ DateTime.Now.ToString("ddMMMyy HH:mm")+ ": " + Environment.NewLine + Convert(text) + Environment.NewLine;

            ex_rtb.AppendTextAsRtf(nama,new Font(this.Font, FontStyle.Underline | FontStyle.Bold), (username == you) ? RtfColor.Brown : RtfColor.Blue);
            ex_rtb.AppendTextAsRtf(" (" + DateTime.Now.ToString("ddMMMyy HH:mm") + ") :", new Font("calibri", 8, FontStyle.Regular | FontStyle.Regular), RtfColor.Teal);
            ex_rtb.AppendTextAsRtf("\n");
            if (ex_rtb.InvokeRequired)
            {
                MethodInvoker myDelegate =
                        delegate
                        {
                            //ex_rtb.AppendText(newMessage);
                            //ex_rtb.Select(start, me.Length);
                            //ex_rtb.SelectionFont = new Font(ex_rtb.Font, FontStyle.Bold);
                            //ex_rtb.SelectionColor = Color.Brown;
                            //ex_rtb.ScrollToCaret();
                        };
                this.Invoke(myDelegate);
            }
            else
            {
                if (type == "text")
                {
                    ex_rtb.AppendText(Convert(text));
                    //ex_rtb.AppendText(text);
                    
                }
                else
                {                   
                    using (Image img = Image.FromFile(text))
                    {
                        ex_rtb.InsertImage(img);
                        img.Dispose();
                    }                    
                }
                ex_rtb.AppendTextAsRtf("\n\n");
                ex_rtb.Focus();                
                ex_rtb.Select(ex_rtb.TextLength, 0); // Scroll to bottom so newly added text is seen.
                ex_rtb.ScrollToCaret();
                rtb_tulis.Focus();                
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (this.isTyping)
            {
                this.isTyping = false;
                return;
            }
           // await WhatSocket.Instance.SendChatAction(user_id,ChatAction.);
            //await Task.Delay(1000); //2000    
            this.timer1.Stop();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           // WhatSocket.Instance.OnGetTyping += wa_OnGetTyping;
            //WhatSocket.Instance.OnGetPaused += wa_OnGetPaused;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image files (*.jpg,*.jpeg,*.png,*.gif)|*.jpg;*.jpeg;*.PNG;*.GIF;";
            openFileDialog1.Title = "Pilih Gambar";
            openFileDialog1.FileName = "";
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string fname in openFileDialog1.FileNames)
                {
                        using (var photoStream = System.IO.File.Open(fname, FileMode.Open))
                        {
                            var file = new FileToSend(fname, photoStream);
                            await WhatSocket.Instance.SendPhoto(user_id, file);
                        }
                    AddNewText(me, user_id, fname, "img");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //WhatSocket.Instance.SendTextMessage("@", "/start");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //this.AddNewText(me, user_id, "AgADBQAD66cxG3-XhQWw-V_ehSs2u5uyvzIABAoBoETxQpcIrhwCAAEC.jpg", "img");
            //openFileDialog1.Filter = "Image files (*.jpg,*.jpeg,*.png,*.gif)|*.jpg;*.jpeg;*.PNG;*.GIF;";
            //openFileDialog1.Title = "Pilih Gambar";
            //openFileDialog1.FileName= "";
            //openFileDialog1.Multiselect = true;
            //if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    foreach (string fname in openFileDialog1.FileNames)
            //    {
            //        using (var photoStream = System.IO.File.Open(fname, FileMode.Open))
            //        {
            //            MessageBox.Show(fname);
            //        }

            //    }
            //}
            //mydata.is_anggota("4545455", "username", "nm depan");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            frm_reg reg = new frm_reg(user_id,agenid);
            reg.ShowDialog();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mCompleted = true;
            if (mClosePending) this.Close();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!mCompleted)
            {
                backgroundWorker1.CancelAsync();
                this.Enabled = false;
                e.Cancel = true;
                mClosePending = true;
                return;
            }
            base.OnFormClosing(e);
        }

        private async void rtb_tulis_TextChanged(object sender, EventArgs e)
        {
            if (!this.isTyping)
            {
                this.isTyping = true;
                await WhatSocket.Instance.SendChatAction(user_id, ChatAction.Typing);
                await Task.Delay(1000); //2000    
                this.timer1.Start();
            }        
        }
    }
}
