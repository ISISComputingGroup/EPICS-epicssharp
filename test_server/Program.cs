using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CaSharpServer;
using System.Net;
using System.Net.NetworkInformation;

namespace test_server
{
    class Program
    {
        static int counter = 0;
        static CAIntRecord intRecord;
        static CAStringRecord strRecord;

        static String getIP()
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            String ip = (
                       from addr in hostEntry.AddressList
                       where addr.AddressFamily.ToString() == "InterNetwork"
                       select addr.ToString()
                ).FirstOrDefault();
            return ip;
        }

        static int findFreePort()
        {
            int port = 5064;  //Try the default first

            if (isPortUsed(port))
            {
                port = 10000;

                while (isPortUsed(port))
                {
                    ++port;
                }
            }

            return port;
        }

        private static bool isPortUsed(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            IPEndPoint[] udpListerners = ipGlobalProperties.GetActiveUdpListeners();

            //check tcp
            foreach (TcpConnectionInformation tcp in tcpConnInfoArray)
            {
                if (tcp.LocalEndPoint.Port == port)
                {
                    return true;
                }
            }

            //check udp
            for (int i = 0; i < udpListerners.Length; ++i)
            {
                if (udpListerners[i].Port == port)
                {
                    return true;
                }
            }

            return false;
        }

        static void Main(string[] args)
        {
            int port = findFreePort();
            Console.WriteLine("Connecting to tcp port: " + port);
            CAServer server = new CAServer(IPAddress.Parse("130.246.49.5"), port, 5064);
            intRecord = server.CreateRecord<CAIntRecord>("TESTSERVER:INT");
            intRecord.PrepareRecord += new EventHandler(intRecord_PrepareRecord);
            intRecord.Scan = CaSharpServer.Constants.ScanAlgorithm.SEC5;

            strRecord = server.CreateRecord<CAStringRecord>("TESTSERVER:STR");
            strRecord.Value = "Default";

            Console.ReadLine();
        }

        static void intRecord_PrepareRecord(object sender, EventArgs e)
        {
            counter++;
            intRecord.Value = counter;
            if (counter > 100) counter = 0;
        }
    }
}
