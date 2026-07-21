import "../styles/modal-carrinho.css";

// --- CONFIRMA ITEM ADICIONADO E OFERECE IR AO CARRINHO OU CONTINUAR ---
export default function ModalCarrinho({ aberto, item, onIrCarrinho, onContinuar }) {
    if (!aberto) {
        return null;
    }

    return (
        <div className="modal-carrinho__overlay" onClick={onContinuar}>
            <div className="modal-carrinho" onClick={(e) => e.stopPropagation()}>
                <div className="modal-carrinho__icone">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                        <path d="M20 6L9 17l-5-5" />
                    </svg>
                </div>

                {/* --- TITULO --- */}
                <h3 className="modal-carrinho__titulo">Ingresso adicionado!</h3>

                {/* --- RESUMO DO ITEM ADICIONADO --- */}
                {item && (
                    <p className="modal-carrinho__resumo">
                        <strong>{item.setor}</strong> — {item.modalidadeLabel}
                        <span className="modal-carrinho__qtd">Qtd: {item.quantidade}</span>
                    </p>
                )}

                {/* --- ACOES: IR PARA O CARRINHO OU CONTINUAR --- */}
                <div className="modal-carrinho__acoes">
                    <button
                        type="button"
                        className="modal-carrinho__btn modal-carrinho__btn--secundario"
                        onClick={onContinuar}
                    >
                        Continuar comprando
                    </button>
                    <button
                        type="button"
                        className="modal-carrinho__btn modal-carrinho__btn--primario"
                        onClick={onIrCarrinho}
                    >
                        Ir para o carrinho
                    </button>
                </div>
            </div>
        </div>
    );
}
