const API_BASE_URL = "http://localhost:5000/api";

// --- EXECUTA REQUISICAO HTTP AUTENTICADA PARA A API ---
export async function apiRequest(path, options = {}) {
    const token = localStorage.getItem("token");

    // --- MONTA A REQUISICAO COM CABECALHOS PADRAO E TOKEN ---
    const response = await fetch(`${API_BASE_URL}${path}`, {
        headers: {
        "Content-Type": "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...(options.headers || {})
        },
        ...options
    });

    // --- TRATAMENTO DE SESSÃO EXPIRADA ---
    if (response.status === 401) {
        localStorage.removeItem("token");
        localStorage.removeItem("usuario");
        if (!window.location.pathname.startsWith("/auth/login")) {
            const retorno = encodeURIComponent(window.location.pathname + window.location.search);
            window.location.href = `/auth/login?retorno=${retorno}`;
        }
        throw new Error("Sessão expirada. Faça login novamente.");
    }

    if (!response.ok) {
        const payload = await response.json().catch(() => ({}));
        throw new Error(payload.message || "Erro na requisição");
    }

    return response.status === 204 ? null : response.json();
}
