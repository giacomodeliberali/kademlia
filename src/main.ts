import { Coordinator } from "./coordinator.model";
import { Constants } from "./constants";
import * as fs from "fs";

(() => {
    const m = 27;
    const n = 1000;
    const k = 22;
    const alpha = 3;

    const constants = new Constants(k, m, n, alpha);

    const coordinator = new Coordinator(constants);
    coordinator.bootstrapNetwork();

    fs.writeFileSync("./graph.json", coordinator.generateGraphJson());
})();