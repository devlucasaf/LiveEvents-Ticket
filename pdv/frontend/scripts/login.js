(function () {
    if (Auth.estaAutenticado()) {
        window.location.href = Rotas.home;
        return;
    }

    const form        = document.getElementById("form-login");
    const inputLogin  = document.getElementById("login");
    const inputSenha  = document.getElementById("senha");
    const msgErro     = document.getElementById("msg-erro");
    const btnEntrar   = document.getElementById("btn-entrar");
    const btnTexto    = btnEntrar.querySelector(".btn-texto");
    const btnSpinner  = btnEntrar.querySelector(".btn-spinner");

    // --- EXIBE A MENSAGEM DE ERRO NA TELA ---
    function exibirErro(mensagem) {
        msgErro.textContent = mensagem;
        msgErro.classList.remove("oculto");
    }

    // --- LIMPA E OCULTA A MENSAGEM DE ERRO ---
    function limparErro() {
        msgErro.classList.add("oculto");
        msgErro.textContent = "";
    }

    // --- ALTERNA O ESTADO DE CARREGANDO DO BOTAO ---
    function alternarCarregando(carregando) {
        btnEntrar.disabled = carregando;
        btnTexto.classList.toggle("oculto", carregando);
        btnSpinner.classList.toggle("oculto", !carregando);
    }

    // --- AUTENTICA O OPERADOR ---
    form.addEventListener("submit", async (evento) => {
        // EVITA O RECARREGAMENTO PADRAO DA PAGINA
        evento.preventDefault();
        limparErro();

        const login = inputLogin.value.trim();
        const senha = inputSenha.value;

        if (!login || !senha) {
            exibirErro("Informe login e senha.");
            return;
        }

        // ATIVA O ESTADO DE CARREGANDO ENQUANTO CONSULTA O BACKEND
        alternarCarregando(true);
        try {
            const resposta = await Api.post("/auth/login", { login, senha }, { autenticar: false });

            const token = resposta?.token || resposta?.accessToken;
            if (!token) {
                throw new Error("Resposta do servidor não contém token.");
            }

            const usuario = {
                login: login,
                nome: resposta?.nome  || resposta?.usuario?.nome  || login,
                role: resposta?.role  || resposta?.usuario?.role  || "OPERADOR"
            };

            Auth.salvarSessao(token, usuario);
            window.location.href = Rotas.home;
        } catch (erro) {
            exibirErro(erro.message || "Não foi possível efetuar login.");
        } finally {
            alternarCarregando(false);
        }
    });
})();
