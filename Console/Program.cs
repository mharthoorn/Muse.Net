using System;
using System.Threading.Tasks;
using Harthoorn.MuseClient;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleApp
{

    class Program
    {

        static async Task Main(string[] args)
        {
            //await ConsoleScanner.Listen(MyMuse.Address, Channel.EEG_AF7);
            await Collect();
         
        }

        public static async Task Collect()
        {
            var client = new MuseClient(MyMuse.Address);
            var ok = await client.Connect();
            if (ok)
            {
                await client.Subscribe(Channel.EEG_AF7);
                client.NotifyEeg += Client_NotifyEeg;
                await client.Resume();
            }
            Console.ReadKey();


        }

        static List<float> data = new List<float>();
        static int n = 0;

        private static void Client_NotifyEeg(Channel c, Encefalogram e)
        {
            data.AddRange(e.Samples);

            if (data.Count % 1000 == 0)
            {
                Bitmap b = new Bitmap(data.Count, 800);
                
                for (int x = 0; x < data.Count; x++)
                {
                    int y = (int)data[x]+0x800;
                    y = y / 10;
                    b.SetPixel(x, y, Color.White);
                }

                n++;
                string name = $"e:/temp/graph-{n}.bmp";
                b.Save(name);
                Console.WriteLine($"Saved: {name}...");
            }
        }
    }
}
