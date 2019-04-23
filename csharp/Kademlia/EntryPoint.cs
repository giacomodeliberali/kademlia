using System;
namespace Kademlia.Core
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            var coordinator = new Coordinator(
                // n m k alpha
                new Constants(10, 5, 5, 3)
            );

            coordinator.BootstrapNetwork();

            Console.ReadKey();

        }
    }
}
