using System;
using System.Threading.Tasks;
using Harthoorn.MuseClient;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace ConsoleApp
{
    // Doesn't work yet. Not finished.
    public static class FourierTest
    {
        public static async Task Collect()
        {
            var client = new MuseClient(MyMuse.Address);
            Console.WriteLine("Connecting...");
            var ok = await client.Connect();
            if (ok)
            {
                await client.Subscribe(
                    Channel.EEG_AF7,
                    Channel.EEG_AF8,
                    Channel.EEG_TP10,
                    Channel.EEG_TP9,
                    Channel.EEG_AUX);

                client.NotifyEeg += Client_NotifyEeg;
                Console.WriteLine("Starting...");
                await client.Resume();
                await Task.Run(Key);
                await client.Pause();
            }
        }

        static int count = 100;
        static List<double> data = new List<double>();
        static double[] cosines = new double[count];
        static double[] sinuses = new double[count];
        static int x = 0;

        private static void Client_NotifyEeg(Channel channel, Encefalogram e)
        {
            if (++x % 5 == 0)
            {
                var values = e.Samples.Select(f => (double)f).ToArray();
                data.AddRange(values);

                Console.WriteLine(".");
                
                for (int i = 0; i < count; i++)
                {

                    char c = ' ';
                    if (sinuses[i] < 1) c = '_';
                    else if (sinuses[i] < 10) c = 'o';
                    else if (sinuses[i] < 50) c = 'O';
                    Console.Write(c);
                }
            }
        }

        public static void Key()
        {
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
            }
            var key = Console.ReadKey(intercept: true);
        }

        
    }
}
