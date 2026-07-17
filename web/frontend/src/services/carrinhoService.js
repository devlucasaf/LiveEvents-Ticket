// --- SERVICO DO CARRINHO: PERSISTE OS ITENS NO localStorage (FUNCIONA SEM LOGIN) ---

const CHAVE = "carrinho";

// --- LE OS ITENS ATUAIS DO CARRINHO A PARTIR DO localStorage ---
function lerItens() {
    try {
        const bruto = localStorage.getItem(CHAVE);
        const itens = bruto ? JSON.parse(bruto) : [];
        return Array.isArray(itens) ? itens : [];
    } catch {
        return [];
    }
}

// --- GRAVA OS ITENS E AVISA A APLICACAO QUE O CARRINHO MUDOU ---
function gravarItens(itens) {
    localStorage.setItem(CHAVE, JSON.stringify(itens));
    window.dispatchEvent(new Event("carrinho:atualizado"));
    return itens;
}

// --- GERA UMA CHAVE UNICA POR INGRESSO + MODALIDADE + SUBTIPO ---
function chaveItem(item) {
    return `${item.ingressoId}|${item.modalidade}|${item.subtipoMeia || ""}`;
}

export const carrinhoService = {
    listar() {
        return lerItens();
    },

    // --- SOMA A QUANTIDADE TOTAL DE INGRESSOS ---
    contar() {
        return lerItens().reduce((total, item) => total + item.quantidade, 0);
    },

    // --- SOMA O VALOR TOTAL DOS ITENS DO CARRINHO ---
    total() {
        return lerItens().reduce((total, item) => total + item.precoUnitario * item.quantidade, 0);
    },

    // --- ADICIONA UM ITEM E SE JA EXISTIR A MESMA COMBINACAO, SOMA A QUANTIDADE ---
    adicionar(item) {
        const itens = lerItens();
        const chave = chaveItem(item);
        const existente = itens.find((i) => chaveItem(i) === chave);

        if (existente) {
            existente.quantidade += item.quantidade;
        } else {
            itens.push({ ...item });
        }

        return gravarItens(itens);
    },

    // --- DEFINE A QUANTIDADE DE UMA LINHA ---
    atualizarQuantidade(chave, quantidade) {
        let itens = lerItens();
        if (quantidade < 1) {
            itens = itens.filter((i) => chaveItem(i) !== chave);
        } else {
            itens = itens.map((i) => (chaveItem(i) === chave ? { ...i, quantidade } : i));
        }
        return gravarItens(itens);
    },

    // --- REMOVE UMA LINHA ESPECIFICA DO CARRINHO ---
    remover(chave) {
        const itens = lerItens().filter((i) => chaveItem(i) !== chave);
        return gravarItens(itens);
    },

    // --- ESVAZIA TODO O CARRINHO ---
    limpar() {
        return gravarItens([]);
    },

    // --- EXPOE O GERADOR DE CHAVE PARA AS TELAS ---
    chaveDe(item) {
        return chaveItem(item);
    }
};
