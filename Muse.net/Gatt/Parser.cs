using Windows.Storage.Streams;

namespace Harthoorn.MuseClient
{
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
