using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDiscovery.Client
{
    public interface IClient
    {
        void Start();
        void Stop();
        event EventHandler<ServerDiscoveredEventArgs> ServerDiscovered;
    }
}
