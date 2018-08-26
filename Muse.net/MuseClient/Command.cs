namespace Harthoorn.MuseClient
{
    public static class Command
    {
        public const string START = "s";
        public const string PAUSE = "h";
        public const string RESUME = "d";
    }

    public static class Preset
    {
        
        public const string AUX_ACELEROMETER = "p20"; 
        public const string ACELEROMETER = "p21";
        public const string NEITHER = "p22";
        //public const string NEITHER = "p23"; -- same as p22.

        // http://developer.choosemuse.com/hardware-firmware/headband-configuration-presets
    }


}
