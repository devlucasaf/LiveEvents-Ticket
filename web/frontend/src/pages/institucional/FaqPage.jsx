import { useState } from "react";
import "../../styles/institucional.css";

// --- LISTA DE PERGUNTAS E RESPOSTAS ---
const PERGUNTAS = [
    {
        pergunta: "Como compro um ingresso?",
        resposta: "Escolha o evento desejado, selecione o setor ou assento, adicione ao carrinho e finalize a compra no checkout. Você receberá a confirmação e os ingressos na área Meus Ingressos.",
    },
    {
        pergunta: "Onde encontro meus ingressos após a compra?",
        resposta: "Todos os ingressos comprados ficam disponíveis na página Meus Ingressos, acessível pelo menu superior ou pelo rodapé, enquanto estiver logado na sua conta.",
    },
    {
        pergunta: "Posso cancelar ou transferir um ingresso?",
        resposta: "As regras de cancelamento e transferência seguem a Política de Compra e a política do organizador de cada evento. Consulte a Política de Compra para detalhes sobre prazos e reembolsos.",
    },
    {
        pergunta: "Quais formas de pagamento são aceitas?",
        resposta: "As formas de pagamento disponíveis são exibidas na etapa de checkout. Todos os dados são processados de forma segura, conforme nossa Política de Privacidade.",
    },
    {
        pergunta: "Não recebi a confirmação da compra, o que faço?",
        resposta: "Verifique a página Meus Ingressos e a caixa de spam do seu e-mail. Se o problema persistir, entre em contato pelo Suporte ao Fã que ajudaremos você.",
    },
];

// --- ICONE DE SETA ---
function IconeSeta() {
    return (
        <svg className="institucional-faq-icone" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
            <path d="m6 9 6 6 6-6" />
        </svg>
    );
}

// --- PAGINA DE PERGUNTAS FREQUENTES ---
export default function FaqPage() {
    const [aberto, setAberto] = useState(null);

    // --- ALTERNA A ABERTURA DE UM ITEM ---
    function alternar(indice) {
        setAberto((atual) => (atual === indice ? null : indice));
    }

    return (
        <section className="institucional">
            <header className="institucional-cabecalho">
                <h1 className="institucional-titulo">Perguntas Frequentes</h1>
                <p className="institucional-subtitulo">
                    Tire suas dúvidas sobre compras, ingressos e uso da plataforma.
                </p>
            </header>

            <div className="institucional-faq">
                {PERGUNTAS.map((item, indice) => (
                    <div key={indice} className={`institucional-faq-item ${aberto === indice ? "aberto" : ""}`}>
                        <button
                            type="button"
                            className="institucional-faq-pergunta"
                            aria-expanded={aberto === indice}
                            onClick={() => alternar(indice)}
                        >
                            {item.pergunta}
                            <IconeSeta />
                        </button>
                        {aberto === indice && (
                            <div className="institucional-faq-resposta">{item.resposta}</div>
                        )}
                    </div>
                ))}
            </div>
        </section>
    );
}
