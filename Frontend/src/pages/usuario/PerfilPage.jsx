import { useEffect, useState } from 'react';
import { pedidoService } from '../../services/pedidoService';
import '../../styles/perfil.css';

export default function PerfilPage() {
  const [pedidos, setPedidos] = useState([]);
  const [erro, setErro] = useState('');
  const usuario = JSON.parse(localStorage.getItem('usuario') || '{}');

  useEffect(() => {
    pedidoService
      .meusPedidos()
      .then(setPedidos)
      .catch((e) => setErro(e.message));
  }, []);

  return (
    <section className="perfil-page">
      <div className="perfil-page__header">
        <div className="perfil-page__avatar">
          {usuario.nome?.charAt(0).toUpperCase() || 'U'}
        </div>
        <div>
          <div className="perfil-page__name">{usuario.nome || 'Visitante'}</div>
          <div className="perfil-page__email">{usuario.email || 'Sem e-mail autenticado'}</div>
        </div>
      </div>

      <div className="perfil-page__section">
        <h3>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2"/>
            <circle cx="12" cy="7" r="4"/>
          </svg>
          Informações pessoais
        </h3>
        <div className="perfil-page__field">
          <span className="perfil-page__field-label">Nome</span>
          <span className="perfil-page__field-value">{usuario.nome || '—'}</span>
        </div>
        <div className="perfil-page__field">
          <span className="perfil-page__field-label">E-mail</span>
          <span className="perfil-page__field-value">{usuario.email || '—'}</span>
        </div>
        <div className="perfil-page__field">
          <span className="perfil-page__field-label">Telefone</span>
          <span className="perfil-page__field-value">{usuario.telefone || 'Não informado'}</span>
        </div>
        <div className="perfil-page__field">
          <span className="perfil-page__field-label">CPF</span>
          <span className="perfil-page__field-value">{usuario.cpf || 'Não informado'}</span>
        </div>
        <div className="perfil-page__field">
          <span className="perfil-page__field-label">Senha</span>
          <span className="perfil-page__field-value">••••••••</span>
        </div>
      </div>

      <div className="perfil-page__section">
        <h3>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <rect x="2" y="5" width="20" height="14" rx="2"/><path d="M2 10h20"/>
          </svg>
          Histórico de compras
        </h3>
        {erro && <p className="error">{erro}</p>}
        {pedidos.length === 0 ? (
          <p style={{ color: 'var(--muted)', fontSize: '0.85rem' }}>Nenhuma compra realizada.</p>
        ) : (
          <div className="perfil-page__pedidos">
            {pedidos.map((pedido) => (
              <div key={pedido.id} className="perfil-page__pedido-item">
                <span>Pedido #{pedido.id}</span>
                <span>{pedido.status}</span>
                <span style={{ fontWeight: 600 }}>R$ {pedido.valorTotal.toFixed(2)}</span>
              </div>
            ))}
          </div>
        )}
      </div>
    </section>
  );
}
