import { apiRequest } from "./api";

// --- CENTRALIZA LOGIN, CADASTRO E PERFIL ---
export const authService = {

    // --- AUTENTICA O USUARIO E PERSISTE TOKEN + DADOS NA SESSAO ---
    async login(payload) {
        const data = await apiRequest("/usuario/login", {
            method: "POST",
            body: JSON.stringify(payload)
        });

        localStorage.setItem("token", data.token);
        localStorage.setItem("usuario", JSON.stringify(data.usuario));
        return data;
    },

    // --- REGISTRA UM NOVO USUARIO ---
    async cadastro(payload) {
        return apiRequest("/usuario/registrar", {
            method: "POST",
            body: JSON.stringify(payload)
        });
    },

    // --- ENCERRA A SESSAO REMOVENDO TOKEN E USUARIO ---
    logout() {
        localStorage.removeItem("token");
        localStorage.removeItem("usuario");
    },

    // --- ATUALIZA O PERFIL DO USUARIO LOGADO ---
    async atualizarPerfil(payload) {
        const data = await apiRequest("/usuario/me", {
            method: "PUT",
            body: JSON.stringify(payload)
        });

        localStorage.setItem("usuario", JSON.stringify(data));
        return data;
    },

    // --- BUSCA O PERFIL ATUAL DO USUARIO LOGADO ---
    async buscarPerfil() {
        const data = await apiRequest("/usuario/me");

        localStorage.setItem("usuario", JSON.stringify(data));
        return data;
    }
};
