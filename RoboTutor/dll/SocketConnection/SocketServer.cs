using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace SocketConnection
{
    public partial class ucSocketServer : UserControl
    {

        bool ServerUseRecSocket = false;
        ReconSocketServer RecSocServer = null;
        StandardTcpServer StdTcpServer = null;
        public ReconSocketServer.DataReceivedEvent ServerOnDataRecFunc = null;
        public ReconSocketServer.ConnectionStateEvent ServerOnConnectFunc = null;

        public ucSocketServer()
        {
            InitializeComponent();
        }

        #region Server
        /// <summary>
        /// 
        /// </summary>
        /// <param name="confunc"></param>
        /// <param name="datafunc"></param>
        public void StartTcpServer(
            ReconSocketServer.ConnectionStateEvent confunc = null,
            ReconSocketServer.DataReceivedEvent datafunc = null)
        {
            int port = 1986; // read from app.config later

            if (ServerUseRecSocket)
            {
                RecSocServer = new ReconSocketServer(port);
                if (ServerOnConnectFunc != null)
                {
                    RecSocServer.ConnectionState += new ReconSocketServer.ConnectionStateEvent(ServerOnConnectFunc);
                }
                if (ServerOnDataRecFunc != null)
                {
                    RecSocServer.DataReceived += new ReconSocketServer.DataReceivedEvent(ServerOnDataRecFunc);
                }
            }
            else
            {
                StdTcpServer = new StandardTcpServer(port);
                StdTcpServer.DataReceived += new EventHandler<DataReceivedEventArgs>(OnServerDataReceived);
                StdTcpServer.SocketConnected += new EventHandler<IPEndPointEventArgs>(OnServerConnected);
                StdTcpServer.SocketDisconnected += new EventHandler<IPEndPointEventArgs>(OnServerDisconnected);
            }
        }

        void OnServerDisconnected(object sender, IPEndPointEventArgs e)
        {

        }

        IPEndPoint ClientIPEP = null;
        void OnServerConnected(object sender, IPEndPointEventArgs e)
        {
            //IPEndPointEventArgs e_ = new 
            this.ClientIPEP = new IPEndPoint(e.IPEndPoint.Address, e.IPEndPoint.Port);


            IPEndPoint ipEndPoint = e.IPEndPoint;
            IPAddress ip = ipEndPoint.Address;
            int port = ipEndPoint.Port;
            string s = "Client connected: " + ip.ToString() + ":" + port;
            UpdateConnectedClients(s);

        }

        public void UpdateConnectedClients(string s)
        {
            Invoke((MethodInvoker)delegate
            {
                this.txtbConnectedClients.Text += s + Environment.NewLine;
            });
        }

        void OnServerDataReceived(object sender, DataReceivedEventArgs e)
        {
            byte[] msg;
            int size = ParseBufSizeInFront(e.Data, out msg);

            // test
            //string sRecieved = Encoding.ASCII.GetString(msg, 0, size);
            RobotMessage cmsg = RobotMessage.CreateBuilder().MergeFrom(msg).Build();
            string sRecieved = cmsg.BehaviorCmd.Behaviorname + "|" + cmsg.BehaviorCmd.Success;

            ServerOnDataRecFunc(sRecieved);
        }

        public void ServerSendData(string p)
        {
            if (ServerUseRecSocket)
            {
                RecSocServer.SendData(p);
            }
            else
            {
                //StdTcpServer.SendData();
            }
        }

        public void SendMBCMessage(string message)
        {
            string behaviorname = message.Split('|')[0];
            string behaviorsucess = message.Split('|')[1];
            Byte[] msg = CreateMsgBytes(behaviorname, behaviorsucess);

            if (ServerUseRecSocket)
            {
                //RecSocServer.SendDataByte(msg);
            }
            else
            {
                this.StdTcpServer.SendData(msg, this.ClientIPEP);
            }

            Debug.WriteLine("MBC Client: Send RoboTutor Message: " + message + " Bytes Length: " + msg.Length);
        }

        private void btnServerSend_Click(object sender, EventArgs e)
        {
            string s = this.txtbServerMessage.Text;
            if (ServerUseRecSocket)
            {
            }
            else
            {
                byte[] buf = System.Text.Encoding.ASCII.GetBytes(s);
                byte[] buftosend = AppendSizeInFront(buf);
                this.StdTcpServer.SendData(buftosend, this.ClientIPEP);
            }
        }

        #endregion Servers

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
    }
}
