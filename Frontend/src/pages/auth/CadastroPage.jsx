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
          <input 
            className="auth-page__input" 
            type="password" 
            value={senha} 
            onChange={(e) => setSenha(e.target.value)} 
            placeholder="Senha" 
            required 
          />
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
