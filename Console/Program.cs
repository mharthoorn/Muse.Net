using System;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Harthoorn.MuseClient;

namespace ConsoleApp
{

    class Program
    {

        static async Task Main(string[] args)
        {
            await FourierTest.Collect(); 
            
        }

        

        
    }
}
