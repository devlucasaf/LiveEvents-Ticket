import { apiRequest } from "./api";

export const pedidoService = {
    checkout(payload) {
        return apiRequest("/pedido/checkout", {
            method: "POST",
            body: JSON.stringify(payload)
        });
    },

    meusPedidos() {
        return apiRequest("/pedido/meus");
    },

    relatorioVendas() {
        return apiRequest("/admin/relatorio/vendas");
    }
};
