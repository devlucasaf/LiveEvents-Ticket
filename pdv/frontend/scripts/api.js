const API_BASE_URL = "/api";

const Api = {
    async _request(metodo, caminho, corpo = null, { autenticar = true } = {}) {
        const headers = { "Content-Type": "application/json" };

        if (autenticar) {
            const token = Auth.obterToken();
            if (token) {
                headers["Authorization"] = `Bearer ${token}`;
            }
        }

        const opcoes = { method: metodo, headers };
        if (corpo !== null) {
            opcoes.body = JSON.stringify(corpo);
        }

        let resposta;
        try {
            resposta = await fetch(`${API_BASE_URL}${caminho}`, opcoes);
        } catch (erro) {
            throw new Error("Não foi possível conectar ao servidor. Verifique se o backend está rodando.");
        }

        if (resposta.status === 401 && autenticar) {
            Auth.encerrarSessao();
            window.location.href = Rotas.login;
            throw new Error("Sessão expirada.");
        }

        if (resposta.status === 204) {
            return null;
        }

        let dados = null;
        const tipoConteudo = resposta.headers.get("content-type") || "";
        if (tipoConteudo.includes("application/json")) {
            dados = await resposta.json().catch(() => null);
        } else {
            dados = await resposta.text().catch(() => null);
        }

        if (!resposta.ok) {
            const mensagem =
                (dados && typeof dados === "object" && (dados.message || dados.title)) ||
                (typeof dados === "string" && dados) ||
                `Erro ${resposta.status} ao processar a requisição.`;
            throw new Error(mensagem);
        }

        return dados;
    },

    // --- ATALHOS ---
    get(caminho, opcoes) { 
        return this._request("GET", caminho, null,  opcoes); 
    },

    post(caminho, corpo, opcoes) { 
        return this._request("POST", caminho, corpo, opcoes); 
    },

    put(caminho, corpo, opcoes) { 
        return this._request("PUT", caminho, corpo, opcoes); 
    },
    
    delete(caminho, opcoes) { 
        return this._request("DELETE", caminho, null,  opcoes); 
    }
};
