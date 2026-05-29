import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authService } from '../../services/authService';
import '../../styles/auth.css';

export default function CadastroPage() {
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState('');
  const [sucesso, setSucesso] = useState('');
  const navigate = useNavigate();

  async function handleSubmit(event) {
    event.preventDefault();
    setErro('');
    setSucesso('');
    try {
      await authService.cadastro({ nome, email, senha });
      setSucesso('Cadastro realizado com sucesso!');
      setTimeout(() => navigate('/auth/login'), 1200);
    } catch (e) {
      setErro(e.message);
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-page__card">
        <h2>Criar conta</h2>
        <form className="auth-page__form" onSubmit={handleSubmit}>
          <input className="auth-page__input" value={nome} onChange={(e) => setNome(e.target.value)} placeholder="Nome completo" required />
          <input className="auth-page__input" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="E-mail" type="email" required />
          <input className="auth-page__input" type="password" value={senha} onChange={(e) => setSenha(e.target.value)} placeholder="Senha" required />
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
