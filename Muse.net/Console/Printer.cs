using System;
using System.Collections.Generic;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Harthoorn.MuseClient
{

    public static class Printer
    {
        public static void Print(Telemetry t)
        {
            Console.WriteLine($" - Sequence: #{t.SequenceId}");
            Console.WriteLine($" - Battery: {t.BatteryLevel:0.0}%");
            Console.WriteLine($" - Voltage: {t.Voltage / 1000:0.0}V");
            Console.WriteLine($" - Temp: {t.Temperature}°C");
        }

        public static void Print(Accelerometer a, bool full = false)
        {
            if (!full)
            {
                Console.WriteLine($" - Accelerometer: #{a.SequenceId} {AcceleroSample(a.Samples[0])}");
            }
            else
            {
                Console.WriteLine($" - Sequence: #{a.SequenceId}");

                foreach (var sample in a.Samples)
                    Console.WriteLine($" - Sample: {AcceleroSample(sample)}");
            }
        }

        public static void Print(Gyroscope a, bool full = false)
        {
            if (!full)
            {
                Console.WriteLine($" - Accelerometer: #{a.SequenceId} {GyroSample(a.Samples[0])}");
            }
            else
            {
                Console.WriteLine($" - Sequence: #{a.SequenceId}");

                foreach (var sample in a.Samples)
                    Console.WriteLine($" - Sample: {GyroSample(sample)}");
            }
        }

        public static string AcceleroSample(Vector vector)
        {
            return string.Format("X = {0,5:#}, Y = {1,5:#}, Z = {2,5:#}", vector.X, vector.Y, vector.Z);
        }
         
        public static string GyroSample(Vector vector)
        {
            return string.Format("X = {0,5:0.0}, Y = {1,5:0.0}, Z = {2,5:0.0}", vector.X, vector.Y, vector.Z);
        }

        public static void Print(BluetoothLEDevice device)
        {
            Console.WriteLine($"Name: {device.Name}");
            Console.WriteLine($"Status: {device.ConnectionStatus}");
            Console.WriteLine($"Id: {device.DeviceId}");
            Console.WriteLine($"Can pair: {device.DeviceInformation.Pairing.CanPair}");
            Console.WriteLine($"Paired: {device.DeviceInformation.Pairing.IsPaired}");
            Console.WriteLine($"Kind: {device.DeviceInformation.Kind}");
            Console.WriteLine($"Enabled: {device.DeviceInformation.IsEnabled}");

            Console.WriteLine("Characteristics:");
            foreach (var service in device.GattServices)
            {
                Print(service);
            }
        }

        public static void Print(GattDeviceService service)
        {
            Console.WriteLine($"Service: {service.AttributeHandle} - {service.Uuid}");
            foreach (var characteristic in service.GetAllCharacteristics())
            {
                Print(characteristic);
            }
        }

        public static void Print(GattCharacteristic c)
        {
            Console.WriteLine($" - Characteristic {c.AttributeHandle}: {c.CharacteristicProperties} ({c.ProtectionLevel}) - {c.Uuid}");
            //foreach (var d in c.GetAllDescriptors()) Descriptor(d);
            //PresentationFormats(c.PresentationFormats);
        }

        public static void Print(GattDescriptor d)
        {
            Console.WriteLine($"           - Descriptor {d.AttributeHandle}: {d.ProtectionLevel} - {d.Uuid} ");
        }

        public static void Print(IEnumerable<GattPresentationFormat> formats)
        {
            foreach (var format in formats)
            {
                Console.WriteLine($"FORMAT: {format.Description}, {format.FormatType}, {format.Namespace}, {format.Unit}");
            }

        }

        public static void Print(IEnumerable<BluetoothLEManufacturerData> sections)
        {
            foreach (var section in sections)
            {
                Console.WriteLine($"company id: {section.CompanyId}");
                var s = section.Data.AsString();
                Console.WriteLine($"data: {s}");
            }
        }

        public static void Print(IEnumerable<BluetoothLEAdvertisementDataSection> data)
        {
            int i = 0;
            foreach (var section in data)
            {
                var s = section.Data.AsString();
                Console.WriteLine($"{++i} - {s}");
            }
        }
    }

}
