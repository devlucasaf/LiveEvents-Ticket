import { apiRequest } from "./api";

const API_BASE_URL = "http://localhost:5000/api";

// --- CHAMADAS DA API RELACIONADAS A PEDIDOS E POS-COMPRA ---
export const pedidoService = {
    checkout(payload) {
        return apiRequest("/pedido/checkout", {
            method: "POST",
            body: JSON.stringify(payload)
        });
    },

    // --- LISTA OS PEDIDOS DO USUARIO AUTENTICADO ---
    meusPedidos() {
        return apiRequest("/pedido/meus");
    },

    // --- SOLICITA REEMBOLSO COM MOTIVO E DETALHES ---
    solicitarReembolso(pedidoId, payload = {}) {
        const body = typeof payload === "string"
            ? { motivo: payload }
            : {
                motivo: payload.motivo || "",
                motivoCodigo: payload.motivoCodigo || "",
                motivoDetalhe: payload.motivoDetalhe || ""
            };

        // --- ENVIA A SOLICITACAO DE REEMBOLSO PARA O PEDIDO INFORMADO ---
        return apiRequest(`/pedido/${pedidoId}/reembolso/solicitar`, {
            method: "POST",
            body: JSON.stringify(body)
        });
    },

    // --- BAIXA O PDF DO INGRESSO DO PEDIDO ---
    async baixarIngressoPdf(pedidoId) {
        const token = localStorage.getItem("token");

        // --- MONTA E EXECUTA O GET DO PDF DO INGRESSO ---
        const resposta = await fetch(`${API_BASE_URL}/pedido/${pedidoId}/ingresso/pdf`, {
            method: "GET",
            headers: {
                ...(token ? { Authorization: `Bearer ${token}` } : {})
            }
        });

        if (resposta.status === 401) {
            localStorage.removeItem("token");
            localStorage.removeItem("usuario");
            throw new Error("Sessão expirada. Faça login novamente.");
        }

        if (!resposta.ok) {
            const payload = await resposta.json().catch(() => ({}));
            throw new Error(payload.message || "Não foi possível baixar o ingresso em PDF.");
        }

        const blob = await resposta.blob();
        const contentDisposition = resposta.headers.get("content-disposition") || "";
        const nomeMatch = contentDisposition.match(/filename="?([^";]+)"?/i);

        return {
            blob,
            nomeArquivo: nomeMatch?.[1] || `ingresso-pedido-${pedidoId}.pdf`
        };
    },

    // --- BAIXA O COMPROVANTE DE ESTORNO EM PDF ---
    async baixarComprovanteEstornoPdf(pedidoId) {
        const token = localStorage.getItem("token");

        // --- MONTA E EXECUTA O GET DO PDF DE COMPROVANTE DE ESTORNO ---
        const resposta = await fetch(`${API_BASE_URL}/pedido/${pedidoId}/reembolso/comprovante/pdf`, {
            method: "GET",
            headers: {
                ...(token ? { Authorization: `Bearer ${token}` } : {})
            }
        });

        if (resposta.status === 401) {
            localStorage.removeItem("token");
            localStorage.removeItem("usuario");
            throw new Error("Sessão expirada. Faça login novamente.");
        }

        if (!resposta.ok) {
            const payload = await resposta.json().catch(() => ({}));
            throw new Error(payload.message || "Não foi possível baixar o comprovante de estorno.");
        }

        const blob = await resposta.blob();
        const contentDisposition = resposta.headers.get("content-disposition") || "";
        const nomeMatch = contentDisposition.match(/filename="?([^";]+)"?/i);

        return {
            blob,
            nomeArquivo: nomeMatch?.[1] || `comprovante-estorno-pedido-${pedidoId}.pdf`
        };
    },

    // --- GERA LINK TEMPORARIO PARA COMPARTILHAR PDF DO INGRESSO ---
    gerarLinkCompartilhamento(pedidoId, payload = {}) {
        return apiRequest(`/pedido/${pedidoId}/compartilhar`, {
            method: "POST",
            body: JSON.stringify(payload)
        });
    },

    // --- REVOGA LINK DE COMPARTILHAMENTO DO PEDIDO ---
    revogarLinkCompartilhamento(pedidoId) {
        return apiRequest(`/pedido/${pedidoId}/compartilhar/revogar`, {
            method: "POST"
        });
    },

    // --- OBTEM O RELATORIO DE VENDAS DO MODULO ADMIN ---
    relatorioVendas() {
        return apiRequest("/admin/relatorio/vendas");
    }
};
