using System;
using System.Net;
using System.Net.Sockets;
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

        public int Port { get; private set; }

        private AutoResetEvent messageReceived = new AutoResetEvent(false);
        private CancellationTokenSource tokenSource = new CancellationTokenSource();


        public Server(int port)
        {
            this.Port = port;
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

    	public event EventHandler<ClientConnectedEventArgs> ClientConnected;
    }
}
