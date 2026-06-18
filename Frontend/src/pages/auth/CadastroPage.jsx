import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { authService } from "../../services/authService";
import DatePicker from "../../components/DatePicker";
import "../../styles/auth.css";

function mascaraCpf(value) {
  return value
    .replace(/\D/g, "")
    .slice(0, 11)
    .replace(/(\d{3})(\d)/, "$1.$2")
    .replace(/(\d{3})(\d)/, "$1.$2")
    .replace(/(\d{3})(\d{1,2})$/, "$1-$2");
}

function mascaraTelefone(value) {
  return value
    .replace(/\D/g, "")
    .slice(0, 11)
    .replace(/(\d{2})(\d)/, "($1) $2")
    .replace(/(\d{5})(\d{1,4})$/, "$1-$2");
}

export default function CadastroPage() {
  const [nome,            setNome]            = useState("");
  const [sobrenome,       setSobrenome]       = useState("");
  const [email,           setEmail]           = useState("");
  const [cpf,             setCpf]             = useState("");
  const [telefone,        setTelefone]        = useState("");
  const [dataNascimento,  setDataNascimento]  = useState("");
  const [senha,           setSenha]           = useState("");
  const [mostrarSenha,    setMostrarSenha]    = useState(false);
  const [erro,            setErro]            = useState("");
  const [sucesso,         setSucesso]         = useState("");
  const navigate = useNavigate();

  async function handleSubmit(event) {
    event.preventDefault();
    setErro("");
    setSucesso("");
    try {
      await authService.cadastro({ 
        nome, 
        sobrenome, 
        email, 
        cpf, 
        telefone, 
        dataNascimento, 
        senha 
      });
      setSucesso("Cadastro realizado com sucesso!");
      setTimeout(() => navigate("/auth/login"), 1200);
    } catch (e) {
      setErro(e.message);
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-page__card">
        <h2>Criar conta</h2>
        <form className="auth-page__form" onSubmit={handleSubmit}>
          <input 
            className="auth-page__input" 
            value={nome} 
            onChange={(e) => setNome(e.target.value)}
            placeholder="Nome" 
            required 
          />
          <input 
            className="auth-page__input" 
            value={sobrenome} 
            onChange={(e) => setSobrenome(e.target.value)} 
            placeholder="Sobrenome" 
            required 
          />
          <input 
            className="auth-page__input" 
            value={email} 
            onChange={(e) => setEmail(e.target.value)} 
            placeholder="E-mail" 
            type="email" 
            required 
          />
          <input 
            className="auth-page__input" 
            value={cpf} 
            onChange={(e) => setCpf(mascaraCpf(e.target.value))} 
            placeholder="CPF" 
            maxLength={14}
            required 
          />
          <input 
            className="auth-page__input" 
            value={telefone} 
            onChange={(e) => setTelefone(mascaraTelefone(e.target.value))} 
            placeholder="Telefone" 
            maxLength={15}
            required 
          />
          <DatePicker
            value={dataNascimento}
            onChange={(val) => setDataNascimento(val)}
            placeholder="Data de nascimento"
            required
          />
          {/* --- CAMPO DE SENHA COM BOTÃO DE VISUALIZAR --- */}
          <div className="auth-page__senha-wrap">
            <input 
              className="auth-page__input auth-page__input--senha" 
              type={mostrarSenha ? "text" : "password"} 
              value={senha} 
              onChange={(e) => setSenha(e.target.value)} 
              placeholder="Senha" 
              required 
            />
            <button
              type="button"
              className="auth-page__toggle-senha"
              onClick={() => setMostrarSenha(!mostrarSenha)}
              aria-label={mostrarSenha ? "Ocultar senha" : "Mostrar senha"}
            >
              {mostrarSenha ? (
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M17.94 17.94A10.94 10.94 0 0 1 12 20c-7 0-11-8-11-8a19.77 19.77 0 0 1 5.06-5.94M9.9 4.24A10.94 10.94 0 0 1 12 4c7 0 11 8 11 8a19.77 19.77 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/>
                  <line x1="1" y1="1" x2="23" y2="23"/>
                </svg>
              ) : (
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                  <circle cx="12" cy="12" r="3"/>
                </svg>
              )}
            </button>
          </div>
          <button className="auth-page__btn" type="submit">Cadastrar</button>
        </form>
        {erro && <p className="error">{erro}</p>}
        {sucesso && <p className="success">{sucesso}</p>}
        <p className="auth-page__footer">
          Já possui conta? <Link to="/auth/login">Entrar</Link>
        </p>
      </div>
    </div>
  );
}
