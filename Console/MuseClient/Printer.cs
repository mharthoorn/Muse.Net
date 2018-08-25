using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace Harthoorn.MuseClient
{
    public static partial class Print
    {
        public static void DeviceDetails(BluetoothLEDevice device)
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
                ServiceDetails(service);
            }
        }

        public static void ServiceDetails(GattDeviceService service)
        {
            Console.WriteLine($"Service: {service.AttributeHandle} - {service.Uuid}");
            foreach (var characteristic in service.GetAllCharacteristics())
            {
                Characteristic(characteristic);
            }
        }

        public static void Characteristic(GattCharacteristic c)
        {
            Console.WriteLine($" - Characteristic {c.AttributeHandle}: {c.CharacteristicProperties} ({c.ProtectionLevel}) - {c.Uuid}");
            //foreach (var d in c.GetAllDescriptors()) Descriptor(d);
            //PresentationFormats(c.PresentationFormats);
        }

        public static void Descriptor(GattDescriptor d)
        {
            Console.WriteLine($"           - Descriptor {d.AttributeHandle}: {d.ProtectionLevel} - {d.Uuid} ");
        }

        public static void PresentationFormats(IEnumerable<GattPresentationFormat> formats)
        {
            foreach (var format in formats)
            {
                Console.WriteLine($"FORMAT: {format.Description}, {format.FormatType}, {format.Namespace}, {format.Unit}");
            }

        }

        public static void ManufacturerData(IEnumerable<BluetoothLEManufacturerData> sections)
        {
            foreach (var section in sections)
            {
                Console.WriteLine($"company id: {section.CompanyId}");
                var s = section.Data.AsString();
                Console.WriteLine($"data: {s}");
            }
        }

        public static void AdvertisementData(IEnumerable<BluetoothLEAdvertisementDataSection> data)
        {
            int i = 0;
            foreach (var section in data)
            {
                var s = section.Data.AsString();
                Console.WriteLine($"{++i} - {s}");
            }
        }

        public static string AsString(this IBuffer buffer)
        {
            if (buffer is null) return null;

            var bytes = buffer.ToArray();
            var hex = BitConverter.ToString(bytes);
            var parsed = Encoding.ASCII.GetString(bytes);
            parsed = parsed.Replace("\n", "");

            return $"{hex} {parsed}";
        }

      
        public static void Raw(IBuffer buffer)
        {
            var s = buffer.AsString().Replace('\n', ' ');
            Console.WriteLine(s);
        }

        public static string Read(this IBuffer buffer)
        {

            var dataReader = DataReader.FromBuffer(buffer);
            var output = dataReader.ReadString(buffer.Length);
            output = output.Replace('\n', '.');
            return output;
        }

        public static void TelemetryModel(Telemetry t)
        {
            Console.WriteLine("Telemetry:");
            Console.WriteLine($" - seq: {t.SequenceId}");
            Console.WriteLine($" - Battery: {t.BatteryLevel}");
            Console.WriteLine($" - Voltage: {t.Voltage}");
            Console.WriteLine($" - Temp: {t.Temperature}");
        }

        public static void TelemetryModel(IBuffer buffer)
        {
            var t = Telemetry.ParseBuffer(buffer);
            Print.TelemetryModel(t);
        }

    }

    public static class Parser
    {
        public static byte[] ReadBytes(this IBuffer buffer)
        {
            var dataLength = buffer.Length;
            var data = new byte[dataLength];
            using (var reader = DataReader.FromBuffer(buffer))
            {
                reader.ReadBytes(data);
            }
            return data;
        }

        
    }

}
