import { useState, useRef, useEffect } from "react";
import "../styles/datepicker.css";

// --- RÓTULOS EXIBIDOS NO SELETOR DE MÊS ---
const MESES = [
    "Janeiro", 
    "Fevereiro", 
    "Março", 
    "Abril", 
    "Maio", 
    "Junho",
    "Julho", 
    "Agosto", 
    "Setembro", 
    "Outubro", 
    "Novembro", 
    "Dezembro"
];

// --- CABEÇALHO DA GRADE DO CALENDÁRIO ---
const DIAS_SEMANA = [
    "Dom", 
    "Seg", 
    "Ter", 
    "Qua", 
    "Qui", 
    "Sex", 
    "Sáb"
];

// --- RETORNA A QUANTIDADE DE DIAS DO MÊS/ANO INFORMADO ---
function getDiasNoMes(ano, mes) {
    return new Date(ano, mes + 1, 0).getDate();
}

// --- RETORNA O DIA DA SEMANA DO DIA 1 DO MÊS ---
function getPrimeiroDiaSemana(ano, mes) {
    return new Date(ano, mes, 1).getDay();
}

// --- CONVERTE DATE PARA DD/MM/AAAA ---
function formatarData(date) {
    const d = String(date.getDate()).padStart(2, "0");
    const m = String(date.getMonth() + 1).padStart(2, "0");
    const y = date.getFullYear();
    return `${d}/${m}/${y}`;
}

// --- INPUT DE DATA COM CALENDÁRIO E SELETORES CUSTOMIZADOS ---
export default function DatePicker({ value, onChange, placeholder = "Data de nascimento", required, inputClassName = "auth-page__input" }) {
    const [aberto,          setAberto]          = useState(false);
    const [anoAtual,        setAnoAtual]        = useState(new Date().getFullYear() - 20);
    const [mesAtual,        setMesAtual]        = useState(new Date().getMonth());
    const [seletorAberto,   setSeletorAberto]   = useState(null); 

    const ref = useRef(null);
    const refListaAno = useRef(null);

    // --- AO CLICAR FORA DO COMPONENTE, FECHA CALENDÁRIO E SELETORES ---
    useEffect(() => {
        function handleClickFora(e) {
            if (ref.current && !ref.current.contains(e.target)) {
                setAberto(false);
                setSeletorAberto(null);
            }
        }
        document.addEventListener("mousedown", handleClickFora);
        return () => document.removeEventListener("mousedown", handleClickFora);
    }, []);

    // --- AO ABRIR LISTA DE ANOS, ROLA AT\u00c9 O ANO SELECIONADO ---
    useEffect(() => {
        if (seletorAberto === "ano" && refListaAno.current) {
            const itemAtivo = refListaAno.current.querySelector(".datepicker__opcao--ativa");
            if (itemAtivo) {
                itemAtivo.scrollIntoView({ block: "center" });
            }
        }
    }, [seletorAberto]);

    // --- NAVEGA PARA O MÊS ANTERIOR AJUSTANDO O ANO QUANDO NECESSÁRIO ---
    function mesAnterior() {
        if (mesAtual === 0) {
            setMesAtual(11);
            setAnoAtual(anoAtual - 1);
        } else {
            setMesAtual(mesAtual - 1);
        }
    }

    // --- NAVEGA PARA O PRÓXIMO MÊS AJUSTANDO O ANO QUANDO NECESSÁRIO ---
    function mesSeguinte() {
        if (mesAtual === 11) {
            setMesAtual(0);
            setAnoAtual(anoAtual + 1);
        } else {
            setMesAtual(mesAtual + 1);
        }
    }

    // --- CONVERTE O DIA EM ISO, DISPARA ONCHANGE E FECHA O CALENDÁRIO ---
    function selecionarDia(dia) {
        const dataSelecionada = new Date(anoAtual, mesAtual, dia);
        const iso = `${anoAtual}-${String(mesAtual + 1).padStart(2, "0")}-${String(dia).padStart(2, "0")}`;
        onChange(iso);
        setAberto(false);
    }

    // --- MONTA A GRADE COM ESPAÇOS INICIAIS E BOTÕES DOS DIAS ---
    function renderDias() {
        const diasNoMes = getDiasNoMes(anoAtual, mesAtual);
        const primeiroDia = getPrimeiroDiaSemana(anoAtual, mesAtual);
        const celulas = [];

        // --- PREENCHE CÉLULAS VAZIAS ANTES DO PRIMEIRO DIA DO MÊS ---
        for (let i = 0; i < primeiroDia; i++) {
            celulas.push(<span key={`vazio-${i}`} className="datepicker__dia datepicker__dia--vazio" />);
        }

        // --- GERA UM BOTÃO PARA CADA DIA DO MÊS E MARCA O DIA SELECIONADO ---
        const hoje = new Date();
        for (let dia = 1; dia <= diasNoMes; dia++) {
            const ehSelecionado = value && value === `${anoAtual}-${String(mesAtual + 1).padStart(2, "0")}-${String(dia).padStart(2, "0")}`;
            celulas.push(
                <button
                    key={dia}
                    type="button"
                    className={`datepicker__dia ${ehSelecionado ? "datepicker__dia--selecionado" : ""}`}
                    onClick={() => selecionarDia(dia)}
                >
                {dia}
                </button>
            );
        }

        return celulas;
    }

    // --- FORMATA O VALUE ISO PARA EXIBIÇÃO NO INPUT ---
    const textoExibido = value
        ? formatarData(new Date(value + "T00:00:00"))
        : "";

    const anoCorrente = new Date().getFullYear();
    const anosDisponiveis = [];
    for (let a = anoCorrente; a >= anoCorrente - 100; a--) {
        anosDisponiveis.push(a);
    }

    // --- RENDERIZA INPUT, ÍCONE E POPUP DO CALENDÁRIO ---
    return (
        <div className="datepicker" ref={ref}>
            <input
                className={`${inputClassName} datepicker__input`}
                type="text"
                readOnly
                value={textoExibido}
                placeholder={placeholder}
                onClick={() => setAberto(!aberto)}
                required={required}
            />
            <svg className="datepicker__icone" onClick={() => setAberto(!aberto)} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
                <line 
                    x1="16" 
                    y1="2" 
                    x2="16" 
                    y2="6"
                />
                <line 
                    x1="8" 
                    y1="2" 
                    x2="8" 
                    y2="6"
                />
                <line 
                    x1="3" 
                    y1="10" 
                    x2="21" 
                    y2="10"
                />
            </svg>

            {aberto && (
                <div className="datepicker__dropdown">
                    <div className="datepicker__header">
                        {/* --- BOTÃO PARA VOLTAR UM MÊS --- */}
                        <button type="button" className="datepicker__nav" onClick={mesAnterior}>‹</button>
                        <div className="datepicker__seletores">
                            {/* --- SELETOR CUSTOMIZADO DE MÊS --- */}
                            <div className="datepicker__select-wrap">
                                <button
                                    type="button"
                                    className="datepicker__select"
                                    onClick={() => setSeletorAberto(seletorAberto === "mes" ? null : "mes")}
                                >
                                    {MESES[mesAtual]}
                                    <span className="datepicker__select-seta">▾</span>
                                </button>
                                {seletorAberto === "mes" && (
                                    <ul className="datepicker__opcoes">
                                        {MESES.map((m, i) => (
                                            <li
                                                key={m}
                                                className={`datepicker__opcao ${i === mesAtual ? "datepicker__opcao--ativa" : ""}`}
                                                onClick={() => { setMesAtual(i); setSeletorAberto(null); }}
                                            >
                                                {m}
                                            </li>
                                        ))}
                                    </ul>
                                )}
                            </div>

                            {/* --- SELETOR CUSTOMIZADO DE ANO --- */}
                            <div className="datepicker__select-wrap">
                                <button
                                    type="button"
                                    className="datepicker__select"
                                    onClick={() => setSeletorAberto(seletorAberto === "ano" ? null : "ano")}
                                >
                                    {anoAtual}
                                    <span className="datepicker__select-seta">▾</span>
                                </button>
                                {seletorAberto === "ano" && (
                                    <ul className="datepicker__opcoes" ref={refListaAno}>
                                        {anosDisponiveis.map((a) => (
                                            <li
                                                key={a}
                                                className={`datepicker__opcao ${a === anoAtual ? "datepicker__opcao--ativa" : ""}`}
                                                onClick={() => { setAnoAtual(a); setSeletorAberto(null); }}
                                            >
                                                {a}
                                            </li>
                                        ))}
                                    </ul>
                                )}
                            </div>
                        </div>
                        {/* --- BOTÃO PARA AVANÇAR UM MÊS --- */}
                        <button type="button" className="datepicker__nav" onClick={mesSeguinte}>›</button>
                    </div>

                    {/* --- CABEÇALHO DOS DIAS DA SEMANA --- */}
                    <div className="datepicker__semana">
                        {DIAS_SEMANA.map((d) => (
                            <span key={d} className="datepicker__dia-semana">{d}</span>
                        ))}
                    </div>

                    {/* --- GRADE DE DIAS DO MÊS --- */}
                    <div className="datepicker__grid">
                        {renderDias()}
                    </div>
                </div>
            )}
        </div>
    );
}
