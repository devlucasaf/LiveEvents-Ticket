import "../../styles/institucional.css";

// --- PAGINA DE POLITICA DE PRIVACIDADE ---
export default function PoliticaPrivacidadePage() {
    return (
        <section className="institucional">
            <header className="institucional-cabecalho">
                <h1 className="institucional-titulo">Política de Privacidade</h1>
                <p className="institucional-subtitulo">
                    Como coletamos, usamos e protegemos os seus dados pessoais.
                </p>
            </header>

            <div className="institucional-conteudo">
                <div className="institucional-secao">
                    <h2>1. Dados que coletamos</h2>
                    <p>
                        Coletamos os dados fornecidos por você no cadastro e na compra, como
                        nome, e-mail, telefone e CPF, além de informações sobre os seus pedidos
                        e a sua navegação na plataforma.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>2. Como usamos seus dados</h2>
                    <ul className="institucional-lista">
                        <li>Processar compras e disponibilizar seus ingressos.</li>
                        <li>Enviar comunicações sobre pedidos e eventos.</li>
                        <li>Melhorar a experiência e a segurança da plataforma.</li>
                    </ul>
                </div>

                <div className="institucional-secao">
                    <h2>3. Compartilhamento</h2>
                    <p>
                        Seus dados podem ser compartilhados com parceiros estritamente
                        necessários para a prestação do serviço, como organizadores de eventos
                        e processadores de pagamento, sempre respeitando a finalidade da coleta.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>4. Seus direitos</h2>
                    <p>
                        Você pode solicitar acesso, correção ou exclusão dos seus dados, bem
                        como revogar consentimentos, conforme a legislação aplicável. Para isso,
                        entre em contato pelo <a href="mailto:freitas.lucasaf@gmail.com">freitas.lucasaf@gmail.com</a>.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>5. Segurança</h2>
                    <p>
                        Adotamos medidas técnicas e organizacionais para proteger os seus dados
                        contra acessos não autorizados, perda ou uso indevido.
                    </p>
                </div>

                <p className="institucional-nota">Última atualização: julho de 2026.</p>
            </div>
        </section>
    );
}
