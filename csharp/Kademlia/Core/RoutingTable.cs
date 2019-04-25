using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Kademlia.Core
{
    /// <summary>
    /// The Routing table of a Kademlia node.
    /// </summary>
    public class RoutingTable
    {
        #region Fields & Properties

        /// <summary>
        /// Gets the buckets.
        /// </summary>
        /// <value>The buckets.</value>
        public IList<Bucket> Buckets { get; }


        /// <summary>
        /// The node this routing table refers at.
        /// </summary>
        private readonly Node node;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kademlia.Core.RoutingTable"/> class.
        /// </summary>
        /// <param name="node">The node this routing table refers at.</param>
        public RoutingTable(Node node)
        {
            this.node = node;

            // initialize M buckets
            Buckets = new List<Bucket>();
            for (var i = 0; i < Coordinator.Constants.M; i++)
                Buckets.Add(new Bucket(this.node, Coordinator.Constants.K));
        }

        #endregion

        #region Public interface

        /// <summary>
        /// Insert the specified node into the right bucket.
        /// </summary>
        /// <param name="target">The node.</param>
        public void Insert(Node target)
        {
            if (node.Id.Equals(target.Id))
                return;

            var closestBucketIndex = GetClosestBucketIndex(target.Id);
            var closestBucket = Buckets[closestBucketIndex];
            closestBucket.Insert(target);
        }

        /// <summary>
        /// Gets the K closest to the target identifier.
        /// </summary>
        /// <returns>The K closest to the target identifier.</returns>
        /// <param name="target">Identifier.</param>
        public List<Node> GetKClosestTo(Identifier target)
        {

            // take closest bucket
            var bucketIndex = GetClosestBucketIndex(target);
            var closestBucket = Buckets[bucketIndex];

            // get all its nodes
            var kClosestNodes = new List<Node>();
            kClosestNodes.AddRange(closestBucket.Nodes);

            var direction = -1; // -1 equals left, right otherwise
            var left = bucketIndex - 1;
            var right = bucketIndex + 1;


            while (kClosestNodes.Count < Coordinator.Constants.K)
            {
                if (direction < 0 && left >= 0)
                {
                    // I'm going left and the left index is valid
                    kClosestNodes.AddRange(Buckets[left].Nodes);
                    left--;
                }

                if (direction > 0 && right < Buckets.Count)
                {
                    // I'm going right and the right index is valid
                    kClosestNodes.AddRange(Buckets[right].Nodes);
                    right++;
                }

                direction *= -1; // reverse the direction

                if (left < 0 && right >= Buckets.Count)
                {
                    // break if indexes are out of bounds (all cell visited)
                    break;
                }
            }

            return kClosestNodes;
        }

        public void PrintContent()
        {
            Console.Write($"Routing table content of node {node}: ");
            foreach (var bucket in node.RoutingTable.Buckets)
            {
                foreach (var n in bucket.Nodes)
                {
                    Console.Write($"{n} ");
                }
            }
            Console.WriteLine("");
        }

        #endregion

        #region Private interface

        /// <summary>
        /// Gets the closest bucket index.
        /// </summary>
        /// <returns>The closest bucket index.</returns>
        /// <param name="target">Target.</param>
        private int GetClosestBucketIndex(Identifier target)
        {
            var distance = node.Id.GetDistanceTo(target);

            return (int)(Math.Log((long)distance) / Math.Log(2));

            //if (distance == 0)
            //    return 0;

            //for (var i = 0; i < Buckets.Count; i++)
            //{
            //    if (distance >= BigInteger.Pow(2, i) && distance < BigInteger.Pow(2, i + 1))
            //        return i;
            //}

            //throw new ArgumentException("The given identifier does not match the 2^m limit");
        }

        #endregion

    }
}
