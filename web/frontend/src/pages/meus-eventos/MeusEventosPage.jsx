import { useEffect, useState } from "react";
import { pedidoService } from "../../services/pedidoService";
import { eventoService } from "../../services/eventoService";
import "../../styles/meus-eventos.css";

const MOTIVOS_REEMBOLSO = [
  { codigo: "ARREPENDIMENTO_7_DIAS", rotulo: "Arrependimento (7 dias)" },
  { codigo: "IMPEDIMENTO_PESSOAL", rotulo: "Impedimento pessoal" },
  { codigo: "ALTERACAO_DE_PLANOS", rotulo: "Alteração de planos" },
  { codigo: "ERRO_NA_COMPRA", rotulo: "Erro na compra" },
  { codigo: "OUTRO", rotulo: "Outro motivo" }
];

export default function MeusEventosPage() {
  const [tab,     setTab]     = useState("upcoming");
  const [pedidos, setPedidos] = useState([]);
  const [eventos, setEventos] = useState([]);
  const [erro,    setErro]    = useState("");
  const [ingressoSelecionadoId, setIngressoSelecionadoId] = useState(null);
  const [mensagemReembolso, setMensagemReembolso] = useState("");
  const [processandoReembolsoId, setProcessandoReembolsoId] = useState(null);
  const [processandoDownloadId, setProcessandoDownloadId] = useState(null);
  const [processandoComprovanteId, setProcessandoComprovanteId] = useState(null);
  const [processandoCompartilhamentoId, setProcessandoCompartilhamentoId] = useState(null);
  const [copiado, setCopiado] = useState("");
  const [motivoCodigo, setMotivoCodigo] = useState("ARREPENDIMENTO_7_DIAS");
  const [motivoDetalhe, setMotivoDetalhe] = useState("");

  useEffect(() => {
    Promise.all([pedidoService.meusPedidos(), eventoService.listar()])
      .then(([pedidosData, eventosData]) => {
        setPedidos(pedidosData);
        setEventos(eventosData);
      })
      .catch((e) => setErro(e.message));
  }, []);

  const now = new Date();

  const meusEventos = pedidos
    .filter((p) => p.status === "PAGO" || p.status === "REEMBOLSADO")
    .map((p) => {
      const evento = eventos.find((e) => e.id === p.eventoId) || {};
      return { ...p, evento };
    });

  const upcoming = meusEventos.filter((p) => p.evento.dataEvento && new Date(p.evento.dataEvento) >= now);
  const past = meusEventos.filter((p) => p.evento.dataEvento && new Date(p.evento.dataEvento) < now);

  const displayed = tab === "upcoming" ? upcoming : past;
  const ingressoSelecionado = displayed.find((item) => item.id === ingressoSelecionadoId) || displayed[0] || null;

  useEffect(() => {
    if (!displayed.length) {
      setIngressoSelecionadoId(null);
      return;
    }

    const selecionadoExisteNaAba = displayed.some((item) => item.id === ingressoSelecionadoId);
    if (!selecionadoExisteNaAba) {
      setIngressoSelecionadoId(displayed[0].id);
    }
  }, [tab, displayed, ingressoSelecionadoId]);

  function formatarMoeda(valor) {
    return Number(valor || 0).toLocaleString("pt-BR", {
      style: "currency",
      currency: "BRL"
    });
  }

  function formatarData(data) {
    if (!data) {
      return "Data não informada";
    }

    return new Date(data).toLocaleDateString("pt-BR", {
      day: "2-digit",
      month: "long",
      year: "numeric"
    });
  }

  // --- FORMATA DATA/HORA PARA EXIBIR STATUS DE COMPARTILHAMENTO ---
  function formatarDataHora(data) {
    if (!data) {
      return "-";
    }

    return new Date(data).toLocaleString("pt-BR", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit"
    });
  }

  function classeStatus(item) {
    if (item.status === "REEMBOLSADO") {
      return "meus-eventos-page__item-status--reembolsado";
    }

    return new Date(item.evento.dataEvento) >= now
      ? "meus-eventos-page__item-status--upcoming"
      : "meus-eventos-page__item-status--past";
  }

  // --- MONTA ITENS DA LINHA DO TEMPO DE REEMBOLSO/ESTORNO ---
  function obterLinhaDoTempoReembolso(item) {
    return [
      {
        titulo: "Compra confirmada",
        data: item.dataCriacao,
        concluido: true
      },
      {
        titulo: "Reembolso solicitado",
        data: item.reembolsoSolicitadoEm,
        concluido: !!item.reembolsoSolicitadoEm
      },
      {
        titulo: "Reembolso aprovado",
        data: item.reembolsoAprovadoEm,
        concluido: !!item.reembolsoAprovadoEm
      },
      {
        titulo: "Estorno concluído",
        data: item.reembolsoEstornadoEm,
        concluido: !!item.reembolsoEstornadoEm
      }
    ];
  }

  function rotuloStatus(item) {
    if (item.status === "REEMBOLSADO") {
      return "Reembolsado";
    }

    return new Date(item.evento.dataEvento) >= now ? "Em breve" : "Realizado";
  }

  async function solicitarReembolso(pedidoId) {
    setMensagemReembolso("");
    setErro("");

    if (!motivoCodigo) {
      setErro("Selecione um motivo padronizado para solicitar o reembolso.");
      return;
    }

    if (motivoCodigo === "OUTRO" && !motivoDetalhe.trim()) {
      setErro("Detalhe o motivo quando selecionar OUTRO.");
      return;
    }

    setProcessandoReembolsoId(pedidoId);

    try {
      const resposta = await pedidoService.solicitarReembolso(pedidoId, {
        motivoCodigo,
        motivoDetalhe: motivoDetalhe.trim(),
        motivo: motivoCodigo === "OUTRO" ? motivoDetalhe.trim() : ""
      });

      setPedidos((prev) => prev.map((pedido) => {
        if (pedido.id !== pedidoId) {
          return pedido;
        }

        return {
          ...pedido,
          status: resposta.status,
          reembolsoAprovadoEm: resposta.aprovadoEm,
          reembolsoSolicitadoEm: resposta.solicitadoEm,
          reembolsoEstornadoEm: resposta.estornadoEm,
          reembolsoRegraAplicada: resposta.regraAplicada,
          reembolsoMotivoCodigo: resposta.motivoCodigo,
          reembolsoMotivo: resposta.motivoDetalhe
            ? `${resposta.motivoDescricao} Detalhe: ${resposta.motivoDetalhe}`
            : resposta.motivoDescricao,
          reembolsoElegivel: false,
          reembolsoMensagem: resposta.mensagem,
          pagamentoStatus: "REEMBOLSADO",
          reembolsoProtocoloEstorno: resposta.protocoloEstorno
        };
      }));

      setMensagemReembolso(resposta.mensagem || `Pedido #${pedidoId} reembolsado automaticamente.`);
      setMotivoCodigo("ARREPENDIMENTO_7_DIAS");
      setMotivoDetalhe("");
    } catch (e) {
      setErro(e.message);
    } finally {
      setProcessandoReembolsoId(null);
    }
  }

  // --- BAIXA O COMPROVANTE DE ESTORNO DO PEDIDO REEMBOLSADO ---
  async function baixarComprovanteEstornoPdf(pedidoId) {
    setErro("");
    setProcessandoComprovanteId(pedidoId);

    try {
      const { blob, nomeArquivo } = await pedidoService.baixarComprovanteEstornoPdf(pedidoId);

      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = nomeArquivo;
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch (e) {
      setErro(e.message);
    } finally {
      setProcessandoComprovanteId(null);
    }
  }

  async function baixarIngressoPdf(pedidoId) {
    setErro("");
    setProcessandoDownloadId(pedidoId);

    try {
      const { blob, nomeArquivo } = await pedidoService.baixarIngressoPdf(pedidoId);

      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = nomeArquivo;
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch (e) {
      setErro(e.message);
    } finally {
      setProcessandoDownloadId(null);
    }
  }

  // --- GERA LINK TEMPORARIO DE COMPARTILHAMENTO PARA O PEDIDO ---
  async function gerarLinkCompartilhamento(pedidoId) {
    setErro("");
    setCopiado("");
    setProcessandoCompartilhamentoId(pedidoId);

    try {
      const resposta = await pedidoService.gerarLinkCompartilhamento(pedidoId, {
        validadeMinutos: 60,
        maxAcessos: 3
      });

      setPedidos((prev) => prev.map((pedido) => {
        if (pedido.id !== pedidoId) {
          return pedido;
        }

        return {
          ...pedido,
          compartilhamentoToken: resposta.token,
          compartilhamentoExpiraEm: resposta.expiraEm,
          compartilhamentoRevogadoEm: resposta.revogadoEm,
          compartilhamentoMaxAcessos: resposta.maxAcessos,
          compartilhamentoAcessosRealizados: resposta.acessosRealizados,
          compartilhamentoAtivo: resposta.ativo,
          compartilhamentoUrl: resposta.urlCompartilhamento
        };
      }));
    } catch (e) {
      setErro(e.message);
    } finally {
      setProcessandoCompartilhamentoId(null);
    }
  }

  // --- REVOGA O LINK DE COMPARTILHAMENTO GERADO PARA O PEDIDO ---
  async function revogarLinkCompartilhamento(pedidoId) {
    setErro("");
    setCopiado("");
    setProcessandoCompartilhamentoId(pedidoId);

    try {
      const resposta = await pedidoService.revogarLinkCompartilhamento(pedidoId);

      setPedidos((prev) => prev.map((pedido) => {
        if (pedido.id !== pedidoId) {
          return pedido;
        }

        return {
          ...pedido,
          compartilhamentoToken: resposta.token,
          compartilhamentoExpiraEm: resposta.expiraEm,
          compartilhamentoRevogadoEm: resposta.revogadoEm,
          compartilhamentoMaxAcessos: resposta.maxAcessos,
          compartilhamentoAcessosRealizados: resposta.acessosRealizados,
          compartilhamentoAtivo: resposta.ativo,
          compartilhamentoUrl: resposta.urlCompartilhamento
        };
      }));
    } catch (e) {
      setErro(e.message);
    } finally {
      setProcessandoCompartilhamentoId(null);
    }
  }

  // --- COPIA LINK DE COMPARTILHAMENTO PARA AREA DE TRANSFERÊNCIA ---
  async function copiarLinkCompartilhamento(url) {
    try {
      await navigator.clipboard.writeText(url);
      setCopiado("Link copiado com sucesso.");
      setTimeout(() => setCopiado(""), 2200);
    } catch {
      setErro("Não foi possível copiar o link automaticamente.");
    }
  }

  return (
    <section className="meus-eventos-page">
      <h2>
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 002 2h0a2 2 0 002-2V7a2 2 0 00-2-2zm0 8a2 2 0 00-2 2v3a2 2 0 002 2h0a2 2 0 002-2v-3a2 2 0 00-2-2zm14-8a2 2 0 00-2 2v3a2 2 0 002 2h0a2 2 0 002-2V7a2 2 0 00-2-2zm0 8a2 2 0 00-2 2v3a2 2 0 002 2h0a2 2 0 002-2v-3a2 2 0 00-2-2z"/>
        </svg>
        Meus Ingressos
      </h2>

      {erro && <p className="error">{erro}</p>}

      <div className="meus-eventos-page__tabs">
        <button
          className={`meus-eventos-page__tab ${tab === "upcoming" ? "meus-eventos-page__tab--active" : ""}`}
          onClick={() => setTab("upcoming")}
        >
          Em breve
        </button>
        <button
          className={`meus-eventos-page__tab ${tab === "past" ? "meus-eventos-page__tab--active" : ""}`}
          onClick={() => setTab("past")}
        >
          Passados
        </button>
      </div>

      {displayed.length === 0 ? (
        <div className="meus-eventos-page__empty">
          <p>{tab === "upcoming" ? "Nenhum evento em breve." : "Nenhum evento passado."}</p>
        </div>
      ) : (
        <div className="meus-eventos-page__layout">
          <div className="meus-eventos-page__list" role="listbox" aria-label="Ingressos">
            {displayed.map((item) => {
              const selecionado = ingressoSelecionado?.id === item.id;

              return (
                <button
                  type="button"
                  key={item.id}
                  role="option"
                  aria-selected={selecionado}
                  className={`meus-eventos-page__item ${selecionado ? "meus-eventos-page__item--active" : ""}`}
                  onClick={() => {
                    setIngressoSelecionadoId(item.id);
                    setMensagemReembolso("");
                  }}
                >
                  {item.evento.imagemUrl && (
                    <img
                      className="meus-eventos-page__item-img"
                      src={item.evento.imagemUrl}
                      alt={item.evento.titulo}
                    />
                  )}
                  <div className="meus-eventos-page__item-info">
                    <span className="meus-eventos-page__item-title">
                      {item.evento.titulo || `Pedido #${item.id}`}
                    </span>

                    <span className="meus-eventos-page__item-detail">
                      {item.evento.local} • {item.evento.dataEvento && new Date(item.evento.dataEvento).toLocaleDateString("pt-BR")}
                    </span>

                    <span className="meus-eventos-page__item-detail">
                      {formatarMoeda(item.valorTotal)}
                    </span>

                    <span className={`meus-eventos-page__item-status ${classeStatus(item)}`}>
                      {rotuloStatus(item)}
                    </span>
                  </div>
                </button>
              );
            })}
          </div>

          {ingressoSelecionado && (
            <aside className="meus-eventos-page__details" aria-live="polite">
              <h3 className="meus-eventos-page__details-title">
                {ingressoSelecionado.evento.titulo || `Pedido #${ingressoSelecionado.id}`}
              </h3>

              <div className="meus-eventos-page__details-grid">
                <span>Pedido</span>
                <strong>#{ingressoSelecionado.id}</strong>

                <span>Data do evento</span>
                <strong>{formatarData(ingressoSelecionado.evento.dataEvento)}</strong>

                <span>Local</span>
                <strong>{ingressoSelecionado.evento.local || "Local não informado"}</strong>

                <span>Setor</span>
                <strong>{ingressoSelecionado.setor || "Não informado"}</strong>

                <span>Quantidade</span>
                <strong>{ingressoSelecionado.quantidade || 1}</strong>

                <span>Valor pago</span>
                <strong>{formatarMoeda(ingressoSelecionado.valorTotal)}</strong>
              </div>

              <div className="meus-eventos-page__refund">
                <div className="meus-eventos-page__actions">
                  <button
                    type="button"
                    className="meus-eventos-page__ticket-btn"
                    disabled={
                      ingressoSelecionado.status !== "PAGO" ||
                      processandoDownloadId === ingressoSelecionado.id
                    }
                    onClick={() => baixarIngressoPdf(ingressoSelecionado.id)}
                  >
                    {processandoDownloadId === ingressoSelecionado.id
                      ? "Gerando PDF..."
                      : "Baixar ingresso (PDF)"}
                  </button>
                </div>

                <div className="meus-eventos-page__share">
                  <button
                    type="button"
                    className="meus-eventos-page__share-btn"
                    disabled={
                      ingressoSelecionado.status !== "PAGO" ||
                      processandoCompartilhamentoId === ingressoSelecionado.id
                    }
                    onClick={() => gerarLinkCompartilhamento(ingressoSelecionado.id)}
                  >
                    {processandoCompartilhamentoId === ingressoSelecionado.id
                      ? "Gerando link..."
                      : "Gerar link compartilhável"}
                  </button>

                  {ingressoSelecionado.compartilhamentoUrl && (
                    <div className="meus-eventos-page__share-box">
                      <p>
                        <strong>Link:</strong>
                      </p>
                      <a
                        href={ingressoSelecionado.compartilhamentoUrl}
                        target="_blank"
                        rel="noreferrer"
                      >
                        {ingressoSelecionado.compartilhamentoUrl}
                      </a>
                      <p>
                        Expira em: {formatarDataHora(ingressoSelecionado.compartilhamentoExpiraEm)}
                      </p>
                      <p>
                        Acessos: {ingressoSelecionado.compartilhamentoAcessosRealizados || 0}/{ingressoSelecionado.compartilhamentoMaxAcessos || 0}
                      </p>

                      <div className="meus-eventos-page__share-actions">
                        <button
                          type="button"
                          className="meus-eventos-page__share-copy-btn"
                          onClick={() => copiarLinkCompartilhamento(ingressoSelecionado.compartilhamentoUrl)}
                        >
                          Copiar link
                        </button>

                        <button
                          type="button"
                          className="meus-eventos-page__share-revoke-btn"
                          disabled={
                            !!ingressoSelecionado.compartilhamentoRevogadoEm ||
                            processandoCompartilhamentoId === ingressoSelecionado.id
                          }
                          onClick={() => revogarLinkCompartilhamento(ingressoSelecionado.id)}
                        >
                          {ingressoSelecionado.compartilhamentoRevogadoEm ? "Link revogado" : "Revogar link"}
                        </button>
                      </div>

                      {ingressoSelecionado.compartilhamentoRevogadoEm && (
                        <p className="meus-eventos-page__share-revoked-at">
                          Revogado em: {formatarDataHora(ingressoSelecionado.compartilhamentoRevogadoEm)}
                        </p>
                      )}
                    </div>
                  )}

                  {copiado && <p className="meus-eventos-page__share-feedback">{copiado}</p>}
                </div>

                <p className="meus-eventos-page__refund-text">
                  {ingressoSelecionado.status === "REEMBOLSADO"
                    ? "Este pedido já foi reembolsado automaticamente."
                    : (ingressoSelecionado.reembolsoMensagem || "Você pode solicitar reembolso automático quando o pedido estiver elegível.")}
                </p>

                <button
                  type="button"
                  className="meus-eventos-page__refund-btn"
                  disabled={
                    ingressoSelecionado.status === "REEMBOLSADO" ||
                    !ingressoSelecionado.reembolsoElegivel ||
                    processandoReembolsoId === ingressoSelecionado.id
                  }
                  onClick={() => solicitarReembolso(ingressoSelecionado.id)}
                >
                  {processandoReembolsoId === ingressoSelecionado.id
                    ? "Processando..."
                    : ingressoSelecionado.status === "REEMBOLSADO"
                      ? "Reembolsado"
                      : "Solicitar reembolso"}
                </button>

                {ingressoSelecionado.status !== "REEMBOLSADO" && (
                  <div className="meus-eventos-page__refund-motivo-box">
                    <label htmlFor="motivo-padrao" className="meus-eventos-page__refund-label">
                      Motivo padronizado
                    </label>

                    <select
                      id="motivo-padrao"
                      className="meus-eventos-page__refund-select"
                      value={motivoCodigo}
                      onChange={(e) => setMotivoCodigo(e.target.value)}
                    >
                      {MOTIVOS_REEMBOLSO.map((motivo) => (
                        <option key={motivo.codigo} value={motivo.codigo}>
                          {motivo.rotulo}
                        </option>
                      ))}
                    </select>

                    <label htmlFor="motivo-detalhe" className="meus-eventos-page__refund-label">
                      Detalhamento {motivoCodigo === "OUTRO" ? "(obrigatório)" : "(opcional)"}
                    </label>
                    <textarea
                      id="motivo-detalhe"
                      className="meus-eventos-page__refund-textarea"
                      rows={3}
                      maxLength={300}
                      value={motivoDetalhe}
                      onChange={(e) => setMotivoDetalhe(e.target.value)}
                      placeholder="Descreva rapidamente o motivo do estorno"
                    />
                  </div>
                )}

                {mensagemReembolso && ingressoSelecionado.id === ingressoSelecionadoId && (
                  <p className="meus-eventos-page__refund-feedback">{mensagemReembolso}</p>
                )}

                {ingressoSelecionado.reembolsoRegraAplicada && (
                  <p className="meus-eventos-page__refund-rule">
                    Regra aplicada: {ingressoSelecionado.reembolsoRegraAplicada}
                  </p>
                )}

                {ingressoSelecionado.reembolsoProtocoloEstorno && (
                  <p className="meus-eventos-page__refund-rule">
                    Protocolo de estorno: {ingressoSelecionado.reembolsoProtocoloEstorno}
                  </p>
                )}

                {ingressoSelecionado.status === "REEMBOLSADO" && (
                  <button
                    type="button"
                    className="meus-eventos-page__ticket-btn"
                    disabled={processandoComprovanteId === ingressoSelecionado.id}
                    onClick={() => baixarComprovanteEstornoPdf(ingressoSelecionado.id)}
                  >
                    {processandoComprovanteId === ingressoSelecionado.id
                      ? "Gerando comprovante..."
                      : "Baixar comprovante de estorno"}
                  </button>
                )}

                <div className="meus-eventos-page__timeline">
                  <p className="meus-eventos-page__timeline-title">Linha do tempo do reembolso</p>

                  {obterLinhaDoTempoReembolso(ingressoSelecionado).map((etapa) => (
                    <div
                      key={etapa.titulo}
                      className={`meus-eventos-page__timeline-item ${etapa.concluido ? "meus-eventos-page__timeline-item--done" : ""}`}
                    >
                      <span className="meus-eventos-page__timeline-dot" aria-hidden="true" />
                      <div>
                        <strong>{etapa.titulo}</strong>
                        <p>{formatarDataHora(etapa.data)}</p>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </aside>
          )}
        </div>
      )}
    </section>
  );
}
