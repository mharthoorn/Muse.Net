using System;
using Shell.Routing;
using System.Reflection;

namespace ConsoleApp
{

    class Program
    {
        public static Router Router = new Router(Assembly.GetExecutingAssembly());

        public static void Wait()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }


        static void Main(params string[] args)
        {
            var arguments = new Arguments(args, "info");
            try
            {
                var result = Router.Handle(arguments);

                if (!result.Ok)
                    Console.WriteLine($"Error: {result.Status}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error ocurred: {e}");
            }

        }

    }
}
