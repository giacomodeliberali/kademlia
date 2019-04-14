import { Node } from "./node.model";
import { Constants } from "./constants";
import { IdentifierGenerator } from "./identifier-generator.service";

export class Coordinator {

    private nodes: Array<Node>;

    constructor(private params: Constants) {

        this.nodes = [];
    }

    private getNodeById(id: number) {
        return this.nodes.find(node => node.identifier.id == id);
    }

    public bootstrapNetwork() {
        // insert first bootstrap node with empty routing table
        const bootNodeId = IdentifierGenerator.instance.generateIdentifier(this.params.m);
        const bootNode = new Node(bootNodeId, this.params.k, this.params.m);
        this.nodes.push(bootNode);

        for (let i = 1; i < this.params.n; i++) {
            this.joinNewNode();
        }
        console.log("Nodes => " + this.nodes.length);
    }

    private joinNewNode() {

        // generate a new node
        const newNodeId = IdentifierGenerator.instance.generateIdentifier(this.params.m);
        const newNode = new Node(newNodeId, this.params.k, this.params.m);

        // choose bootstrap node of newNode
        const bootstrapNode = this.getNodeById(IdentifierGenerator.instance.getRandomExistingId());

        // insert bootstrap into new node's routing table
        newNode.updateRoutingTable(bootstrapNode);

        bootstrapNode.lookup(newNode);

        this.nodes.push(newNode);


    }

}