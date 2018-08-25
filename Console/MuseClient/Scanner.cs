using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Harthoorn.MuseClient
{
    public static class Scanner
    {
        public static void Listen() 
        {
            // Create Bluetooth Listener
            var watcher = new BluetoothLEAdvertisementWatcher();

            watcher.ScanningMode = BluetoothLEScanningMode.Active;

            // Only activate the watcher when we're recieving values >= -80
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

            // Stop watching if the value drops below -90 (user walked away)
            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

            // Register callback for when we see an advertisements
            watcher.Received += OnAdvertisementReceived;

            // Wait 5 seconds to make sure the device is really out of range
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

            // Starting watching for advertisements
            watcher.Start();

        }

        private static void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {

            // Tell the user we see an advertisement and print some properties
            Console.WriteLine(String.Format("Advertisement:"));
            Console.WriteLine($"  Address: {eventArgs.BluetoothAddress}");

            Console.WriteLine($"  Type: {eventArgs.AdvertisementType}");

            Console.WriteLine($"  FR_NAME: {eventArgs.Advertisement.LocalName}");
            Console.WriteLine($"  DBm: {eventArgs.RawSignalStrengthInDBm}");
            Print.AdvertisementData(eventArgs.Advertisement.DataSections);
            Print.ManufacturerData(eventArgs.Advertisement.ManufacturerData);
            Console.WriteLine();
        }

        public static async Task ScanData(GattCharacteristic control, GattCharacteristic characteristic)
        {
            
            Console.WriteLine("Listening for output. Press any ESC to stop.");
            Console.WriteLine("S = Start, R = Resume, P = Pause.");

            await control.WriteCommand(Command.PAUSE); 

            // Without this line, the ValueChanged event won't fire!
            await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

            characteristic.ValueChanged += Characteristic_ValueChanged;

            bool ready = false;

            do
            {
                var result = await characteristic.ReadValueAsync();
                
                if (result.Status == GattCommunicationStatus.Success)
                {
                    var data = result.Value;
                    if (data != null)
                    {
                        var bytes = data.ReadBytes();
                        var s = Encoding.UTF8.GetString(bytes).Replace('\n', '.');
                        //Console.Write($"<{s}>");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                else
                {
                    Console.Write("x");
                }

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();
                    
                    switch (key.Key)
                    {
                        case ConsoleKey.P: await control.WriteCommand(Command.PAUSE); break;
                        case ConsoleKey.R: await control.WriteCommand(Command.RESUME); break;
                        case ConsoleKey.S: await control.WriteCommand(Command.START); break;
                        case ConsoleKey.Escape:
                        case ConsoleKey.Spacebar:
                            ready = true; break;
                    }

                }
            }
            while (!ready);
            characteristic.ValueChanged -= Characteristic_ValueChanged;
            Console.WriteLine("\nListener stopped.");
        }

        private static void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var s = args.CharacteristicValue.AsString().Replace('\n', ' '); ;
            Console.WriteLine(s);
        }
    }
}

 