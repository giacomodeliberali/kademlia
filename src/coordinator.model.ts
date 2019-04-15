import { Node } from "./node.model";
import { Constants } from "./constants";
import { IdentifierGenerator } from "./identifier-generator.service";
import { Identifier } from "./identifier.model";

export class Coordinator {

    private nodes: Array<Node>;

    constructor(private constants: Constants) {

        this.nodes = [];
    }

    private getNodeById(id: number) {
        return this.nodes.find(node => node.identifier.id == id);
    }

    public bootstrapNetwork() {

        console.log(`Bootstrapping network of ${this.constants.n} nodes.`);
        console.log(`\t - k = ${this.constants.k}`);
        console.log(`\t - m = ${this.constants.m}`);
        console.log(`\t - alpha = ${this.constants.alpha}\n`);

        // insert first bootstrap node with empty routing table
        const bootNode = new Node(this.constants);
        this.nodes.push(bootNode);

        console.log(`Bootstrap node is ${bootNode.identifier.id}.`);

        for (let i = 1; i < this.constants.n; i++) {
            this.joinNewNode();
            //console.log(`Joined node ${i+1} of ${this.constants.n}`);
            process.stdout.write(`Joining nodes... (${Math.ceil((i+1)/this.constants.n * 100)}%) \r`);
        }
        console.log(`Network bootstrap complete.`);

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
            const randomNodeIdentifierInBucket = new Identifier(
                IdentifierGenerator.instance.getUniqueRandomInRange(Math.pow(2, i), Math.pow(2, i + 1) - 1)
            );
            // and make a lookup of the new node, updating the routing table with results
            newNodeToJoin.updateRoutingTable( // TODO: fix this update that is wrong and produces a clustering coefficient of 1 for a lot of nodes (the starting ones)
                newNodeToJoin.lookup(randomNodeIdentifierInBucket)
            );
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