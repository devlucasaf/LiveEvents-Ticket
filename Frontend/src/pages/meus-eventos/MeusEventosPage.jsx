import { useEffect, useState } from "react";
import { pedidoService } from "../../services/pedidoService";
import { eventoService } from "../../services/eventoService";
import "../../styles/meus-eventos.css";

export default function MeusEventosPage() {
  const [tab,     setTab]     = useState("upcoming");
  const [pedidos, setPedidos] = useState([]);
  const [eventos, setEventos] = useState([]);
  const [erro,    setErro]    = useState("");

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
    .filter((p) => p.status === "PAGO")
    .map((p) => {
      const evento = eventos.find((e) => e.id === p.eventoId) || {};
      return { ...p, evento };
    });

  const upcoming = meusEventos.filter((p) => p.evento.dataEvento && new Date(p.evento.dataEvento) >= now);
  const past = meusEventos.filter((p) => p.evento.dataEvento && new Date(p.evento.dataEvento) < now);

  const displayed = tab === "upcoming" ? upcoming : past;

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
        <div className="meus-eventos-page__list">
          {displayed.map((item) => (
            <div key={item.id} className="meus-eventos-page__item">
              {item.evento.imagemUrl && (
                <img 
                  className="meus-eventos-page__item-img" 
                  src={item.evento.imagemUrl} 
                  alt={item.evento.titulo} 
                />
              )}
              <div className="meus-eventos-page__item-info">
                <span className="meus-eventos-page__item-title">{item.evento.titulo || `Pedido #${item.id}`}</span>
                <span className="meus-eventos-page__item-detail">
                  {item.evento.local} • {item.evento.dataEvento && new Date(item.evento.dataEvento).toLocaleDateString("pt-BR")}
                </span>
                <span className="meus-eventos-page__item-detail">R$ {item.valorTotal?.toFixed(2)}</span>
                <span className={`meus-eventos-page__item-status meus-eventos-page__item-status--${tab === "upcoming" ? "upcoming" : "past"}`}>
                  {tab === "upcoming" ? "Em breve" : "Realizado"}
                </span>
              </div>
            </div>
          ))}
        </div>
      )}
    </section>
  );
}
