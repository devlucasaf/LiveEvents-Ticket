import { useState, useEffect } from "react";

export default function CardEvento({ evento, onSelecionar }) {
  const [saved, setSaved] = useState(false);

  // --- VERIFICAR SE O EVENTO JÁ ESTÁ SALVO NO LOCALSTORAGE ---
  useEffect(() => {
    const salvos = JSON.parse(localStorage.getItem("eventosSalvos") || "[]");
    setSaved(salvos.includes(evento.id));
  }, [evento.id]);

  // --- ALTERNAR SALVAR/REMOVER EVENTO DOS FAVORITOS ---
  function toggleSave(e) {
    e.stopPropagation();
    const salvos = JSON.parse(localStorage.getItem("eventosSalvos") || "[]");
    let updated;

    if (salvos.includes(evento.id)) {
      updated = salvos.filter(id => id !== evento.id);
    } else {
      updated = [...salvos, evento.id];
    }
    localStorage.setItem("eventosSalvos", JSON.stringify(updated));
    setSaved(!saved);
    window.dispatchEvent(new Event("salvosUpdated"));
  }

  // --- RENDERIZAÇÃO DO CARD ---
  return (
    <button className="card-evento" onClick={() => onSelecionar(evento.id)}>
      {evento.imagemUrl && (
        <img 
          className="card-evento__img" 
          src={evento.imagemUrl} 
          alt={evento.titulo} 
        />
      )}
      <div className="card-evento__body">
        {/* --- CATEGORIA E TÍTULO --- */}
        <span className="card-evento__tag">{evento.categoria}</span>
        <span className="card-evento__title">{evento.titulo}</span>

        {/* --- LOCALIZAÇÃO DO EVENTO --- */}
        <span className="card-evento__info">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0118 0z"/>
            <circle cx="12" cy="10" r="3"/>
          </svg>
          {evento.local}
        </span>

        {/* --- DATA DO EVENTO --- */}
        <span className="card-evento__info">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <rect x="3" y="4" width="18" height="18" rx="2"/>
            <path d="M16 2v4M8 2v4M3 10h18"/>
          </svg>
          {new Date(evento.dataEvento).toLocaleDateString("pt-BR", { day: "2-digit", month: "short", year: "numeric" })}
        </span>
        
        {/* --- PREÇO E BOTÃO DE SALVAR --- */}
        <div className="card-evento__actions">
          <span className="card-evento__price">
            {evento.precoMinimo ? `A partir de R$ ${evento.precoMinimo.toFixed(2)}` : "Ver ingressos"}
          </span>
          <button
            className={`card-evento__save-btn ${saved ? "card-evento__save-btn--saved" : ""}`}
            onClick={toggleSave}
            title={saved ? "Remover dos salvos" : "Salvar evento"}
          >
            <svg viewBox="0 0 24 24" fill={saved ? "currentColor" : "none"} stroke="currentColor" strokeWidth="2">
              <path d="M19 21l-7-5-7 5V5a2 2 0 012-2h10a2 2 0 012 2z"/>
            </svg>
          </button>
        </div>
      </div>
    </button>
  );
}
