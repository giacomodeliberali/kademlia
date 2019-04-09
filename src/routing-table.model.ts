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
        private k: number) {

        // initialize buckets
        for (let i = 0; i < this.m; i++)
            this.buckets.push(new Bucket(this.k));
    }

    public getKClosestTo(source: Node, target: Identifier): Array<Node> {

        let kClosestNodes: Array<Node> = [];

        const closestBucket = this.getClosestBucket(source, target);

        const bucketLength = closestBucket.length();

        if (bucketLength < this.k) {
            // k-closest = k-closest U closest contacts from other buckets
            kClosestNodes = closestBucket.getNodes();

            const bucketIndex = this.buckets.findIndex(b => b === closestBucket);

            // aggregation logic

            let i = 1;
            //while (kClosestNodes.length < this.k) {


            if (bucketIndex > 0) {
                // push all nodes in prev bucket
                const prevBucket = this.buckets[bucketIndex - i];
                kClosestNodes.push(...prevBucket.getNodes());
            }

            if (bucketIndex < this.buckets.length - 1) {
                // push all nodes in next bucket
                const nextBucket = this.buckets[bucketIndex + i];
                kClosestNodes.push(...nextBucket.getNodes());
            }
            i++;
            //}

        } else {
            kClosestNodes = closestBucket.getNodes();
        }

        kClosestNodes.sort((a, b) => {
            return b.identifier.getDistanceTo(source.identifier) - a.identifier.getDistanceTo(source.identifier);;
        });

        if (kClosestNodes.length > this.k)
            return kClosestNodes.slice(0, this.k);

        return kClosestNodes;
    }

    public insert(source: Node, target: Node) {

        if (source.equals(target))
            return;

        const bucket = this.getClosestBucket(source, target.identifier);
        bucket.insert(target);
    }

    private getClosestBucket(source: Node, target: Identifier): Bucket {

        const distance = source.identifier.getDistanceTo(target);

        let bucket: Bucket = null;
        for (let i = 0; i < this.buckets.length; i++) {
            if (distance >= Math.pow(2, i) && distance < (Math.pow(2, i + 1))) {
                bucket = this.buckets[i];
                break; // TODO: refactor this shit
            }
        }

        return bucket;
    }

}