using System;
using System.Buffers.Binary;

namespace Harthoorn.MuseClient
{
    public static class Parse
    {

        public static Telemetry Telemetry(ReadOnlySpan<byte> span)
        {
            return new Telemetry
            {
                SequenceId = UShort(span, 0),
                BatteryLevel = UShort(span, 2) / 512f, // percentage
                Voltage = UShort(span, 4) * 2.2f, // don't know why
                Temperature = UShort(span, 8)
            };
        }

        public static Gyroscope Gyroscope(ReadOnlySpan<byte> span)
        { 
            return new Gyroscope
            {
                SequenceId = UShort(span, 0),
                Samples = Samples(span.Slice(2), 3, Scale.GYROSCOPE)
            };
        }

        public static Accelerometer Accelerometer(ReadOnlySpan<byte> span)
        {
            return new Accelerometer
            {
                SequenceId = UShort(span, 0),
                Samples = Samples(span.Slice(2), 3, Scale.ACCELEROMETER)
            };
        }

        public static Vector[] Samples(ReadOnlySpan<byte> span, int count, float scale)
        {
            var samples = new Vector[3];
            for (int i = 0; i < count; i++)
                samples[i] = Vector(span.Slice(i*6, 6)) * scale;
            
            return samples;
        }
         
        public static Vector Vector(ReadOnlySpan<byte> span)
        {
            return new Vector
            {
                X = Int16(span, 0),
                Y = Int16(span, 2),
                Z = Int16(span, 4),
            };
        }

        public static ushort UShort(ReadOnlySpan<byte> span, int index = 0)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(span.Slice(index, 2));
        }

        public static short Int16(ReadOnlySpan<byte> span, int index = 0)
        { 
            return BinaryPrimitives.ReadInt16BigEndian(span.Slice(index, 2));
        }


    }
}
