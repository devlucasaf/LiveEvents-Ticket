import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { pedidoService } from "../../services/pedidoService";
import "../../styles/checkout.css";

function mascaraCartao(value) {
  return value
    .replace(/\D/g, "")
    .slice(0, 16)
    .replace(/(\d{4})(?=\d)/g, "$1 ")
    .trim();
}

function formatarMoeda(valor) {
  return valor.toLocaleString(
    "pt-BR", { 
      style: "currency", 
      currency: "BRL" 
    }
  );
}

export default function CheckoutPage() {
  const navigate = useNavigate();
  const item = JSON.parse(localStorage.getItem("checkoutItem") || "{}");

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

  if (!item.ingressoId) {
    return (
      <div className="checkout-page">
        <div className="checkout-page__card">
          <p className="error">Nenhum ingresso selecionado.</p>
        </div>
      </div>
    );
  }

  const precoBase = item.preco || 0;
  const taxaServico = precoBase * 0.10;
  const subtotal = precoBase + taxaServico;
  const valorSeguro = seguro ? subtotal * 0.03 : 0;
  const total = subtotal + valorSeguro;

  const maxParcelas = Math.min(12, Math.floor(total / 10) || 1);

  function gerarOpcoesParcelas() {
    const opcoes = [];
    for (let i = 1; i <= maxParcelas; i++) {
      const valorParcela = total / i;
      opcoes.push({ 
        parcelas: i, 
        valor: valorParcela 
      });
    }
    return opcoes;
  }

  function mascaraValidade(value) {
    return value
      .replace(/\D/g, "")
      .slice(0, 4)
      .replace(/(\d{2})(\d)/, "$1/$2");
  }

  async function finalizarCompra() {
    setErro("");
    try {
      const data = await pedidoService.checkout({
        itens: [{ 
          ingressoId: item.ingressoId, 
          quantidade: item.quantidade 
        }],
        pagamento: {
          tipo: tipoPagamento === "cartao" ? "CARTAO" : "PIX",
          numeroCartao: tipoPagamento === "cartao" ? numeroCartao.replace(/\s/g, "") : null,
          parcelas: tipoPagamento === "cartao" ? parcelas : null
        },
        seguro
      });
      setResultado(data);
      setEtapa("confirmado");
      localStorage.removeItem("checkoutItem");
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

          <div className="checkout-page__item">
            <div className="checkout-page__item-info">
              <h3>{item.eventoTitulo || "Evento"}</h3>
              <p className="checkout-page__item-meta">
                {item.eventoLocal && <span>{item.eventoLocal}</span>}
                {item.eventoData && (
                  <span>{new Date(item.eventoData).toLocaleDateString("pt-BR", { day: "2-digit", month: "long", year: "numeric" })}</span>
                )}
              </p>
              <p className="checkout-page__item-setor">Setor: <strong>{item.setor || "—"}</strong></p>
              <p className="checkout-page__item-qty">Qtd: {item.quantidade}</p>
            </div>
            <span className="checkout-page__item-price">{formatarMoeda(precoBase)}</span>
          </div>

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

          <div className="checkout-page__resumo">
            <h4>Resumo</h4>
            <div className="checkout-page__resumo-linha">
              <span>Ingresso ({item.setor})</span>
              <span>{formatarMoeda(precoBase)}</span>
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

          <button className="checkout-page__btn" onClick={() => setEtapa("pagamento")}>
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

          <button className="checkout-page__voltar" onClick={() => setEtapa("carrinho")}>
            ← Voltar ao carrinho
          </button>

          <h2>Forma de pagamento</h2>

          <div className="checkout-page__metodos">
            <button
              className={`checkout-page__metodo ${tipoPagamento === "cartao" ? "checkout-page__metodo--ativo" : ""}`}
              onClick={() => setTipoPagamento("cartao")}
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <rect x="1" y="4" width="22" height="16" rx="2"/><line x1="1" y1="10" x2="23" y2="10"/>
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
          Seu ingresso foi adquirido com sucesso. Ele já está disponível na sua área de ingressos.
        </p>

        {resultado && (
          <div className="checkout-page__result">
            <p><strong>Pedido:</strong> #{resultado.id}</p>
            <p><strong>Status:</strong> {resultado.pagamentoStatus}</p>
            {resultado.codigoPix && <p><strong>Código PIX:</strong> {resultado.codigoPix}</p>}
            {resultado.qrCodeBase64 && (
              <img 
                src={`data:image/png;base64,${resultado.qrCodeBase64}`} 
                alt="QR Code" 
              />
            )}
          </div>
        )}

        <button className="checkout-page__btn" onClick={() => navigate("/meus-eventos")}>
          Ver meus ingressos
        </button>
      </div>
    </div>
  );
}
