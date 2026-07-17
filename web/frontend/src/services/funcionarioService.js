import { apiRequest } from "./api";

// --- SERVICO DE FUNCIONARIOS DO PDV ---
export const funcionarioService = {
    listar() {
        return apiRequest("/admin/funcionarios");
    },

    // --- CADASTRAR NOVO FUNCIONARIO ---
    criar(payload) {
        return apiRequest("/admin/funcionarios", {
            method: "POST",
            body: JSON.stringify(payload)
        });
    },

    // --- EDITAR FUNCIONARIO ---
    atualizar(id, payload) {
        return apiRequest(`/admin/funcionarios/${id}`, {
            method: "PUT",
            body: JSON.stringify(payload)
        });
    },

    // --- ATIVAR/DESATIVAR FUNCIONARIO ---
    alterarStatus(id, ativo) {
        return apiRequest(`/admin/funcionarios/${id}/status`, {
            method: "PATCH",
            body: JSON.stringify({ ativo })
        });
    }
};
