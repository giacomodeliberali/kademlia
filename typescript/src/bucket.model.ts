import { Node } from "./node.model";

export class Bucket {


    private nodes: Array<Node> = [];

    constructor(private k: number) {

    }

    public insert(target: Node) {

        const targetNode = this.nodes.find(n => n.equals(target));

        if (targetNode) {
            // the node exist in bucket, move it to the tail
            const index = this.nodes.findIndex(n => n.equals(target));
            this.nodes.splice(index, 1);
            this.nodes.push(targetNode);
            return;
        }

        if (!this.isFull()) {
            // the node does not exist and the bucket is not full, append new node in the bucket's tail
            this.nodes.push(target);
            return;
        }

        // the node does not exist and the bucket is full. So ping least seen node 
        const leastSeenNode = this.nodes.shift();

        if (leastSeenNode.ping()) {
            // promote least seen node and discard new node
            this.nodes.push(leastSeenNode);
        } else {
            // promote new node and discard least seen node 
            this.nodes.push(targetNode);
        }
    }

    public length() {
        return this.nodes.length;
    }

    public isFull() {
        return this.length() == this.k;
    }

    public getNodes(predicate: (id: Node) => boolean = () => true) {
        return this.nodes.filter(predicate);
    }
}