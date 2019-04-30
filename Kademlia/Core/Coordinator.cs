using System;
using System.Linq;
using System.Collections.Generic;
using Kademlia.Helpers;
using System.Numerics;
using System.Text;
using System.IO;
using System.Reflection;

namespace Kademlia.Core
{
    /// <summary>
    /// Orchestrates the network construction.
    /// </summary>
    class Coordinator
    {
        #region Fields & Properties 

        /// <summary>
        /// Gets or sets the constants used by all actors.
        /// </summary>
        /// <value>The constants.</value>
        public static Constants Constants { get; set; }

        /// <summary>
        /// The nodes that have already njoined the network.
        /// </summary>
        private readonly List<Node> nodes = new List<Node>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kademlia.Core.Coordinator"/> class.
        /// </summary>
        /// <param name="constants">Constants.</param>
        public Coordinator(Constants constants)
        {
            Constants = constants;
        }

        #endregion

        #region Public APIs

        /// <summary>
        /// Bootstraps the network and start joining the n nodes.
        /// </summary>
        public Coordinator BootstrapNetwork()
        {
            Console.WriteLine($"Bootstrapping network of {Constants.N} nodes");

            // insert first bootstrap node with empty routing table
            var bootNode = new Node();
            nodes.Add(bootNode);

            // print some debug info
            Console.WriteLine($"k={Constants.K}");
            Console.WriteLine($"m={Constants.M}");
            Console.WriteLine($"alpha={Constants.Alpha}");
            Console.WriteLine($"Bootstrap node id is {bootNode.Id}\n");

            // join all n-1 nodes
            for (var i = 0; i < Constants.N - 1; i++)
            {
                JoinNewNode();
                Console.Write($"\rJoined {i}/{Constants.N}");
            }

            // count the generated edges
            var edgesCount = 0;
            nodes.ForEach(node =>
            {
                node.RoutingTable.Buckets.ToList().ForEach(bucket =>
                {
                    edgesCount += bucket.Nodes.Count;
                });
            });

            Console.WriteLine($"\rGenerated {edgesCount} edges (max is {Constants.N * Constants.M * Constants.K})");

            return this;
        }

        /// <summary>
        /// Generates the graph and saves it to a CSV file.
        /// </summary>
        public void GenerateGraph()
        {
            var csv = new StringBuilder();
            //csv.AppendLine($"Source;Target");
            nodes.ForEach(node =>
            {
                node.RoutingTable.Buckets.ToList().ForEach(bucket =>
                {
                    bucket.Nodes.ToList().ForEach(bucketNode =>
                    {
                        csv.AppendLine($"{node};{bucketNode}");
                    });
                });
            });

            String fileName;
            String path;
            int i = 1;
            do
            {
                fileName = $"graph_n{Constants.N}_m{Constants.M}_k{Constants.K}_alpha{Constants.Alpha}-{i++}.csv";
                path = $"./stats/{fileName}";
            } while (File.Exists(path));

            File.WriteAllText(path, csv.ToString());
        }

        #endregion

        #region Private APIs

        /// <summary>
        /// Creates a new node and joins it into the network, populating its routing table.
        /// </summary>
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

        /// <summary>
        /// Gets a random bootstrap node.
        /// </summary>
        /// <returns>The random bootstrap.</returns>
        private Node GetRandomBootstrap()
        {
            return nodes[new Random().Next(nodes.Count)];
        }

        #endregion
    }
}
