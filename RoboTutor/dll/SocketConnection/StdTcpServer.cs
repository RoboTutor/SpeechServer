using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace SocketConnection
{
    public class IPEndPointEventArgs : EventArgs
    {
        public IPEndPointEventArgs(IPEndPoint ipEndPoint)
        {
            IPEndPoint = ipEndPoint;
        }

        public IPEndPoint IPEndPoint { get; private set; }
    }

    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(byte[] data, IPEndPoint ipEndPoint)
        {
            Data = data;
            IPEndPoint = ipEndPoint;
        }

        public byte[] Data { get; private set; }
        public IPEndPoint IPEndPoint { get; private set; }

    }
    /// <summary>
    /// TcpListner wrapper
    /// Encapsulates asynchronous communications using TCP/IP.
    /// </summary>
    public sealed class StandardTcpServer : IDisposable
    {
        //----------------------------------------------------------------------
        //Construction, Destruction
        //----------------------------------------------------------------------
        /// <summary>
        /// Creating server socket
        /// </summary>
        /// <param name="port">Server port number</param>
        public StandardTcpServer(int port)
        {
            connectedSockets = new Dictionary<IPEndPoint, Socket>();
            tcpServer = new TcpListener(IPAddress.Any, port);
            tcpServer.Start();
            tcpServer.BeginAcceptSocket(EndAcceptSocket, tcpServer);
        }

        ~StandardTcpServer()
        {
            DisposeImpl(false);
        }

        public void Dispose()
        {
            DisposeImpl(true);
        }

        //----------------------------------------------------------------------
        //Public Methods
        //----------------------------------------------------------------------
        Object LockSyncHandle = new Object();
        public void SendData(byte[] data, IPEndPoint endPoint)
        {
            Socket sock;
            lock (LockSyncHandle)
            {
                if (!connectedSockets.ContainsKey(endPoint))
                    return;
                sock = connectedSockets[endPoint];
            }

            try
            {
            	sock.Send(data);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Server send data error: " + ex);
            }
        }

        //----------------------------------------------------------------------
        //Events
        //----------------------------------------------------------------------
        public event EventHandler<IPEndPointEventArgs> SocketConnected;
        public event EventHandler<IPEndPointEventArgs> SocketDisconnected;
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        //----------------------------------------------------------------------
        //Private Functions
        //----------------------------------------------------------------------
        #region Private Functions
        // Processing of the new compounds
        private void Connected(Socket socket)
        {
            var endPoint = (IPEndPoint)socket.RemoteEndPoint;

            lock (connectedSocketsSyncHandle)
            {
                if (connectedSockets.ContainsKey(endPoint))
                {
                    Debug.WriteLine("TcpServer.Connected: Socket already connected! Removing from local storage! EndPoint: {0}", endPoint);
                    connectedSockets[endPoint].Close();
                }

                SetDesiredKeepAlive(socket);
                connectedSockets[endPoint] = socket;
            }

            OnSocketConnected(endPoint);
        }

        private static void SetDesiredKeepAlive(Socket socket)
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            const uint time = 10000;
            const uint interval = 20000;
            SetKeepAlive(socket, true, time, interval);
        }

        static void SetKeepAlive(Socket s, bool on, uint time, uint interval)
        {
            /* the native structure
            struct tcp_keepalive {
            ULONG onoff;
            ULONG keepalivetime;
            ULONG keepaliveinterval;
            };
            */

            // marshal the equivalent of the native structure into a byte array
            uint dummy = 0;
            var inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)(on ? 1 : 0)).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)time).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)interval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);
            // of course there are other ways to marshal up this byte array, this is just one way

            // call WSAIoctl via IOControl
            int ignore = s.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);

        }

        //socket disconnected handler
        private void Disconnect(Socket socket)
        {
            var endPoint = (IPEndPoint)socket.RemoteEndPoint;

            lock (connectedSocketsSyncHandle)
            {
                connectedSockets.Remove(endPoint);
            }

            socket.Close();

            OnSocketDisconnected(endPoint);
        }

        private void ReceiveData(byte[] data, IPEndPoint endPoint)
        {
            OnDataReceived(data, endPoint);
        }

        private void EndAcceptSocket(IAsyncResult asyncResult)
        {
            var listener = (TcpListener)asyncResult.AsyncState;
            Debug.WriteLine("TcpServer.EndAcceptSocket");
            if (disposed)
            {
                Debug.WriteLine("TcpServer.EndAcceptSocket: tcp server already disposed!");
                return;
            }

            try
            {
                Socket sock;
                try
                {
                    sock = listener.EndAcceptSocket(asyncResult);
                    Debug.WriteLine("TcpServer.EndAcceptSocket: remote end point: {0}", sock.RemoteEndPoint);
                    Connected(sock);
                }
                finally
                {
                    //EndAcceptSocket can failes, but in any case we want to accept new connections
                    listener.BeginAcceptSocket(EndAcceptSocket, listener);
                }

                //we can use this only from .net framework 2.0 SP1 and higher
                var e = new SocketAsyncEventArgs();
                e.Completed += ReceiveCompleted;
                e.SetBuffer(new byte[SocketBufferSize], 0, SocketBufferSize);
                BeginReceiveAsync(sock, e);

            }
            catch (SocketException ex)
            {
                Debug.WriteLine("TcpServer.EndAcceptSocket: failes!", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TcpServer.EndAcceptSocket: failes!", ex);
            }
        }

        private void BeginReceiveAsync(Socket sock, SocketAsyncEventArgs e)
        {
            if (!sock.ReceiveAsync(e))
            {//IO operation finished synchronously
                //handle received data
                ReceiveCompleted(sock, e);
            }//IO operation finished synchronously
        }

        void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            var sock = (Socket)sender;
            if (!sock.Connected)
                Disconnect(sock);
            try
            {

                int size = e.BytesTransferred;
                if (size == 0)
                {
                    //this implementation based on IO Completion ports, and in this case
                    //receiving zero bytes mean socket disconnection
                    Disconnect(sock);
                }
                else
                {
                    var buf = new byte[size];
                    Array.Copy(e.Buffer, buf, size);
                    ReceiveData(buf, (IPEndPoint)sock.RemoteEndPoint);
                    BeginReceiveAsync(sock, e);
                }
            }
            catch (SocketException ex)
            {
                //We can't truly handle this exception here, but unhandled
                //exception caused process termination.
                //You can add new event to notify observer
                Debug.WriteLine("TcpServer: receive data error!", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TcpServer: receive data error!", ex);
            }
        }

        private void DisposeImpl(bool manualDispose)
        {
            if (manualDispose)
            {
                //We should manually close all connected sockets
                Exception error = null;
                try
                {
                    if (tcpServer != null)
                    {
                        disposed = true;
                        tcpServer.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("TcpServer: tcpServer.Stop() failes!", ex);
                    error = ex;
                }

                try
                {
                    foreach (var sock in connectedSockets.Values)
                    {
                        sock.Close();
                    }
                }
                catch (SocketException ex)
                {
                    //During one socket disconnected we can faced exception
                    Debug.WriteLine("TcpServer: close accepted socket failes!", ex);
                    error = ex;
                }
                if ( error != null )
                    throw error;
            }
        }


        private void OnSocketConnected(IPEndPoint ipEndPoint)
        {
            var handler = SocketConnected;
            if (handler != null)
                handler(this, new IPEndPointEventArgs(ipEndPoint));
        }

        private void OnSocketDisconnected(IPEndPoint ipEndPoint)
        {
            var handler = SocketDisconnected;
            if (handler != null)
                handler(this, new IPEndPointEventArgs(ipEndPoint));
        }

        private void OnDataReceived(byte[] data, IPEndPoint ipEndPoint)
        {
            var handler = DataReceived;
            if ( handler != null )
                handler(this, new DataReceivedEventArgs(data, ipEndPoint));
        }

        #endregion Private Functions

        //----------------------------------------------------------------------
        //Private Fields
        //----------------------------------------------------------------------
        #region Private Fields
        private const int SocketBufferSize = 1024;
        private readonly TcpListener tcpServer;
        private bool disposed;
        private readonly Dictionary<IPEndPoint, Socket> connectedSockets;
        private readonly object connectedSocketsSyncHandle = new object();
        #endregion Private Fields
    }
}
