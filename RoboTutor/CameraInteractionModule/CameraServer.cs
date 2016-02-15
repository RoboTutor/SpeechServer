using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Messenger;
using System.Diagnostics;

namespace CameraInteractionModule
{
    public partial class CameraServer : UserControl, IMessenger
    {

        private const int NAO_RESPONSE_DELAY = 18000;
        private TcpListener tcpListener;
        private Boolean continueListening = true;
        private Thread serverThread;
        //private bool act = false;
        private Button button1;
        private GroupBox groupBox4;
        private TextBox password2TextBox;
        private Label label4;
        private TextBox username2TextBox;
        private Label label7;
        private TextBox cam2IPAddress;
        private Label label8;
        private GroupBox groupBox1;
        private TextBox password1TextBox;
        private Label label3;
        private TextBox username1TextBox;
        private Label label2;
        private TextBox cam1IPAddress;
        private Label label1;
        private MessageParameters param = new MessageParameters();

        #region Component Designer generated code

        public CameraServer()
        {
            InitializeComponent();
        }

        private void playButton_Click_1(object sender, EventArgs e)
        {
           
            /*if (!act)
            {
                SendMessage("NaoManager", new MessageEventArgs("InitSentiText", new string[] { }));
                act = true;
            }
            SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "Waving" }));
            Thread.Sleep(500);
            SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "Hello my dear friends" }));            
            */
            bool okToProceed = validateUI();
            if (!okToProceed)
            {
                return;
            }
            serverThread = new Thread(startServer);
            serverThread.Start();
            Thread.Sleep(1000);
            //StartApplication();
            updateGUI(false);

        }

        private void StartApplication()
        {
            ProcessStartInfo p = new ProcessStartInfo();
            p.Arguments = cam1IPAddress.Text.Trim() + " " + username1TextBox.Text.Trim() + " " + password1TextBox.Text.Trim() + " " +
                cam2IPAddress.Text.Trim() + " " + username2TextBox.Text.Trim() + " " + password2TextBox.Text.Trim() + " " +
                compIPAddress.Text.Trim() + " " + portNumber.Text.Trim();
            String currDir = Directory.GetCurrentDirectory();
            currDir = currDir.Substring(0 , currDir.LastIndexOf("\\"));
            currDir = currDir.Substring(0, currDir.LastIndexOf("\\"));
            currDir = currDir + "/CameraInteractionModule/BackDiff/Debug/BackDiff.exe";
            p.FileName = @currDir;
            p.WindowStyle = ProcessWindowStyle.Normal;
            Process proc = Process.Start(p);
          
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Stop the server
                if (this.continueListening)
                {
                    while (tcpListener != null && tcpListener.Pending()) ;
                    tcpListener.Stop();
                    serverThread.Abort();
                    updateGUI(true);
                    AppendToTextBox("Server Closed....." + "\n");
                }
            }
            catch (Exception ex)
            {
                Console.Write("Error..... " + ex.StackTrace);
            }
        }

        private bool validateUI()
        {
            if (this.cam1IPAddress.Text == null || this.cam1IPAddress.Text.Trim().Equals("") ||
                this.cam2IPAddress.Text == null || this.cam2IPAddress.Text.Trim().Equals("") ||
                this.compIPAddress.Text == null || this.compIPAddress.Text.Trim().Equals("") ||
                this.portNumber.Text == null || this.portNumber.Text.Trim().Equals(""))
            {
                MessageBox.Show("Invalid Settings Detected");
                return false;
            }
            return true;
        }

        private void startServer()
        {
            IPAddress ipAd = IPAddress.Parse(this.compIPAddress.Text);
            try
            {
                // use local m/c IP address, and 
                // use the same in the client

                /* Initializes the Listener */
                tcpListener = new TcpListener(ipAd, Convert.ToInt32(this.portNumber.Text));

                /* Start Listeneting at the specified port */
                tcpListener.Start();
            }
            catch (SocketException soEx)
            {
                MessageBox.Show("Error...\n Socket: " + this.portNumber.Text + " of IP address: " + ipAd +
                    " could not be opened.");
                Console.Write("Error..... " + soEx.StackTrace);
                tcpListener.Stop();
                tcpListener = null;
                updateGUI(true);
                return;
            }
            int clientCounter = 0;
            try
            {
                AppendToTextBox("Ready....." + "\n");
                while (this.continueListening && serverThread.IsAlive)
                {
                    //if (NaoConnected) // Comment this line for debugging purposes
                    {
                        clientCounter = clientCounter + 1;
                        TcpClient clientSocket = tcpListener.AcceptTcpClient();
                        HandleClient client = new HandleClient();
                        client.startClient(clientSocket, this, clientCounter);
                    }
                }

                tcpListener.Stop();

            }
            catch (Exception ex)
            {
                Console.Write("Error..... " + ex.StackTrace);
            }
        }

        public void AppendToTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendToTextBox), new object[] { value });
                return;
            }
            this.response.AppendText(value);
        }

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.portNumber = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.compIPAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.response = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.password1TextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.username1TextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cam1IPAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.password2TextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.username2TextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cam2IPAddress = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.portNumber);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.compIPAddress);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.stopButton);
            this.groupBox2.Controls.Add(this.playButton);
            this.groupBox2.Location = new System.Drawing.Point(36, 37);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(344, 448);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // portNumber
            // 
            this.portNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.portNumber.Location = new System.Drawing.Point(128, 352);
            this.portNumber.Name = "portNumber";
            this.portNumber.Size = new System.Drawing.Size(201, 26);
            this.portNumber.TabIndex = 27;
            this.portNumber.Text = "5005";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(17, 354);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 23);
            this.label6.TabIndex = 26;
            this.label6.Text = "Port (Socket):";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // compIPAddress
            // 
            this.compIPAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.compIPAddress.Location = new System.Drawing.Point(128, 316);
            this.compIPAddress.Name = "compIPAddress";
            this.compIPAddress.Size = new System.Drawing.Size(201, 26);
            this.compIPAddress.TabIndex = 25;
            this.compIPAddress.Text = "127.0.0.1";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(17, 319);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 23);
            this.label5.TabIndex = 24;
            this.label5.Text = "Computer IP:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(173, 398);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(150, 31);
            this.stopButton.TabIndex = 29;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(17, 398);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(150, 31);
            this.playButton.TabIndex = 28;
            this.playButton.Text = "Connect";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click_1);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.response);
            this.groupBox3.Location = new System.Drawing.Point(386, 37);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(654, 495);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Server";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 454);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "Clear Messages";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // response
            // 
            this.response.BackColor = System.Drawing.SystemColors.Window;
            this.response.Location = new System.Drawing.Point(6, 25);
            this.response.Multiline = true;
            this.response.Name = "response";
            this.response.ReadOnly = true;
            this.response.Size = new System.Drawing.Size(642, 423);
            this.response.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.password1TextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.username1TextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cam1IPAddress);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(332, 135);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera - 1";
            // 
            // password1TextBox
            // 
            this.password1TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.password1TextBox.Location = new System.Drawing.Point(108, 90);
            this.password1TextBox.Name = "password1TextBox";
            this.password1TextBox.PasswordChar = '*';
            this.password1TextBox.Size = new System.Drawing.Size(215, 26);
            this.password1TextBox.TabIndex = 27;
            this.password1TextBox.Text = "passcode123";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(12, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 26;
            this.label3.Text = "Password";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // username1TextBox
            // 
            this.username1TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.username1TextBox.Location = new System.Drawing.Point(108, 54);
            this.username1TextBox.Name = "username1TextBox";
            this.username1TextBox.Size = new System.Drawing.Size(215, 26);
            this.username1TextBox.TabIndex = 25;
            this.username1TextBox.Text = "root";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(11, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 24;
            this.label2.Text = "Username";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cam1IPAddress
            // 
            this.cam1IPAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cam1IPAddress.Location = new System.Drawing.Point(108, 22);
            this.cam1IPAddress.Name = "cam1IPAddress";
            this.cam1IPAddress.Size = new System.Drawing.Size(215, 26);
            this.cam1IPAddress.TabIndex = 23;
            this.cam1IPAddress.Text = "192.168.123.154";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 22;
            this.label1.Text = "Camera IP:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.password2TextBox);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.username2TextBox);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.cam2IPAddress);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Location = new System.Drawing.Point(6, 166);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(332, 135);
            this.groupBox4.TabIndex = 31;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Camera - 2";
            // 
            // password2TextBox
            // 
            this.password2TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.password2TextBox.Location = new System.Drawing.Point(108, 90);
            this.password2TextBox.Name = "password2TextBox";
            this.password2TextBox.PasswordChar = '*';
            this.password2TextBox.Size = new System.Drawing.Size(215, 26);
            this.password2TextBox.TabIndex = 27;
            this.password2TextBox.Text = "passcode123";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(12, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 26;
            this.label4.Text = "Password";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // username2TextBox
            // 
            this.username2TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.username2TextBox.Location = new System.Drawing.Point(108, 54);
            this.username2TextBox.Name = "username2TextBox";
            this.username2TextBox.Size = new System.Drawing.Size(215, 26);
            this.username2TextBox.TabIndex = 25;
            this.username2TextBox.Text = "root";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.Location = new System.Drawing.Point(11, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 23);
            this.label7.TabIndex = 24;
            this.label7.Text = "Username";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cam2IPAddress
            // 
            this.cam2IPAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cam2IPAddress.Location = new System.Drawing.Point(108, 22);
            this.cam2IPAddress.Name = "cam2IPAddress";
            this.cam2IPAddress.Size = new System.Drawing.Size(215, 26);
            this.cam2IPAddress.TabIndex = 23;
            this.cam2IPAddress.Text = "192.168.123.155";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Location = new System.Drawing.Point(12, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 23);
            this.label8.TabIndex = 22;
            this.label8.Text = "Camera IP:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CameraServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "CameraServer";
            this.Size = new System.Drawing.Size(1077, 580);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox compIPAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox portNumber;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.TextBox response;

        #region Messenger
        public string ID
        {
            get { return "CameraServer"; }
        }

        public event ehSendMessage evSendMessage;
        public MessageEventArgs SendMessage(string sendto, MessageEventArgs msg)
        {
            if (evSendMessage != null)
            {
                return evSendMessage(sendto, this.ID, msg);
            }
            else
                return null;
        }

        public MessageEventArgs MessageHandler(string sendfrom, MessageEventArgs message)
        {
            if (message.MsgType == MessageEventArgs.MessageType.COMMAND)
            {
                /*if (message.Cmd == "Say")
                {            

                }*/
            }
            return null;
        }

        #endregion Messenger


        private void updateGUI(Boolean status)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<Boolean>(updateGUI), new object[] { status });
                return;
            }
            this.compIPAddress.Enabled = status;
            this.cam1IPAddress.Enabled = status;
            this.username1TextBox.Enabled = status;
            this.password1TextBox.Enabled = status;
            this.cam2IPAddress.Enabled = status;
            this.username2TextBox.Enabled = status;
            this.password2TextBox.Enabled = status;
            this.portNumber.Enabled = status;
            this.playButton.Enabled = status;
            this.stopButton.Enabled = !status;
            this.continueListening = !status;
        }

        public class HandleClient
        {
            private TcpClient clientSocket;
            private CameraServer server;
            private int clientNumber;

            public void startClient(TcpClient inClientSocket, CameraServer serv, int count)
            {
                this.clientSocket = inClientSocket;
                this.server = serv;
                this.clientNumber = count;
                Thread ctThread = new Thread(getResponse);
                ctThread.Start();
            }
            private void getResponse()
            {

                byte[] bytesFrom = new byte[100];
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    int b;
                    StringBuilder builder = new StringBuilder();

                    while ((b = (int)networkStream.ReadByte()) != -1)
                    builder.Append(Convert.ToChar(b));


                    string str = builder.ToString();
                    string displayStr;
                    displayStr = string.Copy(str);
                    // Display the output
                    if (!displayStr.EndsWith("\n"))
                        displayStr = displayStr + "\n";
                    this.server.AppendToTextBox(displayStr);

                    // Extract information from message
                    if (this.server.informNao(str))
                    {
                        

                        this.server.sendMessageToNao();
                    }

                    networkStream.Flush();
                    clientSocket.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error..... " + ex.StackTrace);
                }
            }

            
        }

        void sendMessageToNao()
        {
            SendMessage("NaoManager", new MessageEventArgs("PointToRaisedHands", new string[] { param.x , param.y }));
            SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "Do you have a question?" }));
        }

        bool informNao(String msg)
        {
            return parseCommand(msg);
         }

        public struct MessageParameters
        {
            public int camera;
            public long time;
            public string x;
            public string y;

        };

        bool parseCommand(string msg)
        {
            string par;
            par = msg.Substring(0, msg.IndexOf(','));
            msg = msg.Substring(par.Length + 1);
            int camera = Convert.ToInt32(par);

            par = msg.Substring(0, msg.IndexOf(','));
            msg = msg.Substring(par.Length + 1);
            long time = Convert.ToInt64(par);

            if (Math.Abs(param.time - time) < NAO_RESPONSE_DELAY)
                return false;

            param.time = time;
            param.camera = camera;

            par = msg.Substring(0, msg.IndexOf(','));
            msg = msg.Substring(par.Length + 1);
            param.x = par;

            param.y = msg;

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.response.Text = "";
        }
    }
}
