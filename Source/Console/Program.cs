using AutoDiscovery.Server;
using System;

namespace ConsoleTest
{
    class Program
    {
        static private int Port = 45982;
       

        static void Main(string[] args)
        {
            IServer serv;
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
