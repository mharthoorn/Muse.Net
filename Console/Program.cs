using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Harthoorn.MuseClient;

namespace ConsoleApp
{

    class Program
    {

        static void Main(string[] args)
        {
            Test().Wait();
            Console.ReadKey();

        }

        public static async Task Test()
        {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(MyMuse.Address);
            
            Print.DeviceDetails(device);
            var service = device.GetGattService(MuseGuid.PRIMARY_SERVICE);

            var ch = service.GetCharacteristic(MuseGuid.ACELEROMETER);
            var control = service.GetCharacteristic(MuseGuid.CONTROL);

            Console.WriteLine("Connecting to Muse Headband...");
            
            bool ok = await control.Start(ch);

            if (ok)
            {
                await Scanner.ScanData(control, ch);
            }
            else
            {
                Console.WriteLine("Device cannot be reached.");
            }
            Console.WriteLine();
        }
    }
}
