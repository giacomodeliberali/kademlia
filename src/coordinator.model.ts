import { Node } from "./node.model";
import { Constants } from "./constants";
import { IdentifierGenerator } from "./identifier-generator.service";

export class Coordinator {

    private nodes: Array<Node>;

    constructor(private constants: Constants) {

        this.nodes = [];
    }

    private getNodeById(id: number) {
        return this.nodes.find(node => node.identifier.id == id);
    }

    public bootstrapNetwork() {
        // insert first bootstrap node with empty routing table
        const bootNode = new Node(this.constants);
        this.nodes.push(bootNode);

        for (let i = 1; i < this.constants.n; i++) {
            this.joinNewNode();
        }
        console.log("Nodes => " + this.nodes.length);

        return this.nodes;
    }

    private joinNewNode() {

        // generate a new node
        const newNodeToJoin = new Node(this.constants);

        // choose bootstrap node of newNode
        const bootstrapNode = this.getNodeById(IdentifierGenerator.instance.getRandomExistingId());

        // insert bootstrap into new node's routing table
        newNodeToJoin.updateRoutingTable(bootstrapNode);

        // bootstrapNode.lookup(newNode);

        for (let i = 0; i < this.constants.m; i++) {
            // generate a random id (never extracted) for each bucket range
            const nodeId = IdentifierGenerator.instance.getUniqueRandomInRange(Math.pow(2, i), Math.pow(2, i + 1) - 1);
            const randomNodeInBucket = new Node(this.constants, nodeId);

            // and make a lookup of the new node
            newNodeToJoin.lookup(randomNodeInBucket);
        }

        this.nodes.push(newNodeToJoin);

    }

    public generateGraphJson() {

        const json: any = {
            elements: {
                nodes: [],
                edges: []
            }
        };
        this.nodes.forEach(n => {
            json.elements.nodes.push({
                data: {
                    id: n.identifier.id.toString()
                }
            });
            n.getRoutingTable().getBuckets().forEach(bucket => {
                bucket.getNodes().forEach(link => {
                    json.elements.edges.push({
                        data: {
                            source: n.identifier.id.toString(),
                            target: link.identifier.id.toString()
                        }
                    });
                });
            });
        });
        return JSON.stringify(json);
    }

}