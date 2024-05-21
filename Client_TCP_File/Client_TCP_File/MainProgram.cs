using System;
using System.Threading;

namespace Client_TCP_File
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.Start();
        }

    }
}
