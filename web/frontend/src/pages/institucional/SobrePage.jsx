import "../../styles/institucional.css";

// --- PÁGINA SOBRE A LIVEEVENTS ---
export default function SobrePage() {
    return (
        <section className="institucional">
            <header className="institucional-cabecalho">
                <h1 className="institucional-titulo">Sobre a LiveEvents</h1>
                <p className="institucional-subtitulo">
                    Conectamos pessoas às melhores experiências ao vivo — shows, festivais,
                    teatro e muito mais.
                </p>
            </header>

            <div className="institucional-conteudo">
                <div className="institucional-secao">
                    <h2>Nossa missão</h2>
                    <p>
                        A LiveEvents nasceu para tornar a descoberta e a compra de ingressos
                        simples, segura e acessível. Acreditamos que cada evento é uma
                        oportunidade de criar memórias inesquecíveis, e nosso papel é
                        aproximar o público dos momentos que importam.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>O que oferecemos</h2>
                    <ul className="institucional-lista">
                        <li>Catálogo de eventos com busca e filtros inteligentes.</li>
                        <li>Seleção de setores e assentos de forma visual e intuitiva.</li>
                        <li>Checkout rápido e seguro, com histórico de pedidos.</li>
                        <li>Gestão dos seus ingressos em um só lugar.</li>
                    </ul>
                </div>

                <div className="institucional-secao">
                    <h2>Nossos valores</h2>
                    <div className="institucional-cards">
                        <div className="institucional-card">
                            <h3>Transparência</h3>
                            <p>Informações claras sobre preços, taxas e condições de compra.</p>
                        </div>
                        <div className="institucional-card">
                            <h3>Segurança</h3>
                            <p>Proteção dos seus dados e das suas transações em cada etapa.</p>
                        </div>
                        <div className="institucional-card">
                            <h3>Acessibilidade</h3>
                            <p>Uma plataforma pensada para todas as pessoas aproveitarem.</p>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
}
