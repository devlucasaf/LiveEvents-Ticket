(function () {
    Auth.exigirLogin();

    const selEvento             = document.getElementById("evento");
    const selIngresso           = document.getElementById("ingresso");
    const inputQtd              = document.getElementById("quantidade");
    const inputCpf              = document.getElementById("cpf");
    const inputTelefone         = document.getElementById("telefone");
    const inputCep              = document.getElementById("cep");
    const inputLogradouro       = document.getElementById("logradouro");
    const inputBairro           = document.getElementById("bairro");
    const inputCidade           = document.getElementById("cidade");
    const inputEstado           = document.getElementById("estado");
    const resumoIngresso        = document.getElementById("resumo-ingresso");
    const listaAcompanhantes    = document.getElementById("lista-acompanhantes");
    const form                  = document.getElementById("form-venda");
    const btnRegistrar          = document.getElementById("btn-registrar");
    const btnLimpar             = document.getElementById("btn-limpar");
    const msgFeedback           = document.getElementById("msg-feedback");
    const modal                 = document.getElementById("modal-comprovante");
    const corpoComprov          = document.getElementById("comprovante-corpo");
    const btnImprimir           = document.getElementById("btn-imprimir");
    const btnNovaVenda          = document.getElementById("btn-nova-venda");
    const modalPagamento        = document.getElementById("modal-pagamento");
    const etapaEscolha          = document.getElementById("pagamento-etapa-escolha");
    const etapaConfirmacao      = document.getElementById("pagamento-etapa-confirmacao");
    const opcoesPagamento       = modalPagamento.querySelectorAll(".pagamento-opcao");
    const lblPagamentoValor     = document.getElementById("pagamento-valor");
    const confIcone             = document.getElementById("conf-icone");
    const confForma             = document.getElementById("conf-forma");
    const confValor             = document.getElementById("conf-valor");
    const btnPagCancelar        = document.getElementById("btn-pagamento-cancelar");
    const btnPagVoltar          = document.getElementById("btn-pagamento-voltar");
    const btnPagContinuar       = document.getElementById("btn-pagamento-continuar");
    const btnPagConfirmar       = document.getElementById("btn-pagamento-confirmar");
    const grupoSubtipo          = document.getElementById("grupo-subtipo");
    const selSubtipo            = document.getElementById("subtipo-meia");
    const avisoSocial           = document.getElementById("aviso-social");
    const docsComprador         = document.getElementById("documentos-comprador");

    let cacheIngressos = [];

    let payloadPendente = null;
    let totalPendente   = 0;
    let formaSelecionada = null;

    const SOCIAL_ACRESCIMO = 20;
    const IDADE_MAXIMA_MENOR18 = 18;

    // --- CAMPOS EXTRAS DE DOCUMENTO POR SUBTIPO ---
    const CATALOGO_MEIA = {
        ESTUDANTIL: [
            { 
                chave: "instituicaoEnsino",     
                rotulo: "Instituição de ensino",        
                tipo: "text", 
                obrigatorio: true 
            },
            { 
                chave: "matricula",             
                rotulo: "Matrícula",                     
                tipo: "text", 
                obrigatorio: true 
            },
            { 
                chave: "dataPrevistaConclusao", 
                rotulo: "Data prevista de conclusão",    
                tipo: "date", 
                obrigatorio: true 
            },
            { 
                chave: "curso",                 
                rotulo: "Curso (se faculdade)",          
                tipo: "text", 
                obrigatorio: false 
            }
        ],
        PCD: [
            { 
                chave: "cartaoBpc", 
                rotulo: "Cartão BPC",       
                tipo: "text", 
                obrigatorio: true 
            },
            { 
                chave: "cidOuInss", 
                rotulo: "CID ou nº do INSS", 
                tipo: "text", 
                obrigatorio: true 
            }
        ],
        PROFESSOR: [
            {
                chave: "numeroCarteiraFuncional", 
                rotulo: "Número da carteira funcional", 
                tipo: "text", 
                obrigatorio: true 
            },
            { 
                chave: "matricula",               
                rotulo: "Matrícula",                    
                tipo: "text", 
                obrigatorio: true 
            },
            { 
                chave: "dataValidadeCarteira",    
                rotulo: "Data de validade da carteira", 
                tipo: "date", 
                obrigatorio: true 
            }
        ],
        JOVEM_BAIXA_RENDA: [
            { 
                chave: "carteiraIdentidadeJovem", 
                rotulo: "Carteira de identidade jovem", 
                tipo: "text", 
                obrigatorio: true 
            },
            { 
                chave: "dataValidadeCarteira",    
                rotulo: "Data de validade da carteira", 
                tipo: "date", 
                obrigatorio: true 
            }
        ],
        IDOSO: [
            { 
                chave: "numeroInss", 
                rotulo: "Número do INSS", 
                tipo: "text", 
                obrigatorio: true 
            }
        ],
        DOADOR_SANGUE: [
            { 
                chave: "idDoador", 
                rotulo: "ID do doador", 
                tipo: "text", 
                obrigatorio: true 
            }
        ],
        MENOR_18: []
    };

    // --- ROTULOS E ICONES DAS FORMAS DE PAGAMENTO ---
    const ICONE_CREDITO = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><rect x="2" y="5" width="20" height="14" rx="2"/><line x1="2" y1="10" x2="22" y2="10"/><line x1="6" y1="15" x2="10" y2="15"/></svg>';
    const ICONE_DEBITO = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><line x1="3" y1="21" x2="21" y2="21"/><path d="M12 3 3 8h18z"/><line x1="5" y1="10" x2="5" y2="18"/><line x1="10" y1="10" x2="10" y2="18"/><line x1="14" y1="10" x2="14" y2="18"/><line x1="19" y1="10" x2="19" y2="18"/></svg>';
    const ICONE_PIX = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M12 3.2 20.8 12 12 20.8 3.2 12z"/><path d="M12 8.2 15.8 12 12 15.8 8.2 12z"/></svg>';

    const FORMAS_PAGAMENTO = {
        CREDITO: { rotulo: "Crédito", icone: ICONE_CREDITO },
        DEBITO:  { rotulo: "Débito",  icone: ICONE_DEBITO },
        PIX:     { rotulo: "Pix",     icone: ICONE_PIX }
    };

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

    // --- MASCARA DE CPF ---
    function mascararCpf(valor) {
        const digitos = valor.replace(/\D/g, "").slice(0, 11);
        return digitos
            .replace(/^(\d{3})(\d)/, "$1.$2")
            .replace(/^(\d{3})\.(\d{3})(\d)/, "$1.$2.$3")
            .replace(/^(\d{3})\.(\d{3})\.(\d{3})(\d)/, "$1.$2.$3-$4");
    }

    // --- MASCARA DE TELEFONE ---
    function mascararTelefone(valor) {
        const digitos = valor.replace(/\D/g, "").slice(0, 11);
        return digitos
            .replace(/^(\d{2})(\d)/, "($1) $2")
            .replace(/(\d{5})(\d)/, "$1-$2");
    }

    // --- MASCARA DE CEP ---
    function mascararCep(valor) {
        const digitos = valor.replace(/\D/g, "").slice(0, 8);
        return digitos.replace(/^(\d{5})(\d)/, "$1-$2");
    }

    // --- APLICA A MASCARA ---
    function aplicarMascara(input, fnMascara) {
        input.addEventListener("input", () => {
            input.value = fnMascara(input.value);
        });
    }

    aplicarMascara(inputCpf, mascararCpf);
    aplicarMascara(inputTelefone, mascararTelefone);
    aplicarMascara(inputCep, mascararCep);

    // --- BUSCA O ENDERECO PELO CEP NA BASE OFICIAL ---
    async function buscarEnderecoPorCep() {
        const cep = inputCep.value.replace(/\D/g, "");

        if (cep.length !== 8) {
            return;
        }

        try {
            const resposta = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
            const dados = await resposta.json();

            if (!dados || dados.erro) {
                return;
            }

            if (dados.logradouro) {
                inputLogradouro.value = dados.logradouro;
            }

            if (dados.bairro) {
                inputBairro.value = dados.bairro;
            }

            if (dados.localidade) {
                inputCidade.value = dados.localidade;
            }

            if (dados.uf) {
                inputEstado.value = dados.uf;
            }
        } catch (erro) {
            // FALHA DE REDE OU CEP INVALIDO
        }
    }

    inputCep.addEventListener("blur", buscarEnderecoPorCep);
    inputCep.addEventListener("input", () => {
        if (inputCep.value.replace(/\D/g, "").length === 8) {
            buscarEnderecoPorCep();
        }
    });

    // --- OBTEM O TIPO DE ENTRADA SELECIONADO ---
    function tipoEntradaSelecionado() {
        const radio = form.querySelector("input[name='tipoEntrada']:checked");
        return radio ? radio.value : "INTEIRA";
    }

    // --- ROTULO AMIGAVEL DO TIPO DE ENTRADA ---
    function rotuloEntrada(tipo) {
        if (tipo === "MEIA") {
            return "Meia";
        }

        if (tipo === "SOCIAL") {
            return "Social";
        }
        return "Inteira";
    }

    // --- CALCULA O PRECO UNITARIO CONFORME O TIPO DE ENTRADA ---
    function precoUnitarioAtual(ingresso) {
        const tipo = tipoEntradaSelecionado();
        if (tipo === "MEIA") {
            return ingresso.preco / 2;
        }

        if (tipo === "SOCIAL") {
            return ingresso.preco / 2 + SOCIAL_ACRESCIMO;
        }
        return ingresso.preco;
    }

    // --- CALCULA A IDADE COMPLETA A PARTIR DE UMA DATA ISO ---
    function calcularIdade(iso) {
        const nasc = new Date(iso);
        if (isNaN(nasc.getTime())) {
            return 0;
        }
        const hoje = new Date();
        let idade = hoje.getFullYear() - nasc.getFullYear();
        const mes = hoje.getMonth() - nasc.getMonth();
        if (mes < 0 || (mes === 0 && hoje.getDate() < nasc.getDate())) {
            idade--;
        }
        return idade;
    }

    // --- VALIDA OS DOCUMENTOS EXTRAS + REGRA DE IDADE DE UMA PESSOA ---
    function validarPessoaDoc(subtipo, documentos, dataNascimento, quem) {
        const campos = CATALOGO_MEIA[subtipo] || [];
        for (const campo of campos) {
            if (campo.obrigatorio && !(documentos && documentos[campo.chave])) {
                return `${quem}: informe "${campo.rotulo}" da meia entrada.`;
            }
        }
        
        if (subtipo === "MENOR_18" && dataNascimento) {
            const idade = calcularIdade(dataNascimento);
            if (idade >= IDADE_MAXIMA_MENOR18) {
                return `${quem}: cliente com 18 anos ou mais não pode usar a meia Menor de 18. Selecione outra meia.`;
            }
        }
        return null;
    }

    // --- MONTA O HTML DOS CAMPOS DE DOCUMENTO ---
    function htmlCamposDoc(campos, montarAttrs, valores) {
        let html = "";
        for (let k = 0; k < campos.length; k += 2) {
            html += '<div class="form-grid">';
            for (let j = k; j < Math.min(k + 2, campos.length); j++) {
                const campo = campos[j];
                const val = (valores && valores[campo.chave]) || "";
                const opcional = campo.obrigatorio ? "" : ' <span class="campo-opcional">(opcional)</span>';
                html += `
                    <div class="campo">
                        <label>${campo.rotulo}${opcional}</label>
                        <input 
                            type="${campo.tipo}" ${montarAttrs(campo)} 
                            value="${val}" 
                        />
                    </div>`;
            }
            html += '</div>';
        }
        return html;
    }

    // --- COLETA OS DOCUMENTOS DA MEIA DO COMPRADOR ---
    function coletarDocsComprador() {
        const docs = {};
        docsComprador.querySelectorAll("input[data-doc-comprador]").forEach((input) => {
            docs[input.dataset.docChave] = input.value.trim();
        });
        return docs;
    }

    // --- RENDERIZA OS DOCUMENTOS DA MEIA DO COMPRADOR CONFORME O SUBTIPO ---
    function renderizarDocumentosComprador() {
        if (tipoEntradaSelecionado() !== "MEIA" || !selSubtipo.value) {
            docsComprador.innerHTML = "";
            return;
        }
        const sub = selSubtipo.value;
        const campos = CATALOGO_MEIA[sub] || [];
        const valores = coletarDocsComprador();

        let html = '<h3 class="secao-subtitulo">Documentos da meia — Ingresso 1</h3>';
        if (campos.length === 0) {
            html += '<p class="painel-subtitulo">Informe a data de nascimento acima (deve ser menor de 18 anos).</p>';
        } else {
            html += htmlCamposDoc(
                campos,
                (c) => `data-doc-comprador="1" data-doc-chave="${c.chave}"`,
                valores
            );
        }
        docsComprador.innerHTML = html;

        if (window.DatePicker) {
            window.DatePicker.aplicar(docsComprador);
        }
    }

    // --- ATUALIZA A VISIBILIDADE DE SUBTIPO/SOCIAL/DOCUMENTOS AO TROCAR A MODALIDADE ---
    function atualizarModalidade() {
        const tipo = tipoEntradaSelecionado();
        grupoSubtipo.classList.toggle("oculto", tipo !== "MEIA");
        avisoSocial.classList.toggle("oculto", tipo !== "SOCIAL");
        if (tipo !== "MEIA") {
            selSubtipo.value = "";
        }
        renderizarDocumentosComprador();
        renderizarAcompanhantes();
        atualizarResumo();
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
        btnPagConfirmar.disabled = salvando;
        btnPagVoltar.disabled = salvando;
        btnPagCancelar.disabled = salvando;
        btnPagConfirmar.querySelector(".btn-texto").classList.toggle("oculto", salvando);
        btnPagConfirmar.querySelector(".btn-spinner").classList.toggle("oculto", !salvando);
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
                opt.value = ing.id;
                const esgotado = ing.quantidadeDisponivel <= 0;
                opt.textContent = `${ing.setor} — ${formatarMoeda(ing.preco)}` +
                    (esgotado ? " (esgotado)" : ` — ${ing.quantidadeDisponivel} disp.`);
                opt.disabled = esgotado;
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
        const qtd = parseInt(inputQtd.value, 10) || 0;

        if (!ingresso || qtd <= 0) {
            resumoIngresso.classList.add("oculto");
            resumoIngresso.innerHTML = "";
            return;
        }

        const tipo = tipoEntradaSelecionado();
        const precoUnitario = precoUnitarioAtual(ingresso);
        const total = precoUnitario * qtd;

        resumoIngresso.classList.remove("oculto");
        resumoIngresso.innerHTML = `
            <strong>${ingresso.setor}</strong> • ${rotuloEntrada(tipo)} • ${qtd}x
            ${formatarMoeda(precoUnitario)} =
            <strong>${formatarMoeda(total)}</strong>
        `;
    }

    // --- RENDERIZA OS DADOS COMPLETOS DE CADA INGRESSO ADICIONAL ---
    function renderizarAcompanhantes() {
        const qtd = parseInt(inputQtd.value, 10) || 0;
        const totalAcompanhantes = Math.max(0, qtd - 1);

        const anteriores = coletarAcompanhantes();

        if (totalAcompanhantes <= 0) {
            listaAcompanhantes.innerHTML = "";
            return;
        }

        const meiaComSubtipo = tipoEntradaSelecionado() === "MEIA" && selSubtipo.value;
        const camposDocAcomp = meiaComSubtipo ? (CATALOGO_MEIA[selSubtipo.value] || []) : [];
        let html = "";
        for (let i = 0; i < totalAcompanhantes; i++) {
            const dados = anteriores[i] || {};
            const numeroIngresso = i + 2; 
            html += `
                <h3 class="secao-subtitulo">Ingresso ${numeroIngresso} — Acompanhante</h3>
                <div class="form-grid">
                    <div class="campo">
                        <label for="acomp-nome-${i}">Nome</label>
                        <input type="text" id="acomp-nome-${i}" data-acomp="${i}" data-campo="nome" placeholder="João" value="${dados.nome || ""}" />
                    </div>
                    <div class="campo">
                        <label for="acomp-sobrenome-${i}">Sobrenome</label>
                        <input type="text" id="acomp-sobrenome-${i}" data-acomp="${i}" data-campo="sobrenome" placeholder="Silva" value="${dados.sobrenome || ""}" />
                    </div>
                </div>
                <div class="form-grid">
                    <div class="campo">
                        <label for="acomp-email-${i}">E-mail</label>
                        <input type="email" id="acomp-email-${i}" data-acomp="${i}" data-campo="email" placeholder="cliente@email.com" value="${dados.email || ""}" />
                    </div>
                    <div class="campo">
                        <label for="acomp-cpf-${i}">CPF</label>
                        <input type="text" id="acomp-cpf-${i}" data-acomp="${i}" data-campo="cpf" data-mascara="cpf" placeholder="000.000.000-00" maxlength="14" value="${dados.cpf || ""}" />
                    </div>
                </div>
                <div class="form-grid">
                    <div class="campo">
                        <label for="acomp-telefone-${i}">Telefone</label>
                        <input type="text" id="acomp-telefone-${i}" data-acomp="${i}" data-campo="telefone" data-mascara="telefone" placeholder="(00) 00000-0000" value="${dados.telefone || ""}" />
                    </div>
                    <div class="campo">
                        <label for="acomp-dataNascimento-${i}">Data de nascimento</label>
                        <input type="date" id="acomp-dataNascimento-${i}" data-acomp="${i}" data-campo="dataNascimento" value="${dados.dataNascimento || ""}" />
                    </div>
                </div>
            `;

            if (camposDocAcomp.length > 0) {
                html += htmlCamposDoc(
                    camposDocAcomp,
                    (c) => `data-acomp="${i}" data-doc-chave="${c.chave}"`,
                    dados.documentos || {}
                );
            }
        }

        listaAcompanhantes.innerHTML = html;

        if (window.DatePicker) {
            window.DatePicker.aplicar(listaAcompanhantes);
        }
    }

    // --- COLETA OS DADOS COMPLETOS INFORMADOS PARA CADA ACOMPANHANTE ---
    function coletarAcompanhantes() {
        const mapa = [];
        listaAcompanhantes.querySelectorAll("input[data-acomp]").forEach((input) => {
            const idx = parseInt(input.dataset.acomp, 10);
            if (!mapa[idx]) {
                mapa[idx] = {};
            }
            
            if (input.dataset.campo) {
                mapa[idx][input.dataset.campo] = input.value.trim();
            } else if (input.dataset.docChave) {
                if (!mapa[idx].documentos) {
                    mapa[idx].documentos = {};
                }
                mapa[idx].documentos[input.dataset.docChave] = input.value.trim();
            }
        });
        return mapa;
    }

    // --- APLICA MASCARA NOS CAMPOS DINAMICOS DE CPF E TELEFONE ---
    listaAcompanhantes.addEventListener("input", (e) => {
        const alvo = e.target;
        if (alvo.dataset.mascara === "cpf") {
            alvo.value = mascararCpf(alvo.value);
        } else if (alvo.dataset.mascara === "telefone") {
            alvo.value = mascararTelefone(alvo.value);
        }
    });

    // --- RESETA TODO O FORMULARIO ---
    function limparFormulario() {
        form.reset();
        cacheIngressos = [];
        selIngresso.disabled = true;
        selIngresso.innerHTML = "<option value=''>Selecione um evento primeiro</option>";
        resumoIngresso.classList.add("oculto");
        resumoIngresso.innerHTML = "";
        grupoSubtipo.classList.add("oculto");
        avisoSocial.classList.add("oculto");
        selSubtipo.value = "";
        docsComprador.innerHTML = "";
        renderizarAcompanhantes();
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
                <span class="valor">${venda.setor} • ${rotuloEntrada(venda.tipoEntrada)} • ${venda.quantidade}x</span>
            </div>
            <div class="linha">
                <span class="label">Pagamento</span>
                <span class="valor">${(FORMAS_PAGAMENTO[venda.formaPagamento] || {}).rotulo || venda.formaPagamento || "--"}</span>
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
                    <img 
                        class="qr-img" 
                        src="data:image/png;base64,${venda.qrCodeBase64}" 
                        alt="QR Code do ingresso" 
                    />
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

    // --- CALCULA O TOTAL ATUAL DA VENDA ---
    function calcularTotal() {
        const ingresso = cacheIngressos.find((i) => String(i.id) === String(selIngresso.value));
        const qtd = parseInt(inputQtd.value, 10) || 0;
        if (!ingresso || qtd <= 0) {
            return 0;
        }
        const precoUnitario = precoUnitarioAtual(ingresso);
        return precoUnitario * qtd;
    }

    // --- ABRE O MODAL DE PAGAMENTO NA ETAPA DE ESCOLHA ---
    function abrirPagamento() {
        formaSelecionada = null;
        opcoesPagamento.forEach((op) => op.classList.remove("selecionada"));
        btnPagContinuar.disabled = true;
        mostrarEtapaEscolha();

        lblPagamentoValor.textContent = formatarMoeda(totalPendente);
        modalPagamento.classList.remove("oculto");
    }

    // --- FECHA O MODAL DE PAGAMENTO ---
    function fecharPagamento() {
        modalPagamento.classList.add("oculto");
    }

    // --- EXIBE A ETAPA 1 ---
    function mostrarEtapaEscolha() {
        etapaEscolha.classList.remove("oculto");
        etapaConfirmacao.classList.add("oculto");
        btnPagContinuar.classList.remove("oculto");
        btnPagCancelar.classList.remove("oculto");
        btnPagConfirmar.classList.add("oculto");
        btnPagVoltar.classList.add("oculto");
    }

    // --- EXIBE A ETAPA 2 ---
    function mostrarEtapaConfirmacao() {
        const info = FORMAS_PAGAMENTO[formaSelecionada];

        confIcone.innerHTML = info.icone;
        confForma.textContent = info.rotulo;
        confValor.textContent = formatarMoeda(totalPendente);

        etapaEscolha.classList.add("oculto");
        etapaConfirmacao.classList.remove("oculto");
        btnPagContinuar.classList.add("oculto");
        btnPagCancelar.classList.add("oculto");
        btnPagConfirmar.classList.remove("oculto");
        btnPagVoltar.classList.remove("oculto");
    }

    // --- SELECIONA UMA FORMA DE PAGAMENTO ---
    opcoesPagamento.forEach((opcao) => {
        opcao.addEventListener("click", () => {
            formaSelecionada = opcao.dataset.forma;
            opcoesPagamento.forEach((op) => op.classList.toggle("selecionada", op === opcao));
            btnPagContinuar.disabled = false;
        });
    });

    btnPagContinuar.addEventListener("click", () => {
        if (!formaSelecionada) {
            return;
        }
        mostrarEtapaConfirmacao();
    });

    btnPagVoltar.addEventListener("click", mostrarEtapaEscolha);

    btnPagCancelar.addEventListener("click", fecharPagamento);

    btnPagConfirmar.addEventListener("click", async () => {
        if (!payloadPendente || !formaSelecionada) {
            return;
        }

        payloadPendente.formaPagamento = formaSelecionada;

        alternarBotaoSalvando(true);
        try {
            const venda = await Api.post("/balcao/vender", payloadPendente);
            fecharPagamento();
            abrirComprovante(venda);
            payloadPendente = null;
        } catch (erro) {
            fecharPagamento();
            exibirFeedback("erro", erro.message || "Falha ao finalizar a venda.");
        } finally {
            alternarBotaoSalvando(false);
        }
    });

    // --- HANDLERS DE INTERACAO ---
    selEvento.addEventListener("change", (e) => {
        carregarIngressos(e.target.value);
    });

    selIngresso.addEventListener("change", atualizarResumo);
    inputQtd.addEventListener("input", () => {
        atualizarResumo();
        renderizarAcompanhantes();
    });

    form.querySelectorAll("input[name='tipoEntrada']").forEach((r) => {
        r.addEventListener("change", atualizarModalidade);
    });

    selSubtipo.addEventListener("change", () => {
        renderizarDocumentosComprador();
        renderizarAcompanhantes();
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
            nome: form.elements["nome"].value.trim(),
            sobrenome: form.elements["sobrenome"].value.trim(),
            email: form.elements["email"].value.trim(),
            cpf: form.elements["cpf"].value.trim(),
            telefone: form.elements["telefone"].value.trim(),
            dataNascimento: form.elements["dataNascimento"].value || null,
            cep: form.elements["cep"].value.trim() || null,
            logradouro: form.elements["logradouro"].value.trim() || null,
            numero: form.elements["numero"].value.trim() || null,
            complemento: form.elements["complemento"].value.trim() || null,
            bairro: form.elements["bairro"].value.trim() || null,
            cidade: form.elements["cidade"].value.trim() || null,
            estado: form.elements["estado"].value.trim() || null
        };

        if (!cliente.nome || !cliente.sobrenome || !cliente.email || !cliente.cpf || !cliente.telefone) {
            return exibirFeedback("erro", "Preencha nome, sobrenome, e-mail, CPF e telefone do cliente.");
        }

        const tipoEntrada = tipoEntradaSelecionado();
        const subtipo = tipoEntrada === "MEIA" ? selSubtipo.value : null;

        if (tipoEntrada === "MEIA") {
            if (!subtipo) {
                return exibirFeedback("erro", "Selecione o tipo de meia entrada.");
            }
            
            cliente.documentos = coletarDocsComprador();
            const erroComprador = validarPessoaDoc(subtipo, cliente.documentos, cliente.dataNascimento, "Ingresso 1");
            if (erroComprador) {
                return exibirFeedback("erro", erroComprador);
            }
        }

        const acompanhantes = coletarAcompanhantes();
        if (acompanhantes.length !== quantidade - 1) {
            return exibirFeedback("erro", "Preencha os dados de todos os ingressos.");
        }

        for (let i = 0; i < acompanhantes.length; i++) {
            const a = acompanhantes[i];
            if (!a.nome || !a.sobrenome || !a.email || !a.cpf || !a.telefone) {
                return exibirFeedback("erro", `Preencha nome, sobrenome, e-mail, CPF e telefone do Ingresso ${i + 2}.`);
            }
            a.dataNascimento = a.dataNascimento || null;

            if (tipoEntrada === "MEIA") {
                const erroAcomp = validarPessoaDoc(subtipo, a.documentos, a.dataNascimento, `Ingresso ${i + 2}`);
                if (erroAcomp) {
                    return exibirFeedback("erro", erroAcomp);
                }
            }
        }

        const payload = {
            eventoId:      parseInt(eventoId, 10),
            ingressoId:    parseInt(ingressoId, 10),
            tipoEntrada:   tipoEntrada,
            subtipo:       subtipo,
            quantidade:    quantidade,
            cliente:       cliente,
            acompanhantes: acompanhantes
        };

        payloadPendente = payload;
        totalPendente = calcularTotal();
        abrirPagamento();
    });

    carregarEventos();
    renderizarAcompanhantes();
})();
