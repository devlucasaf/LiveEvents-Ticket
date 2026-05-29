import { apiRequest } from './api';

export const eventoService = {
    listar() {
        return apiRequest('/evento');
    },

    buscar(id) {
        return apiRequest(`/evento/${id}`);
    },

    listarIngressos(eventoId) {
        return apiRequest(`/ingresso/evento/${eventoId}`);
    }
};
