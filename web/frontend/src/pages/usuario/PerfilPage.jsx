import { useEffect, useState } from "react";
import { pedidoService } from "../../services/pedidoService";
import { authService } from "../../services/authService";
import "../../styles/perfil.css";

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

export default function PerfilPage() {
  const [pedidos,     setPedidos]     = useState([]);
  const [erro,        setErro]        = useState("");
  const [usuario,     setUsuario]     = useState(JSON.parse(localStorage.getItem("usuario") || "{}"));
  const [editando,    setEditando]    = useState(false);
  const [salvando,    setSalvando]    = useState(false);
  const [sucesso,     setSucesso]     = useState("");

  const [nome,        setNome]        = useState(usuario.nome || "");
  const [sobrenome,   setSobrenome]   = useState(usuario.sobrenome || "");
  const [email,       setEmail]       = useState(usuario.email || "");
  const [telefone,    setTelefone]    = useState(usuario.telefone || "");
  const [cpf,         setCpf]         = useState(usuario.cpf || "");
  const [senhaAtual,  setSenhaAtual]  = useState("");
  const [novaSenha,   setNovaSenha]   = useState("");

  useEffect(() => {
    pedidoService
      .meusPedidos()
      .then(setPedidos)
      .catch((e) => setErro(e.message));
  }, []);

  function iniciarEdicao() {
    setEditando(true);
    setSucesso("");
    setErro("");
  }

  function cancelarEdicao() {
    setEditando(false);
    setNome(usuario.nome || "");
    setSobrenome(usuario.sobrenome || "");
    setEmail(usuario.email || "");
    setTelefone(usuario.telefone || "");
    setCpf(usuario.cpf || "");
    setSenhaAtual("");
    setNovaSenha("");
    setErro("");
  }

  async function salvarAlteracoes() {
    setSalvando(true);
    setErro("");
    setSucesso("");
    try {
      const payload = { 
        nome, 
        sobrenome, 
        email, 
        telefone, 
        cpf 
      };
      if (novaSenha) {
        payload.senhaAtual = senhaAtual;
        payload.novaSenha = novaSenha;
      }
      const atualizado = await authService.atualizarPerfil(payload);
      setUsuario(atualizado);
      setEditando(false);
      setSenhaAtual("");
      setNovaSenha("");
      setSucesso("Dados atualizados com sucesso!");
    } catch (e) {
      setErro(e.message);
    } finally {
      setSalvando(false);
    }
  }

  return (
    <section className="perfil-page">
      <div className="perfil-page__header">
        <div className="perfil-page__avatar">
          {usuario.nome?.charAt(0).toUpperCase() || "U"}
        </div>
        
        <div>
          <div className="perfil-page__name">
            {usuario.nome || "Visitante"}{usuario.sobrenome ? ` ${usuario.sobrenome}` : ""}
          </div>
          <div className="perfil-page__email">{usuario.email || "Sem e-mail autenticado"}</div>
        </div>
      </div>

      <div className="perfil-page__section">
        <div className="perfil-page__section-header">
          <h3>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2"/>
              <circle cx="12" cy="7" r="4"/>
            </svg>
            Informações pessoais
          </h3>
          {!editando && (
            <button className="perfil-page__edit-btn" onClick={iniciarEdicao}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/>
                <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/>
              </svg>
              Editar
            </button>
          )}
        </div>

        {sucesso && <p className="perfil-page__sucesso">{sucesso}</p>}
        {erro && <p className="error">{erro}</p>}

        {!editando ? (
          <>
            <div className="perfil-page__field">
              <span className="perfil-page__field-label">Nome</span>
              <span className="perfil-page__field-value">{usuario.nome || "—"}</span>
            </div>

            <div className="perfil-page__field">
              <span className="perfil-page__field-label">Sobrenome</span>
              <span className="perfil-page__field-value">{usuario.sobrenome || "—"}</span>
            </div>

            <div className="perfil-page__field">
              <span className="perfil-page__field-label">E-mail</span>
              <span className="perfil-page__field-value">{usuario.email || "—"}</span>
            </div>

            <div className="perfil-page__field">
              <span className="perfil-page__field-label">Telefone</span>
              <span className="perfil-page__field-value">{usuario.telefone || "Não informado"}</span>
            </div>

            <div className="perfil-page__field">
              <span className="perfil-page__field-label">CPF</span>
              <span className="perfil-page__field-value">{usuario.cpf || "Não informado"}</span>
            </div>

            <div className="perfil-page__field">
              <span className="perfil-page__field-label">Senha</span>
              <span className="perfil-page__field-value">••••••••</span>
            </div>
          </>
        ) : (
          <div className="perfil-page__form">
            <div className="perfil-page__form-field">
              <label>Nome</label>
              <input 
                value={nome} 
                onChange={(e) => setNome(e.target.value)} 
                placeholder="Nome" 
              />
            </div>

            <div className="perfil-page__form-field">
              <label>Sobrenome</label>
              <input 
                value={sobrenome} 
                onChange={(e) => setSobrenome(e.target.value)} 
                placeholder="Sobrenome" 
              />
            </div>

            <div className="perfil-page__form-field">
              <label>E-mail</label>
              <input 
                type="email" 
                value={email} 
                onChange={(e) => setEmail(e.target.value)} 
                placeholder="E-mail" 
              />
            </div>

            <div className="perfil-page__form-field">
              <label>Telefone</label>
              <input  
                value={telefone} 
                onChange={(e) => setTelefone(mascaraTelefone(e.target.value))} 
                placeholder="(11) 91111-2222" 
                maxLength={15} 
              />
            </div>

            <div className="perfil-page__form-field">
              <label>CPF</label>
              <input 
                value={cpf} 
                onChange={(e) => setCpf(mascaraCpf(e.target.value))} 
                placeholder="111.111.111-01" 
                maxLength={14} 
              />
            </div>

            <div className="perfil-page__form-divider">Alterar senha (opcional)</div>
            <div className="perfil-page__form-field">
              <label>Senha atual</label>
              <input 
                type="password" 
                value={senhaAtual} 
                onChange={(e) => setSenhaAtual(e.target.value)} 
                placeholder="Digite a senha atual" 
              />
            </div>

            <div className="perfil-page__form-field">
              <label>Nova senha</label>
              <input 
                type="password" 
                value={novaSenha} 
                onChange={(e) => setNovaSenha(e.target.value)} 
                placeholder="Digite a nova senha" 
              />
            </div>

            <div className="perfil-page__form-actions">
              <button className="perfil-page__save-btn" onClick={salvarAlteracoes} disabled={salvando}>
                {salvando ? "Salvando..." : "Salvar alterações"}
              </button>
              <button className="perfil-page__cancel-btn" onClick={cancelarEdicao}>Cancelar</button>
            </div>
          </div>
        )}
      </div>

      <div className="perfil-page__section">
        <h3>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <rect x="2" y="5" width="20" height="14" rx="2"/>
            <path d="M2 10h20"/>
          </svg>
          Histórico de compras
        </h3>
        {pedidos.length === 0 ? (
          <p style={{ color: "var(--muted)", fontSize: "0.85rem" }}>Nenhuma compra realizada.</p>
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
