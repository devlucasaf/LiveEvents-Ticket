import { useState, useRef, useEffect } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import '../styles/navbar.css';

export default function Navbar() {
  const location = useLocation();
  const navigate = useNavigate();
  const [dropdownOpen,  setDropdownOpen]  = useState(false);
  const [theme,         setTheme]         = useState(() => localStorage.getItem('theme') || 'light');
  const dropdownRef = useRef(null);

  const usuario = JSON.parse(localStorage.getItem('usuario') || 'null');
  const loggedIn = !!usuario;

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
  }, [theme]);

  useEffect(() => {
    function handleClick(e) {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target)) {
        setDropdownOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClick);
    return () => document.removeEventListener('mousedown', handleClick);
  }, []);

  function toggleTheme() {
    setTheme(prev => prev === 'light' ? 'dark' : 'light');
  }

  function handleLogout() {
    localStorage.removeItem('token');
    localStorage.removeItem('usuario');
    setDropdownOpen(false);
    navigate('/');
    window.location.reload();
  }

  function isActive(path) {
    return location.pathname === path ? 'navbar__link--active' : '';
  }

  return (
    <nav className="navbar">
      {/* --- LOGO --- */}
      <Link to="/" className="navbar__logo">
        <svg viewBox="0 0 24 24" fill="currentColor">
          <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/>
        </svg>
        LiveEvents
      </Link>

      {/* --- LINKS CENTRAIS --- */}
      <div className="navbar__center">
        <Link to="/" className={`navbar__link ${isActive('/')}`}>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z"/>
          </svg>
          Eventos
        </Link>

        <Link to="/busca" className={`navbar__link ${isActive('/busca')}`}>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/>
          </svg>
          Buscar
        </Link>

        {loggedIn && (
          <>
            <Link to="/meus-eventos" className={`navbar__link ${isActive('/meus-eventos')}`}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 002 2h0a2 2 0 002-2V7a2 2 0 00-2-2zm0 8a2 2 0 00-2 2v3a2 2 0 002 2h0a2 2 0 002-2v-3a2 2 0 00-2-2zm14-8a2 2 0 00-2 2v3a2 2 0 002 2h0a2 2 0 002-2V7a2 2 0 00-2-2zm0 8a2 2 0 00-2 2v3a2 2 0 002 2h0a2 2 0 002-2v-3a2 2 0 00-2-2z"/>
              </svg>
              Meus Ingressos
            </Link>

            <Link to="/salvos" className={`navbar__link ${isActive('/salvos')}`}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M19 21l-7-5-7 5V5a2 2 0 012-2h10a2 2 0 012 2z"/>
              </svg>
              Salvos
            </Link>
          </>
        )}
      </div>

      {/* --- DIREITA --- */}
      <div className="navbar__right">
        <button className="navbar__theme-btn" onClick={toggleTheme} title="Alternar tema">
          {theme === 'light' ? (
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M21 12.79A9 9 0 1111.21 3 7 7 0 0021 12.79z"/>
            </svg>
          ) : (
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <circle cx="12" cy="12" r="5"/><path d="M12 1v2m0 18v2M4.22 4.22l1.42 1.42m12.72 12.72l1.42 1.42M1 12h2m18 0h2M4.22 19.78l1.42-1.42M18.36 5.64l1.42-1.42"/>
            </svg>
          )}
        </button>

        {loggedIn ? (
          <div ref={dropdownRef} style={{ position: 'relative' }}>
            <button
              className="navbar__profile-btn"
              onClick={() => setDropdownOpen(!dropdownOpen)}
              title="Meu perfil"
            >
              {usuario.nome?.charAt(0).toUpperCase() || 'U'}
            </button>

            {dropdownOpen && (
              <div className="navbar__dropdown">
                <Link to="/usuario/perfil" className="navbar__dropdown-item" onClick={() => setDropdownOpen(false)}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2"/>
                    <circle cx="12" cy="7" r="4"/>
                  </svg>
                  Meus dados
                </Link>

                <Link to="/meus-eventos" className="navbar__dropdown-item" onClick={() => setDropdownOpen(false)}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 002 2h0a2 2 0 002-2V7a2 2 0 00-2-2zm0 8a2 2 0 00-2 2v3a2 2 0 002 2h0a2 2 0 002-2v-3a2 2 0 00-2-2z"/>
                  </svg>
                  Meus ingressos
                </Link>

                <Link to="/salvos" className="navbar__dropdown-item" onClick={() => setDropdownOpen(false)}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M19 21l-7-5-7 5V5a2 2 0 012-2h10a2 2 0 012 2z"/>
                  </svg>
                  Eventos salvos
                </Link>

                {usuario.role === 'ADMIN' && (
                  <Link to="/admin/dashboard" className="navbar__dropdown-item" onClick={() => setDropdownOpen(false)}>
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                      <rect x="3" y="3" width="18" height="18" rx="2"/>
                      <path d="M9 9h6v6H9z"/>
                    </svg>
                    Painel Admin
                  </Link>
                )}
                <div className="navbar__dropdown-divider" />
                <button className="navbar__dropdown-item" onClick={handleLogout}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M9 21H5a2 2 0 01-2-2V5a2 2 0 012-2h4m7 14l5-5-5-5m5 5H9"/>
                  </svg>
                  Sair
                </button>
              </div>
            )}
          </div>
        ) : (
          <Link to="/auth/login" className="navbar__login-btn">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M15 3h4a2 2 0 012 2v14a2 2 0 01-2 2h-4m-5-4l5-5-5-5m5 5H3"/>
            </svg>
            Entrar
          </Link>
        )}
      </div>
    </nav>
  );
}
