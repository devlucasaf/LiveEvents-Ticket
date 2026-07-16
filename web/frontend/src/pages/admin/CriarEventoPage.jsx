import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { eventoService } from "../../services/eventoService";
import DatePicker from "../../components/DatePicker";
import Select from "../../components/Select";
import "../../styles/admin.css";

// --- OPCOES DE CATEGORIA DO EVENTO ---
const CATEGORIAS = ["Show", "Festival", "Teatro", "Esporte", "Stand-up", "Conferência", "Outro"];

export default function CriarEventoPage() {
    const navigate = useNavigate();
    const [titulo,      setTitulo]      = useState("");
    const [categoria,   setCategoria]   = useState("");
    const [local,       setLocal]       = useState("");
    const [dataEvento,  setDataEvento]  = useState("");
    const [descricao,   setDescricao]   = useState("");
    const [imagemUrl,   setImagemUrl]   = useState("");
    const [ingressos,   setIngressos]   = useState([{ setor: "", preco: "", quantidade: "" }]);
    const [erro,        setErro]        = useState("");
    const [salvando,    setSalvando]    = useState(false);

    function adicionarSetor() {
        setIngressos([...ingressos, { setor: "", preco: "", quantidade: "" }]);
    }

    function removerSetor(index) {
        setIngressos(ingressos.filter((_, i) => i !== index));
    }

    function atualizarIngresso(index, campo, valor) {
        const novos = [...ingressos];
        novos[index][campo] = valor;
        setIngressos(novos);
    }

    async function handleSubmit(e) {
        e.preventDefault();
        setErro("");
        setSalvando(true);

        try {
            const evento = await eventoService.criar({
                titulo,
                categoria,
                local,
                dataEvento,
                descricao,
                imagemUrl
        });

        for (const ingresso of ingressos) {
            if (ingresso.setor && ingresso.preco && ingresso.quantidade) {
                await eventoService.criarIngresso({
                    eventoId: evento.id,
                    setor: ingresso.setor,
                    preco: parseFloat(ingresso.preco),
                    quantidadeDisponivel: parseInt(ingresso.quantidade)
                });
            }
        }

        navigate("/admin/dashboard");
        } catch (err) {
            setErro(err.message);
        } finally {
            setSalvando(false);
        }
    }

    return (
        <section className="admin-page">
            <div className="admin-criar__card">
                <h2>Criar novo evento</h2>

                <form className="admin-criar__form" onSubmit={handleSubmit}>
                    <div className="admin-criar__field">
                        <label>Título do evento</label>
                        <input
                            value={titulo}
                            onChange={(e) => setTitulo(e.target.value)}
                            placeholder="Ex: Rock in Rio 2026"
                            required
                        />
                    </div>

                    <div className="admin-criar__row">
                        <div className="admin-criar__field">
                        <label>Categoria</label>
                            <select value={categoria} onChange={(e) => setCategoria(e.target.value)} required>
                                <option value="">Selecione...</option>
                                <option value="Show">Show</option>
                                <option value="Festival">Festival</option>
                                <option value="Teatro">Teatro</option>
                                <option value="Esporte">Esporte</option>
                                <option value="Stand-up">Stand-up</option>
                                <option value="Conferência">Conferência</option>
                                <option value="Outro">Outro</option>
                            </select>
                        </div>
                        <div className="admin-criar__field">
                            <label>Data do evento</label>
                            <DatePicker
                                value={dataEvento}
                                onChange={setDataEvento}
                                placeholder="Selecione a data"
                                required
                            />
                        </div>
                    </div>

                    <div className="admin-criar__field">
                        <label>Local</label>
                        <input
                            value={local}
                            onChange={(e) => setLocal(e.target.value)}
                            placeholder="Ex: Maracanã, Rio de Janeiro"
                            required
                        />
                    </div>

                    <div className="admin-criar__field">
                        <label>Descrição</label>
                        <textarea
                            value={descricao}
                            onChange={(e) => setDescricao(e.target.value)}
                            placeholder="Descreva o evento..."
                            rows={4}
                            required
                        />
                    </div>

                    <div className="admin-criar__field">
                        <label>URL da imagem (banner)</label>
                        <input
                            value={imagemUrl}
                            onChange={(e) => setImagemUrl(e.target.value)}
                            placeholder="https://exemplo.com/imagem.jpg"
                        />
                    </div>

                    <div className="admin-criar__ingressos">
                        <div className="admin-criar__ingressos-header">
                            <h3>Ingressos</h3>
                            <button type="button" className="admin-criar__add-btn" onClick={adicionarSetor}>
                                + Adicionar setor
                            </button>
                        </div>

                        {ingressos.map((ingresso, index) => (
                        <div key={index} className="admin-criar__ingresso-item">
                            <div className="admin-criar__ingresso-fields">
                                <input
                                    placeholder="Setor (ex: Pista)"
                                    value={ingresso.setor}
                                    onChange={(e) => atualizarIngresso(index, "setor", e.target.value)}
                                    required
                                />
                                <input
                                    type="number"
                                    step="0.01"
                                    min="0"
                                    placeholder="Preço (R$)"
                                    value={ingresso.preco}
                                    onChange={(e) => atualizarIngresso(index, "preco", e.target.value)}
                                    required
                                />
                                <input
                                    type="number"
                                    min="1"
                                    placeholder="Quantidade"
                                    value={ingresso.quantidade}
                                    onChange={(e) => atualizarIngresso(index, "quantidade", e.target.value)}
                                    required
                                />
                            </div>
                            {ingressos.length > 1 && (
                            <button
                                type="button"
                                className="admin-criar__remove-btn"
                                onClick={() => removerSetor(index)}
                            >
                                ✕
                            </button>
                            )}
                        </div>
                        ))}
                    </div>

                    {erro && <p className="error">{erro}</p>}

                    <button className="admin-criar__submit" type="submit" disabled={salvando}>
                        {salvando ? "Criando evento..." : "Criar evento"}
                    </button>
                </form>
            </div>
        </section>
    );
}
