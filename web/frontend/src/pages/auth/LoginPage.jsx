import { useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { authService } from '../../services/authService';
import '../../styles/auth.css';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro,  setErro]  = useState('');
  // --- CONTROLA SE A SENHA ESTA VISIVEL OU OCULTA ---
  const [mostrarSenha, setMostrarSenha] = useState(false);
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
          {/* --- CAMPO DE SENHA COM BOTAO DE OLHO PARA MOSTRAR/OCULTAR --- */}
          <div className="auth-page__senha-wrap">
            <input
              className="auth-page__input auth-page__input--senha"
              type={mostrarSenha ? 'text' : 'password'}
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              placeholder="Senha"
              required
            />
            {/* --- BOTAO ALTERNA A VISIBILIDADE DA SENHA AO CLICAR --- */}
            <button
              type="button"
              className="auth-page__toggle-senha"
              onClick={() => setMostrarSenha((v) => !v)}
              aria-label={mostrarSenha ? 'Ocultar senha' : 'Mostrar senha'}
              title={mostrarSenha ? 'Ocultar senha' : 'Mostrar senha'}
            >
              {mostrarSenha ? (
                // --- ICONE DE OLHO CORTADO ---
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" />
                  <line x1="1" y1="1" x2="23" y2="23" />
                </svg>
              ) : (
                // --- ICONE DE OLHO ABERTO ---
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
                  <circle cx="12" cy="12" r="3" />
                </svg>
              )}
            </button>
          </div>
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
