using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Storage.Streams;

namespace Harthoorn.MuseClient
{
    public static partial class Print
    {

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


        public static void TelemetryModel(IBuffer buffer)
        {
            Console.WriteLine("Telemetry");
            var bytes = buffer.ReadBytes();
            var t = Telemetry.Parse(bytes);
            Printer.Print(t);
        }
    }

}
