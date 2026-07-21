import { useEffect, useState }          from  "react";
import { Link, useParams, useNavigate } from  "react-router-dom";
import { eventoService }                from  "../../services/eventoService";
import { useCarrinho }                  from  "../../context/CarrinhoContext";
import SelectCustom                     from  "../../components/SelectCustom";
import ModalCarrinho                    from  "../../components/ModalCarrinho";
import                                        "../../styles/evento-detalhe.css";

// --- FORMATA UM VALOR NUMERICO COMO MOEDA BRASILEIRA ---
function formatarMoeda(valor) {
  return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

// --- ESCOLHE MODALIDADE + QUANTIDADE ---
function SetorCard({ ingresso, evento, modalidades }) {
  const { adicionar } = useCarrinho();
  const navigate = useNavigate();
  const [modalidade,  setModalidade]  = useState("INTEIRA");
  const [subtipoMeia, setSubtipoMeia] = useState("");
  const [quantidade,  setQuantidade]  = useState(1);
  const [modalAberto, setModalAberto] = useState(false);
  const [itemModal,   setItemModal]   = useState(null);

  // --- DESCOBRE O PRECO UNITARIO CONFORME A MODALIDADE SELECIONADA ---
  function precoDaModalidade() {
    if (modalidade === "MEIA")   {
      return ingresso.precoMeia;
    }

    if (modalidade === "SOCIAL") {
      return ingresso.precoSocial;
    }
    return ingresso.preco;
  }

  // --- MONTA O ROTULO DESCRITIVO DA MODALIDADE PARA O CARRINHO ---
  function rotuloModalidade() {
    if (modalidade === "MEIA") {
      const sub = modalidades?.meiaSubtipos.find((s) => s.codigo === subtipoMeia);
      return sub ? sub.nome : "Meia entrada";
    }

    if (modalidade === "SOCIAL") {
      return "Social (1kg de alimento)";
    }
    return "Inteira";
  }

  // --- ADICIONA O SETOR ESCOLHIDO AO CARRINHO ---
  function adicionarAoCarrinho() {
    if (modalidade === "MEIA" && !subtipoMeia) {
      return;
    }

    // --- DESCOBRE OS CAMPOS DE DOCUMENTO EXIGIDOS PELA MEIA ESCOLHIDA ---
    const subInfo = modalidade === "MEIA"
      ? modalidades?.meiaSubtipos.find((s) => s.codigo === subtipoMeia)
      : null;

    // --- MONTA A LINHA DO CARRINHO COM OS DADOS DO EVENTO E DA MODALIDADE ---
    const linha = {
      ingressoId: ingresso.id,
      eventoId: evento.id,
      eventoTitulo: evento.titulo,
      eventoData: evento.dataEvento,
      eventoLocal: evento.local,
      eventoImagem: evento.imagemUrl,
      setor: ingresso.setor,
      setorCodigo: ingresso.setorCodigo,
      modalidade,
      modalidadeLabel: rotuloModalidade(),
      subtipoMeia: modalidade === "MEIA" ? subtipoMeia : null,
      camposDocumento: subInfo?.campos || [],
      precoUnitario: precoDaModalidade(),
      quantidade
    };

    // --- INSERE NO CARRINHO ---
    adicionar(linha);

    // --- ABRE O MODAL PERGUNTANDO IR AO CARRINHO OU CONTINUAR --- 
    setItemModal(linha);
    setModalAberto(true);
  }

  const meiaObrigatoriaPendente = modalidade === "MEIA" && !subtipoMeia;

  return (
    <div className="setor-card">
      {/* --- CABECALHO: NOME DO SETOR + ESTOQUE --- */}
      <div className="setor-card__head">
        <strong>{ingresso.setor}</strong>
        <span className="setor-card__estoque">{ingresso.quantidadeDisponivel} disponíveis</span>
      </div>

      {/* --- SELETOR DE MODALIDADE --- */}
      <div className="setor-card__modalidades">
        <button
          type="button"
          className={`setor-card__mod ${modalidade === "INTEIRA" ? "setor-card__mod--ativo" : ""}`}
          onClick={() => setModalidade("INTEIRA")}
        >
          <span>Inteira</span>
          <strong>{formatarMoeda(ingresso.preco)}</strong>
        </button>
        <button
          type="button"
          className={`setor-card__mod ${modalidade === "MEIA" ? "setor-card__mod--ativo" : ""}`}
          onClick={() => setModalidade("MEIA")}
        >
          <span>Meia</span>
          <strong>{formatarMoeda(ingresso.precoMeia)}</strong>
        </button>
        <button
          type="button"
          className={`setor-card__mod ${modalidade === "SOCIAL" ? "setor-card__mod--ativo" : ""}`}
          onClick={() => setModalidade("SOCIAL")}
        >
          <span>Social</span>
          <strong>{formatarMoeda(ingresso.precoSocial)}</strong>
        </button>
      </div>

      {/* --- SUBTIPO DA MEIA ENTRADA --- */}
      {modalidade === "MEIA" && (
        <div className="setor-card__meia">
          <label>Tipo de meia entrada</label>
          <SelectCustom
            value={subtipoMeia}
            onChange={(valor) => setSubtipoMeia(valor)}
            options={(modalidades?.meiaSubtipos || []).map((s) => ({ value: s.codigo, label: s.nome }))}
            placeholder="Selecione..."
          />
        </div>
      )}

      {/* --- AVISO DO INGRESSO SOCIAL --- */}
      {modalidade === "SOCIAL" && (
        <p className="setor-card__social-info">
          Leve 1kg de alimento não perecível no dia do evento. Valor: meia + {formatarMoeda(20)}.
        </p>
      )}

      {/* --- QUANTIDADE + ADICIONAR AO CARRINHO --- */}
      <div className="setor-card__acoes">
        <div className="setor-card__qtd">
          <button type="button" onClick={() => setQuantidade((q) => Math.max(1, q - 1))} aria-label="Diminuir">−</button>
          <span>{quantidade}</span>
          <button type="button" onClick={() => setQuantidade((q) => q + 1)} aria-label="Aumentar">+</button>
        </div>
        <button
          type="button"
          className="setor-card__add"
          onClick={adicionarAoCarrinho}
          disabled={meiaObrigatoriaPendente}
        >
          Adicionar
        </button>
      </div>

      {/* --- IR PARA O CARRINHO OU CONTINUAR COMPRANDO --- */}
      <ModalCarrinho
        aberto={modalAberto}
        item={itemModal}
        onIrCarrinho={() => navigate("/carrinho")}
        onContinuar={() => setModalAberto(false)}
      />
    </div>
  );
}

// --- PAGINA DE DETALHE DO EVENTO ---
export default function EventoDetalhePage() {
  const { id }                        = useParams();
  const [evento,      setEvento]      = useState(null);
  const [ingressos,   setIngressos]   = useState([]);
  const [modalidades, setModalidades] = useState(null);
  const [erro,        setErro]        = useState("");

  // --- CARREGA EVENTO, SETORES E O CATALOGO DE MODALIDADES ---
  useEffect(() => {
    Promise.all([
      eventoService.buscar(id),
      eventoService.listarIngressos(id),
      eventoService.listarModalidades()
    ])
      .then(([eventoData, ingressosData, modalidadesData]) => {
        setEvento(eventoData);
        setIngressos(ingressosData);
        setModalidades(modalidadesData);
      })
      .catch((e) => setErro(e.message));
  }, [id]);

  if (!evento) {
    return (
      <p style={{ textAlign: "center", padding: "3rem", color: "var(--muted)" }}>Carregando...</p>
    );
  }

  return (
    <section className="evento-detalhe">
      {evento.imagemUrl && (
        <img
          className="evento-detalhe__banner"
          src={evento.imagemUrl}
          alt={evento.titulo}
        />
      )}

      <div className="evento-detalhe__header">
        <span className="evento-detalhe__tag">{evento.categoria}</span>
        <h2 className="evento-detalhe__title">{evento.titulo}</h2>
        <div className="evento-detalhe__meta">
          <span className="evento-detalhe__meta-item">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0118 0z"/>
              <circle cx="12" cy="10" r="3"/>
            </svg>
            {evento.local}
          </span>
          <span className="evento-detalhe__meta-item">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <rect x="3" y="4" width="18" height="18" rx="2"/>
              <path d="M16 2v4M8 2v4M3 10h18"/>
            </svg>
            {new Date(evento.dataEvento).toLocaleDateString("pt-BR", { weekday: "long", day: "2-digit", month: "long", year: "numeric" })}
          </span>
        </div>
      </div>

      <p className="evento-detalhe__desc">{evento.descricao}</p>

      <div className="evento-detalhe__ingressos">
        <div className="evento-detalhe__ingressos-head">
          <h3>Ingressos disponíveis</h3>
          <Link to="/carrinho" className="evento-detalhe__ver-carrinho">Ver carrinho</Link>
        </div>
        {erro && <p className="error">{erro}</p>}

        <div className="evento-detalhe__setores">
          {ingressos.map((ingresso) => (
            <SetorCard
              key={ingresso.id}
              ingresso={ingresso}
              evento={evento}
              modalidades={modalidades}
            />
          ))}
        </div>
      </div>
    </section>
  );
}
