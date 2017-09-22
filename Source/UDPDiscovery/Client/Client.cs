using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

        public void Broadcast()
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

                UdpState state = new UdpState()
                {
                    Endpoint = serverEndpoint,
                    Server = client
                };

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

        private void ReceiveCallback(IAsyncResult result)
        {
            UdpClient client = ((UdpState)(result.AsyncState)).Server;
            IPEndPoint endpoint = ((UdpState)(result.AsyncState)).Endpoint;

            byte[] receivedBytes = client.EndReceive(result, ref endpoint);

            if (endpoint.Address.ToString() == "0.0.0.0")
            {
                return;
            }

            ServerDiscoveredEventArgs args = new ServerDiscoveredEventArgs(endpoint.Address.ToString(), receivedBytes);
            ServerDiscovered?.Invoke(this, args);

            messageReceived.Set();
        }

        
    }
}
