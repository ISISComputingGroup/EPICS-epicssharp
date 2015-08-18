using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CaSharpServer;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace test_server
{
    class Program
    {
        static int counter = 0;
        static CAIntRecord intRecord;
        static CAStringRecord strRecord;

        static void Main(string[] args)
        {
            int port = 5064;          
            CAServer server = null;

            try
            {
                //null = IP.Any
                server = new CAServer(null, port, 5064, 5065);
                Console.WriteLine("Connected to configured TCP port (" + port + ")");
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    //the port is already in use, so ask the OS for a free port
                    server = new CAServer(null, 0, 5064, 5065);
                    Console.WriteLine("Configured TCP port was unavailable.");
                    Console.WriteLine("Using dynamically assigned TCP port " + server.TcpPort);
                }
                else
                {
                    Console.WriteLine("Could not create CAServer: " + e.Message);
                }
            }

            intRecord = server.CreateRecord<CAIntRecord>("TESTSERVER:INT");
            intRecord.PrepareRecord += new EventHandler(intRecord_PrepareRecord);
            intRecord.Scan = CaSharpServer.Constants.ScanAlgorithm.SEC5;

            strRecord = server.CreateRecord<CAStringRecord>("TESTSERVER:STR");
            strRecord.Value = "Default";

            Console.ReadLine();
            server.Dispose();
        }

        static void intRecord_PrepareRecord(object sender, EventArgs e)
        {
            counter++;
            intRecord.Value = counter;
            if (counter > 100) counter = 0;
        }
    }
}
