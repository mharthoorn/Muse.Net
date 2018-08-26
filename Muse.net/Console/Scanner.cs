using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Harthoorn.MuseClient
{
    public static class ConsoleScanner
    {
        public static async Task ReceiveLoop(ulong address)
        {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(address);
             
            Printer.Print(device);
            var service = device.GetGattService(MuseGuid.PRIMARY_SERVICE);

            var channel = service.GetCharacteristic(MuseGuid.ACELEROMETER);
            var control = service.GetCharacteristic(MuseGuid.CONTROL);

            Console.WriteLine("Connecting to Muse Headband...");

            bool ok = await control.Start(channel);

            if (ok)
            {
                await ConsoleScanner.ReceiveData(control, channel);
            }
            else
            {
                Console.WriteLine("Device cannot be reached.");
            }
            Console.WriteLine();
        }

        public static void Scan() 
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
            Printer.Print(eventArgs.Advertisement.DataSections);
            Printer.Print(eventArgs.Advertisement.ManufacturerData);
            Console.WriteLine();
        }

        public static async Task ReceiveData(GattCharacteristic control, GattCharacteristic characteristic)
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

            if (sender.Uuid.Equals(MuseGuid.TELEMETRY))
                Print.TelemetryModel(args.CharacteristicValue);

            else if (sender.Uuid.Equals(MuseGuid.ACELEROMETER))
                Print.AccelerometerModel(args.CharacteristicValue);

            else if (sender.Uuid.Equals(MuseGuid.GYROSCOPE))
                Print.GyroscopeModel(args.CharacteristicValue);

            

            else
                Print.Raw(args.CharacteristicValue);
        }

        public static async Task Listen(ulong address, params Channel[] channels)
        {
            var client = new MuseClient(address);
            var ok = await client.Connect();
            if (ok)
            {
                await client.Subscribe(channels);
                client.NotifyAccelerometer += Client_NotifyAccelerometer;
                client.NotifyGyroscope += Client_NotifyGyroscope;
                client.NotifyTelemetry += Client_NotifyTelemetry;
                client.NotifyEeg += Client_NotifyEeg;
                await client.Resume();
            }
        }

        private static void Client_NotifyEeg(Channel arg1, Encefalogram gram)
        {
            Printer.Print(gram);
        }

        private static void Client_NotifyTelemetry(Telemetry tele)
        {
            Printer.Print(tele);
        }

        private static void Client_NotifyGyroscope(Gyroscope gyro)
        {
            Printer.Print(gyro);
        }

        private static void Client_NotifyAccelerometer(Accelerometer accelerometer)
        {
            Printer.Print(accelerometer);
        }


    }

    
}

 