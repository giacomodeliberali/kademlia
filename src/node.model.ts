import { Identifier } from "./identifier.model";
import { RoutingTable } from "./routing-table.model";

export class Node {

    private routingTable: RoutingTable;

    public identifier: Identifier;

    constructor(
        id: number,
        private k: number,
        private m: number) {

        this.identifier = new Identifier(id);
        this.routingTable = new RoutingTable(m, k, this);
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

    private containsCloserNodes(source: Array<Node>, target: Array<Node>) {
        return false;
    }

    /**
     * Return the k absolute closest nodes to the target
     * 
     * @param target The target node
     */
    public lookup(target: Node): Array<Node> {

        const kAbsoluteClosest: Array<Node> = [];

        const kRelativeCloser = this.findNode(target);

        //kClosest.sort((a, b) => b.getDistanceTo(a));

        //TODO: alpha as parameter
        const alphaRelativeCloserNodes = kRelativeCloser;//kClosest.slice(0, kClosest.length > 3 ? 3 : kClosest.length);

        /*
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

}