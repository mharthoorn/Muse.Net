using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Storage.Streams;

namespace Harthoorn.MuseClient
{
    public static class Language
    {
        public static IBuffer EncodeCommand(string command)
        {
            string text = $"X{command}\n";
            var bytes = Encoding.UTF8.GetBytes(text);
            bytes[0] = (byte)(bytes.Length - 1);
            var buffer = bytes.AsBuffer();
            return buffer;
           
        }
    }
}
