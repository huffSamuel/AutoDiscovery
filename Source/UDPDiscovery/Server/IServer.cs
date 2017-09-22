using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDiscovery.Server
{
    public interface IServer
    {
        void Start();
        void Stop();
        event EventHandler<ClientConnectedEventArgs> ClientConnected;
    }
}
