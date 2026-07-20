import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { pedidoService } from "../../services/pedidoService";
import { useCarrinho } from "../../context/CarrinhoContext";
import "../../styles/checkout.css";

// --- MASCARA PARA O NUMERO DO CARTAO ---
function mascaraCartao(value) {
    return value
        .replace(/\D/g, "")
        .slice(0, 16)
        .replace(/(\d{4})(?=\d)/g, "$1 ")
        .trim();
}

// --- FORMATA UM VALOR COMO MOEDA BRASILEIRA ---
function formatarMoeda(valor) {
    return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

// --- CAMPOS COMUNS EXIGIDOS EM QUALQUER MEIA ENTRADA ---
const CAMPOS_COMUNS = [
    { chave: "nome",           rotulo: "Nome completo",       tipo: "text", obrigatorio: true },
    { chave: "cpf",            rotulo: "CPF",                 tipo: "text", obrigatorio: true },
    { chave: "dataNascimento", rotulo: "Data de nascimento",  tipo: "date", obrigatorio: true }
];

// --- RETORNA OS CAMPOS DE DOCUMENTO DE UM ITEM DE MEIA ---
function camposDoItem(item) {
    return item.camposDocumento && item.camposDocumento.length ? item.camposDocumento : CAMPOS_COMUNS;
}

// --- CALCULA A IDADE COMPLETA A PARTIR DE UMA DATA ISO ---
function calcularIdade(dataIso) {
    if (!dataIso) {
        return null;
    }
    const nasc = new Date(dataIso);
    if (Number.isNaN(nasc.getTime())) {
        return null;
    }
    const hoje = new Date();
    let idade = hoje.getFullYear() - nasc.getFullYear();
    const m = hoje.getMonth() - nasc.getMonth();
    if (m < 0 || (m === 0 && hoje.getDate() < nasc.getDate())) {
        idade--;
    }
    return idade;
}

// --- TELA DE CHECKOUT: FINALIZA A COMPRA DO CARRINHO ---
export default function CheckoutPage() {
    const navigate = useNavigate();
    const { itens, limpar } = useCarrinho();
    const token = localStorage.getItem("token");

    const [etapa,           setEtapa]           = useState("carrinho");
    const [seguro,          setSeguro]          = useState(false);
    const [tipoPagamento,   setTipoPagamento]   = useState("");
    const [numeroCartao,    setNumeroCartao]    = useState("");
    const [nomeCartao,      setNomeCartao]      = useState("");
    const [validadeCartao,  setValidadeCartao]  = useState("");
    const [cvv,             setCvv]             = useState("");
    const [parcelas,        setParcelas]        = useState(1);
    const [erro,            setErro]            = useState("");
    const [resultado,       setResultado]       = useState(null);
    const [compraSnapshot,  setCompraSnapshot]  = useState(null);

    // --- DOCUMENTOS DA MEIA ENTRADA ---
    const [documentos,      setDocumentos]      = useState({});
    const [erroDoc,         setErroDoc]         = useState("");

    if (!token) {
        return (
        <div className="checkout-page">
            <div className="checkout-page__card">
                <h2>Faça login para continuar</h2>
                <p>Você precisa estar logado para finalizar a compra do ingresso.</p>
                <button
                    className="checkout-page__btn"
                    onClick={() => navigate("/auth/login?retorno=/carrinho")}
                >
                    Ir para o login
                </button>
            </div>
        </div>
        );
    }

    // --- CARRINHO VAZIO ---
    if (itens.length === 0 && etapa !== "confirmado") {
        return (
        <div className="checkout-page">
            <div className="checkout-page__card">
                <p className="error">Seu carrinho está vazio.</p>
                <button className="checkout-page__btn" onClick={() => navigate("/")}>
                    Explorar eventos
                </button>
            </div>
        </div>
        );
    }

    // --- CALCULA OS TOTAIS SOBRE TODOS OS ITENS DO CARRINHO ---
    const subtotalIngressos = itens.reduce((t, i) => t + i.precoUnitario * i.quantidade, 0);
    const taxaServico = subtotalIngressos * 0.10;
    const subtotal = subtotalIngressos + taxaServico;
    const valorSeguro = seguro ? subtotal * 0.03 : 0;
    const total = subtotal + valorSeguro;

    const maxParcelas = Math.min(12, Math.floor(total / 10) || 1);

    // --- ITENS QUE EXIGEM DOCUMENTOS (MEIA ENTRADA) ---
    const itensComDocumento = itens
        .map((item, idx) => ({ item, idx }))
        .filter(({ item }) => item.modalidade === "MEIA");

    // --- LE O VALOR DE UM DOCUMENTO NO ESTADO ---
    function valorDoc(idxItem, unidade, chave) {
        return documentos?.[idxItem]?.[unidade]?.[chave] ?? "";
    }

    // --- ATUALIZA UM CAMPO DE DOCUMENTO NO ESTADO ---
    function definirDoc(idxItem, unidade, chave, valor) {
        setErroDoc("");
        setDocumentos((prev) => {
            const copia = { ...prev };
            const doItem = { ...(copia[idxItem] || {}) };
            const daUnidade = { ...(doItem[unidade] || {}) };
            daUnidade[chave] = valor;
            doItem[unidade] = daUnidade;
            copia[idxItem] = doItem;
            return copia;
        });
    }

    // --- VALIDA TODOS OS DOCUMENTOS ANTES DE AVANCAR PARA O PAGAMENTO ---
    function validarDocumentos() {
        for (const { item, idx } of itensComDocumento) {
            const campos = camposDoItem(item);
            for (let u = 0; u < item.quantidade; u++) {
                for (const campo of campos) {
                    if (campo.obrigatorio && !String(valorDoc(idx, u, campo.chave)).trim()) {
                        setErroDoc(`Preencha "${campo.rotulo}" no ${item.setor} (ingresso ${u + 1}).`);
                        return false;
                    }
                }

                // --- REGRA DA MEIA MENOR DE 18 ANOS ---
                if (item.subtipoMeia === "MENOR_18") {
                    const idade = calcularIdade(valorDoc(idx, u, "dataNascimento"));
                    if (idade !== null && idade >= 18) {
                        setErroDoc(
                            `No ${item.setor} (ingresso ${u + 1}): cliente com 18 anos ou mais não pode usar a meia Menor de 18. Troque a modalidade (ex.: Estudantil) no carrinho.`
                        );
                        return false;
                    }
                }
            }
        }
        return true;
    }

    // --- AVANCA DO CARRINHO: VAI PARA DOCUMENTOS SE HOUVER MEIA, SENAO PARA PAGAMENTO ---
    function avancarDoCarrinho() {
        if (itensComDocumento.length > 0) {
            setEtapa("documentos");
        } else {
            setEtapa("pagamento");
        }
    }

    // --- MONTA O ARRAY DE DOCUMENTOS (UM POR UNIDADE) DE UM ITEM ---
    function documentosDoItem(idxItem, item) {
        if (item.modalidade !== "MEIA") {
            return null;
        }
        const lista = [];
        for (let u = 0; u < item.quantidade; u++) {
            lista.push({ ...(documentos?.[idxItem]?.[u] || {}) });
        }
        return lista;
    }

    // --- GERA AS OPCOES DE PARCELAMENTO ATE O MAXIMO PERMITIDO ---
    function gerarOpcoesParcelas() {
        const opcoes = [];
        for (let i = 1; i <= maxParcelas; i++) {
            opcoes.push({ 
                parcelas: i, 
                valor: total / i 
            });
        }
        return opcoes;
    }

    // --- MASCARA PARA A VALIDADE DO CARTAO ---
    function mascaraValidade(value) {
        return value
            .replace(/\D/g, "")
            .slice(0, 4)
            .replace(/(\d{2})(\d)/, "$1/$2");
    }

    // --- ENVIA O PEDIDO COM TODOS OS ITENS E A FORMA DE PAGAMENTO ---
    async function finalizarCompra() {
        setErro("");
        try {
            const data = await pedidoService.checkout({
                itens: itens.map((i, idx) => ({
                    ingressoId: i.ingressoId,
                    quantidade: i.quantidade,
                    modalidade: i.modalidade,
                    subtipoMeia: i.subtipoMeia,
                    documentos: documentosDoItem(idx, i)
                })),
                pagamento: {
                    tipo: tipoPagamento === "cartao" ? "CARTAO" : "PIX",
                    numeroCartao: tipoPagamento === "cartao" ? numeroCartao.replace(/\s/g, "") : null,
                    parcelas: tipoPagamento === "cartao" ? parcelas : null
                },
                seguro
            });
            setCompraSnapshot({
                itens,
                subtotalIngressos,
                taxaServico,
                valorSeguro,
                total,
                seguro,
                tipoPagamento,
                parcelas,
                numeroCartao
            });
            setResultado(data);
            setEtapa("confirmado");
            limpar();
        } catch (e) {
            setErro(e.message);
        }
    }

    // --- CARRINHO ---
    if (etapa === "carrinho") {
        return (
        <div className="checkout-page">
            <div className="checkout-page__card">
                <div className="checkout-page__step-indicator">
                    <span className="checkout-page__step checkout-page__step--active">1. Carrinho</span>
                    <span className="checkout-page__step">2. Pagamento</span>
                </div>

                <h2>Carrinho</h2>

                {/* --- LISTA DE ITENS DO CARRINHO --- */}
                {itens.map((item, idx) => (
                    <div key={idx} className="checkout-page__item">
                        <div className="checkout-page__item-info">
                            <h3>{item.eventoTitulo || "Evento"}</h3>
                            <p className="checkout-page__item-meta">
                                {item.eventoLocal && <span>{item.eventoLocal}</span>}
                                {item.eventoData && (
                                    <span>
                                        {new Date(item.eventoData).toLocaleDateString("pt-BR", { day: "2-digit", month: "long", year: "numeric" })}
                                    </span>
                                )}
                            </p>
                            <p className="checkout-page__item-setor">
                                Setor: <strong>{item.setor}</strong>
                            </p>
                            <p className="checkout-page__item-setor">
                                Tipo: <strong>{item.modalidadeLabel}</strong>
                            </p>
                            <p className="checkout-page__item-qty">
                                Qtd: {item.quantidade}
                            </p>
                        </div>
                        <span className="checkout-page__item-price">
                            {formatarMoeda(item.precoUnitario * item.quantidade)}
                        </span>
                    </div>
                ))}

                {/* --- SEGURO OPCIONAL --- */}
                <div className="checkout-page__seguro">
                    <label className="checkout-page__seguro-label">
                        <input
                            type="checkbox"
                            checked={seguro}
                            onChange={(e) => setSeguro(e.target.checked)}
                        />
                        <div className="checkout-page__seguro-text">
                            <strong>Seguro Ingresso Protegido</strong>
                            <span>Proteja seu ingresso contra imprevistos (+3%)</span>
                        </div>
                        <span className="checkout-page__seguro-preco">+{formatarMoeda(subtotal * 0.03)}</span>
                    </label>
                </div>

                {/* --- RESUMO DE VALORES --- */}
                <div className="checkout-page__resumo">
                    <h4>Resumo</h4>
                    <div className="checkout-page__resumo-linha">
                        <span>Ingressos</span>
                        <span>{formatarMoeda(subtotalIngressos)}</span>
                    </div>

                    <div className="checkout-page__resumo-linha">
                        <span>Taxa de serviço (10%)</span>
                        <span>{formatarMoeda(taxaServico)}</span>
                    </div>

                    {seguro && (
                        <div className="checkout-page__resumo-linha">
                            <span>Seguro Ingresso Protegido (3%)</span>
                            <span>{formatarMoeda(valorSeguro)}</span>
                        </div>
                    )}
                    <div className="checkout-page__resumo-linha checkout-page__resumo-total">
                        <strong>Total</strong>
                        <strong>{formatarMoeda(total)}</strong>
                    </div>
                </div>

                <button className="checkout-page__btn" onClick={avancarDoCarrinho}>
                    Continuar
                </button>
            </div>
        </div>
        );
    }

    // --- DOCUMENTOS DA MEIA ENTRADA ---
    if (etapa === "documentos") {
        return (
        <div className="checkout-page">
            <div className="checkout-page__card">
                <div className="checkout-page__step-indicator">
                    <span className="checkout-page__step checkout-page__step--done">1. Carrinho</span>
                    <span className="checkout-page__step checkout-page__step--active">2. Documentos</span>
                    <span className="checkout-page__step">3. Pagamento</span>
                </div>

                <button className="checkout-page__voltar" onClick={() => setEtapa("carrinho")}>
                    ← Voltar ao carrinho
                </button>

                <h2>Documentos da meia entrada</h2>
                <p className="checkout-page__doc-intro">
                    Informe os dados do beneficiário de cada meia entrada.
                </p>

                {itensComDocumento.map(({ item, idx }) => (
                    <div key={idx} className="checkout-page__doc-item">
                        <div className="checkout-page__doc-item-head">
                            <strong>{item.setor}</strong>
                            <span>{item.modalidadeLabel}</span>
                        </div>

                        {Array.from({ length: item.quantidade }).map((_, u) => (
                            <div key={u} className="checkout-page__doc-unidade">
                                {item.quantidade > 1 && (
                                    <h4 className="checkout-page__doc-unidade-titulo">Ingresso {u + 1}</h4>
                                )}

                                <div className="checkout-page__doc-grid">
                                    {camposDoItem(item).map((campo) => (
                                        <div key={campo.chave} className="checkout-page__field">
                                            <label>
                                                {campo.rotulo}
                                                {!campo.obrigatorio && <span className="checkout-page__opcional"> (opcional)</span>}
                                            </label>
                                            <input
                                                className="checkout-page__input"
                                                type={campo.tipo === "date" ? "date" : "text"}
                                                value={valorDoc(idx, u, campo.chave)}
                                                onChange={(e) => definirDoc(idx, u, campo.chave, e.target.value)}
                                            />
                                        </div>
                                    ))}
                                </div>
                            </div>
                        ))}
                    </div>
                ))}

                {erroDoc && <p className="error">{erroDoc}</p>}

                <button
                    className="checkout-page__btn"
                    onClick={() => {
                        if (validarDocumentos()) {
                            setEtapa("pagamento");
                        }
                    }}
                >
                    Continuar
                </button>
            </div>
        </div>
        );
    }

    // --- PAGAMENTO ---
    if (etapa === "pagamento") {
        return (
        <div className="checkout-page">
            <div className="checkout-page__card">
                <div className="checkout-page__step-indicator">
                    <span className="checkout-page__step checkout-page__step--done">1. Carrinho</span>
                    <span className="checkout-page__step checkout-page__step--active">2. Pagamento</span>
                </div>

                <button className="checkout-page__voltar" onClick={() => setEtapa(itensComDocumento.length > 0 ? "documentos" : "carrinho")}>
                    ← Voltar
                </button>

                <h2>Forma de pagamento</h2>

                <div className="checkout-page__metodos">
                    <button
                        className={`checkout-page__metodo ${tipoPagamento === "cartao" ? "checkout-page__metodo--ativo" : ""}`}
                        onClick={() => setTipoPagamento("cartao")}
                    >
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <rect 
                            x="1" 
                            y="4" 
                            width="22" 
                            height="16" 
                            rx="2"
                            />
                        <line 
                            x1="1" 
                            y1="10" 
                            x2="23" 
                            y2="10"
                        />
                    </svg>
                    Cartão de Crédito
                    </button>
                    <button
                        className={`checkout-page__metodo ${tipoPagamento === "pix" ? "checkout-page__metodo--ativo" : ""}`}
                        onClick={() => setTipoPagamento("pix")}
                    >
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                            <path d="M13.5 3.5L6 11l1.5 1.5L12 8l4.5 4.5L18 11l-4.5-7.5zM6 13l7.5 7.5L18 13"/>
                        </svg>
                        PIX
                    </button>
                </div>

                {tipoPagamento === "cartao" && (
                    <div className="checkout-page__form-cartao">
                        <div className="checkout-page__field">
                            <label>Número do cartão</label>
                            <input
                            className="checkout-page__input"
                            value={numeroCartao}
                            onChange={(e) => setNumeroCartao(mascaraCartao(e.target.value))}
                            placeholder="0000 0000 0000 0000"
                            maxLength={19}
                            />
                        </div>
                        <div className="checkout-page__field">
                            <label>Nome no cartão</label>
                            <input
                            className="checkout-page__input"
                            value={nomeCartao}
                            onChange={(e) => setNomeCartao(e.target.value)}
                            placeholder="Nome impresso no cartão"
                            />
                        </div>

                        <div className="checkout-page__row">
                            <div className="checkout-page__field">
                                <label>Validade</label>
                                <input
                                    className="checkout-page__input"
                                    value={validadeCartao}
                                    onChange={(e) => setValidadeCartao(mascaraValidade(e.target.value))}
                                    placeholder="MM/AA"
                                    maxLength={5}
                                />
                            </div>

                            <div className="checkout-page__field">
                                <label>CVV</label>
                                <input
                                    className="checkout-page__input"
                                    value={cvv}
                                    onChange={(e) => setCvv(e.target.value.replace(/\D/g, "").slice(0, 4))}
                                    placeholder="123"
                                    maxLength={4}
                                />
                            </div>
                        </div>

                        <div className="checkout-page__field">
                            <label>Parcelas</label>
                            <select
                                className="checkout-page__select"
                                value={parcelas}
                                onChange={(e) => setParcelas(Number(e.target.value))}
                                >
                                {gerarOpcoesParcelas().map((op) => (
                                    <option key={op.parcelas} value={op.parcelas}>
                                        {op.parcelas}x de {formatarMoeda(op.valor)}
                                        {op.parcelas === 1 ? " (à vista)" : ""}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>
                )}

                {tipoPagamento === "pix" && (
                    <div className="checkout-page__pix-info">
                        <p>Ao confirmar, um código PIX será gerado para pagamento imediato.</p>
                    </div>
                )}

                <div className="checkout-page__resumo-mini">
                    <span>Total:</span>
                    <strong>{formatarMoeda(total)}</strong>
                </div>

                {erro && <p className="error">{erro}</p>}

                <button
                    className="checkout-page__btn"
                    onClick={finalizarCompra}
                    disabled={!tipoPagamento || (tipoPagamento === "cartao" && numeroCartao.replace(/\s/g, "").length < 16)}
                >
                    Finalizar compra
                </button>
            </div>
        </div>
        );
    }

    // --- CONFIRMADO ---
    const snap = compraSnapshot || {};
    const ultimosDigitos = snap.numeroCartao
        ? snap.numeroCartao.replace(/\s/g, "").slice(-4)
        : null;

    return (
        <div className="checkout-page">
        <div className="checkout-page__card checkout-page__card--confirmado">
            <div className="checkout-page__sucesso-icone">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                <path d="M20 6L9 17l-5-5"/>
            </svg>
            </div>
            <h2>Compra confirmada!</h2>
            <p className="checkout-page__sucesso-msg">
            Seus ingressos já estão disponíveis em <strong>Meus Ingressos</strong>.
            </p>

            {/* --- RESUMO DETALHADO DA COMPRA --- */}
            {snap.itens && (
            <div className="checkout-page__confirmado-resumo">
                {snap.itens.map((item, idx) => (
                <div key={idx} className="checkout-page__confirmado-evento">
                    <h3>{item.eventoTitulo || "Evento"}</h3>
                    <div className="checkout-page__confirmado-detalhes">
                    <div className="checkout-page__confirmado-linha">
                        <span>Setor</span>
                        <strong>{item.setor}</strong>
                    </div>
                    <div className="checkout-page__confirmado-linha">
                        <span>Tipo</span>
                        <strong>{item.modalidadeLabel}</strong>
                    </div>
                    <div className="checkout-page__confirmado-linha">
                        <span>Quantidade</span>
                        <strong>{item.quantidade}</strong>
                    </div>
                    <div className="checkout-page__confirmado-linha">
                        <span>Subtotal</span>
                        <strong>{formatarMoeda(item.precoUnitario * item.quantidade)}</strong>
                    </div>
                    </div>
                </div>
                ))}

                <div className="checkout-page__confirmado-detalhes">
                <div className="checkout-page__confirmado-linha">
                    <span>Taxa de serviço</span>
                    <strong>{formatarMoeda(snap.taxaServico)}</strong>
                </div>
                {snap.seguro && (
                    <div className="checkout-page__confirmado-linha">
                    <span>Seguro Ingresso Protegido</span>
                    <strong>{formatarMoeda(snap.valorSeguro)}</strong>
                    </div>
                )}
                <div className="checkout-page__confirmado-linha checkout-page__confirmado-linha--total">
                    <span>Total pago</span>
                    <strong>{formatarMoeda(snap.total)}</strong>
                </div>
                <div className="checkout-page__confirmado-linha">
                    <span>Forma de pagamento</span>
                    <strong>
                    {snap.tipoPagamento === "cartao"
                        ? `Cartão •••• ${ultimosDigitos} (${snap.parcelas}x)`
                        : "PIX"}
                    </strong>
                </div>
                {resultado && (
                    <div className="checkout-page__confirmado-linha">
                    <span>Nº do pedido</span>
                    <strong>#{resultado.id}</strong>
                    </div>
                )}
                </div>
            </div>
            )}

            {/* --- DADOS DO PIX / QR CODE --- */}
            {resultado?.codigoPix && (
            <div className="checkout-page__pix-box">
                <p className="checkout-page__pix-titulo">Código PIX</p>
                <code>{resultado.codigoPix}</code>
            </div>
            )}
            {resultado?.qrCodeBase64 && (
            <img
                className="checkout-page__qr"
                src={`data:image/png;base64,${resultado.qrCodeBase64}`}
                alt="QR Code do ingresso"
            />
            )}

            <div className="checkout-page__confirmado-acoes">
            <button className="checkout-page__btn" onClick={() => navigate("/meus-eventos")}>
                Ver Meus Ingressos
            </button>
            <button
                className="checkout-page__btn checkout-page__btn--secundario"
                onClick={() => navigate("/")}
            >
                Explorar mais eventos
            </button>
            </div>
        </div>
        </div>
    );
}
