using AutoDiscovery.Client;
using AutoDiscovery.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static private int Port = 45982;
        static Server serv;

        static void Main(string[] args)
        {
            serv = new Server(Port);

            serv.ClientConnected += OnClientConnected;
            serv.Start();
            Console.WriteLine("Server is listening. Press ENTER to stop.");
            Console.ReadLine();
            serv.Stop();
        }

        private static void OnClientConnected(object sender, AutoDiscovery.ClientConnectedEventArgs e)
        {
            var bg = Console.BackgroundColor;
            var fg = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.WriteLine("Client connected from " + e.IPAddress);

            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
        }
        

    }
}
