import { Node } from "./node.model";

export class Coordinator {

    private nodes: Array<Node>;

    constructor(
        private m: number,
        private n: number,
        private k: number) {

        this.nodes = [];
    }

    public bootstrapNetwork() {

        // insert first bootstrap node with empty routing table
        const limit = Math.pow(2, this.m) - 1;
        const bootNodeId = this.getRandomInRange(0, limit);
        const bootNode = new Node(bootNodeId, this.k, this.m);
        this.nodes.push(bootNode);

        for (let i = 1; i < this.n; i++) {
            this.joinNewNode();
        }
        console.log("Nodes => " + this.nodes.length);
    }

    private joinNewNode() {
        const limit = Math.pow(2, this.m) - 1;
        const newNodeId = this.getRandomInRange(0, limit, this.nodes.map(n => n.identifier.id));
        const newNode = new Node(newNodeId, this.k, this.m);

        const bootstrapNode = this.nodes[this.getRandomInRange(0, this.nodes.length - 1)]; // will be bootstrap node of newNode

        newNode.updateRoutingTable([bootstrapNode]);

        bootstrapNode.findNode(newNode);

        const extractedNodes: Array<Node> = [];
        const j = 5; // WTF?
        for (let i = 0; i < j; i++) {
            const chosenNode = this.nodes[this.getRandomInRange(0, this.nodes.length - 1, extractedNodes.map(n => n.identifier.id))];
            extractedNodes.push(chosenNode);

            const closesKNodes: Array<Node> = chosenNode.findNode(newNode);
            newNode.updateRoutingTable(closesKNodes);

            // extra calls to further enrich routing tables???
        }
        this.nodes.push(newNode);
    }


    private getRandomInRange(min: number, max: number, exclude: Array<number> = []) {
        let random = Math.floor(Math.random() * Math.floor(max)) + min;
        while (exclude.find(n => n == random)) {
            random = Math.floor(Math.random() * Math.floor(max)) + min;
        }
        return random;
    }
}