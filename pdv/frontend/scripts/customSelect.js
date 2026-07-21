(function () {
    "use strict";

    // --- TRANSFORMA UM SELECT NATIVO EM DROPDOWN CUSTOMIZADO ---
    function transformarSelect(select) {
        if (select.dataset.enhanced === "true") {
            return;
        }
        select.dataset.enhanced = "true";

        const involucro = document.createElement("div");
        involucro.className = "cselect";
        select.parentNode.insertBefore(involucro, select);
        involucro.appendChild(select);
        select.classList.add("cselect__native");

        const botaoGatilho = document.createElement("button");

        botaoGatilho.type = "button";
        botaoGatilho.className = "cselect__trigger";
        botaoGatilho.innerHTML =
            "<span class='cselect__label'></span><span class='cselect__arrow' aria-hidden='true'></span>";

        involucro.appendChild(botaoGatilho);

        const lista = document.createElement("ul");

        lista.className = "cselect__list oculto";
        lista.setAttribute("role", "listbox");

        involucro.appendChild(lista);

        const elementoRotulo = botaoGatilho.querySelector(".cselect__label");

        // --- RECONSTROI A LISTA A PARTIR DAS <option> DO SELECT ---
        function construirOpcoes() {
            lista.innerHTML = "";

            Array.from(select.options).forEach(function (opcao) {
                const item = document.createElement("li");

                item.className = "cselect__option";
                item.setAttribute("role", "option");
                item.dataset.value = opcao.value;
                item.textContent = opcao.textContent;

                // MARCA VISUALMENTE AS OPCOES DESABILITADAS
                if (opcao.disabled) {
                    item.classList.add("cselect__option--disabled");
                }

                item.addEventListener("click", function () {
                    if (opcao.disabled) {
                        return;
                    }
                    definirValor(opcao.value);
                    fechar();
                });

                lista.appendChild(item);
            });
        }

        // --- ESPELHA O ESTADO DO SELECT NATIVO NA UI CUSTOMIZADA ---
        function sincronizarComNativo() {
            const opcaoSelecionada = select.options[select.selectedIndex];
            elementoRotulo.textContent = opcaoSelecionada ? opcaoSelecionada.textContent : "";

            Array.from(lista.children).forEach(function (item) {
                item.classList.toggle("cselect__option--ativa", item.dataset.value === select.value);
            });

            botaoGatilho.classList.toggle("cselect__trigger--placeholder", !select.value);

            botaoGatilho.disabled = select.disabled;
            involucro.classList.toggle("cselect--disabled", select.disabled);
        }

        // --- APLICA O VALOR NO SELECT NATIVO E DISPARA "change" ---
        function definirValor(valor) {
            select.value = valor;
            select.dispatchEvent(new Event("change", { bubbles: true }));
            sincronizarComNativo();
        }

        // --- EXIBE A LISTA ---
        function abrir() {
            if (select.disabled) {
                return;
            }
            
            document.querySelectorAll(".cselect__list").forEach(function (outraLista) {
                outraLista.classList.add("oculto");
            });

            document.querySelectorAll(".cselect").forEach(function (outroComponente) {
                outroComponente.classList.remove("cselect--aberto");
            });

            lista.classList.remove("oculto");
            involucro.classList.add("cselect--aberto");
        }

        // --- OCULTA A LISTA ---
        function fechar() {
            lista.classList.add("oculto");
            involucro.classList.remove("cselect--aberto");
        }

        // --- ALTERNA ABRIR/FECHAR ---
        function alternar() {
            if (lista.classList.contains("oculto")) {
                abrir();
            } else {
                fechar();
            }
        }

        // ABRE/FECHA AO CLICAR NO GATILHO
        botaoGatilho.addEventListener("click", function (evento) {
            evento.stopPropagation();
            alternar();
        });

        // FECHA AO CLICAR FORA DO COMPONENTE
        document.addEventListener("click", function (evento) {
            if (!involucro.contains(evento.target)) {
                fechar();
            }
        });

        // FECHA COM A TECLA ESC
        document.addEventListener("keydown", function (evento) {
            if (evento.key === "Escape") {
                fechar();
            }
        });

        select.addEventListener("change", sincronizarComNativo);

        // --- OBSERVA MUDANCAS ---
        const observador = new MutationObserver(function () {
            construirOpcoes();
            sincronizarComNativo();
        });
        observador.observe(select, {
            childList: true,
            attributes: true,
            attributeFilter: ["disabled"]
        });

        // RESINCRONIZA APOS O RESET DO FORMULARIO
        if (select.form) {
            select.form.addEventListener("reset", function () {
                setTimeout(sincronizarComNativo, 0);
            });
        }

        construirOpcoes();
        sincronizarComNativo();
    }

    // --- APLICA O DROPDOWN CUSTOMIZADO A TODOS OS <select> DA PAGINA ---
    function inicializar() {
        document.querySelectorAll("select").forEach(transformarSelect);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", inicializar);
    } else {
        inicializar();
    }
})();
