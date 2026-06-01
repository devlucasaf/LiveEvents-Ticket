import { useState, useRef, useEffect } from "react";
import "../styles/datepicker.css";

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

const DIAS_SEMANA = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"];

function getDiasNoMes(ano, mes) {
    return new Date(ano, mes + 1, 0).getDate();
}

function getPrimeiroDiaSemana(ano, mes) {
    return new Date(ano, mes, 1).getDay();
}

function formatarData(date) {
    const d = String(date.getDate()).padStart(2, "0");
    const m = String(date.getMonth() + 1).padStart(2, "0");
    const y = date.getFullYear();
    return `${d}/${m}/${y}`;
}

export default function DatePicker({ value, onChange, placeholder = "Data de nascimento", required }) {
    const [aberto,      setAberto]      = useState(false);
    const [anoAtual,    setAnoAtual]    = useState(new Date().getFullYear() - 20);
    const [mesAtual,    setMesAtual]    = useState(new Date().getMonth());
    const ref = useRef(null);

    useEffect(() => {
        function handleClickFora(e) {
            if (ref.current && !ref.current.contains(e.target)) {
                setAberto(false);
            }
        }
        document.addEventListener("mousedown", handleClickFora);
        return () => document.removeEventListener("mousedown", handleClickFora);
    }, []);

    function mesAnterior() {
        if (mesAtual === 0) {
            setMesAtual(11);
            setAnoAtual(anoAtual - 1);
        } else {
            setMesAtual(mesAtual - 1);
        }
    }

    function mesSeguinte() {
        if (mesAtual === 11) {
            setMesAtual(0);
            setAnoAtual(anoAtual + 1);
        } else {
            setMesAtual(mesAtual + 1);
        }
    }

    function selecionarDia(dia) {
        const dataSelecionada = new Date(anoAtual, mesAtual, dia);
        const iso = `${anoAtual}-${String(mesAtual + 1).padStart(2, "0")}-${String(dia).padStart(2, "0")}`;
        onChange(iso);
        setAberto(false);
    }

    function renderDias() {
        const diasNoMes = getDiasNoMes(anoAtual, mesAtual);
        const primeiroDia = getPrimeiroDiaSemana(anoAtual, mesAtual);
        const celulas = [];

        for (let i = 0; i < primeiroDia; i++) {
            celulas.push(<span key={`vazio-${i}`} className="datepicker__dia datepicker__dia--vazio" />);
        }

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

    const textoExibido = value
        ? formatarData(new Date(value + "T00:00:00"))
        : "";

    return (
        <div className="datepicker" ref={ref}>
            <input
                className="auth-page__input datepicker__input"
                type="text"
                readOnly
                value={textoExibido}
                placeholder={placeholder}
                onClick={() => setAberto(!aberto)}
                required={required}
            />
            <svg className="datepicker__icone" onClick={() => setAberto(!aberto)} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
                <line x1="16" y1="2" x2="16" y2="6"/>
                <line x1="8" y1="2" x2="8" y2="6"/>
                <line x1="3" y1="10" x2="21" y2="10"/>
            </svg>

            {aberto && (
                <div className="datepicker__dropdown">
                    <div className="datepicker__header">
                        <button type="button" className="datepicker__nav" onClick={mesAnterior}>‹</button>
                        <span className="datepicker__titulo">{MESES[mesAtual]} {anoAtual}</span>
                        <button type="button" className="datepicker__nav" onClick={mesSeguinte}>›</button>
                    </div>

                    <div className="datepicker__semana">
                        {DIAS_SEMANA.map((d) => (
                            <span key={d} className="datepicker__dia-semana">{d}</span>
                        ))}
                    </div>

                    <div className="datepicker__grid">
                        {renderDias()}
                    </div>
                </div>
            )}
        </div>
    );
}
