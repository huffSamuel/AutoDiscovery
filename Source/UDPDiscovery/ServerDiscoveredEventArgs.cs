using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDiscovery
{
    public class ServerDiscoveredEventArgs : EventArgs
    {
        public byte[] Message
        {
            get;
            private set;
        }

        public string IPAddress
        {
            get;
            private set;
        }

        public ServerDiscoveredEventArgs(string ipAddress, byte[] message)
        {
            this.IPAddress = ipAddress;
            this.Message = message;
        }

    }
}
