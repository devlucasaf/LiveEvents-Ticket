import { useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { authService } from '../../services/authService';
import '../../styles/auth.css';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro,  setErro]  = useState('');
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const retorno = searchParams.get('retorno') || '/';

  async function handleSubmit(event) {
    event.preventDefault();
    setErro('');
    try {
      await authService.login({ 
        email, 
        senha 
      });
      navigate(retorno);
      window.location.reload();
    } catch (e) {
      setErro(e.message);
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-page__card">
        <h2>Entrar</h2>
        <form className="auth-page__form" onSubmit={handleSubmit}>
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
            type="password"
            value={senha}
            onChange={(e) => setSenha(e.target.value)}
            placeholder="Senha"
            required
          />
          <button className="auth-page__btn" type="submit">Acessar</button>
        </form>
        {erro && <p className="error">{erro}</p>}
        <p className="auth-page__footer">
          Ainda não tem conta? <Link to="/auth/cadastro">Cadastre-se</Link>
        </p>
        <p className="auth-page__footer">
          <Link to="/auth/recuperar-senha">Esqueci minha senha</Link>
        </p>
      </div>
    </div>
  );
}
