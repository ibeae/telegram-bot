using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace telegram_app
{
    class mydata
    {

        FbCommand cmd;
        public static string ConnectionString
        {
            get
            {
                return "ServerType=1;User=SYSDBA;Password=masterkey;Database=" + Application.StartupPath + "\\MYDATA.fdb";
            }
        }

        private static FbConnection _connection;
        public static FbConnection Connection
        {
            get
            {
                if (_connection == null)
                    _connection = new FbConnection(ConnectionString);
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                return _connection;
            }
        }
        private static FbConnection _connection1;
        public static FbConnection Connection1
        {
            get
            {
                if (_connection1 == null)
                    _connection1 = new FbConnection(ConnectionString);
                if (_connection1.State == ConnectionState.Closed)
                    _connection1.Open();
                return _connection1;
            }
        }
        #region form_open_close

        internal static anggota fanggota;
        public static void Openanggota(Form parent, Panel content)
        {
            if (fanggota == null)
            {

                close(content);
                fanggota = new anggota();
                fanggota.MdiParent = parent;
                content.Controls.Add(fanggota);
                fanggota.Show();
            }
            fanggota.Focus();
        }
        
        public static void close(Panel content)
        {
            if (content.Controls.Count > 0)
            {
                foreach (Form aform in content.Controls.OfType<Form>())
                {
                    aform.Close();
                }
            }
        }     
        #endregion
        #region tg
        public static bool is_anggota(string user_id, string username, string nm_depan)
        {
            bool balik = false;            
            try
            {                
                FbCommand cmd;
                cmd = new FbCommand("select user_id from tg_anggota where user_id = '" + user_id + "'", mydata.Connection);
                FbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    balik = true;
                }
                else
                {
                    cmd = new FbCommand("insert into tg_anggota (user_id, username, nama) values ('" + user_id + "','" + username + "','" + nm_depan + "')", mydata.Connection);
                    cmd.ExecuteNonQuery();
                    balik = false;                    
                }
                reader.Close();
                mydata.Connection.Close();                
            }
            catch (Exception x) { MessageBox.Show("gagal ... \n" + x.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return balik;
        }

        public static void tg_pesan(string thread, string from, string pesan)
        {
            try
            {                
                pesan = RemoveSpecialCharacters(pesan);
                FbCommand cmd = new FbCommand("insert into tg_pesan (id_thread, pesan, waktu, dari) values ('" + thread + "','" + pesan + "',CURRENT_TIMESTAMP, '" + from + "')", mydata.Connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception x) { MessageBox.Show("gagal insert inbox... \n insert into tg_pesan (id_thread, pesan, waktu, dari) values ('" + thread + "','" + pesan + "',NOW(), '" + from + "')" + x.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        //public static long insert_trx(string vtype, string tujuan, string agenid)
        //{
        //    long id = 0;
        //    try
        //    {
        //        FbCommand cmd = new FbCommand("insert into tg_transaksi (tanggal, agenid, vtype, tujuan) values (NOW(),'" + agenid + "','" + vtype + "', '" + tujuan + "')", mydata.Connection);
        //        cmd.ExecuteNonQuery();
        //        id = cmd.LastInsertedId;
        //    }
        //    catch (Exception x) { MessageBox.Show("gagal insert transaksi... \n insert into tg_transaksi (tanggal, agenid, vtype, tujuan) values (NOW(),'" + agenid + "','" + vtype + "', '" + tujuan + "')" + x.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        //    return id;
        //}
        public static void update_trx(int trx_id, int status, string pesan)
        {
            try
            {
                FbCommand cmd = new FbCommand("update tg_transaksi set status='"+status+ "',ket='" + pesan + "' where id='" + trx_id + "'", mydata.Connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception x) { MessageBox.Show("gagal update transaksi... \n update tg_transaksi set status ='" + status + "' where id = '" + trx_id + "'" + x.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.*#+-/ ]+", "", RegexOptions.Compiled);
        }

        public static string cek_nama(string hp)
        {
            FbConnection db = new FbConnection(mydata.ConnectionString);
            db.Open();
            FbCommand comand1 = db.CreateCommand();
            comand1.CommandText = "select nama from wa_anggota where hp='" + hp + "' ";
            FbDataReader reader1 = comand1.ExecuteReader();
            if (reader1.HasRows)
            {
                reader1.Read();
                hp = reader1["nama"].ToString();
            }
            reader1.Close();
            return hp.ToString();
        }
        #endregion
        #region form_tree

        #region simple treview
        //public static void PopulateTreeView(TreeNodeCollection parentNode, string parentID, DataTable folders)
        //{
        //    foreach (DataRow folder in folders.Rows)
        //    {
        //        if (folder["is_wa"].ToString() == parentID)
        //        {
        //            String key = folder["hp"].ToString();
        //            String text = folder["nama"].ToString();
        //            TreeNodeCollection newParentNode = parentNode.Add(key, text).Nodes;
        //            PopulateTreeView(newParentNode, folder["is_wa"].ToString(), folders);
        //        }
        //    }
        //}
        #endregion

        #endregion

        #region pulsa
        static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        public static JObject send_request(string url_server, string header, string jsondata = null, string url= "telegram.php")
        {
                JObject joResponse = null;            
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url_server + url);
                httpWebRequest.ContentType = "application/json;";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("ACK", header);
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    // MessageBox.Show(jsondata);
                    streamWriter.Write(jsondata);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
               // MessageBox.Show(result.ToString());
                if (header == "send_chat")
                {
                    joResponse = new JObject();
                    joResponse["message"] = StripHTML(result.ToString());
                    return joResponse;                    
                }
                else {
                    ////Regex regex = new Regex(@"(\[{.*}\])", RegexOptions.Multiline);
                    //Match match = regex.Match(result);
                    //if (match.Success)
                    //{
                    //    result = match.Groups[1].Value;
                     //   MessageBox.Show(result.ToString());
                    //}
                    joResponse = JObject.Parse(result);
                    return joResponse;
                }
            }
        }

       public static bool eksekusi(string sql)
        {
            FbCommand cmd = new FbCommand(sql, mydata.Connection1);
            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected == 1)
                return true;
            else
                return false;
        }
        #endregion

        #region get_mac
       public static string GetMACAddress()
       {
           NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
           String sMacAddress = string.Empty;
           foreach (NetworkInterface adapter in nics)
           {
               if (sMacAddress == String.Empty)// only return MAC Address from first card  
               {
                   //IPInterfaceProperties properties = adapter.GetIPProperties(); Line is not required
                   sMacAddress = adapter.GetPhysicalAddress().ToString();
               }
           } return sMacAddress;
       }
        #endregion

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "super71a";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "super71a";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        #region rubah ke MD5
        public static string MD5(string password)
        {
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(password);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }
        #endregion
        public static string SubRight(string original, int numberCharacters)
        {            
            return original.Substring(original.Length - numberCharacters);
        }


    }
    public class data_agen
    {
        public string nama { get; set; }
        public string alamat { get; set; }
    }


}
