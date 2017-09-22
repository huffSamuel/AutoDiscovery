using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoDiscovery.Server
{
    public class Server : IServer
    {
        protected class ClientMessage
        {
            public string ipAddress;
            public byte[] Message;
            public ClientMessage(string ipAddress, byte[] message)
            {
                this.ipAddress = ipAddress;
                this.Message = message;
            }
        }

        private ConcurrentQueue<ClientMessage> messages;
        public int Port { get; private set; }
        public List<string> DiscoveredClients
        {
            get;
            private set;
        }

        private AutoResetEvent messageReceived = new AutoResetEvent(false);
        private CancellationTokenSource tokenSource = new CancellationTokenSource();


        public Server(int port)
        {
            this.Port = port;
            messages = new ConcurrentQueue<ClientMessage>();
        }

        public void Send(string ipAddress, byte[] message)
        {
            messages.Enqueue(new ClientMessage(ipAddress, message));
        }

    	public void Start()
    	{
            Task.Factory.StartNew(Listen, tokenSource.Token);
        }

        public void Stop()
    	{
            tokenSource.Cancel();
    	}

        private void Listen()
        {
            
            IPEndPoint e = new IPEndPoint(IPAddress.Any, Port);
            UdpClient server = new UdpClient(e);
            server.EnableBroadcast = true;
            byte[] data = new byte[] { 0x00 };
            byte[] dataIn = null;

            while(!tokenSource.IsCancellationRequested)
            {
                CancellationTokenSource internalSource = new CancellationTokenSource();
                e = new IPEndPoint(IPAddress.Any, Port);

                UdpState state = new UdpState()
                {
                    Endpoint = e,
                    Server = server
                };

                Task.Factory.StartNew(() =>
                {
                    dataIn = server.Receive(ref e);
                    messageReceived.Set();
                }, internalSource.Token);

                if(messageReceived.WaitOne(100))
                {
                    ClientConnectedEventArgs args = new ClientConnectedEventArgs(e.Address.ToString(), dataIn);
                    ClientConnected?.Invoke(this, args);
                    server.Send(data, 1, e);
                }

                internalSource.Cancel();
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            UdpClient client = ((UdpState)(result.AsyncState)).Server;
            IPEndPoint endpoint = ((UdpState)(result.AsyncState)).Endpoint;

            try
            {
                byte[] receivedBytes = client.EndReceive(result, ref endpoint);

                string clientIP = endpoint.Address.ToString();

                ClientConnectedEventArgs args = new ClientConnectedEventArgs(clientIP, receivedBytes);
                ClientConnected?.Invoke(this, args);

                IPEndPoint responseEndpoint = new IPEndPoint(endpoint.Address, Port);
                client.Send(receivedBytes, receivedBytes.Length);
            }
            catch(Exception exc)
            {
                
            }
            messageReceived.Set();   
        }

    	public event EventHandler<ClientConnectedEventArgs> ClientConnected;
    }
}
