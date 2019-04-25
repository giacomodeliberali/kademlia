using System;
using System.Collections.Generic;
using Kademlia.Core;

namespace Kademlia.Models
{
    public class FindNodeResponse
    {
        public List<Node> ClosestNodes { get; set; }
        public List<Node> TraveledNodes { get; set; }
    }
}
