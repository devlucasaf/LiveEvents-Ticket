import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { carrinhoService } from "../services/carrinhoService";

const CarrinhoContext = createContext(null);

// --- PROVIDER QUE MANTEM OS ITENS SINCRONIZADOS COM O localStorage ---
export function CarrinhoProvider({ children }) {
    const [itens, setItens] = useState(() => carrinhoService.listar());

    // --- RECARREGA OS ITENS SEMPRE QUE O CARRINHO FOR ALTERADO ---
    useEffect(() => {
        function sincronizar() {
            setItens(carrinhoService.listar());
        }
        window.addEventListener("carrinho:atualizado", sincronizar);
        window.addEventListener("storage", sincronizar);
        return () => {
            window.removeEventListener("carrinho:atualizado", sincronizar);
            window.removeEventListener("storage", sincronizar);
        };
    }, []);

    // --- MONTA O VALOR EXPOSTO PELO CONTEXTO ---
    const valor = useMemo(() => ({
        itens,
        contador: itens.reduce((total, item) => total + item.quantidade, 0),
        total: itens.reduce((total, item) => total + item.precoUnitario * item.quantidade, 0),
        adicionar: (item) => setItens(carrinhoService.adicionar(item)),
        atualizarQuantidade: (chave, qtd) => setItens(carrinhoService.atualizarQuantidade(chave, qtd)),
        remover: (chave) => setItens(carrinhoService.remover(chave)),
        limpar: () => setItens(carrinhoService.limpar()),
        chaveDe: (item) => carrinhoService.chaveDe(item)
    }), [itens]);

    return <CarrinhoContext.Provider value={valor}>{children}</CarrinhoContext.Provider>;
}

// --- HOOK PARA CONSUMIR O CARRINHO EM QUALQUER COMPONENTE ---
export function useCarrinho() {
    const ctx = useContext(CarrinhoContext);
    if (!ctx) {
        throw new Error("useCarrinho deve ser usado dentro de CarrinhoProvider.");
    }
    return ctx;
}
