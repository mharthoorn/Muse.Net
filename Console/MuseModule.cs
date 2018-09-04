using System;
using System.Threading.Tasks;
using Harthoorn.MuseClient;
using Shell.Routing;

namespace ConsoleApp
{
    [Module("Muse")]
    public class MuseModule
    {

        [Command]
        public void Info()
        {
            Console.WriteLine("Muse.Net Console Utils 0.1");
            Console.WriteLine("Commands: telemetry");
        }

        [Command]
        public void Telemetry()
        {
            var telemetry = TelemetryAsync().Result;
            if (telemetry != null)
                Print.Telemetry(telemetry);
        }

        [Command("Find your Muse address")]
        public static void Scan()
        {
            Console.WriteLine("Scanning for devices. Turn on your Muse now. Press any key to stop.");

            var scanner = new BleScanner();
            scanner.OnAdvertise += Advertise;
            scanner.Scan();

            void Advertise(Advertisement adv)
            {
                Console.WriteLine($"Device: {adv.Name} - Address: {adv.Address} - Signal: {adv.SignalStrengh} dBmW");
            }
        }

        

        public async Task<Telemetry> TelemetryAsync()
        {
            var client = new MuseClient(MyMuse.Address);

            Console.Write("\rConnecting...");
            await client.Connect();
            await client.Start();
            await client.Resume();

            Console.WriteLine("\rWaiting for signal...");
            var telemetry = await client.ReadTelemetryAsync();
            
            await client.Disconnect();
            return telemetry;
        }

    
    }
}
