import "../../styles/institucional.css";

// --- PÁGINA DE TERMOS DE USO ---
export default function TermosPage() {
    return (
        <section className="institucional">
            <header className="institucional-cabecalho">
                <h1 className="institucional-titulo">Termos de Uso</h1>
                <p className="institucional-subtitulo">
                    Condições gerais para utilização da plataforma LiveEvents.
                </p>
            </header>

            <div className="institucional-conteudo">
                <div className="institucional-secao">
                    <h2>1. Aceitação dos termos</h2>
                    <p>
                        Ao acessar e utilizar a LiveEvents, você concorda com estes Termos de
                        Uso. Caso não concorde com qualquer condição, recomendamos que não
                        utilize a plataforma.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>2. Cadastro e conta</h2>
                    <p>
                        Para comprar ingressos, é necessário criar uma conta com informações
                        verdadeiras e atualizadas. Você é responsável por manter a
                        confidencialidade das suas credenciais e por toda atividade realizada
                        na sua conta.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>3. Uso da plataforma</h2>
                    <ul className="institucional-lista">
                        <li>Utilizar a plataforma apenas para fins legais e pessoais.</li>
                        <li>Não praticar fraudes, revenda não autorizada ou uso indevido.</li>
                        <li>Respeitar os direitos de propriedade intelectual da plataforma.</li>
                    </ul>
                </div>

                <div className="institucional-secao">
                    <h2>4. Compras e ingressos</h2>
                    <p>
                        As compras estão sujeitas à disponibilidade e às condições descritas na
                        Política de Compra. Cada evento pode ter regras específicas definidas
                        pelo organizador.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>5. Alterações nos termos</h2>
                    <p>
                        Estes termos podem ser atualizados periodicamente. Recomendamos
                        revisá-los com frequência. O uso contínuo da plataforma após alterações
                        representa a aceitação das novas condições.
                    </p>
                </div>

                <p className="institucional-nota">Última atualização: julho de 2026.</p>
            </div>
        </section>
    );
}
