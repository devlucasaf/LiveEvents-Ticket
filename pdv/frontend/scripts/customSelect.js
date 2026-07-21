(function () {
    "use strict";

    // --- TRANSFORMA UM SELECT NATIVO EM DROPDOWN CUSTOMIZADO ---
    function enhanceSelect(select) {
        if (select.dataset.enhanced === "true") {
            return;
        }
        select.dataset.enhanced = "true";

        const wrap = document.createElement("div");
        wrap.className = "cselect";
        select.parentNode.insertBefore(wrap, select);
        wrap.appendChild(select);
        select.classList.add("cselect__native");

        // --- BOTAO GATILHO ---
        const trigger = document.createElement("button");

        trigger.type = "button";
        trigger.className = "cselect__trigger";
        trigger.innerHTML =
            "<span class='cselect__label'></span><span class='cselect__arrow' aria-hidden='true'></span>";
            
        wrap.appendChild(trigger);

        const list = document.createElement("ul");

        list.className = "cselect__list oculto";
        list.setAttribute("role", "listbox");

        wrap.appendChild(list);

        const labelEl = trigger.querySelector(".cselect__label");

        // --- RECONSTROI A LISTA A PARTIR DAS <option> DO SELECT ---
        function buildOptions() {
            list.innerHTML = "";

            Array.from(select.options).forEach(function (opt) {
                const li = document.createElement("li");
                li.className = "cselect__option";
                li.setAttribute("role", "option");
                li.dataset.value = opt.value;
                li.textContent = opt.textContent;

                if (opt.disabled) {
                    li.classList.add("cselect__option--disabled");
                }

                li.addEventListener("click", function () {
                    if (opt.disabled) {
                        return;
                    }
                    setValue(opt.value);
                    fechar();
                });

                list.appendChild(li);
            });
        }

        // --- ESPELHA O ESTADO DO SELECT NATIVO NA UI CUSTOMIZADA ---
        function syncFromNative() {
            const selecionada = select.options[select.selectedIndex];
            labelEl.textContent = selecionada ? selecionada.textContent : "";

            Array.from(list.children).forEach(function (li) {
                li.classList.toggle("cselect__option--ativa", li.dataset.value === select.value);
            });

            trigger.classList.toggle("cselect__trigger--placeholder", !select.value);

            trigger.disabled = select.disabled;
            wrap.classList.toggle("cselect--disabled", select.disabled);
        }

        // --- APLICA O VALOR NO SELECT NATIVO E DISPARA "change" ---
        function setValue(value) {
            select.value = value;
            select.dispatchEvent(new Event("change", { bubbles: true }));
            syncFromNative();
        }

        // --- EXIBE A LISTA ---
        function abrir() {
            if (select.disabled) {
                return;
            }
            document.querySelectorAll(".cselect__list").forEach(function (l) {
                l.classList.add("oculto");
            });
            document.querySelectorAll(".cselect").forEach(function (c) {
                c.classList.remove("cselect--aberto");
            });
            list.classList.remove("oculto");
            wrap.classList.add("cselect--aberto");
        }

        // --- OCULTA A LISTA ---
        function fechar() {
            list.classList.add("oculto");
            wrap.classList.remove("cselect--aberto");
        }

        // --- ALTERNA ABRIR/FECHAR ---
        function toggle() {
            if (list.classList.contains("oculto")) {
                abrir();
            } else {
                fechar();
            }
        }

        // ABRE/FECHA AO CLICAR NO GATILHO
        trigger.addEventListener("click", function (e) {
            e.stopPropagation();
            toggle();
        });

        // FECHA AO CLICAR FORA DO COMPONENTE
        document.addEventListener("click", function (e) {
            if (!wrap.contains(e.target)) {
                fechar();
            }
        });

        // FECHA COM A TECLA ESC
        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape") {
                fechar();
            }
        });

        select.addEventListener("change", syncFromNative);

        // --- OBSERVA MUDANCAS ---
        const observer = new MutationObserver(function () {
            buildOptions();
            syncFromNative();
        });
        observer.observe(select, {
            childList: true,
            attributes: true,
            attributeFilter: ["disabled"]
        });

        if (select.form) {
            select.form.addEventListener("reset", function () {
                setTimeout(syncFromNative, 0);
            });
        }

        buildOptions();
        syncFromNative();
    }

    // --- APLICA O DROPDOWN CUSTOMIZADO A TODOS OS <select> DA PAGINA ---
    function init() {
        document.querySelectorAll("select").forEach(enhanceSelect);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
