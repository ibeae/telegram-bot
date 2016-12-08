using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace telegram_app
{
    enum Command
    {
        Login,      //Log into the server
        Logout,     //Logout of the server
        Message,    //Send a text message to all the chat clients
        List,       //Get a list of users in the chat room from the server
        Null        //No command
    }
    public partial class Form1 : Form
    {
        //Api Bot;
        static string api;
        public static string me_username;
        FbCommand cmd;
        static string agenid = string.Empty;
        static string pin = string.Empty;
        static string mac = string.Empty;

        // public static Form1 MainForm = new Form1();
        private int secondsToWait = 15;
        private DateTime startTime;
        
        struct ClientInfo
        {
            public Socket socket;   //Socket of the client
            public string strName;  //Name by which the user logged into the chat room
        }
        private bool s_Closed;
        //The collection of all clients logged into the room (an array of type ClientInfo)
        ArrayList clientList;
        //The main socket on which the server listens to the clients
        Socket serverSocket;
        byte[] byteData = new byte[1024];
        public Form1(string getapi = "",string name="")
        {
            InitializeComponent();
            
            api = getapi;            
            this.Text = name;
        }

        #region socket
        private void OnAccept(IAsyncResult ar)
        {
            if (s_Closed)
            { return; }
            try
            {
                Socket clientSocket = serverSocket.EndAccept(ar);
                //Start listening for more clients
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
                //Once the client connects then start receiving the commands from her
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), clientSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "On Accept",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            if (s_Closed)
            {
                return;
            }
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                clientSocket.EndReceive(ar);
                //Transform the array of bytes received from the user into an
                //intelligent form of object Data
              // byteData.ToArray();
                Data msgReceived = new Data(byteData);
                //We will send this object in response the users request
                Data msgToSend = new Data();
                byte[] message;
                //If the message is to login, logout, or simple text message
                //then when send to others the type of the message remains the same
                msgToSend.cmdCommand = msgReceived.cmdCommand;
                msgToSend.strName = msgReceived.strName;

                switch (msgReceived.cmdCommand)
                {
                    case Command.Login:

                        //When a user logs in to the server then we add her to our
                        //list of clients

                        ClientInfo clientInfo = new ClientInfo();
                        clientInfo.socket = clientSocket;
                        clientInfo.strName = msgReceived.strName;
                        clientList.Add(clientInfo);

                        //Set the text of the message that we will broadcast to all users
                        msgToSend.strMessage = "server|<<<" + msgReceived.strName + " masuk>>>";
                        break;

                    case Command.Logout:

                        //When a user wants to log out of the server then we search for her 
                        //in the list of clients and close the corresponding connection

                        int nIndex = 0;
                        foreach (ClientInfo client in clientList)
                        {
                            if (client.socket == clientSocket)
                            {
                                clientList.RemoveAt(nIndex);
                                break;
                            }
                            ++nIndex;
                        }

                        clientSocket.Close();
                        msgToSend.strMessage = "server|<<<" + msgReceived.strName + " keluar>>>";
                        break;

                    case Command.Message:

                        //Set the text of the message that we will broadcast to all users
                        //msgToSend.strMessage = "server|"+msgReceived.strName + ": " + msgReceived.strMessage;
                        string[] split = msgReceived.strMessage.Split('|');
                        if (split[0] != "server")
                        {
                            SetTextAnonymousDelegateMiniPatternParams(me_username, me_username + "(" + msgReceived.strName + ")", "", split[0].ToString(), split[1].ToString(), "text");
                            WhatSocket.Instance.SendTextMessage(split[0].ToString(), split[1].ToString());
                        }
                        msgToSend.strMessage = msgReceived.strMessage;
                        
                        break;

                    case Command.List:

                        //Send the names of all users in the chat room to the new user
                        msgToSend.cmdCommand = Command.List;
                        //msgToSend.strName = null;
                        msgToSend.strName = "server";
                        msgToSend.strMessage = null;

                        //Collect the names of the user in the chat room
                        foreach (ClientInfo client in clientList)
                        {
                            //To keep things simple we use asterisk as the marker to separate the user names
                            msgToSend.strMessage += client.strName + "*";
                        }
                        msgToSend.strMessage = "server|" + msgToSend.strMessage;
                        message = msgToSend.ToByte();

                        //Send the name of the users in the chat room
                        clientSocket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                new AsyncCallback(OnSend), clientSocket);
                        break;
                }

                if (msgToSend.cmdCommand != Command.List)   //List messages are not broadcasted
                {                    
                    message = msgToSend.ToByte();
                    foreach (ClientInfo clientInfo in clientList)
                    {
                        if (clientInfo.socket != clientSocket || msgToSend.cmdCommand != Command.Login)
                        {
                            //Send the message to all users
                            clientInfo.socket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                new AsyncCallback(OnSend), clientInfo.socket);
                        }
                    }
                                   

                    //WhatSocket.Instance.SendTextMessage(user_id, pesan);
                    //if (txtLog.InvokeRequired)
                    //{
                    //    txtLog.Invoke(new MethodInvoker(delegate
                    //    {
                    //        txtLog.Text += msgToSend.strMessage + "\r\n";
                    //    }));
                    //}
                    //else
                    //{
                    //    txtLog.Text = msgToSend.strMessage;
                    //}
                }

                //If the user is logging out then we need not listen from her
                if (msgReceived.cmdCommand != Command.Logout)
                {
                    //Start listening to the message send by the user
                    clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "serverTCP44", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Onsend", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void soc_sendtoall(string nama, string pesan)
        {
            Data msgToSend = new Data();            
            msgToSend.strName = nama;
            msgToSend.strMessage = pesan.ToString();
            byte[] message;
            message = msgToSend.ToByte();            
            foreach (ClientInfo clientInfo in clientList)
            {
                clientInfo.socket.BeginSend(message, 0, message.Length, SocketFlags.None,
                 new AsyncCallback(OnSend), clientInfo.socket);
            }            
        }
        bool is_SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
        public bool soc_konek()
        {
            try
            {
                clientList = new ArrayList();
                //We are using TCP sockets
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Assign the any IP of the machine and listen on port number 1000
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1000);
                //Bind and listen on the given address
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(4);
                s_Closed = false;
                //Accept the incoming clients               
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
                txtinvoke("listening", lblstatus);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Start Server Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return false;
        }
        public bool soc_close()
        {
            try
            {
                if (serverSocket != null)
                {
                    s_Closed = true;
                    // Close the clients
                    foreach (ClientInfo clientInfo in clientList)
                    {
                        clientInfo.socket.Close();
                    }
                    serverSocket.Close();
                    serverSocket = null;
                }
                txtinvoke("-", lblstatus);
                return true;
            }
            catch (ObjectDisposedException odex)
            {
                //Debug.Fail(odex.ToString(), "Stop failed");
                MessageBox.Show(odex.Message.ToString(), "Stop gagal");
                return false;
            }
            return false;
        }
        public void Logger(String lines)
        {

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log

            System.IO.StreamWriter file = new System.IO.StreamWriter("d:\\testlog.txt", true);
            file.WriteLine(lines);
            file.Close();
        }

        #endregion


       public static void get_data_server()
        {
            agenid = reg.GetSetting(Application.ProductName, "txt_agenid");
            pin = reg.GetSetting(Application.ProductName, "txt_pin");
        }

        private async Task Run()
        {
            try
            {
                var me = await WhatSocket.Instance.GetMe();
                me_username = me.Username;
                //this.Text = me_username;
                txtinvoke("Connected", lbl_me);
                var offset = 0;
                statusStrip1.BackColor = Color.LightSteelBlue;
                //txtinvoke("Connected", lblstatus);
                timer_check.Start();
                while (true)
                {

                    var updates = await WhatSocket.Instance.GetUpdates(offset);
                    foreach (var update in updates)
                    {
                        //MessageBox.Show("type: "+ update.Message.Type.ToString());
                        string username = (string.IsNullOrEmpty(update.Message.Chat.Username)) ? update.Message.Chat.Id.ToString() : update.Message.Chat.Username;
                        string nm_depan = (!string.IsNullOrEmpty(update.Message.Chat.FirstName)) ? update.Message.Chat.FirstName : username;                        
                        if (!mydata.is_anggota(update.Message.Chat.Id.ToString(), username, nm_depan))
                        {
                            refresh_tree();
                        }
                        if (update.Message.Type == MessageType.TextMessage)
                        {
                            mydata.tg_pesan(update.Message.Chat.Id.ToString(), username, update.Message.Text);
                            Match match = Regex.Match(update.Message.Text, @"^/(.+)$");
                            if (match.Success)
                            {
                                send_server(me.Username, update.Message.Chat.Id.ToString(), username, nm_depan, update.Message.Text);
                            }
                            else
                            {
                                if (serverSocket != null && is_SocketConnected(serverSocket))
                                {
                                    soc_sendtoall(nm_depan, update.Message.Chat.Id.ToString() + "|" + update.Message.Text);
                                    //soc_sendtoall(nm_depan, "92641151|" + update.Message.Text);
                                }
                                //string ob = reg.GetSetting(Application.ProductName, "ck_forward");
                                string ob_id = reg.GetSetting(Application.ProductName, "cb_forward");
                                if (!string.IsNullOrEmpty(ob_id.ToString()) && ob_id.ToString() != "0")
                                {
                                    await WhatSocket.Instance.SendTextMessage(ob_id, "auto fw: " + update.Message.Text);
                                }
                                SetTextAnonymousDelegateMiniPatternParams(me.Username, username, nm_depan, update.Message.Chat.Id.ToString(), update.Message.Text, "text");
                            }
                        }
                        else if (update.Message.Type == MessageType.PhotoMessage)
                        {
                            var file = await WhatSocket.Instance.GetFile(update.Message.Photo.LastOrDefault()?.FileId);
                            Console.WriteLine("Received Photo: {0}", file.FilePath);
                            var filename = file.FileId + "." + file.FilePath.Split('.').Last();
                            using (var profileImageStream = File.Open(filename, FileMode.Create))
                            {
                                await file.FileStream.CopyToAsync(profileImageStream);
                            }
                            SetTextAnonymousDelegateMiniPatternParams(me.Username, username, nm_depan, update.Message.Chat.Id.ToString(), filename, "img");
                        }
                        offset = update.Id + 1;
                        await Task.Delay(1000);
                    }
                }
            }
            catch (Exception x)
            {
                timer_auto.Start();
                timer_check.Stop();
                startTime = DateTime.Now;
                txtinvoke("err \n" + x.Message.ToString(), lblstatus);
                statusStrip1.BackColor = Color.LightSalmon;
                //MessageBox.Show("kesalahan \n" + x.Message.ToString());
            }
        }
        private void send_server(string bot_name, string chat_id, string username, string nama, string pesan)
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
                    string time = DateTime.Now.ToString("h:mm:ss tt").ToString();
                    string sign = mydata.MD5(mydata.SubRight(mac, 8) + agenid + time + "xyz");
                    string jsondata = "{\"telegramid\":\"" + "TGR-"+chat_id + "\"," +
                                        "\"app_id\":\"" + bot_name + "\"," +
                                       "\"pesan\":\"" + pesan + "\"," +
                                       "\"signature\":\"" + sign + "\"," +
                                       "\"time\":\"" + time + "\"," +
                                        "\"upagenid\":\"" + agenid + "\"," +
                                       "\"deviceid\":\"" + mac + "\"}";

                    JObject joResponse = mydata.send_request(url_server, "send_chat", jsondata, "telegram.php");
                    string ket = joResponse["message"].ToString();
                    //MessageBox.Show(pesan);
                    if (pesan == "/start")
                    {
                        object ob = reg.GetSetting(Application.ProductName, "txt_pesan");
                        if (!string.IsNullOrEmpty(ob.ToString()))
                            ket += ob.ToString();
                    }
                    //SetTextAnonymousDelegateMiniPatternParams(me_username, username, nama, chat_id, ket, "text");
                    WhatSocket.Instance.SendTextMessage(chat_id, ket);
                    txtinvoke(agenid + ": " + ket, lbl_me);
                }
                catch (Exception x) { MessageBox.Show(x.Message.ToString()); }
            }
        }
        
#region close_all_open
        private void komponen(bool status)
        {
            Invoke(new MethodInvoker(delegate
            {
                int frm_open = Application.OpenForms.OfType<Form>().ToList().Count(); //
                if (frm_open > 2)
                {                  
                    foreach (Form form in Application.OpenForms.OfType<Form>().ToList())
                    {
                        frm_chat f = form as frm_chat;
                        MessageBox.Show(f.Name.ToString());
                        //f.btn_img.Enabled = status;
                        //f.rtb_tulis.Enabled = status;
                        //f.btnSend.Enabled = status;
                    }
                }
            }));
            
            treeView1.Enabled = status;
            toolStrip2.Enabled = status;
            
        }
#endregion
        private void SetTextAnonymousDelegateMiniPatternParams(string me, string you, string nm_depan, string user_id, string pesan, string type)
        {
            Invoke(new MethodInvoker(delegate
            {
                dataReceived(me, you, nm_depan, user_id, pesan, type);
            }));
        }
        public static void dataReceived(string me, string you, string nm_depan, string user_id, string pesan,string type)
        {            
            foreach (Form form in Application.OpenForms.OfType<Form>().ToList())
            {
                frm_chat details = form as frm_chat;                
                if (form.Name == user_id)
                {                    
                    if (!string.IsNullOrEmpty(pesan))                      
                        details.AddNewText(you, user_id, pesan.ToString(), type);

                    details.Activate(); //details.BringToFront();
                    return;
                }
            }
            
            frm_chat form2 = new frm_chat(me, nm_depan, you, user_id,agenid);
            if (!string.IsNullOrEmpty(pesan))
                    form2.AddNewText(you, user_id, pesan.ToString(),type);

                form2.Show();
            
        }
        private void txtinvoke(string pesan, ToolStripStatusLabel lbl)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new MethodInvoker(delegate
                {
                    lbl.Text = pesan;
                }));
            }
            else
            {
                lbl.Text = pesan;
            }
        }
        private void refresh_tree()
        {
            try
            {
                DataSet dt = new DataSet();
                DataTable dt_anggota = new DataTable();
                dt.Clear();
                FbCommand cmd = new FbCommand("select * from tg_anggota order by nama", mydata.Connection);
                cmd.ExecuteNonQuery();
                FbDataAdapter dam = new FbDataAdapter(cmd);
                dam.Fill(dt, "anggota");
                dam.Fill(dt_anggota);

                foreach (DataRow DataRow in dt_anggota.Rows)
                {
                    TreeNode TreeNode = new TreeNode();
                    TreeNode.Text = DataRow["nama"].ToString();
                    TreeNode.Tag = DataRow["user_id"].ToString();
                    treeView1.Nodes.Add(TreeNode);
                    treeView1.ImageIndex = 1;
                    treeView1.SelectedImageIndex = 1;
                    treeView1.Refresh();
                    treeView1.Update();
                }
                dt.Clear();
                dt_anggota.Clear();
            }
            catch (Exception x) { MessageBox.Show(x.Message.ToString()); Application.Exit(); }
        }
        private async void Form1_Load(object sender, EventArgs e)
        {            
            #region load_trx_user
            mac = mydata.GetMACAddress();
            get_data_server();
            #endregion

            refresh_tree();
            await Run();
        }
        private void listKontakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new anggota())
            {
                frm.ShowDialog();
            }
        }
        
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            aboutbox about = new aboutbox();
            about.ShowDialog();
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;

            FbConnection db = new FbConnection(mydata.ConnectionString);
            db.Open();
            FbCommand comand1 = db.CreateCommand();
            comand1.CommandText = "select user_id, username, nama from tg_anggota where user_id='" + node.Tag + "' ";
            FbDataReader reader1 = comand1.ExecuteReader();
            string username = string.Empty;
            string nama = string.Empty;
            string user_id = string.Empty;
            if (reader1.HasRows)
            {
                reader1.Read();
                username = (!string.IsNullOrEmpty(reader1["username"].ToString())) ? reader1["username"].ToString() : reader1["user_id"].ToString();
                nama = reader1["nama"].ToString();
                user_id = reader1["user_id"].ToString();
            }

            reader1.Close();
            //var me = await WhatSocket.Instance.GetMe();

            // SetTextAnonymousDelegateMiniPatternParams(me.Username, username, nama, user_id, "", "text");
            SetTextAnonymousDelegateMiniPatternParams(me_username, username, nama, user_id, "", "text");

            foreach (Form form in Application.OpenForms.OfType<Form>().ToList())
            {
                frm_chat frm2 = form as frm_chat;                
            }         
        }        
        private async void timer_auto_Tick(object sender, EventArgs e)
        {
            int elapsedSeconds = (int)(DateTime.Now - startTime).TotalSeconds;
            int remainingSeconds = secondsToWait - elapsedSeconds;
            txtinvoke(remainingSeconds + " seconds remaining...", lbl_me);
            if (remainingSeconds <= 0)
            {
                timer_auto.Stop();
                txtinvoke("connecting...", lbl_me);
                await Run();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            soc_sendtoall("tes", "92641151|ini percobaan saja");          
        }
      
        private void bg_check_DoWork(object sender, DoWorkEventArgs e)
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
                    string time = DateTime.Now.ToString("h:mm:ss tt").ToString();
                    string sign = mydata.MD5(mydata.SubRight(mac, 8) + agenid + time + "xyz");
                    string jsondata = "{\"app_id\":\"" + me_username + "\"," +
                                       "\"signature\":\"" + sign + "\"," +
                                       "\"time\":\"" + time + "\"," +
                                        "\"upagenid\":\"" + agenid + "\"," +
                                       "\"deviceid\":\"" + mac + "\"}";

                    JObject joResponse = mydata.send_request(url_server, "check_outbox", jsondata, "telegram.php");                    
                    if (joResponse.Type != JTokenType.Null)
                    {                        
                        if (joResponse["status"].ToString() == "0")
                        {
                            foreach (var row in joResponse["message"])
                            {                     
                                string pesan = row["pesan"].ToString();
                                string chat_id = row["hp"].ToString();
                                chat_id= chat_id.Split('-').Last();
                                WhatSocket.Instance.SendTextMessage(chat_id, pesan);
                            }
                        }
                    }
                }
                catch (Exception x) { MessageBox.Show(x.Message.ToString(),"warning sms"); }
                }
                
        }

        private void timer_check_Tick(object sender, EventArgs e)
        {            
            if (!bg_check.IsBusy)
                bg_check.RunWorkerAsync();
        }

        private void bg_send_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            setting about = new setting();
            about.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                soc_close();
            }
            catch { }
            reg.SaveAllSettings(Application.ProductName, this);
        }
        
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripButton1.Tag.ToString() == "off")
            {
                toolStripButton1.Image = new Bitmap(telegram_app.Properties.Resources.on);
                soc_konek();
                toolStripButton1.Tag = "on";
            }
            else
            {
                toolStripButton1.Image = new Bitmap(telegram_app.Properties.Resources.off);
                soc_close();
                toolStripButton1.Tag = "off";
            }
        }

        private void autoForwardPesanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var node = treeView1.SelectedNode;
            //TreeNode node = treeView1.SelectedNode;
            //if (node != null)
            //{
            //    MessageBox.Show(node.Tag.ToString());
            //}
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
           // if (e.Button == MouseButtons.Right)
               // this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            //MessageBox.Show(e.Node.Name);
        }

        private void menu_aktivasi_Click(object sender, EventArgs e)
        {
            mini_aktivasi mini = new mini_aktivasi();
            mini.ShowDialog();
        }

        private void keluarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }

    class Data
    {
        //Default constructor
        public Data()
        {
            this.cmdCommand = Command.Null;
            this.strMessage = null;
            this.strName = null;
        }

        //Converts the bytes into an object of type Data
        public Data(byte[] data)
        {
            //The first four bytes are for the Command
            this.cmdCommand = (Command)BitConverter.ToInt32(data, 0);

            //The next four store the length of the name
            int nameLen = BitConverter.ToInt32(data, 4);

            //The next four store the length of the message
            int msgLen = BitConverter.ToInt32(data, 8);

            //This check makes sure that strName has been passed in the array of bytes
            if (nameLen > 0)
                this.strName = Encoding.UTF8.GetString(data, 12, nameLen);
            else
                this.strName = null;

            //This checks for a null message field
            if (msgLen > 0)
                this.strMessage = Encoding.UTF8.GetString(data, 12 + nameLen, msgLen);
            else
                this.strMessage = null;
        }

        //Converts the Data structure into an array of bytes
        public byte[] ToByte()
        {
            List<byte> result = new List<byte>();

            //First four are for the Command
            result.AddRange(BitConverter.GetBytes((int)cmdCommand));

            //Add the length of the name
            if (strName != null)
                result.AddRange(BitConverter.GetBytes(strName.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //Length of the message
            if (strMessage != null)
                result.AddRange(BitConverter.GetBytes(strMessage.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //Add the name
            if (strName != null)
                result.AddRange(Encoding.UTF8.GetBytes(strName));

            //And, lastly we add the message text to our array of bytes
            if (strMessage != null)
                result.AddRange(Encoding.UTF8.GetBytes(strMessage));

            return result.ToArray();
        }

        public string strName;      //Name by which the client logs into the room
        public string strMessage;   //Message text
        public Command cmdCommand;  //Command type (login, logout, send message, etcetera)
    }
}
