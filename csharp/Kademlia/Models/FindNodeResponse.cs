using System;
using System.Collections.Generic;
using Kademlia.Core;

namespace Kademlia.Models
{
    public class FindNodeResponse
    {
        public IList<Node> ClosestNodes { get; set; }
        public IList<Node> TraveledNodes { get; set; }
    }
}
