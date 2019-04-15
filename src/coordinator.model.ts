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

        // bootstrapNode.lookup(newNode); // => it is done implicitly at the first lookup() since it's the only node in the routing table 

        for (let i = 0; i < this.constants.m; i++) {
            // generate a random id (never extracted) for each bucket range
            /*
                TODO: the generated id in the bucket must take into account the distance from the newNodeToJoin.
                Now it is not considering the XOR distance but only the identifier int number.
                Basically the identifier (it will be a hash) must by constructed by hand taking the hash of the identifier in a bucket and
                modifying it as a string, so char per char (or bit per bit)
            */
            const nodeId = IdentifierGenerator.instance.getUniqueRandomInRange(Math.pow(2, i), Math.pow(2, i + 1) - 1);
            const randomNodeInBucket = new Node(this.constants, nodeId);

            // and make a lookup of the new node
            newNodeToJoin.lookup(randomNodeInBucket);
            //this.nodes.push(randomNodeInBucket); // TODO: check this push
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
        // TODO: be sure that all nodes are present in the nodes list
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