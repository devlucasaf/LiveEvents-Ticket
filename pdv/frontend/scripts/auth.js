const CHAVE_TOKEN   = "pdv:token";
const CHAVE_USUARIO = "pdv:usuario";
const CHAVE_TEMA    = "theme";

const _estaEmSubpastaPages = window.location.pathname.toLowerCase().includes("/pages/");

const Rotas = {
    login: _estaEmSubpastaPages ? "../index.html" : "index.html",
    home: _estaEmSubpastaPages ? "home.html" : "pages/home.html",
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

// --- TEMA: APLICA O TEMA SALVO (PADRAO: CLARO) ---
function aplicarTemaInicial() {
    const temaSalvo = localStorage.getItem(CHAVE_TEMA) || "light";
    document.documentElement.setAttribute("data-theme", temaSalvo);
}

// --- TEMA: ALTERNA ENTRE LIGHT E DARK ---
function alternarTema() {
    const temaAtual = document.documentElement.getAttribute("data-theme") || "light";
    const proximoTema = temaAtual === "dark" ? "light" : "dark";

    document.documentElement.setAttribute("data-theme", proximoTema);
    localStorage.setItem(CHAVE_TEMA, proximoTema);

    const btnTema = document.getElementById("btn-tema");
    if (btnTema) {
        btnTema.setAttribute("aria-label", proximoTema === "dark" ? "Ativar modo claro" : "Ativar modo escuro");
        btnTema.setAttribute("title", proximoTema === "dark" ? "Ativar modo claro" : "Ativar modo escuro");
        btnTema.textContent = proximoTema === "dark" ? "☀" : "◐";
    }
}

// --- CONFIGURA TOPBAR --- 
document.addEventListener("DOMContentLoaded", () => {
    aplicarTemaInicial();

    const lblUsuario = document.getElementById("lbl-usuario");
    const btnSair    = document.getElementById("btn-sair");
    const btnTema    = document.getElementById("btn-tema");

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

    if (btnTema) {
        const temaAtual = document.documentElement.getAttribute("data-theme") || "light";
        btnTema.setAttribute("aria-label", temaAtual === "dark" ? "Ativar modo claro" : "Ativar modo escuro");
        btnTema.setAttribute("title", temaAtual === "dark" ? "Ativar modo claro" : "Ativar modo escuro");
        btnTema.textContent = temaAtual === "dark" ? "☀" : "◐";
        btnTema.addEventListener("click", alternarTema);
    }
});
