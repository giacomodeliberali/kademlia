import { Coordinator } from "./coordinator.model";
import { Constants } from "./constants";

(() => {
    const m = 8;
    const n = 10;
    const k = 1;

    const constants = new Constants(k, m, n);

    const coordinator = new Coordinator(constants);
    coordinator.bootstrapNetwork();
})();