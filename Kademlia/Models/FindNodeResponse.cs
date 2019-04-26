using System.Collections.Generic;
using Kademlia.Core;

namespace Kademlia.Models
{
    /// <summary>
    /// A FindNode response.
    /// </summary>
    public class FindNodeResponse
    {
        /// <summary>
        /// Gets or sets the closest nodes.
        /// </summary>
        /// <value>The closest nodes.</value>
        public List<Node> ClosestNodes { get; set; }

        /// <summary>
        /// Gets or sets the traveled nodes.
        /// </summary>
        /// <value>The traveled nodes.</value>
        public List<Node> TraveledNodes { get; set; }
    }
}
