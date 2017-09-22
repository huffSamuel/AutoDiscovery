using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDiscovery
{
    public class ClientConnectedEventArgs : EventArgs
    {


        public string IPAddress
        {
            get;
            private set;
        }

        public byte[] Message
        {
            get;
            private set;
        }

        public ClientConnectedEventArgs(string ipAddress, byte[] message)
        {
            this.IPAddress = ipAddress;
            this.Message = message;
        }
    }
}
