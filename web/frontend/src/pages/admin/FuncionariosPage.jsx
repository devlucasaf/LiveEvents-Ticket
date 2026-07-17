import { useEffect, useState }  from "react";
import { Link }                 from "react-router-dom";
import { funcionarioService }   from "../../services/funcionarioService";
import SelectCustom             from "../../components/SelectCustom";
import "../../styles/admin.css";

const FORM_VAZIO = { 
    nome: "", 
    login: "", 
    senha: "", 
    role: "OPERADOR" 
};

// --- OPCOES DE CARGO DO FUNCIONARIO ---
const OPCOES_CARGO = [
    { value: "OPERADOR", label: "Operador" },
    { value: "ADMIN",    label: "Administrador" }
];

export default function FuncionariosPage() {
    const [funcionarios,    setFuncionarios]    = useState([]);
    const [form,            setForm]            = useState(FORM_VAZIO);
    const [editandoId,      setEditandoId]      = useState(null);
    const [erro,            setErro]            = useState("");
    const [carregando,      setCarregando]      = useState(true);
    const [salvando,        setSalvando]        = useState(false);

    async function carregar() {
        setCarregando(true);
        try {
            const dados = await funcionarioService.listar();
            setFuncionarios(dados);
            setErro("");
        } catch (e) {
            setErro(e.message);
        } finally {
            setCarregando(false);
        }
    }

    useEffect(() => {
        carregar();
    }, []);

    // --- ATUALIZA UM CAMPO DO FORMULARIO ---
    function atualizarCampo(campo, valor) {
        setForm((atual) => ({ ...atual, [campo]: valor }));
    }

    // --- ENTRA NO MODO DE EDICAO DE UM FUNCIONARIO ---
    function iniciarEdicao(funcionario) {
        setEditandoId(funcionario.id);
        setForm({ 
            nome: funcionario.nome, 
            login: funcionario.login, 
            senha: "", 
            role: funcionario.role 
        });
        setErro("");
        window.scrollTo({ top: 0, behavior: "smooth" });
    }

    // --- CANCELA A EDICAO E LIMPA O FORMULARIO ---
    function cancelarEdicao() {
        setEditandoId(null);
        setForm(FORM_VAZIO);
        setErro("");
    }

    // --- SALVA O FUNCIONARIO ---
    async function handleSubmit(e) {
        e.preventDefault();
        setErro("");
        setSalvando(true);

        try {
            if (editandoId) {
                await funcionarioService.atualizar(editandoId, {
                    nome: form.nome,
                    role: form.role,
                    senha: form.senha ? form.senha : null
                });
            } else {
                await funcionarioService.criar({
                    nome: form.nome,
                    login: form.login,
                    senha: form.senha,
                    role: form.role
                });
            }

            cancelarEdicao();
            await carregar();
        } catch (err) {
            setErro(err.message);
        } finally {
            setSalvando(false);
        }
    }

    // --- ATIVA/DESATIVA UM FUNCIONARIO ---
    async function alternarStatus(funcionario) {
        setErro("");
        try {
            await funcionarioService.alterarStatus(funcionario.id, !funcionario.ativo);
            await carregar();
        } catch (err) {
            setErro(err.message);
        }
    }

    return (
        <section className="admin-page">
            <div className="admin-page__header">
                <h2>Funcionários do PDV</h2>
                <Link to="/admin/dashboard" className="admin-page__criar-btn">
                    ← Voltar ao painel
                </Link>
            </div>

            {/* --- FORMULARIO DE CADASTRO/EDICAO --- */}
            <div className="admin-criar__card admin-func__card">
                <h3>{editandoId ? "Editar funcionário" : "Cadastrar novo funcionário"}</h3>

                <form className="admin-criar__form" onSubmit={handleSubmit}>
                    <div className="admin-criar__row">
                        <div className="admin-criar__field">
                            <label>Nome</label>
                            <input
                                value={form.nome}
                                onChange={(e) => atualizarCampo("nome", e.target.value)}
                                placeholder="Ex: João da Silva"
                                required
                            />
                        </div>

                        <div className="admin-criar__field">
                            <label>Login</label>
                            <input
                                value={form.login}
                                onChange={(e) => atualizarCampo("login", e.target.value)}
                                placeholder="Ex: joao.silva"
                                required
                                disabled={!!editandoId}
                            />
                        </div>
                    </div>

                    <div className="admin-criar__row">
                        <div className="admin-criar__field">
                            <label>{editandoId ? "Nova senha (deixe em branco para manter)" : "Senha"}</label>
                            <input
                                type="password"
                                value={form.senha}
                                onChange={(e) => atualizarCampo("senha", e.target.value)}
                                placeholder="Mínimo 6 caracteres"
                                required={!editandoId}
                            />
                        </div>

                        <div className="admin-criar__field">
                            <label>Cargo</label>
                            <SelectCustom
                                value={form.role}
                                onChange={(valor) => atualizarCampo("role", valor)}
                                options={OPCOES_CARGO}
                            />
                        </div>
                    </div>

                    {erro && <p className="error">{erro}</p>}

                    <div className="admin-func__form-actions">
                        {editandoId && (
                            <button type="button" className="admin-func__cancel-btn" onClick={cancelarEdicao}>
                                Cancelar
                            </button>
                        )}
                        <button className="admin-criar__submit admin-func__submit" type="submit" disabled={salvando}>
                            {salvando
                                ? "Salvando..."
                                : editandoId
                                    ? "Salvar alterações"
                                    : "Cadastrar funcionário"}
                        </button>
                    </div>
                </form>
            </div>

            {/* --- LISTA DE FUNCIONARIOS --- */}
            {carregando ? (
                <p>Carregando funcionários...</p>
            ) : funcionarios.length === 0 ? (
                <p>Nenhum funcionário cadastrado ainda.</p>
            ) : (
                <table className="admin-page__table">
                    <thead>
                        <tr>
                            <th>Nome</th>
                            <th>Login</th>
                            <th>Cargo</th>
                            <th>Status</th>
                            <th>Ações</th>
                        </tr>
                    </thead>
                    <tbody>
                        {funcionarios.map((f) => (
                            <tr key={f.id}>
                                <td>{f.nome}</td>
                                <td>{f.login}</td>
                                <td>
                                    <span className={`admin-func__badge admin-func__badge--${f.role.toLowerCase()}`}>
                                        {f.role === "ADMIN" ? "Administrador" : "Operador"}
                                    </span>
                                </td>
                                <td>
                                    <span className={`admin-func__status admin-func__status--${f.ativo ? "ativo" : "inativo"}`}>
                                        {f.ativo ? "Ativo" : "Inativo"}
                                    </span>
                                </td>
                                <td>
                                    <div className="admin-func__actions">
                                        <button
                                            className="admin-func__action-btn admin-func__action-btn--edit"
                                            onClick={() => iniciarEdicao(f)}
                                        >
                                            Editar
                                        </button>
                                        <button
                                            className={`admin-func__action-btn ${f.ativo ? "admin-func__action-btn--off" : "admin-func__action-btn--on"}`}
                                            onClick={() => alternarStatus(f)}
                                        >
                                            {f.ativo ? "Desativar" : "Ativar"}
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </section>
    );
}
