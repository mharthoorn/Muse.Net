using System;
using System.Threading.Tasks;
using Harthoorn.MuseClient;
using System.Linq;

namespace ConsoleApp
{

    class Program
    {

        static async Task Main(string[] args)
        {
            await ConsoleScanner.ReceiveLoop(MyMuse.Address);
            //TestTelemetry();
            //Console.ReadKey();

        }
    }
}
