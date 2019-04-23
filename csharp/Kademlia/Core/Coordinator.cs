using System;
using System.Linq;
using System.Collections.Generic;
using Kademlia.Helpers;
using System.Numerics;

namespace Kademlia.Core
{
    class Coordinator
    {
        public static Constants Constants { get; set; }

        private List<Node> nodes = new List<Node>();


        public Coordinator(Constants constants)
        {
            Constants = constants;
        }

        public void BootstrapNetwork()
        {
            Console.WriteLine($"Bootstrapping network of {Constants.N} nodes");

            // insert first bootstrap node with empty routing table
            var bootNode = new Node();
            nodes.Add(bootNode);


            Console.WriteLine($"k={Constants.K}");
            Console.WriteLine($"m={Constants.M}");
            Console.WriteLine($"alpha={Constants.Alpha}");

            Console.WriteLine($"Bootstrap node id is {bootNode.Id}\n");

            for (var i = 1; i < Constants.N; i++)
                JoinNewNode();

            var edgesCount = 0;
            nodes.ForEach(node =>
            {
                foreach (var bucket in node.RoutingTable.Buckets)
                    edgesCount += bucket.Nodes.Count();
            });

            Console.WriteLine($"Generated {edgesCount} edges (max is {Constants.N * Constants.M * Constants.K})");
        }

        private void JoinNewNode()
        {
            // choose bootstrap node of newNode
            var randomBootNode = IdentifierGenerator.Instance.GetRandomExistingId();
            var bootstrapNode = new Node(randomBootNode);

            // generate a new node
            var nodeToJoin = new Node();

            // insert bootstrap into new node's routing table
            nodeToJoin.UpdateRoutingTable(bootstrapNode);

            Console.WriteLine($"Joining node {nodeToJoin} with bootstrap {bootstrapNode}");
            PrintRoutingTableContent(nodeToJoin);

            // perform a self-lookup
            //var kClosest = nodeToJoin.Lookup(nodeToJoin.Id);
            //Console.WriteLine($"  - self lookup return: {string.Join(", ", kClosest.Select(n => n.ToString()))}");
            //nodeToJoin.UpdateRoutingTable(kClosest);
            //PrintRoutingTableContent(nodeToJoin);

            // refreshes all k-buckets 
            for (var bucketIndex = 0; bucketIndex < Constants.M; bucketIndex++)
            {
                // generate a random id (never extracted) for each bucket range
                var randomNodeIdentifierInBucket = IdentifierGenerator.Instance.GenerateRandomInBucket(bucketIndex);
                // and make a lookup of the new node
                var kClosest = nodeToJoin.Lookup(randomNodeIdentifierInBucket);
                Console.WriteLine($"  - refreshing with lookup of {randomNodeIdentifierInBucket} return: {string.Join(", ", kClosest.Select(n => n.ToString()))}");
                nodeToJoin.UpdateRoutingTable(kClosest);
                PrintRoutingTableContent(nodeToJoin);
            }

            nodes.Add(nodeToJoin);
        }

        private void PrintRoutingTableContent(Node node)
        {
            Console.Write($"  - routing table content: ");
            foreach (var bucket in node.RoutingTable.Buckets)
            {
                foreach (var n in bucket.Nodes)
                {
                    Console.Write($"{n} ");
                }
            }
            Console.WriteLine("");
        }
    }
}
