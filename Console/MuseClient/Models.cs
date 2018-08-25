using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Harthoorn.MuseClient 
{
    public class Telemetry
    {
        public ushort SequenceId;
        public double BatteryLevel;
        public double Voltage;
        public ushort Temperature;

        public static Telemetry ParseBuffer(IBuffer buffer) 
        {
            var bytes = buffer.ReadBytes();
            return ParseBuffer(bytes);
        }

        public static Telemetry ParseBuffer(byte[] buffer)
        {
            var array = buffer.AsSpan();
            //var span = MemoryMarshal.Cast<byte, ushort>(array);
            return Parse(array);
        }

        public static Telemetry Parse(Span<byte> span) 
        {
            var telemetry = new Telemetry();
            telemetry.SequenceId = BinaryPrimitives.ReadUInt16BigEndian(span.Slice(0, 2));
            telemetry.BatteryLevel = (double)BinaryPrimitives.ReadUInt16BigEndian(span.Slice(2, 2)) / 512;
            telemetry.Voltage = (double)BinaryPrimitives.ReadInt16BigEndian(span.Slice(4, 2)) * 2.2;
            telemetry.Temperature = BinaryPrimitives.ReadUInt16BigEndian(span.Slice(8, 2));
            return telemetry;
        }

    }
    

    
}
