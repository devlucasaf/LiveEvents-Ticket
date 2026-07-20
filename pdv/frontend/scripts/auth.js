const CHAVE_TOKEN   = "pdv:token";
const CHAVE_USUARIO = "pdv:usuario";
const CHAVE_TEMA    = "theme";

// --- DETECTA SE A PAGINA ATUAL ESTA DENTRO DA PASTA PAGES ---
const _estaEmSubpastaPages = window.location.pathname.toLowerCase().includes("/pages/");

// --- RESOLVE O CAMINHO CERTO CONFORME A LOCALIZACAO DA PAGINA ---
const Rotas = {
    login: _estaEmSubpastaPages ? "../index.html" : "index.html",
    home: _estaEmSubpastaPages ? "home.html" : "pages/home.html",
    venda: _estaEmSubpastaPages ? "venda.html" : "pages/venda.html",
    relatorio: _estaEmSubpastaPages ? "relatorio.html" : "pages/relatorio.html"
};

// --- OBJETO DE AUTENTICACAO: GERENCIA SESSAO DO ATENDENTE ---
const Auth = {
    salvarSessao(token, usuario) {
        localStorage.setItem(CHAVE_TOKEN, token);
        localStorage.setItem(CHAVE_USUARIO, JSON.stringify(usuario));
    },

    obterToken() {
        return localStorage.getItem(CHAVE_TOKEN);
    },

    // --- RECUPERA E DESSERIALIZA O USUARIO SALVO ---
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

    // --- INDICA SE EXISTE UM TOKEN VALIDO GRAVADO ---
    estaAutenticado() {
        return !!this.obterToken();
    },

    // --- REMOVE TOKEN E USUARIO ---
    encerrarSessao() {
        localStorage.removeItem(CHAVE_TOKEN);
        localStorage.removeItem(CHAVE_USUARIO);
    },

    // --- PROTEGE A PAGINA, REDIRECIONANDO AO LOGIN SE NAO AUTENTICADO ---
    exigirLogin() {
        if (!this.estaAutenticado()) {
            window.location.href = Rotas.login;
        }
    }
};

// --- APLICA O TEMA SALVO ---
function aplicarTemaInicial() {
    const temaSalvo = localStorage.getItem(CHAVE_TEMA) || "light";
    document.documentElement.setAttribute("data-theme", temaSalvo);
}

// --- TROCA ENTRE OS MODOS CLARO E ESCURO ---
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

// --- CONFIGURA A TOPBAR APOS O CARREGAMENTO DO DOM ---
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
