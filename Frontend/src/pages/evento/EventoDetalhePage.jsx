import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { eventoService } from '../../services/eventoService';
import '../../styles/evento-detalhe.css';

export default function EventoDetalhePage() {
  const { id } = useParams();
  const [evento, setEvento] = useState(null);
  const [ingressos, setIngressos] = useState([]);
  const [erro, setErro] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    Promise.all([eventoService.buscar(id), eventoService.listarIngressos(id)])
      .then(([eventoData, ingressosData]) => {
        setEvento(eventoData);
        setIngressos(ingressosData);
      })
      .catch((e) => setErro(e.message));
  }, [id]);

  function selecionarIngresso(ingresso) {
    localStorage.setItem('checkoutItem', JSON.stringify({ ingressoId: ingresso.id, quantidade: 1 }));
    navigate('/pedido/checkout');
  }

  if (!evento) return <p style={{ textAlign: 'center', padding: '3rem', color: 'var(--muted)' }}>Carregando...</p>;

  return (
    <section className="evento-detalhe">
      {evento.imagemUrl && (
        <img className="evento-detalhe__banner" src={evento.imagemUrl} alt={evento.titulo} />
      )}

      <div className="evento-detalhe__header">
        <span className="evento-detalhe__tag">{evento.categoria}</span>
        <h2 className="evento-detalhe__title">{evento.titulo}</h2>
        <div className="evento-detalhe__meta">
          <span className="evento-detalhe__meta-item">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0118 0z"/><circle cx="12" cy="10" r="3"/>
            </svg>
            {evento.local}
          </span>
          <span className="evento-detalhe__meta-item">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <rect x="3" y="4" width="18" height="18" rx="2"/><path d="M16 2v4M8 2v4M3 10h18"/>
            </svg>
            {new Date(evento.dataEvento).toLocaleDateString('pt-BR', { weekday: 'long', day: '2-digit', month: 'long', year: 'numeric' })}
          </span>
        </div>
      </div>

      <p className="evento-detalhe__desc">{evento.descricao}</p>

      <div className="evento-detalhe__ingressos">
        <h3>Ingressos disponíveis</h3>
        {erro && <p className="error">{erro}</p>}
        <div className="evento-detalhe__ingresso-list">
          {ingressos.map((ingresso) => (
            <div key={ingresso.id} className="evento-detalhe__ingresso-item">
              <div className="evento-detalhe__ingresso-info">
                <strong>{ingresso.setor}</strong>
                <span>{ingresso.quantidadeDisponivel} disponíveis</span>
              </div>
              <span className="evento-detalhe__ingresso-price">R$ {ingresso.preco.toFixed(2)}</span>
              <button className="evento-detalhe__buy-btn" onClick={() => selecionarIngresso(ingresso)}>
                Comprar
              </button>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
