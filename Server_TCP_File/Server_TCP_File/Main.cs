using System;
using System.Threading;

namespace Server_TCP_File
{
    class MainProgram
    {
        public static void Main(string[] args)
        {
            Thread serverThread = new Thread(() =>
            {
                Server server = new Server();
                server.Start();
            });

            serverThread.Start();
        }
    }
}
