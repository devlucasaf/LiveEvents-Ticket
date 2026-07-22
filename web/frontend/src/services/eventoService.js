import { apiRequest } from "./api";

export const eventoService = {
    // --- RETORNA TODOS OS EVENTOS ---
    listar() {
        return apiRequest("/evento");
    },

    // --- RETORNA UM EVENTO PELO ID ---
    buscar(id) {
        return apiRequest(`/evento/${id}`);
    },

    // --- RETORNA OS INGRESSOS DE UM EVENTO ---
    listarIngressos(eventoId) {
        return apiRequest(`/ingresso/evento/${eventoId}`);
    },

    // --- RETORNA AS MODALIDADES DE INGRESSO DISPONÍVEIS ---
    listarModalidades() {
        return apiRequest("/ingresso/modalidades");
    },

    // --- CADASTRA UM NOVO EVENTO ---
    criar(payload) {
        return apiRequest("/admin/evento", {
            method: "POST",
            body: JSON.stringify(payload)
        });
    },

    // --- CADASTRA UM NOVO INGRESSO PARA UM EVENTO ---
    criarIngresso(payload) {
        return apiRequest("/admin/ingresso", {
            method: "POST",
            body: JSON.stringify(payload)
        });
    }
};
