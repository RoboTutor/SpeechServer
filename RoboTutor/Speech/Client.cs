using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Speech
{
    class Client
    {
        string host;
        int port;
        TcpClient socket;

        public Client(string h, int p)
        {
            this.host = h;
            this.port = p;
        }

        public void connect()
        {
            socket = new TcpClient();
            try
            {
                socket.Connect(this.host, this.port);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void send(NetworkStream ns, string s)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(s);
            ns.Write(buffer, 0, buffer.Length);
        }

        public string read(NetworkStream ns)
        {
            byte[] buffer = new byte[socket.ReceiveBufferSize];
            while (true)
            {
                int bytesRead = ns.Read(buffer, 0, buffer.Length);
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                if (dataReceived.Length > 0)
                    return dataReceived;
            }
        }

        public string run()
        {
            //if (socket.Connected)
            //{
                NetworkStream ns = socket.GetStream();
                send(ns, "start");
                return read(ns);
            //}
            //return "nee";
        }

        public void close()
        {
            try
            {
                NetworkStream ns = socket.GetStream();
                send(ns, "stop");
                socket.Close();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
