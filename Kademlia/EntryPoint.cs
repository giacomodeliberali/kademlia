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
            var coordinator = new Coordinator(
                // n m k alpha
                new Constants(10000, 160, 20, 3)
            );

            coordinator
                .BootstrapNetwork()
                .GenerateGraph();

            Console.ReadKey();
        }
    }
}
