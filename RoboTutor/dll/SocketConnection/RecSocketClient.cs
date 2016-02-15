using System;
using System.Text;
using DotNetUtils;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;


namespace SocketConnection
{
    public class ReconSocketClient
    {
        public delegate void ConnectionStateEvent(bool state);
        public event ConnectionStateEvent ConnectionState;

        public delegate void DataReceivedEvent(string text);
        public event DataReceivedEvent DataReceived;

        ReconnectingSocket RecSocketClient;

        public bool Verbose
        {
            get
            {
                return RecSocketClient.Verbose;
            }
            set
            {
                RecSocketClient.Verbose = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="ipport"></param>
        /// <param name="usePrefix"></param>
        public ReconSocketClient(string ipaddress, int ipport, bool usePrefix = false)
        {
            //int port = AppConfigReader.GetIntKeyValue("Port");
            //string IPAddress = AppConfigReader.GetStringKeyValue("IPAddress");

            // RecSocket used come additional message packing
            RecSocketClient = new ReconnectingSocket();
            RecSocketClient.Verbose = true;
            RecSocketClient.InitClient(ipaddress, ipport, false);
            RecSocketClient.OnConnection += new ReconnectingSocket.OnConnectionEvent(OnSocketClientConnection);
            RecSocketClient.OnData += new ReconnectingSocket.OnDataEvent(OnSocketClientReceiveData);
        }

        void OnSocketClientReceiveData(byte[] data)
        {
            string strData = Encoding.ASCII.GetString(data);
            if (strData.Equals("\0") == false)
            {
                strData = strData.Replace("\0", "");
                if (DataReceived != null)
                {
                    strData = strData.Replace(Environment.NewLine, "");
                    DataReceived(strData);
                }
            }
        }

        void OnSocketClientConnection(int value)
        {
            mConnected = (value == 1);
            if (ConnectionState != null)
                ConnectionState(mConnected);
        }

        bool mConnected = false;

        public void StopClient()
        {
            if (mConnected == true)
            {
                RecSocketClient.Close();
                mConnected = false;
                if (ConnectionState != null)
                    ConnectionState(mConnected);
            }
        }

        public void SendData(string text)
        {
            // Send message
            RecSocketClient.SendData(text + '\n');
        }

        public void SendDataByte(byte[] bytes)
        {
            // Reconnecting socket did extra message wrapper
            RecSocketClient.SendData(bytes);
        }
    }

}
