using System;
namespace Kademlia.Core
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            if(args.Length != 3)
            {
                Console.WriteLine($"Required parameters: n m k");
                return;
            }

            var n = int.Parse(args[0]);
            var m = int.Parse(args[1]);
            var k = int.Parse(args[2]);

            // check if identifier space is good
            if (Math.Pow(2, m) - 1 < n)
                return;

            var coordinator = new Coordinator(
                // n m k alpha
                new Constants(n, m, k, 3)
            );

            coordinator
                .BootstrapNetwork()
                .GenerateGraph();
        }
    }
}
