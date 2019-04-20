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

        console.log(`Bootstrapping network of ${this.constants.n} nodes [${new Date()}].`);
        console.log(`\t - k = ${this.constants.k}`);
        console.log(`\t - m = ${this.constants.m}`);
        console.log(`\t - alpha = ${this.constants.alpha}`);

        // insert first bootstrap node with empty routing table
        const bootNode = new Node(this.constants);
        this.nodes.push(bootNode);

        console.log(`Bootstrap node is ${bootNode.identifier.id}.`);

        let date = new Date();
        for (let i = 1; i < this.constants.n; i++) {
            this.joinNewNode();
            //console.log(`Joined node ${i+1} of ${this.constants.n}`);
            //process.stdout.write(`Joining nodes... (${Math.ceil((i + 1) / this.constants.n * 100)}%) \r`);
            process.stdout.write(`Joining nodes... (${i}/${this.constants.n}) \r`);
        }

        let edgeCount = 0;
        this.nodes.forEach(n => {
            n.getRoutingTable().getBuckets().forEach(bucket => {
                bucket.getNodes().forEach(link => {
                    edgeCount++;
                });
            });
        });


        console.log(`Network bootstrap complete (${edgeCount} edges) [${Math.abs((new Date().getTime() - date.getTime()) / 1000)}sec].\n`);

        return this.nodes;
    }

    private joinNewNode() {

        // generate a new node
        const newNodeToJoin = new Node(this.constants);

        // choose bootstrap node of newNode
        const bootstrapNode = this.getNodeById(IdentifierGenerator.instance.getRandomExistingId());

        // insert bootstrap into new node's routing table
        newNodeToJoin.updateRoutingTable(bootstrapNode);

        // perform a self-lookup
        const selfLookup = newNodeToJoin.lookup(newNodeToJoin.identifier);
        //console.log(`SelfLookup of ${newNodeToJoin.identifier.id}: `, selfLookup);
        newNodeToJoin.updateRoutingTable(selfLookup);

        // refreshes all k-buckets 
        for (let i = 0; i < this.constants.m; i++) {
            // generate a random id (never extracted) for each bucket range
            const randomNodeIdentifierInBucket = new Identifier(
                IdentifierGenerator.instance.getUniqueRandomInRange(Math.pow(2, i), Math.pow(2, i + 1) - 1)
            );
            // and make a lookup of the new node
            newNodeToJoin.updateRoutingTable(
                //CHECK: this updateRoutingTable is correct?
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

    public generateGraphCsv() {
        let csv = "Source;Target";
        this.nodes.forEach(n => {
            n.getRoutingTable().getBuckets().forEach(bucket => {
                bucket.getNodes().forEach(link => {
                    csv += `${n.identifier.id.toString()};${link.identifier.id.toString()}\n`;
                });
            });
        });
        return csv;
    }

}