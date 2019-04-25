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

        private readonly List<Node> nodes = new List<Node>();


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

            for (var i = 0; i < Constants.N - 1; i++)
            {
                JoinNewNode();
                Console.Write($"\rJoined {i}/{Constants.N}");
            }

            var edgesCount = 0;
            nodes.ForEach(node =>
            {
                node.RoutingTable.Buckets.ToList().ForEach(bucket =>
                {
                    edgesCount += bucket.Nodes.Count;
                });
            });

            Console.WriteLine($"\rGenerated {edgesCount} edges (max is {Constants.N * Constants.M * Constants.K})");
        }

        private void JoinNewNode()
        {
            // choose bootstrap node of newNode
            var bootstrapNode = GetRandomBootstrap();

            // generate a new node
            var nodeToJoin = new Node();

            // insert bootstrap into new node's routing table
            nodeToJoin.UpdateRoutingTable(bootstrapNode);

            // refreshes all k-buckets 
            for (var bucketIndex = 0; bucketIndex < Constants.M; bucketIndex++)
            {
                // generate a random id (never extracted) for each bucket range
                var randomNodeIdentifierInBucket = IdentifierGenerator.Instance.GenerateRandomInBucket(bucketIndex);
                // and make a lookup of the new node
                nodeToJoin.UpdateRoutingTable(
                    nodeToJoin.Lookup(randomNodeIdentifierInBucket)
                );
            }
            nodes.Add(nodeToJoin);
        }

        private Node GetRandomBootstrap()
        {
            return nodes[new Random().Next(nodes.Count)];
        }

    }
}
