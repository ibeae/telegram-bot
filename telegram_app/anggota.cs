using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace telegram_app
{
    public partial class anggota : Form
    {
        int i = 0;
        int TotalCheckBoxes = 0;
        int TotalCheckedCheckBoxes = 0;
        CheckBox HeaderCheckBox = null;
        bool IsHeaderCheckBoxClicked = false;
        bool edit = false;
        int g_id = 0;
        FbCommand cmd;
        
        public anggota()
        {
            InitializeComponent();
        }

        #region cekbox
        private void AddHeaderCheckBox()
        {
            HeaderCheckBox = new CheckBox();
            HeaderCheckBox.Size = new Size(15, 15);
            //Add the CheckBox into the DataGridView
            this.dgv_anggota.Controls.Add(HeaderCheckBox);
        }
        private void HeaderCheckBox_MouseClick(object sender, MouseEventArgs e)
        {
            HeaderCheckBoxClick((CheckBox)sender);
            //if (IsHeaderCheckBoxClicked)
            //{
            //    MessageBox.Show("klik");
            //}
            //else
            //{
            //    MessageBox.Show("lepas");
            //}
        }
        private void HeaderCheckBoxClick(CheckBox HCheckBox)
        {
            IsHeaderCheckBoxClicked = true;
            foreach (DataGridViewRow Row in dgv_anggota.Rows)
                ((DataGridViewCheckBoxCell)Row.Cells["chkBxSelect"]).Value = HCheckBox.Checked;


            dgv_anggota.RefreshEdit();
            TotalCheckedCheckBoxes = HCheckBox.Checked ? TotalCheckBoxes : 0;
            IsHeaderCheckBoxClicked = false;
            //RowCheckBoxClicks();
        }
        private void ResetHeaderCheckBoxLocation(int ColumnIndex, int RowIndex)
        {
            //Get the column header cell bounds
            Rectangle oRectangle = this.dgv_anggota.GetCellDisplayRectangle(ColumnIndex, RowIndex, true);
            Point oPoint = new Point();
            oPoint.X = oRectangle.Location.X + (oRectangle.Width - HeaderCheckBox.Width) / 2 + 1;
            oPoint.Y = oRectangle.Location.Y + (oRectangle.Height - HeaderCheckBox.Height) / 2 + 1;
            //Change the location of the CheckBox to make it stay on the header
            HeaderCheckBox.Location = oPoint;

        }
        #endregion
        private void anggota_Load(object sender, EventArgs e)
        {           
            load_data();            
        }
        private void load_data()
        {
            try
            {
                AddHeaderCheckBox();
                HeaderCheckBox.MouseClick += new MouseEventHandler(HeaderCheckBox_MouseClick);

                FbCommand com = new FbCommand(@"SELECT * from tg_anggota order by nama", mydata.Connection);
                FbDataReader reader = com.ExecuteReader();
                dgv_anggota.Rows.Clear();
                int i = 0;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        dgv_anggota.RowCount++;
                        dgv_anggota.Rows[i].Cells["no"].Value = (i + 1).ToString();
                        dgv_anggota.Rows[i].Cells["user_id"].Value = reader["user_id"].ToString();
                        dgv_anggota.Rows[i].Cells["username"].Value = reader["username"].ToString();
                        dgv_anggota.Rows[i].Cells["nama"].Value = reader["nama"].ToString();
                        dgv_anggota.Rows[i].Cells["alamat"].Value = reader["alamat"].ToString();
                        i++;
                    }
                }
                reader.Close();
                TotalCheckBoxes = dgv_anggota.RowCount;
                TotalCheckedCheckBoxes = 0;
            }
            catch (Exception x)
            { MessageBox.Show("kesalahan: " + x.Message.ToString()); }
            finally
            {
                mydata.Connection.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Apakah yakin menghapus data yang dipilih ?", "Peringatan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dgv_anggota.Rows)
                {
                    DataGridViewCheckBoxCell chk = row.Cells[0] as DataGridViewCheckBoxCell;
                    if (Convert.ToBoolean(chk.Value) == true)
                    {
                        try
                        {
                            FbCommand cmd = new FbCommand();
                            cmd.Connection = mydata.Connection;
                            cmd.CommandText = "delete from wa_anggota where id='" + row.Cells["id"].Value.ToString() + "'";
                            cmd.ExecuteNonQuery();
                        }
                        catch { MessageBox.Show("Tidak ada data"); }
                        finally { mydata.Connection.Close(); }
                    }
                }
                load_data();
                HeaderCheckBox.Checked = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            try
            {                
                if (edit)
                {
                   
                    cmd = new FbCommand("update tg_anggota set nama ='" + txt_nama.Text + "', alamat='" + txt_alamat.Text + "'  where user_id='" + g_id + "' ", mydata.Connection);
                    cmd.ExecuteNonQuery();               
                }
                //else
                //{                 
                //    cmd = new FbCommand("insert into tg_anggota (nama, alamat) values ('" + txt_nama.Text + "','" + txt_alamat.Text + "')", mydata.Connection);
                //    cmd.ExecuteNonQuery();
                //}               
            }
            catch (Exception x) { MessageBox.Show("gagal proses... \n semua harus diisi"  + x.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally
            {
                edit = false;
                g_id = 0;
                button1.Enabled = false;
                txt_alamat.ReadOnly = true;
                txt_nama.ReadOnly = true;
                foreach (TextBox aform in panel1.Controls.OfType<TextBox>())
                {
                    aform.Text = string.Empty;
                }
                load_data();
            }
        }

        private void anggota_FormClosed(object sender, FormClosedEventArgs e)
        {
            mydata.fanggota = null;
        }

        private void kirimPesanToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv_anggota["hp", dgv_anggota.CurrentCell.RowIndex].Value.ToString() != null && dgv_anggota["hp", dgv_anggota.CurrentCell.RowIndex].Value.ToString() != String.Empty)
                {
                    if (dgv_anggota["is_wa", dgv_anggota.CurrentCell.RowIndex].Value.ToString() == "1")
                    {
                       // var user = User.UserExists(dgv_anggota["hp", dgv_anggota.CurrentCell.RowIndex].Value.ToString(), dgv_anggota["nama", dgv_anggota.CurrentCell.RowIndex].Value.ToString());
                       // Form1.dataReceived(user);
                    }
                }
                return;
            }
            catch (Exception x)
            { MessageBox.Show("kesalahan: " + x.Message.ToString()); }
        }

        private void dgv_anggota_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == 0)
                ResetHeaderCheckBoxLocation(e.ColumnIndex, e.RowIndex);
            for (int i = 0; i < dgv_anggota.Rows.Count; i++)
            {
                DataGridViewCheckBoxCell chk = dgv_anggota["chkBxSelect", i] as DataGridViewCheckBoxCell;
                dgv_anggota.Rows[i].DefaultCellStyle.BackColor = (i % 2 == 0) ? Color.PapayaWhip : Color.Honeydew;
            }           
        }

        private void dgv_anggota_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int bariske = dgv_anggota.SelectedCells[0].RowIndex;
                txt_alamat.Text = dgv_anggota["alamat", bariske].Value.ToString();
                txt_nama.Text = dgv_anggota["nama", bariske].Value.ToString();                
               // cmb_group.Text = dgv_anggota["group", bariske].Value.ToString();
                edit = true;
                g_id = int.Parse(dgv_anggota["user_id", bariske].Value.ToString());
                button1.Enabled = true;
                txt_alamat.ReadOnly = false;
                txt_nama.ReadOnly = false;
            }
            catch (Exception x)
            { MessageBox.Show("kesalahan: " + x.Message.ToString()); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Showkirimpesan();
        }
        void Showkirimpesan()
        {
            Form prompt = new Form();
            prompt.Width = 260;
            prompt.Height = 140;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.Text = "Kirim Pesan";
            prompt.MaximizeBox = false;
            prompt.MinimizeBox = false;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            Label labelpesan = new Label() { Left = 9, Top = 7, Font = new Font("Arial", 12), Text = "pesan: " };            
            TextBox textpesan = new TextBox() { Left = 10, Top = 29, Width = 250 };
            textpesan.Multiline = true;
            textpesan.Size = new System.Drawing.Size(217, 37);
            Button confirmation = new Button() { Text = "Kirim", Left = 60, Height=30, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textpesan);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(labelpesan);
            prompt.Text = "";
            prompt.AcceptButton = confirmation;
            prompt.StartPosition = FormStartPosition.CenterParent;
            if (prompt.ShowDialog() == DialogResult.OK)
            {
                List<String> xx = new List<String>();
                foreach (DataGridViewRow row in dgv_anggota.Rows)
                {
                    DataGridViewCheckBoxCell chk = row.Cells[0] as DataGridViewCheckBoxCell;
                    if (Convert.ToBoolean(chk.Value) == true)
                    {
                        WhatSocket.Instance.SendTextMessage(row.Cells["user_id"].Value.ToString(), textpesan.Text.Trim());
                        // if (row.Cells["is_wa"].Value.ToString() == "1")
                        // {
                        //xx.Add(row.Cells["hp"].Value.ToString());
                        // }
                    }                    
                }
               // string s = String.Join(",", xx);MessageBox.Show(s.ToString());
                //string[] nom = xx.ToArray();           
                //WhatSocket.Instance.SendTextMessage(nom, textpesan.Text.Trim());                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void anggota_Paint(object sender, PaintEventArgs e)
        {
            Form frm = (Form)sender;
            ControlPaint.DrawBorder(e.Graphics, frm.ClientRectangle,
            Color.LightBlue, 5, ButtonBorderStyle.Solid,
            Color.LightBlue, 5, ButtonBorderStyle.Solid,
            Color.LightBlue, 5, ButtonBorderStyle.Solid,
            Color.LightBlue, 5, ButtonBorderStyle.Solid);
        }
    }
}
