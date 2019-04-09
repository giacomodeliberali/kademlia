import { Coordinator } from "./coordinator.model";

(() => {
    const m = 25;
    const n = 10000;
    const k = 20;
    const coordinator = new Coordinator(m, n, k);
    coordinator.bootstrapNetwork();
})();