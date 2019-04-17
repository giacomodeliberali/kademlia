import { Node } from "./node.model";
import { Identifier } from "./identifier.model";

export class NodeArrayHelper {
    public static from(source: Array<Node>) {
        return new ConfigurableArray(source);
    }

    public static fromMerge(first: Array<Node>, second: Array<Node>) {
        return new ConfigurableArray([...first, ...second]);
    }
}


class ConfigurableArray {

    constructor(private array: Array<Node>) {

    }

    public limit(limit: number) {
        if (this.array.length > limit)
            this.array = this.array.slice(0, limit);
        return this;
    }

    public selectDistinct() {
        const map = new Map<number, boolean>();
        this.array = this.array
            .map(node => { // remove duplicated nodes (selected distinct)
                if (!map.has(node.identifier.id)) {
                    map.set(node.identifier.id, true);
                    return node;
                }
                return null;
            })
            .filter(node => !!node) // remove non valid nodes

        return this;
    }

    public removeAnyInside(alreadyQueried: Iterable<number>) {
        for (let id of alreadyQueried) {
            const index = this.array.findIndex(node => node.identifier.id == id);
            if (index >= 0)
                this.array.splice(index, 1);
        }
        return this;
    }

    public sortByXorDistanceTo(target: Identifier) {
        this.array.sort((a, b) => b.getDistanceTo(target) - a.getDistanceTo(target)); // sort DESC

        return this;
    }

    public get() {
        return this.array;
    }
}