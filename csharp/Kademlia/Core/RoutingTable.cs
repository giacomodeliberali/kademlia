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
            this.Buckets = new List<Bucket>();
            for (var i = 0; i < Coordinator.Constants.M; i++)
                this.Buckets.Add(new Bucket(Coordinator.Constants.K));
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

            var closestBucket = GetClosestBucket(target.Id);
            closestBucket.Insert(target);
        }

        /// <summary>
        /// Gets the K closest to the target identifier.
        /// </summary>
        /// <returns>The K closest to the target identifier.</returns>
        /// <param name="target">Identifier.</param>
        public IList<Node> GetKClosestTo(Identifier target)
        {

            // take closes bucket
            var closestBucket = GetClosestBucket(target);
            var bucketIndex = Buckets.IndexOf(closestBucket);

            // get all its nodes
            var kClosestNodes = new List<Node>();
            kClosestNodes.AddRange(closestBucket.Nodes);

            var direction = -1;
            var left = bucketIndex - 1;
            var right = bucketIndex + 1;


            while (kClosestNodes.Count() < Coordinator.Constants.K)
            {
                if (direction < 0 && left >= 0)
                {
                    kClosestNodes.AddRange(Buckets[left].Nodes);
                    left--;
                }

                if (direction > 0 && right < Buckets.Count())
                {
                    kClosestNodes.AddRange(Buckets[right].Nodes);
                    right++;
                }

                direction *= -1;

                if (left < 0 && right >= Buckets.Count())
                {
                    break;
                }
            }

            return kClosestNodes.ToList();

        }

        #endregion

        #region Private interface

        /// <summary>
        /// Gets the closest bucket.
        /// </summary>
        /// <returns>The closest bucket.</returns>
        /// <param name="target">Target.</param>
        private Bucket GetClosestBucket(Identifier target)
        {
            var distance = this.node.Id.GetDistanceTo(target);

            if (distance == 0)
                return this.Buckets[0];

            for (var i = 0; i < Buckets.Count; i++)
            {
                if (distance >= BigInteger.Pow(2, i) && distance < (BigInteger.Pow(2, i + 1)))
                    return Buckets[i];
            }

            throw new ArgumentException("The given identifier does not match the 2^m limit");
        }

        #endregion

    }
}
