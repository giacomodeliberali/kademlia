import { Identifier } from "./identifier.model";
import { RoutingTable } from "./routing-table.model";
import { Constants } from "./constants";
import { IdentifierGenerator } from "./identifier-generator.service";
import { NodeArrayHelper } from "../array-helper.helper";
export class Node {

    private routingTable: RoutingTable;

    public identifier: Identifier;

    constructor(private constants: Constants, id?: number) {

        const randomId = id || IdentifierGenerator.instance.generateIdentifier(this.constants.m);
        this.identifier = new Identifier(randomId);
        this.routingTable = new RoutingTable(this.constants.m, this.constants.k, this);
    }



    /**
     * Return the k closest nodes to the target that this node knows,
     * without further requests to other nodes
     * 
     * @param target The target node
     */
    //FIXME: add traveled node (and add this)
    public findNode(target: Identifier): Array<Node> {
        return this.routingTable.getKClosestTo(target);
    }



    /**
     * Return the k absolute closest nodes to the target
     * 
     * @param target The target node
     */
    public lookup(target: Identifier): Array<Node> {

        // contains the k absolute closest nodes to the target (returned value)
        let kAbsoluteClosest: Array<Node> = [];

        // get alpha nodes that for this node are closer to the target
        let myClosestNodes = NodeArrayHelper
            .from(this.findNode(target))
            .selectDistinct()
            .sortByXorDistanceTo(target)
            .limit(this.constants.alpha)
            .get();

        // the closest known node
        let closestNode: Node = myClosestNodes[0]; // there is always at least the head of this array (the bootstrap node)

        // the nodes returned by the current cycle findNode()
        let maybeQueriedNodes: Array<Node> = myClosestNodes;

        // contains the identifiers already queried
        const queriedIdentifiers: Map<number, boolean> = new Map();

        // indicates if there are more closer node and the findNode() should advance
        let hasCloserNodes: boolean = true;

        do {

            const currentNodes: Array<Node> = [];

            maybeQueriedNodes.forEach(node => {
                // add all nodes if they are not already queried
                if (!queriedIdentifiers.has(node.identifier.id)) {
                    // this not hasn't been queried yet
                    currentNodes.push(
                        // add all k nodes returned from this node
                        ...node.findNode(target)
                    );
                    // mark it as queried
                    queriedIdentifiers.set(node.identifier.id, true);
                }
            });

            // compute closest node of this run
            const newClosest = currentNodes.find(n => n.getDistanceTo(target) < closestNode.getDistanceTo(target));

            if (newClosest) {
                // a new closest node has been found
                closestNode = newClosest;

                // returned nodes are more closer, merge them with actual results and pick k best
                kAbsoluteClosest = NodeArrayHelper
                    .fromMerge(kAbsoluteClosest, currentNodes)
                    .selectDistinct()
                    .sortByXorDistanceTo(target)
                    .limit(this.constants.k)
                    .get()

                // make a step closer, choose alpha closest nodes in which perform a findNode()
                maybeQueriedNodes = NodeArrayHelper
                    .from(kAbsoluteClosest)
                    .selectDistinct()
                    .sortByXorDistanceTo(target)
                    .limit(this.constants.k)
                    .get()

            } else {
                // the nodes returned by the alpha nodes are no closer than actual ones, stop lookup
                hasCloserNodes = false;
            }

        } while (hasCloserNodes);

        // call a findNode() to all k closest nodes not already queried
        kAbsoluteClosest.forEach(node => {
            if (!queriedIdentifiers.has(node.identifier.id))
                node.findNode(target);
        });

        return kAbsoluteClosest;
    }

    public ping(): boolean {
        return true;
    }

    public updateRoutingTable(nodes: Array<Node> | Node) {
        if (Array.isArray(nodes))
            nodes.forEach(n => this.routingTable.insert(n));
        else
            this.routingTable.insert(nodes);
    }

    equals(node: Node) {
        return this.identifier.equals(node.identifier);
    }


    getDistanceTo(target: Identifier) {
        return this.identifier.getDistanceTo(target);
    }

    getRoutingTable() {
        return this.routingTable;
    }

}