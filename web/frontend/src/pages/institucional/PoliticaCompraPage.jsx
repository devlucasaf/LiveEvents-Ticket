import "../../styles/institucional.css";

// --- PAGINA DE POLITICA DE COMPRA ---
export default function PoliticaCompraPage() {
    return (
        <section className="institucional">
            <header className="institucional-cabecalho">
                <h1 className="institucional-titulo">Política de Compra</h1>
                <p className="institucional-subtitulo">
                    Regras sobre compras, pagamentos, cancelamentos e reembolsos.
                </p>
            </header>

            <div className="institucional-conteudo">
                <div className="institucional-secao">
                    <h2>1. Processo de compra</h2>
                    <p>
                        A compra é concluída após a confirmação do pagamento. Enquanto o
                        pagamento não é aprovado, os ingressos não ficam garantidos e podem
                        voltar a ficar disponíveis para outros clientes.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>2. Pagamentos</h2>
                    <p>
                        As formas de pagamento disponíveis são exibidas na etapa de checkout.
                        Os valores podem incluir taxas de serviço, sempre apresentadas antes da
                        confirmação da compra.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>3. Cancelamento e reembolso</h2>
                    <ul className="institucional-lista">
                        <li>O direito de arrependimento segue a legislação vigente.</li>
                        <li>Cancelamentos de eventos são comunicados pelo organizador.</li>
                        <li>Prazos e condições de reembolso podem variar por evento.</li>
                    </ul>
                </div>

                <div className="institucional-secao">
                    <h2>4. Responsabilidade do evento</h2>
                    <p>
                        A realização, datas, local e programação são de responsabilidade do
                        organizador. Alterações e cancelamentos seguem as regras informadas
                        para cada evento.
                    </p>
                </div>

                <p className="institucional-nota">Última atualização: julho de 2026.</p>
            </div>
        </section>
    );
}
