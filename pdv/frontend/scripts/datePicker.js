(function () {
    "use strict";

    // --- CONSTANTES DE CALENDARIO ---
    const MESES = [
        "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
        "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
    ];
    const DIAS_SEMANA = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"];

    // --- QUANTOS DIAS TEM O MES ---
    function getDiasNoMes(ano, mes) {
        return new Date(ano, mes + 1, 0).getDate();
    }

    // --- DIA DA SEMANA DO PRIMEIRO DIA DO MES ---
    function getPrimeiroDiaSemana(ano, mes) {
        return new Date(ano, mes, 1).getDay();
    }

    // --- CONVERTE ANO/MES/DIA EM ISO ---
    function paraIso(ano, mes, dia) {
        return `${ano}-${String(mes + 1).padStart(2, "0")}-${String(dia).padStart(2, "0")}`;
    }

    // --- FORMATA UMA DATA ISO PARA EXIBICAO ---
    function formatarExibicao(iso) {
        if (!iso) {
            return "";
        }
        const [ano, mes, dia] = iso.split("-");
        return `${dia}/${mes}/${ano}`;
    }

    // --- TRANSFORMA UM <input type="date"> EM CALENDARIO CUSTOMIZADO ---
    function enhanceDate(nativo) {
        if (nativo.dataset.enhanced === "true") {
            return;
        }
        nativo.dataset.enhanced = "true";

        const wrap = document.createElement("div");
        wrap.className = "datepicker";
        nativo.parentNode.insertBefore(wrap, nativo);
        wrap.appendChild(nativo);
        nativo.classList.add("datepicker__native");

        const campo = document.createElement("input");

        campo.type = "text";
        campo.readOnly = true;
        campo.className = "datepicker__input";
        campo.placeholder = nativo.getAttribute("placeholder") || "dd/mm/aaaa";

        wrap.appendChild(campo);

        const icone = document.createElementNS("http://www.w3.org/2000/svg", "svg");

        icone.setAttribute("class", "datepicker__icone");
        icone.setAttribute("viewBox", "0 0 24 24");
        icone.setAttribute("fill", "none");
        icone.setAttribute("stroke", "currentColor");
        icone.setAttribute("stroke-width", "2");
        icone.innerHTML =
            "<rect x='3' y='4' width='18' height='18' rx='2' ry='2'/>" +
            "<line x1='16' y1='2' x2='16' y2='6'/>" +
            "<line x1='8' y1='2' x2='8' y2='6'/>" +
            "<line x1='3' y1='10' x2='21' y2='10'/>";
        wrap.appendChild(icone);

        const dropdown = document.createElement("div");
        dropdown.className = "datepicker__dropdown oculto";
        wrap.appendChild(dropdown);

        const hoje = new Date();
        let anoAtual = hoje.getFullYear() - 20;
        let mesAtual = hoje.getMonth();
        let seletorAberto = null; 

        // --- SINCRONIZA O CAMPO VISIVEL COM O VALOR DO INPUT NATIVO ---
        function syncFromNative() {
            campo.value = formatarExibicao(nativo.value);
            if (nativo.value) {
                const [ano, mes] = nativo.value.split("-");
                anoAtual = parseInt(ano, 10);
                mesAtual = parseInt(mes, 10) - 1;
            }
        }

        // --- APLICA O VALOR NO INPUT NATIVO E DISPARA "change" ---
        function setValue(iso) {
            nativo.value = iso;
            nativo.dispatchEvent(new Event("change", { bubbles: true }));
            syncFromNative();
        }

        // --- NAVEGA PARA O MES ANTERIOR ---
        function mesAnterior() {
            if (mesAtual === 0) {
                mesAtual = 11;
                anoAtual -= 1;
            } else {
                mesAtual -= 1;
            }
            renderCalendario();
        }

        // --- NAVEGA PARA O MES SEGUINTE ---
        function mesSeguinte() {
            if (mesAtual === 11) {
                mesAtual = 0;
                anoAtual += 1;
            } else {
                mesAtual += 1;
            }
            renderCalendario();
        }

        // --- MONTA O CABECALHO COM SETORES DE MES E ANO ---
        function renderCabecalho() {
            const anoCorrente = new Date().getFullYear();

            let anosHtml = "";
            for (let a = anoCorrente; a >= anoCorrente - 100; a--) {
                const ativa = a === anoAtual ? " datepicker__opcao--ativa" : "";
                anosHtml += `<li class="datepicker__opcao${ativa}" data-ano="${a}">${a}</li>`;
            }

            // GERA A LISTA DE MESES
            let mesesHtml = "";
            MESES.forEach((m, i) => {
                const ativa = i === mesAtual ? " datepicker__opcao--ativa" : "";
                mesesHtml += `<li class="datepicker__opcao${ativa}" data-mes="${i}">${m}</li>`;
            });

            return `
                <div class="datepicker__header">
                    <button type="button" class="datepicker__nav" data-nav="ant">‹</button>
                    <div class="datepicker__seletores">
                        <div class="datepicker__select-wrap">
                            <button type="button" class="datepicker__select" data-sel="mes">
                                ${MESES[mesAtual]}<span class="datepicker__select-seta">▾</span>
                            </button>
                            <ul class="datepicker__opcoes${seletorAberto === "mes" ? "" : " oculto"}" data-lista="mes">${mesesHtml}</ul>
                        </div>
                        <div class="datepicker__select-wrap">
                            <button type="button" class="datepicker__select" data-sel="ano">
                                ${anoAtual}<span class="datepicker__select-seta">▾</span>
                            </button>
                            <ul class="datepicker__opcoes${seletorAberto === "ano" ? "" : " oculto"}" data-lista="ano">${anosHtml}</ul>
                        </div>
                    </div>
                    <button type="button" class="datepicker__nav" data-nav="prox">›</button>
                </div>
            `;
        }

        // --- MONTA A GRADE DE DIAS DO MES ---
        function renderDias() {
            const diasNoMes = getDiasNoMes(anoAtual, mesAtual);
            const primeiroDia = getPrimeiroDiaSemana(anoAtual, mesAtual);
            let html = "";

            for (let i = 0; i < primeiroDia; i++) {
                html += `<span class="datepicker__dia datepicker__dia--vazio"></span>`;
            }

            for (let dia = 1; dia <= diasNoMes; dia++) {
                const iso = paraIso(anoAtual, mesAtual, dia);
                const sel = nativo.value === iso ? " datepicker__dia--selecionado" : "";
                html += `<button type="button" class="datepicker__dia${sel}" data-dia="${dia}">${dia}</button>`;
            }

            return html;
        }

        // --- RENDERIZA O CALENDARIO COMPLETO NO DROPDOWN ---
        function renderCalendario() {
            const semanaHtml = DIAS_SEMANA
                .map((d) => `<span class="datepicker__dia-semana">${d}</span>`)
                .join("");

            dropdown.innerHTML =
                renderCabecalho() +
                `<div class="datepicker__semana">${semanaHtml}</div>` +
                `<div class="datepicker__grid">${renderDias()}</div>`;
        }

        // --- ABRE O CALENDARIO ---
        function abrir() {
            seletorAberto = null;
            syncFromNative();
            renderCalendario();
            document.querySelectorAll(".datepicker__dropdown").forEach((d) => d.classList.add("oculto"));
            dropdown.classList.remove("oculto");
        }

        // --- FECHA O CALENDARIO ---
        function fechar() {
            seletorAberto = null;
            dropdown.classList.add("oculto");
        }

        // --- ALTERNA ABRIR/FECHAR ---
        function toggle() {
            if (dropdown.classList.contains("oculto")) {
                abrir();
            } else {
                fechar();
            }
        }

        campo.addEventListener("click", (e) => { e.stopPropagation(); toggle(); });
        icone.addEventListener("click", (e) => { e.stopPropagation(); toggle(); });

        dropdown.addEventListener("click", (e) => {
            e.stopPropagation();
            const alvo = e.target.closest("[data-nav], [data-sel], [data-dia], [data-mes], [data-ano]");
            if (!alvo) {
                return;
            }

            if (alvo.dataset.nav === "ant") {
                mesAnterior();
                return;
            }

            if (alvo.dataset.nav === "prox") {
                mesSeguinte();
                return;
            }

            if (alvo.dataset.sel) {
                seletorAberto = seletorAberto === alvo.dataset.sel ? null : alvo.dataset.sel;
                renderCalendario();
                return;
            }

            if (alvo.dataset.mes !== undefined) {
                mesAtual = parseInt(alvo.dataset.mes, 10);
                seletorAberto = null;
                renderCalendario();
                return;
            }

            if (alvo.dataset.ano !== undefined) {
                anoAtual = parseInt(alvo.dataset.ano, 10);
                seletorAberto = null;
                renderCalendario();
                return;
            }

            if (alvo.dataset.dia !== undefined) {
                setValue(paraIso(anoAtual, mesAtual, parseInt(alvo.dataset.dia, 10)));
                fechar();
            }
        });

        document.addEventListener("click", (e) => {
            if (!wrap.contains(e.target)) {
                fechar();
            }
        });

        document.addEventListener("keydown", (e) => {
            if (e.key === "Escape") {
                fechar();
            }
        });

        if (nativo.form) {
            nativo.form.addEventListener("reset", () => setTimeout(syncFromNative, 0));
        }

        syncFromNative();
    }

    // --- APLICA O CALENDARIO CUSTOMIZADO A TODOS OS <input type="date"> ---
    function init() {
        document.querySelectorAll("input[type='date']").forEach(enhanceDate);
    }

    // --- EXPOE A API PARA APLICAR O CALENDARIO EM CAMPOS CRIADOS DINAMICAMENTE ---
    window.DatePicker = {
        aplicar(container) {
            const raiz = container || document;
            raiz.querySelectorAll("input[type='date']").forEach(enhanceDate);
        }
    };

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
