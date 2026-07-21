import { Link } from "react-router-dom";
import "../../styles/institucional.css";

// --- PAGINA DE SUPORTE AO FA ---
export default function SuportePage() {
    return (
        <section className="institucional">
            <header className="institucional-cabecalho">
                <h1 className="institucional-titulo">Suporte ao Fã</h1>
                <p className="institucional-subtitulo">
                    Precisa de ajuda? Escolha o melhor canal para falar com a nossa equipe.
                </p>
            </header>

            <div className="institucional-conteudo">
                <div className="institucional-secao">
                    <h2>Canais de atendimento</h2>
                    <div className="institucional-cards">
                        <div className="institucional-card">
                            <h3>E-mail</h3>
                            <p>
                                Envie sua dúvida para{" "}
                                <a href="mailto:freitas.lucasaf@gmail.com">freitas.lucasaf@gmail.com</a> e
                                responderemos o mais rápido possível.
                            </p>
                        </div>
                        <div className="institucional-card">
                            <h3>Discord</h3>
                            <p>
                                Fale com a gente pelo{" "}
                                <a href="https://discord.com/users/1043680764140736612" target="_blank" rel="noopener noreferrer">Discord</a>.
                            </p>
                        </div>
                        <div className="institucional-card">
                            <h3>Instagram</h3>
                            <p>
                                Nos mande uma mensagem no{" "}
                                <a href="https://instagram.com/__.fr3it4s.__" target="_blank" rel="noopener noreferrer">Instagram</a>.
                            </p>
                        </div>
                    </div>
                </div>

                <div className="institucional-secao">
                    <h2>Antes de falar com a gente</h2>
                    <p>
                        Muitas dúvidas já estão respondidas na nossa{" "}
                        <Link to="/faq">Central de Perguntas Frequentes</Link>. Vale a pena dar
                        uma olhada — pode ser mais rápido!
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>Problemas com um ingresso?</h2>
                    <p>
                        Consulte seus pedidos em <Link to="/meus-eventos">Meus Ingressos</Link> e
                        confira as condições na <Link to="/politica-compra">Política de Compra</Link>.
                    </p>
                </div>
            </div>
        </section>
    );
}
