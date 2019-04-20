using System;
using System.Collections.Generic;
using Kademlia.Core;

namespace Kademlia.Models
{
    public class FindNodeRequest
    {
        public Identifier Target { get; set; }
        public IList<Node> TraveledNodes { get; set; }
    }
}
