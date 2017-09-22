using AutoDiscovery.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client(45982, 2000);
            client.ServerDiscovered += OnServerFound;
            client.Start();
            Console.WriteLine("Client broadcasting. Press ENTER to stop.");
            Console.ReadLine();
            client.Stop();
        }


        private static void OnServerFound(object sender, AutoDiscovery.ServerDiscoveredEventArgs e)
        {
            var bg = Console.BackgroundColor;
            var fg = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Server found at " + e.IPAddress);

            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
        }
    }
}
