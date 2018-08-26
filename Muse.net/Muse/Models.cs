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
        public float BatteryLevel;
        public float Voltage;
        public ushort Temperature;
    }

    public class Accelerometer
    {
        public ushort SequenceId;
        public Vector[] Samples;
    }

    public class Gyroscope
    { 
        public ushort SequenceId;
        public Vector[] Samples;
    }

    public class Encefalogram
    {
        public float Index; //?
        public DateTimeOffset Timestamp;
        public float[] Samples; // Each message holds 12 samples each time
        public byte[] Raw;
    }


    public struct Vector
    {
        public float X;
        public float Y;
        public float Z;

        public static Vector operator * (Vector vector, float f)
        {
            return new Vector { X = vector.X * f, Y= vector.Y * f, Z = vector.Z * f };
        }
    }

}
