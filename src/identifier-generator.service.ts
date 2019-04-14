export class IdentifierGenerator {

    public static instance: IdentifierGenerator = new IdentifierGenerator();

    private extractedIds: Array<number>;

    private constructor() {
        this.extractedIds = [];
    }

    public getRandomExistingId(): number {
        const indexPosition = this.getRandomInRange(0, this.extractedIds.length - 1);
        return this.extractedIds[indexPosition];
    }

    public generateIdentifier(m: number): number {

        const limit = Math.pow(2, m) - 1;

        let nodeId = this.getRandomInRange(0, limit);

        while (!!this.extractedIds.find(id => id == nodeId)) {
            nodeId = this.getRandomInRange(0, limit);
        }

        this.extractedIds.push(nodeId);

        return nodeId;
    }

    private getRandomInRange(min: number, max: number) {
        return Math.floor(Math.random() * Math.floor(max)) + min;
    }
}