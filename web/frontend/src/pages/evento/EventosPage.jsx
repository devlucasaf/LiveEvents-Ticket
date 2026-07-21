import { useEffect, useState }  from "react";
import { useNavigate }          from "react-router-dom";
import CardEvento               from "../../components/CardEvento";
import { eventoService }        from "../../services/eventoService";
import "../../styles/eventos.css";

export default function EventosPage() {
  const [eventos, setEventos] = useState([]);
  const [erro,    setErro]    = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    eventoService
      .listar()
      .then(setEventos)
      .catch((e) => setErro(e.message));
  }, []);

  return (
    <section>
      <div className="eventos-page__hero">
        <h2>Encontre os melhores eventos</h2>
        <p>Shows, festivais, teatro e experiências únicas. Garanta seus ingressos com QR Code instantâneo.</p>
      </div>

      {erro && <p className="error">{erro}</p>}

      <div className="eventos-page__grid">
        {eventos.map((evento) => (
          <CardEvento key={evento.id} evento={evento} onSelecionar={(id) => navigate(`/evento/${id}`)} />
        ))}
      </div>
    </section>
  );
}
