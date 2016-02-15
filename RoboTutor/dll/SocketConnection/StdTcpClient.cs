using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;

namespace SocketConnection
{
    public class StandardTcpClient
    {
        TcpClient mTcpClient;

        IPAddress IP;
        int Port;

        private Socket m_sock = null;
        /// <summary>
        /// If the connection already exists it is destroyed. 
        /// A Socket is then created and an end point established. 
        /// The commented out code allows for the simpler blocking connection attempt. 
        /// BeginConnect is used to commence a non blocking connection attempt. 
        /// Note, even if a non-blocking connection is attempted, 
        /// the connection will block until the machine name is resolved into an IP address, 
        /// for this reason it is better to use the IP address than the machine name if possible to avoid blocking. 
        /// The following method is called once the connection attempt is complete, 
        /// it displays connection error or sets up the receive data callback if connected OK.
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="ipport"></param>
        /// <param name="usePrefix"></param>
        public StandardTcpClient(string ipaddress, int ipport, bool usePrefix = true)
        {            
            // CSharp standard TCP/IP socket
            IP = IPAddress.Parse(ipaddress);
            Port = ipport;

            try
            {
                mTcpClient = new TcpClient();
                

                //TcpClient.Connect(IP, Port);

                // Close the socket if it is still open
                if (m_sock != null && m_sock.Connected)
                {
                    m_sock.Shutdown(SocketShutdown.Both);
                    System.Threading.Thread.Sleep(10);
                    m_sock.Close();
                }

                // Create the socket object
                m_sock = new Socket(AddressFamily.InterNetwork,
                         SocketType.Stream, ProtocolType.Tcp);

                // Define the Server address and port
                IPEndPoint epServer =
                  new IPEndPoint(IP, Port);

                // Connect to the server blocking method
                // and setup callback for received data
                // m_sock.Connect( epServer );
                // SetupRecieveCallback( m_sock );

                // Connect to server non-Blocking method
                m_sock.Blocking = false;
                AsyncCallback onconnect = new AsyncCallback(OnClientConnect);
                m_sock.BeginConnect(epServer, onconnect, m_sock);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TCP Client failed to connect to server: " + ex);
            }
        }

        public void Disconnect()
        {
            if (m_sock.Connected == true)
            {
            	m_sock.Disconnect(false);
            }
        }

        // http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.connected.aspx
        public event EventHandler EvDisconnected;
        public event EventHandler EvConnected;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        void OnClientConnect(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;

            // Check if we were successful
            try
            {
                //    sock.EndConnect( ar );
                if (sock.Connected)
                    SetupReceiveCallback(sock);
                else
                    Debug.WriteLine("Socket unable to connect to remote machine!");

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unusual error during Connect: " + ex);
            }

            if(EvConnected != null) EvConnected(this, null);
        }

        private byte[] m_byBuff = new byte[256];    // Received data buffer
        /// <summary>
        /// To receive data asynchronously, it is necessary to setup 
        /// an AsyncCallback to handle events triggered by the Socket 
        /// such as new data and loss of connection. 
        /// This is done using the following method;
        /// </summary>
        /// <param name="sock"></param>
        public void SetupReceiveCallback(Socket sock)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                sock.BeginReceive(m_byBuff, 0, m_byBuff.Length,
                                   SocketFlags.None, recieveData, sock);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Setup Receive Callback failed: " + ex);
            }
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// The SetupRecieveCallback method starts a BeginReceive 
        /// using a delegate pointing to the OnReceveData method that follows. 
        /// It also passes a buffer for the receive data to be inserted into.
        /// 
        /// When the above event is fired the receive data is assumed to be ASCII. 
        /// The new data is sent to the display by invoking a delegate. 
        /// Although it is possible to call Add() on the list to display the new data, 
        /// it is a very bad idea because the received data will most likely be running in another thread. 
        /// Note the receive callback must also be established again to continue to receive more events. 
        /// Even if more data was received than can be placed in the input buffer, 
        /// reestablishing the receive callback will cause it to trigger until all data has been read.
        /// </summary>
        /// <param name="ar"></param>
        public void OnRecievedData(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;

            // Check if we got any data
            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    byte[] data = m_byBuff.Take(nBytesRec).ToArray();
                    DataReceived(this, new DataReceivedEventArgs(data, (IPEndPoint)sock.RemoteEndPoint));

                    // If the connection is still usable reestablish the callback
                    SetupReceiveCallback(sock);
                }
                else
                {
                    // If no data was received then the connection is probably dead
                    Console.WriteLine("Client {0}, disconnected", sock.RemoteEndPoint);
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unusual error during Receive: " + ex);
            }
        }

        public void SendDataByte(byte[] bytes)
        {
            // Check we are connected
            if (m_sock == null || !m_sock.Connected)
            {
                Debug.WriteLine("Must be connected to Send a message");
                return;
            }

            try
            {
                m_sock.Send(bytes, bytes.Length, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Client: Send Message Failed: " + ex);
            }


            // Send the message to the connected TcpServer.
            //NetworkStream tcpstream = mTcpClient.GetStream();
            //tcpstream.Write(bytes, 0, bytes.Length);
        }

    }
}
