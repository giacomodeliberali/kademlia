import { Node } from "./node.model";
import { Identifier } from "./identifier.model";
import { Bucket } from "./bucket.model";

export class RoutingTable {

    /**
     * Array of m items containing a bucket where:
     * 
     * - d(node,target) >= 2^i and < 2^i-1
     */
    private buckets: Array<Bucket> = [];

    constructor(
        private m: number,
        private k: number,
        private node: Node) {

        // initialize buckets
        for (let i = 0; i < this.m; i++)
            this.buckets.push(new Bucket(this.k));
    }

    public getKClosestTo(target: Identifier): Array<Node> {

        // k-closest = k-closest U closest contacts from other buckets

        // take closes bucket
        const closestBucket = this.getClosestBucket(target);
        const bucketIndex = this.buckets.findIndex(b => b === closestBucket);

        // get all its nodes
        const kClosestNodes: Array<Node> = closestBucket.getNodes();

        // aggregation logic
        let i = -1;
        let nextBucketIndex = Math.abs((bucketIndex + i)) % this.buckets.length;
        while (kClosestNodes.length < this.k && nextBucketIndex != bucketIndex) {
            // zig zag with pattern: 0, -1, +1, -2, +2 
            // until I find k nodes or I return to original bucket
            const nextBucket = this.buckets[nextBucketIndex];
            kClosestNodes.push(...nextBucket.getNodes());
            i *= -1;
            if (i < 0)
                i--;
            nextBucketIndex = Math.abs((bucketIndex + i)) % this.buckets.length;
        }

        return kClosestNodes;
    }

    public insert(target: Node) {

        if (this.node.equals(target))
            return;

        const bucket = this.getClosestBucket(target.identifier);
        bucket.insert(target);
    }

    private getClosestBucket(target: Identifier): Bucket {

        const distance = this.node.identifier.getDistanceTo(target);

        if (distance == 0)
            return this.buckets[0];

        for (let i = 0; i < this.buckets.length; i++) {
            if (distance >= Math.pow(2, i) && distance < (Math.pow(2, i + 1)))
                return this.buckets[i];
        }

        throw new Error("The given identifier does not match the 2^m limit");
    }

    public getBuckets() {
        return this.buckets;
    }

}