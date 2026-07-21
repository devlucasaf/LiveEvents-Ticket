import { useEffect, useState }  from "react";
import { useNavigate }          from "react-router-dom";
import { eventoService }        from "../../services/eventoService";
import CardEvento               from "../../components/CardEvento";
import "../../styles/busca.css";

export default function BuscaPage() {
  const [termo,       setTermo]       = useState("");
  const [eventos,     setEventos]     = useState([]);
  const [resultados,  setResultados]  = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    eventoService.listar().then(setEventos).catch(() => {});
  }, []);

  useEffect(() => {
    if (!termo.trim()) {
      setResultados([]);
      return;
    }
    const busca = termo.toLowerCase();
    const filtrados = eventos.filter(
      (e) =>
        e.titulo.toLowerCase().includes(busca) ||
        e.categoria.toLowerCase().includes(busca) ||
        e.local.toLowerCase().includes(busca)
    );
    setResultados(filtrados);
  }, [termo, eventos]);

  return (
    <section className="busca-page">
      <h2>Buscar eventos</h2>

      <div className="busca-page__input-wrapper">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <circle cx="11" cy="11" r="8"/>
          <path d="M21 21l-4.35-4.35"/>
        </svg>
        <input
          className="busca-page__input"
          placeholder="Pesquisar por nome, categoria ou local..."
          value={termo}
          onChange={(e) => setTermo(e.target.value)}
          autoFocus
        />
      </div>

      {termo.trim() && resultados.length === 0 && (
        <div className="busca-page__empty">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <circle cx="11" cy="11" r="8"/>
            <path d="M21 21l-4.35-4.35"/>
          </svg>
          <p>Nenhum evento encontrado para "{termo}"</p>
        </div>
      )}

      <div className="busca-page__results">
        {resultados.map((evento) => (
          <CardEvento key={evento.id} evento={evento} onSelecionar={(id) => navigate(`/evento/${id}`)} />
        ))}
      </div>
    </section>
  );
}
