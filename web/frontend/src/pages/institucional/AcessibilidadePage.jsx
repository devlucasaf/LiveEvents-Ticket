import "../../styles/institucional.css";

// --- PAGINA DE ACESSIBILIDADE ---
export default function AcessibilidadePage() {
    return (
        <section className="institucional">
            <header className="institucional-cabecalho">
                <h1 className="institucional-titulo">Acessibilidade</h1>
                <p className="institucional-subtitulo">
                    Nosso compromisso é oferecer uma experiência utilizável por todas as pessoas.
                </p>
            </header>

            <div className="institucional-conteudo">
                <div className="institucional-secao">
                    <h2>Nosso compromisso</h2>
                    <p>
                        Trabalhamos para que a LiveEvents siga boas práticas de acessibilidade
                        digital, buscando conformidade com as diretrizes WCAG. Nosso objetivo é
                        que qualquer pessoa consiga navegar, encontrar eventos e comprar
                        ingressos sem barreiras.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>Recursos disponíveis</h2>
                    <ul className="institucional-lista">
                        <li>Navegação completa por teclado, com indicadores de foco visíveis.</li>
                        <li>Textos alternativos e rótulos descritivos em elementos interativos.</li>
                        <li>Contraste de cores adequado nos modos claro e escuro.</li>
                        <li>Estrutura semântica que favorece leitores de tela.</li>
                    </ul>
                </div>

                <div className="institucional-secao">
                    <h2>Fale conosco</h2>
                    <p>
                        Encontrou alguma barreira de acessibilidade? Sua opinião nos ajuda a
                        melhorar. Entre em contato pelo <a href="mailto:freitas.lucasaf@gmail.com">freitas.lucasaf@gmail.com</a> e
                        relate o problema.
                    </p>
                </div>
            </div>
        </section>
    );
}
