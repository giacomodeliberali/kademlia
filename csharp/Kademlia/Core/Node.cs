using System.Linq;
using System.Collections.Generic;
using Kademlia.Helpers;
using Kademlia.Models;
using System;

namespace Kademlia.Core
{
    public class Node
    {

        public Identifier Id { get; }
        public RoutingTable RoutingTable { get; }

        public Node()
        {
            Id = IdentifierGenerator.Instance.GenerateIdentifier();
            RoutingTable = new RoutingTable(this);
        }

        public Node(Identifier identifier)
        {
            Id = identifier;
            RoutingTable = new RoutingTable(this);
        }

        public void UpdateRoutingTable(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
                UpdateRoutingTable(node);
        }

        public void UpdateRoutingTable(Node node)
        {
            if (Id.Equals(node.Id))
                return;

            RoutingTable.Insert(node);
        }
        public FindNodeResponse FindNode(Identifier target)
        {
            return FindNode(target, new List<Node>());
        }

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

        public IList<Node> Lookup(Identifier target)
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


        public bool Ping()
        {
            return true;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
