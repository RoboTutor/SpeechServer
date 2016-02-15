using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace SocketConnection
{
    public partial class ucSocketClient : UserControl
    {
        public ucSocketClient()
        {
            InitializeComponent();

            cbbClientIP.SelectedIndex = 0;
            cbbClientPort.SelectedIndex = 0;
        }

        string ClientIP;
        int ClientPort;

        //

        ReconSocketClient RecSocClient = null;
        StandardTcpClient StdTcpClient = null;

        public ReconSocketClient.DataReceivedEvent ClientOnDataRecFunc = null;
        public ReconSocketClient.ConnectionStateEvent ClientOnConnectFunc = null;
 
        public bool ClientUseRecSocket = false;

        #region Client
        bool btnStateConnect = false;
        private void btnClientConnect_Click(object sender, EventArgs e)
        {
            if (btnStateConnect == false)
            {
	            ClientIP = cbbClientIP.Text;
	            ClientPort = int.Parse(cbbClientPort.Text);

                Connect(ClientIP, ClientPort);
            } 
            else
            {
                if (ClientUseRecSocket)
                {
                }
                else
                {
                    StdTcpClient.Disconnect();
                }

                btnSocketClientConnect.Text = "Connect";
                btnStateConnect = false;
            }
        }

        public void Connect(string ip, int port)
        {
            if (ClientUseRecSocket)
            {
                RecSocClient = new ReconSocketClient(ip, port);
                RecSocClient.ConnectionState += new ReconSocketClient.ConnectionStateEvent(ClientOnConnectFunc);
                RecSocClient.DataReceived += new ReconSocketClient.DataReceivedEvent(ClientOnDataRecFunc);
            }
            else
            {
                StdTcpClient = new StandardTcpClient(ip, port);
                StdTcpClient.DataReceived += new EventHandler<DataReceivedEventArgs>(OnClientDataReceived);
                StdTcpClient.EvConnected += new EventHandler(OnConnected);
                //StdTcpClient.EvDisconnected += new EventHandler(OnDisconnected);
            }

            btnSocketClientConnect.Text = "Disconnect";
            btnStateConnect = true;
        }

        private void OnConnected(object sender, EventArgs e)
        {
            this.btnSocketClientConnect.BackColor = Color.Orange;
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            this.btnSocketClientConnect.BackColor = Color.LightGreen;
        }

        private void btnClientSend_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("SocketClient: Not Implemented!");
        }

        /// <summary>
        /// Format: "behaviorname|behaviorsucess"
        /// </summary>
        /// <param name="message"></param>
        public void SendRobotTutorMessage(string message)
        {
            string behaviorname = message.Split('|')[0];
            string behaviorsucess = message.Split('|')[1];
            Byte[] msg = CreateMsgBytes(behaviorname, behaviorsucess);

            try
            {
	            if (ClientUseRecSocket)
	            {
	            	RecSocClient.SendDataByte(msg);
	            } 
	            else
	            {
	                StdTcpClient.SendDataByte(msg);
	            }

                Invoke((MethodInvoker)delegate
                {
                    this.txtbOutput.Text = "Send: " + message + Environment.NewLine + this.txtbOutput.Text;
                });
                

	            Debug.WriteLine("MBC Client: Send RoboTutor Message: " + message + " Bytes Length: " + msg.Length);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Send Robotutor error:" + ex);
            }
        }

        void OnClientDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnReceiveRoboTutorMessage(e.Data);
        }

        void OnReceiveRoboTutorMessage(byte[] data)
        {

            byte[] msg_bytes;

            try
            {
	            ParseBufSizeInFront(data, out msg_bytes);
	
	            // test 
	            // string sRecieved = Encoding.ASCII.GetString(msg_bytes, 0, size);
	
	            RobotMessage msg = RobotMessage.CreateBuilder().MergeFrom(msg_bytes).Build();

                Debug.WriteLine("SocketClient receive behavior data: Name: "
                    + msg.BehaviorCmd.Behaviorname
                    + " Sucess: "
                    + msg.BehaviorCmd.Success
                    );

                // Show in UI
                Invoke((MethodInvoker)delegate
                {
                    this.txtbOutput.Text = "Received behavior: " + msg.BehaviorCmd.Behaviorname + Environment.NewLine + this.txtbOutput.Text;
                });

                // Test
                //SendRobotTutorMessage("Behavior|Done");
                ClientOnDataRecFunc("CallBehavior|" + msg.BehaviorCmd.Behaviorname);

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("!!!!!!!!! " + ex);
            }


        }
        #endregion Client


        private Byte[] CreateMsgBytes(string behaviorname, string behaviorsucess)
        {
            BehaviorCommand.Builder bcBuilder = BehaviorCommand.CreateBuilder();
            bcBuilder.SetBehaviorname(behaviorname);
            bcBuilder.SetSuccess(behaviorsucess);
            BehaviorCommand bc = bcBuilder.Build();

            ClientMessage.Builder msgBuilder = ClientMessage.CreateBuilder();
            msgBuilder.SetBehaviorCmd(bc);
            ClientMessage msg = msgBuilder.Build();

            byte[] bytes;
            using (MemoryStream stream = new MemoryStream())
            {
                //Save the person to a stream
                msg.WriteTo(stream);
                bytes = stream.ToArray();
            }

            byte[] bytes_with_size = AppendSizeInFront(bytes);
            return bytes_with_size;

            //return bytes;
        }

        private byte[] AppendSizeInFront(byte[] bytes, bool reverse = true)
        {
            int size = bytes.Length;

            // Transform an "int" size into byte[]
            byte[] intBytes = BitConverter.GetBytes(size);
            if (reverse == true)
            {
                Array.Reverse(intBytes);
            }
            byte[] size_in_bytes = intBytes;

            byte[] res_bytes = new byte[size_in_bytes.Length + bytes.Length];
            System.Buffer.BlockCopy(size_in_bytes, 0, res_bytes, 0, size_in_bytes.Length);
            System.Buffer.BlockCopy(bytes, 0, res_bytes, size_in_bytes.Length, bytes.Length);

            return res_bytes;
        }

        private int ParseBufSizeInFront(byte[] inbytes, out byte[] outbytes, bool reverse = true)
        {
            byte[] size_bytes = inbytes.Take(4).ToArray();
            Array.Reverse(size_bytes);
            int size = BitConverter.ToInt32(size_bytes, 0);
            byte[] msg_bytes = new byte[size];
            Array.Copy(inbytes, 4, msg_bytes, 0, size);

            outbytes = msg_bytes;

            return size;
        }
    }
}
