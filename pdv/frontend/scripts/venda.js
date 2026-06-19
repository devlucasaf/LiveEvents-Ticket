(function () {
    Auth.exigirLogin();

    const selEvento      = document.getElementById("evento");
    const listaAssentos  = document.getElementById("lista-assentos");
    const inputAssentoId = document.getElementById("assento-id");
    const resumoAssento  = document.getElementById("resumo-assento");
    const form           = document.getElementById("form-venda");
    const btnRegistrar   = document.getElementById("btn-registrar");
    const btnLimpar      = document.getElementById("btn-limpar");
    const msgFeedback    = document.getElementById("msg-feedback");
    const modal          = document.getElementById("modal-comprovante");
    const corpoComprov   = document.getElementById("comprovante-corpo");
    const btnImprimir    = document.getElementById("btn-imprimir");
    const btnNovaVenda   = document.getElementById("btn-nova-venda");

    let assentoSelecionado = null;
    let cacheAssentos      = [];

    // --- UTILITÁRIOS ---

    const formatadorBRL = new Intl.NumberFormat("pt-BR", {
        style: "currency",
        currency: "BRL"
    });

    function formatarMoeda(valor) {
        return formatadorBRL.format(Number(valor) || 0);
    }

    function formatarDataHora(iso) {
        if (!iso) {
            return "--";
        }

        const data = new Date(iso);
        return data.toLocaleString("pt-BR", { dateStyle: "short", timeStyle: "short" });
    }

    function exibirFeedback(tipo, mensagem) {
        msgFeedback.className = `alerta alerta-${tipo}`;
        msgFeedback.textContent = mensagem;
        msgFeedback.classList.remove("oculto");
    }

    function limparFeedback() {
        msgFeedback.classList.add("oculto");
    }

    function alternarBotaoSalvando(salvando) {
        btnRegistrar.disabled = salvando;
        btnRegistrar.querySelector(".btn-texto").classList.toggle("oculto", salvando);
        btnRegistrar.querySelector(".btn-spinner").classList.toggle("oculto", !salvando);
    }

    // --- CARREGAR EVENTOS ---

    async function carregarEventos() {
        try {
            const eventos = await Api.get("/eventos");

            selEvento.innerHTML = "<option value=''>Selecione um evento</option>";
            if (!eventos || eventos.length === 0) {
                selEvento.innerHTML = "<option value=''>Nenhum evento ativo</option>";
                return;
            }

            eventos.forEach((ev) => {
                const opt = document.createElement("option");
                opt.value       = ev.id;
                opt.textContent = `${ev.nome} — ${formatarDataHora(ev.dataEvento)}`;
                opt.dataset.local = ev.local || "";
                selEvento.appendChild(opt);
            });
        } catch (erro) {
            exibirFeedback("erro", `Não foi possível carregar eventos: ${erro.message}`);
        }
    }

    // --- CARREGAR ASSENTOS POR EVENTO ---

    async function carregarAssentos(eventoId) {
        if (!eventoId) {
            listaAssentos.innerHTML = "<p class='placeholder-grade'>Selecione um evento para ver os assentos.</p>";
            return;
        }

        listaAssentos.innerHTML = "<p class='placeholder-grade'>Carregando assentos...</p>";
        try {
            const assentos = await Api.get(`/eventos/${eventoId}/assentos`);
            cacheAssentos = assentos || [];
            renderizarAssentos(cacheAssentos);
        } catch (erro) {
            listaAssentos.innerHTML = `<p class='placeholder-grade'>Erro ao carregar: ${erro.message}</p>`;
        }
    }

    // --- RENDERIZAR GRADE DE ASSENTOS ---

    function renderizarAssentos(assentos) {
        listaAssentos.innerHTML = "";

        if (assentos.length === 0) {
            listaAssentos.innerHTML = "<p class='placeholder-grade'>Nenhum assento cadastrado.</p>";
            return;
        }

        const grupos = {};
        assentos.forEach((a) => {
            if (!grupos[a.setor]) {
                grupos[a.setor] = {};
            }

            if (!grupos[a.setor][a.fileira]) {
                grupos[a.setor][a.fileira] = [];
            }
            grupos[a.setor][a.fileira].push(a);
        });

        Object.keys(grupos).sort().forEach((setor) => {
            const blocoSetor = document.createElement("div");
            blocoSetor.className = "setor-bloco";

            const tituloSetor = document.createElement("h3");
            tituloSetor.className = "setor-titulo";
            tituloSetor.textContent = setor.replace(/_/g, " ");
            blocoSetor.appendChild(tituloSetor);

            Object.keys(grupos[setor]).sort().forEach((fileira) => {
                const linha = document.createElement("div");
                linha.className = "linha-assentos";

                grupos[setor][fileira]
                    .sort((a, b) => a.numero - b.numero)
                    .forEach((assento) => {
                        const btn = document.createElement("button");
                        btn.type = "button";
                        btn.className = `assento ${assento.status.toLowerCase()}`;
                        btn.textContent = `${assento.fileira}${assento.numero}`;
                        btn.title = `${setor} • Fila ${assento.fileira} • Nº ${assento.numero} • ${formatarMoeda(assento.preco)}`;
                        btn.dataset.id = assento.id;

                        if (assento.status === "DISPONIVEL") {
                            btn.addEventListener("click", () => selecionarAssento(assento, btn));
                        } else {
                            btn.disabled = true;
                        }

                        linha.appendChild(btn);
                    });

                blocoSetor.appendChild(linha);
            });

            listaAssentos.appendChild(blocoSetor);
        });
    }

    function selecionarAssento(assento, botao) {
        document.querySelectorAll(".assento.selecionado").forEach((b) => b.classList.remove("selecionado"));

        botao.classList.add("selecionado");
        assentoSelecionado = assento;
        inputAssentoId.value = assento.id;

        resumoAssento.classList.remove("oculto");
        resumoAssento.innerHTML = `
            Assento selecionado:
            <strong>${assento.setor.replace(/_/g, " ")} — Fileira ${assento.fileira} / Nº ${assento.numero}</strong>
            • Valor: <strong>${formatarMoeda(assento.preco)}</strong>
        `;
    }

    function limparSelecao() {
        document.querySelectorAll(".assento.selecionado").forEach((b) => b.classList.remove("selecionado"));
        assentoSelecionado = null;
        inputAssentoId.value = "";
        resumoAssento.classList.add("oculto");
        resumoAssento.innerHTML = "";
    }

    function limparFormulario() {
        form.reset();
        limparSelecao();
        limparFeedback();
        listaAssentos.innerHTML = "<p class='placeholder-grade'>Selecione um evento para ver os assentos.</p>";
    }

    // --- HISTÓRICO LOCAL ---

    function salvarVendaNoHistorico(venda) {
        const historico = JSON.parse(localStorage.getItem("pdv:historico") || "[]");
        historico.unshift({
            id: venda.id,
            eventoId: venda.eventoId,
            assentoId: venda.assentoId,
            metodoPagamento: venda.metodoPagamento,
            valor: venda.valor,
            dataVenda: venda.dataVenda,
            assentoLabel: assentoSelecionado
                ? `${assentoSelecionado.setor.replace(/_/g, " ")} • ${assentoSelecionado.fileira}${assentoSelecionado.numero}`
                : "--"
        });
        localStorage.setItem("pdv:historico", JSON.stringify(historico.slice(0, 200)));
    }

    // --- MODAL DE COMPROVANTE ---

    function abrirComprovante(venda) {
        const assentoLabel = assentoSelecionado
            ? `${assentoSelecionado.setor.replace(/_/g, " ")} — Fileira ${assentoSelecionado.fileira} / Nº ${assentoSelecionado.numero}`
            : "--";

        const eventoLabel = selEvento.options[selEvento.selectedIndex]?.text || "--";

        corpoComprov.innerHTML = `
            <div class="linha">
                <span class="label">Comprovante</span>
                <span class="valor codigo-curto">
                    ${venda.id.slice(0, 8).toUpperCase()}
                </span>
            </div>

            <div class="linha">
                <span class="label">Evento</span>
                <span class="valor">
                    ${eventoLabel}
                </span>
            </div>

            <div class="linha">
                <span class="label">Assento</span>
                <span class="valor">
                    ${assentoLabel}
                </span>
            </div>

            <div class="linha">
                <span class="label">Pagamento</span>
                <span class="valor">
                    ${venda.metodoPagamento}
                </span>
            </div>

            <div class="linha">
                <span class="label">Data</span>
                <span class="valor">
                    ${formatarDataHora(venda.dataVenda)}
                </span>
            </div>

            <div class="linha">
                <span class="label">Total</span>
                <span class="valor valor-grande">
                    ${formatarMoeda(venda.valor)}
                </span>
            </div>
        `;
        modal.classList.remove("oculto");
    }

    function fecharComprovante() {
        modal.classList.add("oculto");
    }

    // --- HANDLERS ---

    selEvento.addEventListener("change", (e) => {
        limparSelecao();
        carregarAssentos(e.target.value);
    });

    btnLimpar.addEventListener("click", limparFormulario);

    btnNovaVenda.addEventListener("click", () => {
        fecharComprovante();
        limparFormulario();
        if (selEvento.value) {
            carregarAssentos(selEvento.value);
        }
    });

    btnImprimir.addEventListener("click", () => window.print());

    form.addEventListener("submit", async (evento) => {
        evento.preventDefault();
        limparFeedback();

        const eventoId  = selEvento.value;
        const assentoId = inputAssentoId.value;
        const metodo    = form.elements["metodoPagamento"].value;

        if (!eventoId)  {
            return exibirFeedback("erro", "Selecione um evento.");
        }
        
        if (!assentoId) {
            return exibirFeedback("erro", "Selecione um assento disponível.");
        }

        if (!metodo) {
            return exibirFeedback("erro", "Escolha o método de pagamento.");
        }

        alternarBotaoSalvando(true);
        try {
            const venda = await Api.post("/vendas-fisicas", {
                eventoId:        eventoId,
                assentoId:       assentoId,
                metodoPagamento: metodo
            });

            salvarVendaNoHistorico(venda);
            abrirComprovante(venda);
        } catch (erro) {
            exibirFeedback("erro", erro.message || "Falha ao registrar venda.");
        } finally {
            alternarBotaoSalvando(false);
        }
    });

    carregarEventos();
})();
