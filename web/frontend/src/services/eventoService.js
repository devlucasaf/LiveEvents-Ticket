import { apiRequest } from "./api";

export const eventoService = {
    listar() {
        return apiRequest("/evento");
    },

    buscar(id) {
        return apiRequest(`/evento/${id}`);
    },

    listarIngressos(eventoId) {
        return apiRequest(`/ingresso/evento/${eventoId}`);
    },

    listarModalidades() {
        return apiRequest("/ingresso/modalidades");
    },

    criar(payload) {
        return apiRequest("/admin/evento", {
            method: "POST",
            body: JSON.stringify(payload)
        });
    },

    criarIngresso(payload) {
        return apiRequest("/admin/ingresso", {
            method: "POST",
            body: JSON.stringify(payload)
        });
    }
};
