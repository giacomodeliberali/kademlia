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
        this.routingTable = new RoutingTable(m, k);
    }

    public findNode(node: Node): Array<Node> {
        this.routingTable.insert(this, node);
        return this.routingTable.getKClosestTo(this, node.identifier);
    }

    public ping(): boolean {
        return true;
    }

    public updateRoutingTable(nodes: Array<Node>) {
        nodes.forEach(n => this.routingTable.insert(this, n));
    }

    equals(node: Node) {
        return this.identifier.equals(node.identifier);
    }

    getDistanceTo(node:Node){
        return this.identifier.getDistanceTo(node.identifier);
    }

}




