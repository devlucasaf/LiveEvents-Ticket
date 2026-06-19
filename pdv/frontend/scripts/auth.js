const CHAVE_TOKEN   = "pdv:token";
const CHAVE_USUARIO = "pdv:usuario";

const _estaEmSubpastaPages = window.location.pathname.toLowerCase().includes("/pages/");

const Rotas = {
    login: _estaEmSubpastaPages ? "../index.html" : "index.html",
    venda: _estaEmSubpastaPages ? "venda.html" : "pages/venda.html",
    relatorio: _estaEmSubpastaPages ? "relatorio.html" : "pages/relatorio.html"
};

const Auth = {
    salvarSessao(token, usuario) {
        localStorage.setItem(CHAVE_TOKEN, token);
        localStorage.setItem(CHAVE_USUARIO, JSON.stringify(usuario));
    },

    obterToken() {
        return localStorage.getItem(CHAVE_TOKEN);
    },

    obterUsuario() {
        const bruto = localStorage.getItem(CHAVE_USUARIO);
        if (!bruto) {
            return null;
        }

        try { 
            return JSON.parse(bruto); 
        } catch { 
            return null; 
        }
    },

    estaAutenticado() {
        return !!this.obterToken();
    },

    encerrarSessao() {
        localStorage.removeItem(CHAVE_TOKEN);
        localStorage.removeItem(CHAVE_USUARIO);
    },

    exigirLogin() {
        if (!this.estaAutenticado()) {
            window.location.href = Rotas.login;
        }
    }
};

/* --- CONFIGURA TOPBAR --- */
document.addEventListener("DOMContentLoaded", () => {
    const lblUsuario = document.getElementById("lbl-usuario");
    const btnSair    = document.getElementById("btn-sair");

    if (lblUsuario) {
        const usuario = Auth.obterUsuario();
        lblUsuario.textContent = usuario?.nome || usuario?.login || "Atendente";
    }

    if (btnSair) {
        btnSair.addEventListener("click", () => {
            Auth.encerrarSessao();
            window.location.href = Rotas.login;
        });
    }
});
