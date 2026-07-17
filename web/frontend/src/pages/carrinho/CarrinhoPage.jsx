import { Link, useNavigate } from "react-router-dom";
import { useCarrinho } from "../../context/CarrinhoContext";
import "../../styles/carrinho.css";

function formatarMoeda(valor) {
    return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

// --- LISTA OS INGRESSOS ESCOLHIDOS ---
export default function CarrinhoPage() {
    const navigate = useNavigate();
    const { itens, total, atualizarQuantidade, remover, chaveDe } = useCarrinho();

    const logado = !!localStorage.getItem("token");

    // --- ENCAMINHA PARA O CHECKOUT OU PARA O LOGIN (COM RETORNO AO CARRINHO) ---
    function finalizar() {
        if (!logado) {
            navigate("/auth/login?retorno=/carrinho");
            return;
        }
        navigate("/pedido/checkout");
    }

    // --- CONVIDA O CLIENTE A EXPLORAR EVENTOS ---
    if (itens.length === 0) {
        return (
            <div className="carrinho-page">
                <div className="carrinho-page__vazio">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
                        <circle cx="9" cy="21" r="1" /><circle cx="20" cy="21" r="1" />
                        <path d="M1 1h4l2.68 13.39a2 2 0 002 1.61h9.72a2 2 0 002-1.61L23 6H6" />
                    </svg>
                    <h2>Seu carrinho está vazio</h2>
                    <p>Explore os eventos e adicione seus ingressos preferidos.</p>
                    <Link to="/" className="carrinho-page__explorar">Ver eventos</Link>
                </div>
            </div>
        );
    }

    return (
        <div className="carrinho-page">
            <div className="carrinho-page__conteudo">
                {/* --- LISTA DE ITENS DO CARRINHO --- */}
                <div className="carrinho-page__lista">
                    <h2 className="carrinho-page__titulo">Meu carrinho</h2>

                    {itens.map((item) => {
                        // --- CHAVE UNICA DA LINHA (INGRESSO + MODALIDADE + SUBTIPO) ---
                        const chave = chaveDe(item);
                        return (
                            <div key={chave} className="carrinho-item">
                                {item.eventoImagem && (
                                    <img className="carrinho-item__img" src={item.eventoImagem} alt={item.eventoTitulo} />
                                )}

                                <div className="carrinho-item__info">
                                    <h3>{item.eventoTitulo}</h3>
                                    {item.eventoData && (
                                        <span className="carrinho-item__meta">
                                            {new Date(item.eventoData).toLocaleDateString("pt-BR", { day: "2-digit", month: "long", year: "numeric" })}
                                        </span>
                                    )}
                                    <span className="carrinho-item__setor">{item.setor}</span>
                                    <span className="carrinho-item__modalidade">{item.modalidadeLabel}</span>
                                </div>

                                {/* --- CONTROLE DE QUANTIDADE --- */}
                                <div className="carrinho-item__qtd">
                                    <button onClick={() => atualizarQuantidade(chave, item.quantidade - 1)} aria-label="Diminuir">−</button>
                                    <span>{item.quantidade}</span>
                                    <button onClick={() => atualizarQuantidade(chave, item.quantidade + 1)} aria-label="Aumentar">+</button>
                                </div>

                                {/* --- PRECO DA LINHA + REMOVER --- */}
                                <div className="carrinho-item__valores">
                                    <span className="carrinho-item__preco">{formatarMoeda(item.precoUnitario * item.quantidade)}</span>
                                    <span className="carrinho-item__unit">{formatarMoeda(item.precoUnitario)} un.</span>
                                    <button className="carrinho-item__remover" onClick={() => remover(chave)}>Remover</button>
                                </div>
                            </div>
                        );
                    })}
                </div>

                {/* --- RESUMO E FINALIZACAO --- */}
                <aside className="carrinho-resumo">
                    <h3>Resumo</h3>
                    <div className="carrinho-resumo__linha">
                        <span>Itens</span>
                        <span>{itens.reduce((t, i) => t + i.quantidade, 0)}</span>
                    </div>
                    <div className="carrinho-resumo__linha carrinho-resumo__total">
                        <strong>Total</strong>
                        <strong>{formatarMoeda(total)}</strong>
                    </div>
                    <button className="carrinho-resumo__btn" onClick={finalizar}>
                        {logado ? "Finalizar compra" : "Entrar para finalizar"}
                    </button>
                    <Link to="/" className="carrinho-resumo__continuar">Continuar comprando</Link>
                </aside>
            </div>
        </div>
    );
}
