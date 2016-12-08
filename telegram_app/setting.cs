using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace telegram_app
{
    public partial class setting : Form
    {
        private delegate void UpdateStatusCallback(string strMessage);
        DataTable dtkab;
        public setting()
        {
            InitializeComponent();
        }

        private void setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            reg.SaveChildSettings(Application.ProductName,groupBox7);
        }

        private void setting_Load(object sender, EventArgs e)
        {          
            
            using (FbCommand Command = mydata.Connection.CreateCommand())
            {
                Command.CommandText = "SELECT USER_ID, NAMA FROM TG_ANGGOTA";
                FbDataAdapter dataAdapter = new FbDataAdapter(Command);
                dtkab = new DataTable();
                dataAdapter.Fill(dtkab);
                DataRow Row = dtkab.NewRow();
                Row["NAMA"] = "--Pilih ID--";
                Row["USER_ID"] = "0";
                dtkab.Rows.InsertAt(Row, 0);
                cb_forward.DisplayMember = "NAMA";
                cb_forward.ValueMember = "USER_ID";
                cb_forward.DataSource = dtkab;
            }
            reg.LoadChildSettings(Application.ProductName, groupBox7);
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            
        }        
    }
}
