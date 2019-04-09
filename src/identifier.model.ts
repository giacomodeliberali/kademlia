export class Identifier {

    constructor(public readonly id: number) {

    }

    getDistanceTo(identifier: Identifier) {
        return this.id ^ identifier.id;
    }
    equals(identifier: Identifier) {
        return this.id == identifier.id;
    }

}