(function () {
    Auth.exigirLogin();

    const corpoTabela       = document.getElementById("corpo-tabela");
    const filtroPagamento   = document.getElementById("filtro-pagamento");
    const btnAtualizar      = document.getElementById("btn-atualizar");
    const totalVendasLbl    = document.getElementById("total-vendas");
    const totalFatLbl       = document.getElementById("total-faturamento");
    const totalTicketLbl    = document.getElementById("total-ticket-medio");
    const msgFeedback       = document.getElementById("msg-feedback");

    let cacheVendas = [];

    const formatadorBRL = new Intl.NumberFormat("pt-BR", {
        style: "currency",
        currency: "BRL"
    });

    function formatarMoeda(valor) {
        return formatadorBRL.format(Number(valor) || 0);
    }

    function formatarDataHora(iso) {
        if (!iso) return "--";
        return new Date(iso).toLocaleString("pt-BR", { dateStyle: "short", timeStyle: "short" });
    }

    function tagPagamento(metodo) {
        const classe = `tag tag-${(metodo || "").toLowerCase()}`;
        return `<span class="${classe}">${metodo || "—"}</span>`;
    }

    function exibirFeedback(tipo, mensagem) {
        msgFeedback.className = `alerta alerta-${tipo}`;
        msgFeedback.textContent = mensagem;
        msgFeedback.classList.remove("oculto");
    }

    function limparFeedback() {
        msgFeedback.classList.add("oculto");
    }

    // --- HISTÓRICO LOCAL ---

    function obterHistoricoLocal() {
        try {
            return JSON.parse(localStorage.getItem("pdv:historico") || "[]");
        } catch {
            return [];
        }
    }

    function mesclarComLocal(vendasBackend) {
        const locais = obterHistoricoLocal();
        const mapaLabels = {};
        locais.forEach((v) => {
            if (v.id) mapaLabels[v.id] = v.assentoLabel;
        });

        return vendasBackend.map((v) => ({
            ...v,
            assentoLabel: mapaLabels[v.id] || "--"
        }));
    }

    // --- CARREGAR VENDAS ---

    async function carregarVendas() {
        corpoTabela.innerHTML = "<tr><td colspan='5' class='celula-vazia'>Carregando...</td></tr>";
        limparFeedback();

        try {
            const vendas = await Api.get("/vendas-fisicas");
            cacheVendas = mesclarComLocal(vendas || []);
            renderizar();
        } catch (erro) {
            // --- Fallback: usa apenas o histórico local ---
            const locais = obterHistoricoLocal();
            cacheVendas = locais;
            renderizar();
            exibirFeedback("erro", `Backend indisponível, mostrando histórico local: ${erro.message}`);
        }
    }

    // --- RENDERIZAÇÃO ---

    function renderizar() {
        const filtro = filtroPagamento.value;
        const vendasFiltradas = filtro
            ? cacheVendas.filter((v) => v.metodoPagamento === filtro)
            : cacheVendas;

        renderizarTotais(vendasFiltradas);
        renderizarTabela(vendasFiltradas);
    }

    function renderizarTotais(vendas) {
        const qtd          = vendas.length;
        const faturamento  = vendas.reduce((acc, v) => acc + (Number(v.valor) || 0), 0);
        const ticket       = qtd > 0 ? faturamento / qtd : 0;

        totalVendasLbl.textContent = qtd.toString();
        totalFatLbl.textContent    = formatarMoeda(faturamento);
        totalTicketLbl.textContent = formatarMoeda(ticket);
    }

    function renderizarTabela(vendas) {
        if (vendas.length === 0) {
            corpoTabela.innerHTML = "<tr><td colspan='5' class='celula-vazia'>Nenhuma venda registrada.</td></tr>";
            return;
        }

        const html = vendas.map((v) => `
            <tr>
                <td>
                    ${formatarDataHora(v.dataVenda)}
                </td>

                <td>    
                    <span class="codigo-curto">
                        ${(v.id || "").slice(0, 8).toUpperCase()}
                    </span>
                </td>

                <td>
                    ${v.assentoLabel || "--"}
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

    // --- HANDLERS ---

    filtroPagamento.addEventListener("change", renderizar);
    btnAtualizar.addEventListener("click", carregarVendas);

    carregarVendas();
})();
