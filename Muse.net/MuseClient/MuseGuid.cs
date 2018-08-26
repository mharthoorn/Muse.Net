using System;

namespace Harthoorn.MuseClient
{
    public static class MuseGuid
    {
        public static readonly Guid PRIMARY_SERVICE = new Guid("0000fe8d-0000-1000-8000-00805f9b34fb");

        public static readonly Guid CONTROL = new Guid("273e0001-4c4d-454d-96be-f03bac821358");
        public static readonly Guid TELEMETRY = new Guid("273e000b-4c4d-454d-96be-f03bac821358");
        public static readonly Guid ACELEROMETER = new Guid("273e0009-4c4d-454d-96be-f03bac821358");
        public static readonly Guid GYROSCOPE = new Guid("273e000a-4c4d-454d-96be-f03bac821358");

        public static readonly Guid EEG_TP9 = new Guid("273e0003-4c4d-454d-96be-f03bac821358");
        public static readonly Guid EEG_AF7 = new Guid("273e0004-4c4d-454d-96be-f03bac821358");
        public static readonly Guid EEG_AF8 = new Guid("273e0005-4c4d-454d-96be-f03bac821358");
        public static readonly Guid EEG_TP10 = new Guid("273e0006-4c4d-454d-96be-f03bac821358");
        public static readonly Guid EEG_AUX = new Guid("273e0007-4c4d-454d-96be-f03bac821358");
    }

    public static class Scale
    {
        public static float GYROSCOPE= 0.0000610352f;
        public static float ACCELEROMETER = 0.0074768f;
    }

}
