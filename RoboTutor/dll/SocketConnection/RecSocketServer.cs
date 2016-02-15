using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nito.Async.Sockets;
using DotNetUtils;
using System.Diagnostics;

namespace SocketConnection
{
    public class ReconSocketServer
    {
        private ReconnectingSocket RecSocketServer;

        public delegate void ConnectionStateEvent(bool state);
        public event ConnectionStateEvent ConnectionState;

        public delegate void DataReceivedEvent(string text);
        public event DataReceivedEvent DataReceived;

        public ReconSocketServer(int ipport)
        {
            //int port = AppConfigReader.GetIntKeyValue("Port");
            //private string IPAddress = AppConfigReader.GetStringKeyValue("IPAddress");

            try
            {
	            RecSocketServer = new ReconnectingSocket();
	            RecSocketServer.Verbose = true;
	            RecSocketServer.InitServer(ipport, false);
	            RecSocketServer.OnConnection += new ReconnectingSocket.OnConnectionEvent(OnSocketServerConnection);
	            RecSocketServer.OnData += new ReconnectingSocket.OnDataEvent(OnServerSocketData);

                Debug.WriteLine("TCP server is listening on port: " + ipport);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("TCP server establish error: " + ex);
            }

        }

        public bool Verbose
        {
            get
            {
                return RecSocketServer.Verbose;
            }
            set
            {
                RecSocketServer.Verbose = value;
            }
        }


        bool mConnected = false;

        void OnServerSocketData(byte[] data)
        {
            string strData = Encoding.ASCII.GetString(data);
            if (strData.Equals("\0") == false)
            {
                strData = strData.Replace("\0", "");
                if (DataReceived != null)
                {
                    DataReceived(strData);
                }
            }
        }

        void OnSocketServerConnection(int value)
        {
            mConnected = (value == 1);
            if (ConnectionState != null)
                ConnectionState(mConnected);
        }

        public void StopServer()
        {
            if (mConnected == true)
            {
                RecSocketServer.Close();
                mConnected = false;
                if (ConnectionState != null)
                    ConnectionState(mConnected);
            }
        }

        public void SendData(string text)
        {
            // Send message
            RecSocketServer.SendData(text);
            //Debug.WriteLine("Finish sending");
        }

        void OnReceiveRoboTutorMessage(byte[] data)
        {
            byte[] size_bytes = data.Take(4).ToArray();
            int size = BitConverter.ToInt32(size_bytes, 0);
            byte[] msg_bytes = new byte[size];
            data.CopyTo(msg_bytes, size);

            RobotMessage msg = RobotMessage.CreateBuilder().MergeFrom(msg_bytes).Build();

            Debug.Write("ReconnectingSocket receive behavior data: Name: "
                + msg.BehaviorCmd.Behaviorname
                + " Sucess: "
                + msg.BehaviorCmd.Success
                );
        }
    }
}
