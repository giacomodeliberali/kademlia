using System;
namespace Kademlia.Core
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            var coordinator = new Coordinator(
                // n m k alpha
                new Constants(1000, 32, 20, 3)
            );

            coordinator.BootstrapNetwork();

            Console.ReadKey();

        }
    }
}
