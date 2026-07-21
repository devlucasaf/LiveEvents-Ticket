import { useState } from "react";
import "../../styles/auth.css";

export default function RecuperarSenhaPage() {
  const [email,     setEmail]     = useState("");
  const [mensagem,  setMensagem]  = useState("");

  function handleSubmit(event) {
    event.preventDefault();
    setMensagem(`Instruções de recuperação enviadas para ${email}.`);
  }

  return (
    <div className="auth-page">
      <div className="auth-page__card">
        <h2>Recuperar senha</h2>
        <form className="auth-page__form" onSubmit={handleSubmit}>
          <input className="auth-page__input" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="E-mail" type="email" required />
          <button className="auth-page__btn" type="submit">Enviar instruções</button>
        </form>
        {mensagem && <p className="success">{mensagem}</p>}
      </div>
    </div>
  );
}
