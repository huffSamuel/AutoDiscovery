using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AutoDiscovery.Client
{
    public class Client : IClient
    {
        public int Port { get; private set; }
        public int PollingTime { get; set; }
        private CancellationTokenSource tokenSource;
        private AutoResetEvent messageReceived;

        public Client(int port, int pollingTime)
        {
            this.Port = port;
            this.PollingTime = pollingTime;
            messageReceived = new AutoResetEvent(false);
            tokenSource = new CancellationTokenSource();
        }

        public event EventHandler<ServerDiscoveredEventArgs> ServerDiscovered;

        public void Start()
        {
            Task.Factory.StartNew(Broadcast, tokenSource.Token);
        }

        public void Stop()
        {
            tokenSource.Cancel();
        }

        private void Broadcast()
        {
            UdpClient client = new UdpClient();
            client.AllowNatTraversal(true);
            client.EnableBroadcast = true;

            while(!tokenSource.IsCancellationRequested)
            {
                CancellationTokenSource internalSource = new CancellationTokenSource();

                IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Any, Port);

                byte[] data = new byte[] { 0x00 };

                client.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, Port));

                Task.Factory.StartNew(() =>
                {
                    client.Receive(ref serverEndpoint);
                    messageReceived.Set();
                }
                , internalSource.Token);

                if(messageReceived.WaitOne(PollingTime))
                {
                    ServerDiscoveredEventArgs args = new ServerDiscoveredEventArgs(serverEndpoint.Address.ToString(), new byte[] { 0x00 });
                    ServerDiscovered?.Invoke(this, args);
                }

                internalSource.Cancel();
            }
        }        
    }
}
