using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSI.EpicsClient2;

namespace test_client
{
    class Program
    {
        static void Main(string[] args)
        {
            EpicsClient client = new EpicsClient();
            EpicsChannel<int> test_channel_1 = client.CreateChannel<int>("NDW1033:dpk62:SIMPLE:VALUE1");
            int PV_value = test_channel_1.Get<int>();
            Console.WriteLine(PV_value);
            test_channel_1.Put<int>(2);
        }
    }
}
