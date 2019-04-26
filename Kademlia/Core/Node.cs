using System.Linq;
using System.Collections.Generic;
using Kademlia.Helpers;
using Kademlia.Models;

namespace Kademlia.Core
{
    /// <summary>
    /// A Kademlia node.
    /// </summary>
    public class Node
    {
        #region Fields & Properties

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Identifier Id { get; }

        /// <summary>
        /// Gets the routing table.
        /// </summary>
        /// <value>The routing table.</value>
        public RoutingTable RoutingTable { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kademlia.Core.Node"/> class.
        /// Generates a new <see cref="T:Kademlia.Core.Identifier"/> for this node.
        /// </summary>
        public Node()
        {
            Id = IdentifierGenerator.Instance.GenerateIdentifier();
            RoutingTable = new RoutingTable(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kademlia.Core.Node"/> class.
        /// Uses the specified <see cref="T:Kademlia.Core.Identifier"/> for this node.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        public Node(Identifier identifier)
        {
            Id = identifier;
            RoutingTable = new RoutingTable(this);
        }

        #endregion

        #region Public APIs

        /// <summary>
        /// Updates the routing table.
        /// </summary>
        /// <param name="nodes">Nodes.</param>
        public void UpdateRoutingTable(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
                UpdateRoutingTable(node);
        }

        /// <summary>
        /// Updates the routing table.
        /// </summary>
        /// <param name="node">Node.</param>
        public void UpdateRoutingTable(Node node)
        {
            if (Id.Equals(node.Id))
                return;

            RoutingTable.Insert(node);
        }

        /// <summary>
        /// Return all the known closest nodes to the target and update the routing table with traveled nodes.
        /// </summary>
        /// <returns>The nodes and the traveled nodes with this instance appended.</returns>
        /// <param name="target">The target node.</param>
        public FindNodeResponse FindNode(Identifier target)
        {
            return FindNode(target, new List<Node>());
        }

        /// <summary>
        /// Return all the known closest nodes to the target and update the routing table with traveled nodes.
        /// </summary>
        /// <returns>The nodes and the traveled nodes with this instance appended.</returns>
        /// <param name="target">The target node.</param>
        public FindNodeResponse FindNode(Identifier target, List<Node> traveledNodes)
        {
            // retrive closest known nodes
            var closestNodes = RoutingTable.GetKClosestTo(target);

            // add myself to the list of visited nodes
            var traveledNodesPlusMe = new List<Node>();
            traveledNodesPlusMe.AddRange(traveledNodes);
            traveledNodesPlusMe.Add(this);

            // update my routing table with the traveled nodes
            UpdateRoutingTable(traveledNodes);

            // return the k-closest nodes and the traveled nodes
            return new FindNodeResponse
            {
                TraveledNodes = traveledNodesPlusMe,
                ClosestNodes = closestNodes
            };
        }

        /// <summary>
        /// Lookup the specified target.
        /// </summary>
        /// <returns>The k absolute closest node in the network to the target node.</returns>
        /// <param name="target">The target node.</param>
        public List<Node> Lookup(Identifier target)
        {
            // get alpha nodes that for this node are closer to the target
            var findNodeResponse = FindNode(target);

            // the traveled nodes
            var traveledNodes = findNodeResponse.TraveledNodes;

            var myClosestNodes = findNodeResponse.ClosestNodes
                .OrderBy(n => n.Id.GetDistanceTo(target))
                .Take(Coordinator.Constants.Alpha)
                .ToList();

            // i do not know any closer node
            if (myClosestNodes.Count == 0)
            {
                //Console.WriteLine($" - end lookup = none");
                return myClosestNodes;
            }

            // contains the k absolute closest nodes to the target (returned value)
            var kAbsoluteClosest = new List<Node>()
                .Concat(myClosestNodes)
                .ToList();

            // the closest known node => there is always at least the head of this list (the bootstrap node)
            var closestNode = myClosestNodes.First();

            // the nodes returned by the current cycle findNode()
            var alphaMaybeQueriedNodes = myClosestNodes;

            // contains the identifiers already queried
            var queriedIdentifiers = new HashSet<Identifier>
            {
                // a FindNode() on my self for this target has already been performed
                Id
            };

            // indicates if there are more closer nodes, so if the findNode() should advance
            var hasCloserNodes = true;

            do
            {
                // contains at most the k*alpha nodes returned by the alpha nodes in the alphaMaybeQueriedNodes list
                var currentNodes = new List<Node>();

                // the following cycle will always be performed in an array
                // of at max this.constants.alpha times, by design
                alphaMaybeQueriedNodes.ForEach(node =>
                {
                    // add all nodes if they are not already queried
                    if (!queriedIdentifiers.Contains(node.Id))
                    {
                        // this node hasn't been queried yet
                        var response = node.FindNode(target, traveledNodes);

                        //traveledNodes = response.TraveledNodes;

                        // add all k nodes returned from this node
                        currentNodes = currentNodes.Concat(response.ClosestNodes).ToList();

                        // mark it as queried
                        queriedIdentifiers.Add(node.Id);
                    }
                });

                // insert all k*alpha nodes in routing table
                UpdateRoutingTable(currentNodes);

                // compute closest node of this run
                var runClosestNode = currentNodes.FirstOrDefault(n => n.Id.GetDistanceTo(target) < closestNode.Id.GetDistanceTo(target));

                if (runClosestNode != null)
                {
                    // a new closest node has been found
                    closestNode = runClosestNode;

                    // returned nodes are more closer, merge them with actual results and pick k best
                    kAbsoluteClosest = kAbsoluteClosest
                        .Concat(currentNodes)
                        .Distinct()
                        .OrderBy(n => n.Id.GetDistanceTo(target))
                        .Take(Coordinator.Constants.K)
                        .ToList();

                    // make a step closer, choose alpha closest nodes in which perform a findNode()
                    alphaMaybeQueriedNodes = kAbsoluteClosest
                        .Where(n => !queriedIdentifiers.Contains(n.Id))
                        .Take(Coordinator.Constants.Alpha)
                        .ToList();
                }
                else
                {
                    // the nodes returned by the alpha nodes are no closer than actual ones, stop lookup
                    hasCloserNodes = false;
                }

            } while (hasCloserNodes);

            // call a findNode() to all k closest nodes not already queried
            kAbsoluteClosest.ForEach(node =>
            {
                if (!queriedIdentifiers.Contains(node.Id))
                    node.FindNode(target, traveledNodes);
            });

            return kAbsoluteClosest;
        }

        /// <summary>
        /// Ping this instance.
        /// </summary>
        /// <returns>The ping (always true).</returns>
        public bool Ping()
        {
            return true;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Kademlia.Core.Node"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Kademlia.Core.Node"/>.</returns>
        public override string ToString()
        {
            return Id.ToString();
        }

        #endregion
    }
}
