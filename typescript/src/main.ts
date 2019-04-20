import { Coordinator } from "./coordinator.model";
import { Constants } from "./constants";
import * as fs from "fs";

(() => {
    const m = 25;
    const n = 100;
    const k = 3;
    const alpha = 3;

    const constants = new Constants(k, m, n, alpha);

    const coordinator = new Coordinator(constants);
    coordinator.bootstrapNetwork();

    fs.writeFileSync(`./graphs/graph_n${constants.n}_m${constants.m}_k${constants.k}_alpha${constants.alpha}.json`, coordinator.generateGraphJson());
    fs.writeFileSync(`./graphs/graph_n${constants.n}_m${constants.m}_k${constants.k}_alpha${constants.alpha}.csv`, coordinator.generateGraphCsv());
})();

