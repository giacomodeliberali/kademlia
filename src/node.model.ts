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
    public findNode(target: Node): Array<Node> {
        this.routingTable.insert(target);
        return this.routingTable.getKClosestTo(target.identifier);
    }

    /**
     * Return the k absolute closest nodes to the target
     * 
     * @param target The target node
     */
    public lookup(target: Node): Array<Node> {

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

        const kAbsoluteClosest: Array<Node> = [];

        // get alpha nodes that for this node are closer to the target
        let myClosestNodes = this.findNode(target);
        if (myClosestNodes.length > this.constants.alpha)
            myClosestNodes = myClosestNodes.slice(0, this.constants.alpha);

        // indicate if there exist node closer
        let hasCloserNodes = true;

        // save the nodes returned by previous findNode() calls
        // to detect when there is no node closer to target
        let prevNodes: Array<Node> = [];

        // the nodes returned by the current cycle findNode()
        let currentNodes: Array<Node> = myClosestNodes;

        do {

            const nextClosest: Array<Node> = [];

            currentNodes.forEach(node => {
                // add all nodes
                nextClosest.push(...node.findNode(target));
            });

            const prevFarthest = Math.min(...prevNodes.map(q => q.getDistanceTo(target))) || 0;

            if (!nextClosest.some(n => n.getDistanceTo(target) > prevFarthest)) {
                // non esiste un nodo più vicino di quello più lontano ritornato dal ciclo precedente,
                // interrompi il while
                hasCloserNodes = false;
            } else {
                // returned nodes are more closer, add them
                kAbsoluteClosest.push(...nextClosest);
            }

            prevNodes = currentNodes;

        } while (hasCloserNodes);


        kAbsoluteClosest.sort((a, b) => b.getDistanceTo(target) - a.getDistanceTo(target));

        if (kAbsoluteClosest.length > this.constants.k)
            return kAbsoluteClosest.slice(0, this.constants.k);

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

    getDistanceTo(node: Node) {
        return this.identifier.getDistanceTo(node.identifier);
    }

    getRoutingTable() {
        return this.routingTable;
    }

}