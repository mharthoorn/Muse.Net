using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Harthoorn.MuseClient;
using System.Linq;

namespace ConsoleApp
{

    class Program
    {

        static async Task Main(string[] args)
        {
            await Test();
            //TestTelemetry();
            Console.ReadKey();

        }

        public static void TestTelemetry()
        {
            var array = new byte[] { 1, 74, 181, 184, 7, 64, 15, 127, 0, 27, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var t = Telemetry.ParseBuffer(array);
            Print.TelemetryModel(t);
        }

        public static async Task Test()
        {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(MyMuse.Address);
            
            Print.DeviceDetails(device);
            var service = device.GetGattService(MuseGuid.PRIMARY_SERVICE);

            var channel = service.GetCharacteristic(MuseGuid.TELEMETRY);
            var control = service.GetCharacteristic(MuseGuid.CONTROL);

            Console.WriteLine("Connecting to Muse Headband...");
            
            bool ok = await control.Start(channel);

            if (ok)
            {
                await Scanner.ScanData(control, channel);
            }
            else
            {
                Console.WriteLine("Device cannot be reached.");
            }
            Console.WriteLine();
        }
    }
}
