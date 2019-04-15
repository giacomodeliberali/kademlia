import { Identifier } from "./identifier.model";
import { RoutingTable } from "./routing-table.model";
import { Constants } from "./constants";
import { IdentifierGenerator } from "./identifier-generator.service";

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
    public findNode(target: Identifier): Array<Node> {
        return this.routingTable.getKClosestTo(target);
    }

    private sliceCloserTo(source: Array<Node>, target: Identifier, limit: number) {

        source.sort((a, b) => b.getDistanceTo(target) - a.getDistanceTo(target)); // sort DESC

        if (source.length > limit)
            return source.slice(0, limit);

        return source;
    }

    /**
     * Return the k absolute closest nodes to the target
     * 
     * @param target The target node
     */
    public lookup(target: Identifier): Array<Node> {

        /*
                IDEA:
                
                kAbsoluteClosest = [];
                myClosestNodes = this.findNode(target)
                alphaNodes = choose alpha closer node from myClosestNodes 

                do{
                    foreach node in alphaNodes {
                        kNextCloser = dammi i k più vicini secondo il prossimo nodo
                        kAbsoluteClosest.addAll(kNextCloser)
                    }
                } while(esistono nodi piu vicini) 
                
                break while when --> quando i kNextCloser non contengono nodi più vicini rispetto alla chiamata precedente

                kAbsoluteClosest.sortByDistance

                return i primi k di kAbsoluteClosest
        */

        let kAbsoluteClosest: Array<Node> = [];

        // get alpha nodes that for this node are closer to the target
        let myClosestNodes = this.sliceCloserTo(this.findNode(target), target, this.constants.alpha);

        // indicate if there exist node closer
        let hasCloserNodes = true;

        // the nodes returned by the current cycle findNode()
        let currentNodes: Array<Node> = myClosestNodes;

        do {

            const nextClosest: Array<Node> = [];

            currentNodes.forEach(node => {
                // add all nodes
                nextClosest.push(...node.findNode(target));
            });

            const farthestDistance = kAbsoluteClosest.length > 0 ? Math.min(...kAbsoluteClosest.map(q => q.getDistanceTo(target))) : 0;

            if (nextClosest.some(n => n.getDistanceTo(target) > farthestDistance)) {
                // returned nodes are more closer, merge them with actual results and pick k best
                kAbsoluteClosest = this.sliceCloserTo([...kAbsoluteClosest, ...nextClosest], target, this.constants.k);

                // make a step closer, choose alpha closest nodes in which perform a findNode()
                currentNodes = this.sliceCloserTo(kAbsoluteClosest, target, this.constants.alpha);

            } else {
                // the nodes returned by the alpha nodes are no closer than actual ones, stop lookup
                hasCloserNodes = false;
            }

        } while (hasCloserNodes);

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