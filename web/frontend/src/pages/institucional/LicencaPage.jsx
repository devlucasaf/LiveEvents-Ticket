import "../../styles/institucional.css";

// --- TEXTO ORIGINAL DA LICENCA MIT ---
const TEXTO_LICENCA = `MIT License

Copyright (c) 2026 Lucas Freitas

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.`;

// --- PÁGINA DE LICENÇA ---
export default function LicencaPage() {
    return (
        <section className="institucional">
            <header className="institucional-cabecalho">
                <h1 className="institucional-titulo">Licença</h1>
                <p className="institucional-subtitulo">
                    A LiveEvents-Ticket é um projeto de código aberto distribuído sob a Licença MIT.
                </p>
            </header>

            <div className="institucional-conteudo">
                <div className="institucional-secao">
                    <h2>O que a Licença MIT permite</h2>
                    <ul className="institucional-lista">
                        <li>Usar o software para qualquer finalidade, inclusive comercial.</li>
                        <li>Copiar, modificar e distribuir o código livremente.</li>
                        <li>Incorporar o projeto em softwares proprietários ou abertos.</li>
                    </ul>
                </div>

                <div className="institucional-secao">
                    <h2>Condição</h2>
                    <p>
                        O aviso de copyright e o texto desta licença devem ser incluídos em
                        todas as cópias ou partes substanciais do software. O software é
                        fornecido "como está", sem qualquer garantia.
                    </p>
                </div>

                <div className="institucional-secao">
                    <h2>Texto oficial</h2>
                    <pre className="institucional-licenca">{TEXTO_LICENCA}</pre>
                </div>

                <div className="institucional-secao">
                    <p>
                        Consulte também o arquivo original no{" "}
                        <a href="https://github.com/devlucasaf/LiveEvents-Ticket/blob/main/LICENSE" target="_blank" rel="noopener noreferrer">repositório no GitHub</a>.
                    </p>
                </div>
            </div>
        </section>
    );
}
