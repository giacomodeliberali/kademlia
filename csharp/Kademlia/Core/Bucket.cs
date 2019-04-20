using System.Collections.Generic;
using System.Linq;

namespace Kademlia.Core
{
    /// <summary>
    /// A bucket contained in a routing table
    /// </summary>
    public class Bucket
    {

        #region Fields & Properties

        /// <summary>
        /// The number of max nodes contained in this bucket
        /// </summary>
        private readonly int k;

        /// <summary>
        /// Gets the nodes present in this bucket
        /// </summary>
        /// <value>The nodes.</value>
        public IList<Node> Nodes { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Kademlia.Core.Bucket"/> is full.
        /// </summary>
        /// <value><c>true</c> if is full; otherwise, <c>false</c>.</value>
        public bool IsFull
        {
            get
            {
                return Nodes.Count == this.k;
            }
        }


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kademlia.Core.Bucket"/> class.
        /// </summary>
        /// <param name="k">The number of max nodes contained in this bucket</param>
        public Bucket(int k)
        {
            this.k = k;
            this.Nodes = new List<Node>();
        }

        #endregion

        #region Public interface

        /// <summary>
        /// Insert the specified target into the right bucket.
        /// </summary>
        /// <param name="target">The node to insert</param>
        public void Insert(Node target)
        {
            if (Nodes.Contains(target))
            {
                // the node exist in bucket, move it to the tail
                Nodes.Remove(target);
                Nodes.Add(target);
                return;
            }

            if (!IsFull)
            {
                // the node does not exist and the bucket is not full, append new node in the bucket's tail
                Nodes.Add(target);
                return;
            }

            // the node does not exist and the bucket is full. So ping least seen node 
            var leastSeenNode = Nodes[0];

            if (leastSeenNode.Ping())
            {
                // promote least seen node and discard new node
                Nodes.Add(leastSeenNode);
            }
            else
            {
                // promote new node and discard least seen node 
               Nodes.Add(target);
            }
        }

        public override string ToString()
        {
            return Nodes.Count().ToString();
        }

        #endregion

    }
}
