using System;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

namespace Networking
{
    class Server
    {
        private TcpListener _tcpListener;
        private Thread _listenThread;
        private volatile bool _running;
        private volatile bool _newData;
        private String _message;
        private object _syncLock;

        public Server(string port)
        {
            _running = true;
            _newData = false;
            _syncLock = new object();

            this._tcpListener = new TcpListener(IPAddress.Any, Int32.Parse(port));
            this._listenThread = new Thread(new ThreadStart(ListenForClients));

            this._listenThread.Start();
        }

        public void Stop()
        {
            _running = false;
            _listenThread.Join();
        }

        public void Send(string message)
        {
            lock (_syncLock)
            {
                _newData = true;
                _message = String.Copy(message);
            }
        }

        // As written only handles a single client (_newData and _message are the limits)t
        private void ListenForClients()
        {
            this._tcpListener.Start();

            while (_running)
            {
                // Blocks until a client has connected to the server
                TcpClient client = this._tcpListener.AcceptTcpClient();

                // Create a thread to handle communication with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();
            bool _error = false;

            while (_running && !_error)
            {
                Thread.Sleep(20);

                if (!tcpClient.Connected)
                {
                    _error = true;
                    continue;
                }

                lock (_syncLock)
                {
                    if (_newData)
                    {
                        byte[] buffer = encoder.GetBytes(_message + "\n");

                        try
                        {
                            clientStream.Write(buffer, 0, buffer.Length);
                            clientStream.Flush();
                            _newData = false;
                        }
                        catch (IOException)
                        {
                            _error = true;
                        }
                    }
                }
            }

            tcpClient.Close();
        }
    }
}
