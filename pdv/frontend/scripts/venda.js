(function () {
    Auth.exigirLogin();

    const selEvento      = document.getElementById("evento");
    const selIngresso    = document.getElementById("ingresso");
    const inputQtd       = document.getElementById("quantidade");
    const resumoIngresso = document.getElementById("resumo-ingresso");
    const form           = document.getElementById("form-venda");
    const btnRegistrar   = document.getElementById("btn-registrar");
    const btnLimpar      = document.getElementById("btn-limpar");
    const msgFeedback    = document.getElementById("msg-feedback");
    const modal          = document.getElementById("modal-comprovante");
    const corpoComprov   = document.getElementById("comprovante-corpo");
    const btnImprimir    = document.getElementById("btn-imprimir");
    const btnNovaVenda   = document.getElementById("btn-nova-venda");

    let cacheIngressos = [];

    // --- UTILITARIOS DE FORMATACAO ---
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

    // --- OBTEM O TIPO DE ENTRADA SELECIONADO ---
    function tipoEntradaSelecionado() {
        const radio = form.querySelector("input[name='tipoEntrada']:checked");
        return radio ? radio.value : "INTEIRA";
    }

    // --- EXIBE ALERTA DE ERRO/SUCESSO ---
    function exibirFeedback(tipo, mensagem) {
        msgFeedback.className = `alerta alerta-${tipo}`;
        msgFeedback.textContent = mensagem;
        msgFeedback.classList.remove("oculto");
    }

    // --- OCULTA O ALERTA ---
    function limparFeedback() {
        msgFeedback.classList.add("oculto");
    }

    // --- ALTERNA ESTADO DE CARREGANDO ---
    function alternarBotaoSalvando(salvando) {
        btnRegistrar.disabled = salvando;
        btnRegistrar.querySelector(".btn-texto").classList.toggle("oculto", salvando);
        btnRegistrar.querySelector(".btn-spinner").classList.toggle("oculto", !salvando);
    }

    // --- BUSCA OS EVENTOS DISPONíVEIS NO WEB ---
    async function carregarEventos() {
        try {
            const eventos = await Api.get("/balcao/eventos");

            selEvento.innerHTML = "<option value=''>Selecione um evento</option>";
            if (!eventos || eventos.length === 0) {
                selEvento.innerHTML = "<option value=''>Nenhum evento disponível</option>";
                return;
            }

            eventos.forEach((ev) => {
                const opt = document.createElement("option");
                opt.value       = ev.id;
                opt.textContent = `${ev.titulo} — ${ev.local} — ${formatarDataHora(ev.dataEvento)}`;
                selEvento.appendChild(opt);
            });
        } catch (erro) {
            exibirFeedback("erro", `Não foi possível carregar eventos: ${erro.message}`);
        }
    }

    // --- BUSCA OS SETORES DO EVENTO ESCOLHIDO ---
    async function carregarIngressos(eventoId) {
        if (!eventoId) {
            cacheIngressos = [];
            selIngresso.disabled = true;
            selIngresso.innerHTML = "<option value=''>Selecione um evento primeiro</option>";
            atualizarResumo();
            return;
        }

        selIngresso.disabled = true;
        selIngresso.innerHTML = "<option value=''>Carregando ingressos...</option>";
        try {
            const ingressos = await Api.get(`/balcao/eventos/${eventoId}/ingressos`);
            cacheIngressos = ingressos || [];

            if (cacheIngressos.length === 0) {
                selIngresso.innerHTML = "<option value=''>Nenhum ingresso disponível</option>";
                selIngresso.disabled = true;
                atualizarResumo();
                return;
            }

            selIngresso.innerHTML = "<option value=''>Selecione o tipo de ingresso</option>";
            cacheIngressos.forEach((ing) => {
                const opt = document.createElement("option");
                opt.value       = ing.id;
                const esgotado  = ing.quantidadeDisponivel <= 0;
                opt.textContent = `${ing.setor} — ${formatarMoeda(ing.preco)}` +
                    (esgotado ? " (esgotado)" : ` — ${ing.quantidadeDisponivel} disp.`);
                opt.disabled    = esgotado;
                selIngresso.appendChild(opt);
            });
            selIngresso.disabled = false;
        } catch (erro) {
            selIngresso.innerHTML = "<option value=''>Erro ao carregar</option>";
            exibirFeedback("erro", `Não foi possível carregar ingressos: ${erro.message}`);
        }
        atualizarResumo();
    }

    // --- CALCULA E EXIBE O TOTAL DA VENDA ---
    function atualizarResumo() {
        const ingresso = cacheIngressos.find((i) => String(i.id) === String(selIngresso.value));
        const qtd      = parseInt(inputQtd.value, 10) || 0;

        if (!ingresso || qtd <= 0) {
            resumoIngresso.classList.add("oculto");
            resumoIngresso.innerHTML = "";
            return;
        }

        const meia          = tipoEntradaSelecionado() === "MEIA";
        const precoUnitario = meia ? ingresso.preco / 2 : ingresso.preco;
        const total         = precoUnitario * qtd;

        resumoIngresso.classList.remove("oculto");
        resumoIngresso.innerHTML = `
            <strong>${ingresso.setor}</strong> • ${meia ? "Meia" : "Inteira"} • ${qtd}x
            ${formatarMoeda(precoUnitario)} =
            <strong>${formatarMoeda(total)}</strong>
        `;
    }

    // --- RESETA TODO O FORMULARIO ---
    function limparFormulario() {
        form.reset();
        cacheIngressos = [];
        selIngresso.disabled = true;
        selIngresso.innerHTML = "<option value=''>Selecione um evento primeiro</option>";
        resumoIngresso.classList.add("oculto");
        resumoIngresso.innerHTML = "";
        limparFeedback();
    }

    // --- MONTA O CORPO DO COMPROVANTE ---
    function abrirComprovante(venda) {
        let html = `
            <div class="linha">
                <span class="label">Ticket</span>
                <span class="valor codigo-curto">${venda.codigoTicket}</span>
            </div>
            <div class="linha">
                <span class="label">Cliente</span>
                <span class="valor">${venda.clienteNome || "--"}</span>
            </div>
            <div class="linha">
                <span class="label">E-mail</span>
                <span class="valor">${venda.clienteEmail || "--"}</span>
            </div>
            <div class="linha">
                <span class="label">Evento</span>
                <span class="valor">${venda.eventoTitulo || "--"}</span>
            </div>
            <div class="linha">
                <span class="label">Local</span>
                <span class="valor">${venda.eventoLocal || "--"}</span>
            </div>
            <div class="linha">
                <span class="label">Data do evento</span>
                <span class="valor">${formatarDataHora(venda.eventoData)}</span>
            </div>
            <div class="linha">
                <span class="label">Ingresso</span>
                <span class="valor">${venda.setor} • ${venda.tipoEntrada === "MEIA" ? "Meia" : "Inteira"} • ${venda.quantidade}x</span>
            </div>
            <div class="linha">
                <span class="label">Emitido em</span>
                <span class="valor">${formatarDataHora(venda.dataVenda)}</span>
            </div>
            <div class="linha">
                <span class="label">Total</span>
                <span class="valor valor-grande">${formatarMoeda(venda.valorTotal)}</span>
            </div>
        `;

        if (venda.contaCriada) {
            html += `
                <div class="aviso-conta">
                    <strong>Conta criada para o cliente!</strong>
                    <p>Acesse o site com:</p>
                    <div class="linha">
                        <span class="label">Login</span>
                        <span class="valor">${venda.clienteEmail}</span>
                    </div>
                    <div class="linha">
                        <span class="label">Senha inicial</span>
                        <span class="valor codigo-curto">${venda.senhaInicial}</span>
                    </div>
                    <small>Oriente o cliente a alterar a senha após o primeiro acesso.</small>
                </div>
            `;
        }

        if (venda.qrCodeBase64) {
            html += `
                <div class="qr-container">
                    <img class="qr-img" src="data:image/png;base64,${venda.qrCodeBase64}" alt="QR Code do ingresso" />
                    <div class="ticket-codigo">${venda.codigoTicket}</div>
                    <small class="qr-dica">O ingresso já está disponível na conta do cliente, na seção "Ingressos".</small>
                </div>
            `;
        }

        corpoComprov.innerHTML = html;
        modal.classList.remove("oculto");
    }

    // --- FECHA O MODAL DE COMPROVANTE ---
    function fecharComprovante() {
        modal.classList.add("oculto");
    }

    // --- HANDLERS DE INTERACAO ---
    selEvento.addEventListener("change", (e) => {
        carregarIngressos(e.target.value);
    });

    selIngresso.addEventListener("change", atualizarResumo);
    inputQtd.addEventListener("input", atualizarResumo);
    form.querySelectorAll("input[name='tipoEntrada']").forEach((r) => {
        r.addEventListener("change", atualizarResumo);
    });

    btnLimpar.addEventListener("click", limparFormulario);

    btnNovaVenda.addEventListener("click", () => {
        fecharComprovante();
        limparFormulario();
    });

    btnImprimir.addEventListener("click", () => window.print());

    // --- MONTA O PAYLOAD E ENVIA A VENDA ---
    form.addEventListener("submit", async (evento) => {
        evento.preventDefault();
        limparFeedback();

        const eventoId   = selEvento.value;
        const ingressoId = selIngresso.value;
        const quantidade = parseInt(inputQtd.value, 10) || 0;

        if (!eventoId) {
            return exibirFeedback("erro", "Selecione um evento.");
        }

        if (!ingressoId) {
            return exibirFeedback("erro", "Selecione o tipo de ingresso.");
        }

        if (quantidade <= 0) {
            return exibirFeedback("erro", "Informe uma quantidade válida.");
        }

        const cliente = {
            nome:           form.elements["nome"].value.trim(),
            sobrenome:      form.elements["sobrenome"].value.trim(),
            email:          form.elements["email"].value.trim(),
            cpf:            form.elements["cpf"].value.trim(),
            telefone:       form.elements["telefone"].value.trim(),
            dataNascimento: form.elements["dataNascimento"].value || null,
            cep:            form.elements["cep"].value.trim() || null,
            logradouro:     form.elements["logradouro"].value.trim() || null,
            numero:         form.elements["numero"].value.trim() || null,
            complemento:    form.elements["complemento"].value.trim() || null,
            bairro:         form.elements["bairro"].value.trim() || null,
            cidade:         form.elements["cidade"].value.trim() || null,
            estado:         form.elements["estado"].value.trim() || null
        };

        if (!cliente.nome || !cliente.sobrenome || !cliente.email || !cliente.cpf || !cliente.telefone) {
            return exibirFeedback("erro", "Preencha nome, sobrenome, e-mail, CPF e telefone do cliente.");
        }

        const payload = {
            eventoId:    parseInt(eventoId, 10),
            ingressoId:  parseInt(ingressoId, 10),
            tipoEntrada: tipoEntradaSelecionado(),
            quantidade:  quantidade,
            cliente:     cliente
        };

        alternarBotaoSalvando(true);
        try {
            const venda = await Api.post("/balcao/vender", payload);
            abrirComprovante(venda);
        } catch (erro) {
            exibirFeedback("erro", erro.message || "Falha ao finalizar a venda.");
        } finally {
            alternarBotaoSalvando(false);
        }
    });

    carregarEventos();
})();
