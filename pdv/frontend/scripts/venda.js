(function () {
    Auth.exigirLogin();

    const selEvento      = document.getElementById("evento");
    const listaAssentos  = document.getElementById("lista-assentos");
    const inputAssentoId = document.getElementById("assento-id");
    const inputValor     = document.getElementById("valor");
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

        // --- PREENCHE AUTOMATICAMENTE O CAMPO VALOR COM O PREÇO DO ASSENTO ---
        inputValor.value = Number(assento.preco).toFixed(2);

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
        inputValor.value = "";
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
            codigoTicket: venda.codigoTicket,
            eventoId: venda.eventoId,
            eventoNome: venda.eventoNome,
            assentoId: venda.assentoId,
            assentoLabel: `${(venda.assentoSetor || "").replace(/_/g, " ")} • ${venda.assentoFileira}${venda.assentoNumero}`,
            metodoPagamento: venda.metodoPagamento,
            valor: venda.valor,
            dataVenda: venda.dataVenda
        });
        localStorage.setItem("pdv:historico", JSON.stringify(historico.slice(0, 200)));
    }

    // --- MODAL DE COMPROVANTE (TICKET FÍSICO COM QR CODE) ---

    function abrirComprovante(venda) {
        const assentoLabel = `${(venda.assentoSetor || "").replace(/_/g, " ")} — Fileira ${venda.assentoFileira} / Nº ${venda.assentoNumero}`;

        corpoComprov.innerHTML = `
            <div class="linha">
                <span class="label">Ticket</span>
                <span class="valor codigo-curto">
                    ${venda.codigoTicket}
                </span>
            </div>

            <div class="linha">
                <span class="label">Evento</span>
                <span class="valor">
                    ${venda.eventoNome || "--"}
                </span>
            </div>

            <div class="linha">
                <span class="label">Local</span>
                <span class="valor">
                    ${venda.eventoLocal || "--"}
                </span>
            </div>

            <div class="linha">
                <span class="label">Data do evento</span>
                <span class="valor">
                    ${formatarDataHora(venda.eventoData)}
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
                <span class="label">Emitido em</span>
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

            <div class="qr-container">
                <canvas id="qr-canvas"></canvas>
                <div class="ticket-codigo" id="ticket-codigo">${venda.codigoTicket}</div>
                <small class="qr-dica">Apresente este ticket na entrada do evento.</small>
            </div>
        `;

        modal.classList.remove("oculto");

        // --- RENDERIZA O QR CODE ---
        renderizarQrCode(venda);
    }

    function renderizarQrCode(venda) {
        const canvas = document.getElementById("qr-canvas");
        const codigoFallback = document.getElementById("ticket-codigo");

        if (!canvas) {
            return;
        }

        // --- PAYLOAD DO QR (texto simples se a lib não carregar) ---
        const payload = JSON.stringify({
            t:  venda.codigoTicket,
            id: venda.id,
            ev: venda.eventoId,
            as: venda.assentoId
        });

        if (typeof QRCode === "undefined") {
            // --- FALLBACK: mostra só o código do ticket ---
            canvas.style.display = "none";
            if (codigoFallback) {
                codigoFallback.textContent = venda.codigoTicket;
            }
            return;
        }

        QRCode.toCanvas(canvas, payload, { width: 200, margin: 1 }, (err) => {
            if (err) {
                canvas.style.display = "none";
            }
        });
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
        const valor     = parseFloat(inputValor.value);

        if (!eventoId)  {
            return exibirFeedback("erro", "Selecione um evento.");
        }
        
        if (!assentoId) {
            return exibirFeedback("erro", "Selecione um assento disponível.");
        }

        if (!metodo) {
            return exibirFeedback("erro", "Escolha o método de pagamento.");
        }

        if (!valor || valor <= 0) {
            return exibirFeedback("erro", "Valor inválido para a venda.");
        }

        alternarBotaoSalvando(true);
        try {
            const venda = await Api.post("/pontovenda/registrar", {
                eventoId:        eventoId,
                assentoId:       assentoId,
                metodoPagamento: metodo,
                valor:           valor
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
