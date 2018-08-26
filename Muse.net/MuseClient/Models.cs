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

        public static Telemetry Parse(Span<byte> span) 
        {
            return new Telemetry
            {
                SequenceId = BinaryPrimitives.ReadUInt16BigEndian(span.Slice(0, 2)),
                BatteryLevel = (double)BinaryPrimitives.ReadUInt16BigEndian(span.Slice(2, 2)) / 512,
                Voltage = (double)BinaryPrimitives.ReadInt16BigEndian(span.Slice(4, 2)) * 2.2,
                Temperature = BinaryPrimitives.ReadUInt16BigEndian(span.Slice(8, 2))
            };
        }

    }

}
