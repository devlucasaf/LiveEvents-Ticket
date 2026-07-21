import { useEffect, useState }  from "react";
import { useNavigate }          from "react-router-dom";
import { eventoService }        from "../../services/eventoService";
import CardEvento               from "../../components/CardEvento";
import "../../styles/salvos.css";

export default function SalvosPage() {
  const [eventos, setEventos] = useState([]);
  const [salvos,  setSalvos]  = useState([]);
  const navigate = useNavigate();

  function loadSalvos() {
    const ids = JSON.parse(localStorage.getItem("eventosSalvos") || "[]");
    setSalvos(ids);
  }

  useEffect(() => {
    eventoService.listar().then(setEventos).catch(() => {});
    loadSalvos();

    function handleUpdate() { loadSalvos(); }
    window.addEventListener("salvosUpdated", handleUpdate);
    return () => window.removeEventListener("salvosUpdated", handleUpdate);
  }, []);

  const eventosSalvos = eventos.filter((e) => salvos.includes(e.id));

  return (
    <section className="salvos-page">
      <h2>
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <path d="M19 21l-7-5-7 5V5a2 2 0 012-2h10a2 2 0 012 2z"/>
        </svg>
        Eventos Salvos
      </h2>

      {eventosSalvos.length === 0 ? (
        <div className="salvos-page__empty">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M19 21l-7-5-7 5V5a2 2 0 012-2h10a2 2 0 012 2z"/>
          </svg>
          <p>Você ainda não salvou nenhum evento.</p>
        </div>
      ) : (
        <div className="salvos-page__grid">
          {eventosSalvos.map((evento) => (
            <CardEvento key={evento.id} evento={evento} onSelecionar={(id) => navigate(`/evento/${id}`)} />
          ))}
        </div>
      )}
    </section>
  );
}
