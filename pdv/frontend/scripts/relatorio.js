(function () {
    Auth.exigirLogin();

    // --- ELEMENTOS DO DOM ---
    const corpoTabela            = document.getElementById("corpo-tabela");
    const corpoTabelaEvento      = document.getElementById("corpo-tabela-evento");
    const corpoTabelaAtendente   = document.getElementById("corpo-tabela-atendente");
    const filtroPagamento        = document.getElementById("filtro-pagamento");
    const btnAtualizar           = document.getElementById("btn-atualizar");
    const totalVendasLbl         = document.getElementById("total-vendas");
    const totalFatLbl            = document.getElementById("total-faturamento");
    const totalTicketLbl         = document.getElementById("total-ticket-medio");
    const msgFeedback            = document.getElementById("msg-feedback");
    const abas                   = document.querySelectorAll(".aba");
    const wrapperTodas           = document.getElementById("tabela-todas");
    const wrapperEvento          = document.getElementById("tabela-evento");
    const wrapperAtendente       = document.getElementById("tabela-atendente");

    let abaAtiva       = "todas";
    let cacheVendas    = [];
    let cacheEventos   = [];
    let cacheAtendentes = [];

    // --- FORMATADORES ---
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

        return new Date(iso).toLocaleString("pt-BR", { dateStyle: "short", timeStyle: "short" });
    }

    function formatarData(iso) {
        if (!iso) {
            return "--";
        }

        return new Date(iso).toLocaleDateString("pt-BR");
    }

    // --- ROTULOS AMIGAVEIS DAS FORMAS DE PAGAMENTO ---
    const ROTULOS_PAGAMENTO = {
        CREDITO: "Crédito",
        DEBITO: "Débito",
        PIX: "Pix"
    };

    function tagPagamento(metodo) {
        const chave = (metodo || "").toUpperCase();
        const rotulo = ROTULOS_PAGAMENTO[chave] || metodo || "—";
        const classe = `tag tag-${(metodo || "").toLowerCase()}`;
        return `<span class="${classe}">${rotulo}</span>`;
    }

    function exibirFeedback(tipo, mensagem) {
        msgFeedback.className = `alerta alerta-${tipo}`;
        msgFeedback.textContent = mensagem;
        msgFeedback.classList.remove("oculto");
    }

    function limparFeedback() {
        msgFeedback.classList.add("oculto");
    }

    // --- ABAS ---
    function trocarAba(nome) {
        abaAtiva = nome;

        abas.forEach((a) => a.classList.toggle("ativa", a.dataset.aba === nome));

        wrapperTodas.classList.toggle("oculto", nome !== "todas");
        wrapperEvento.classList.toggle("oculto", nome !== "evento");
        wrapperAtendente.classList.toggle("oculto", nome !== "atendente");

        filtroPagamento.disabled = nome !== "todas";

        carregar();
    }

    abas.forEach((a) => a.addEventListener("click", () => trocarAba(a.dataset.aba)));

    // --- CARREGAMENTO POR ABA ---
    async function carregar() {
        limparFeedback();
        if (abaAtiva === "todas") {
            return carregarVendas();
        }

        if (abaAtiva === "evento") {
            return carregarPorEvento();
        }

        if (abaAtiva === "atendente") {
            return carregarPorAtendente();
        }
    }

    async function carregarVendas() {
        corpoTabela.innerHTML = "<tr><td colspan='7' class='celula-vazia'>Carregando...</td></tr>";

        try {
            const vendas = await Api.get("/balcao/vendas");
            cacheVendas = vendas || [];
            renderizarTodas();
        } catch (erro) {
            cacheVendas = [];
            renderizarTodas();
            exibirFeedback("erro", `Falha ao carregar vendas: ${erro.message}`);
        }
    }

    async function carregarPorEvento() {
        corpoTabelaEvento.innerHTML = "<tr><td colspan='6' class='celula-vazia'>Carregando...</td></tr>";

        try {
            cacheEventos = await Api.get("/balcao/relatorios/por-evento") || [];
            renderizarEvento();
            renderizarTotaisAgregado(cacheEventos);
        } catch (erro) {
            cacheEventos = [];
            renderizarEvento();
            exibirFeedback("erro", `Falha ao carregar relatório por evento: ${erro.message}`);
        }
    }

    async function carregarPorAtendente() {
        corpoTabelaAtendente.innerHTML = "<tr><td colspan='5' class='celula-vazia'>Carregando...</td></tr>";

        try {
            cacheAtendentes = await Api.get("/balcao/relatorios/por-atendente") || [];
            renderizarAtendente();
            renderizarTotaisAgregado(cacheAtendentes);
        } catch (erro) {
            cacheAtendentes = [];
            renderizarAtendente();
            exibirFeedback("erro", `Falha ao carregar relatório por atendente: ${erro.message}`);
        }
    }

    // --- RENDERIZA TODAS AS VENDAS ---
    function renderizarTodas() {
        const filtro = filtroPagamento.value;
        const vendasFiltradas = filtro
            ? cacheVendas.filter((v) => (v.tipoEntrada || "").toUpperCase() === filtro)
            : cacheVendas;

        renderizarTotaisVendas(vendasFiltradas);

        if (vendasFiltradas.length === 0) {
            corpoTabela.innerHTML = "<tr><td colspan='7' class='celula-vazia'>Nenhuma venda registrada.</td></tr>";
            return;
        }

        const html = vendasFiltradas.map((v) => `
            <tr>
                <td>
                    ${formatarDataHora(v.dataVenda)}
                </td>

                <td>
                    <span class="codigo-curto">
                        ${v.codigoTicket || (v.id || "").slice(0, 8).toUpperCase()}
                    </span>
                </td>

                <td>
                    ${v.eventoNome || "--"}
                </td>

                <td>
                    ${formatarSetor(v)}
                </td>

                <td>
                    ${v.operadorNome || "--"}
                </td>

                <td>
                    ${tagPagamento(v.metodoPagamento)}
                </td>

                <td class="alinhar-direita">
                    ${formatarMoeda(v.valor)}
                </td>
            </tr>
        `).join("");

        corpoTabela.innerHTML = html;
    }

    function formatarSetor(v) {
        if (!v.setor) {
            return "--";
        }

        // --- MONTA "SETOR • TIPO" ---
        const setor = (v.setor || "").replace(/_/g, " ");
        const tipo = v.tipoEntrada ? capitalizar(v.tipoEntrada) : "";
        const qtd = Number(v.quantidade) > 1 ? ` (x${v.quantidade})` : "";
        return tipo ? `${setor} • ${tipo}${qtd}` : `${setor}${qtd}`;
    }

    // --- CAPITALIZA A PRIMEIRA LETRA ---
    function capitalizar(texto) {
        const t = (texto || "").toLowerCase();
        return t.charAt(0).toUpperCase() + t.slice(1);
    }

    function renderizarTotaisVendas(vendas) {
        const qtd = vendas.length;
        const faturamento = vendas.reduce((acc, v) => acc + (Number(v.valor) || 0), 0);
        const ticket = qtd > 0 ? faturamento / qtd : 0;

        totalVendasLbl.textContent = qtd.toString();
        totalFatLbl.textContent    = formatarMoeda(faturamento);
        totalTicketLbl.textContent = formatarMoeda(ticket);
    }

    function renderizarTotaisAgregado(linhas) {
        const qtd = linhas.reduce((acc, l) => acc + (Number(l.quantidadeVendas) || 0), 0);
        const faturamento = linhas.reduce((acc, l) => acc + (Number(l.faturamentoTotal) || 0), 0);
        const ticket = qtd > 0 ? faturamento / qtd : 0;

        totalVendasLbl.textContent = qtd.toString();
        totalFatLbl.textContent    = formatarMoeda(faturamento);
        totalTicketLbl.textContent = formatarMoeda(ticket);
    }

    // --- RENDERIZAÇÃO ---
    function renderizarEvento() {
        if (cacheEventos.length === 0) {
            corpoTabelaEvento.innerHTML = "<tr><td colspan='6' class='celula-vazia'>Nenhum evento com vendas.</td></tr>";
            return;
        }

        const html = cacheEventos.map((r) => `
            <tr>
                <td>    
                    ${r.eventoNome || "--"}
                </td>

                <td>
                    ${r.eventoLocal || "--"}
                </td>

                <td>
                    ${formatarData(r.eventoData)}
                </td>

                <td class="alinhar-direita">
                    ${r.quantidadeVendas}
                </td>

                <td class="alinhar-direita">
                    ${formatarMoeda(r.faturamentoTotal)}
                </td>
                
                <td class="alinhar-direita">
                    ${formatarMoeda(r.ticketMedio)}
                </td>
            </tr>
        `).join("");

        corpoTabelaEvento.innerHTML = html;
    }

    // --- RENDERIZAÇÃO POR ATENDENTE ---
    function renderizarAtendente() {
        if (cacheAtendentes.length === 0) {
            corpoTabelaAtendente.innerHTML = "<tr><td colspan='5' class='celula-vazia'>Nenhum atendente com vendas.</td></tr>";
            return;
        }

        const html = cacheAtendentes.map((r) => `
            <tr>
                <td>
                    ${r.operadorNome || "(desconhecido)"}
                </td>

                <td>    
                    <span class="codigo-curto">
                        ${r.operadorLogin || "--"}
                    </span>
                </td>

                <td class="alinhar-direita">
                    ${r.quantidadeVendas}
                </td>

                <td class="alinhar-direita">
                    ${formatarMoeda(r.faturamentoTotal)}
                </td>

                <td class="alinhar-direita">
                    ${formatarMoeda(r.ticketMedio)}
                </td>
            </tr>
        `).join("");

        corpoTabelaAtendente.innerHTML = html;
    }

    // --- HANDLERS ---
    filtroPagamento.addEventListener("change", () => {
        if (abaAtiva === "todas") {
            renderizarTodas();
        }
    });

    btnAtualizar.addEventListener("click", carregar);

    carregar();
})();
