import { useState, useRef, useEffect } from "react";
import "../styles/select.css";

// --- DROPDOWN CUSTOMIZADO ---
export default function Select({ value, onChange, options = [], placeholder = "Selecione...", required }) {
    const [aberto, setAberto] = useState(false);
    const ref = useRef(null);

    const itens = options.map((opt) =>
        typeof opt === "string" ? { value: opt, label: opt } : opt
    );

    const selecionado = itens.find((item) => item.value === value);

    // --- FECHA O DROPDOWN AO CLICAR FORA ---
    useEffect(() => {
        function handleClickFora(e) {
            if (ref.current && !ref.current.contains(e.target)) {
                setAberto(false);
            }
        }
        document.addEventListener("mousedown", handleClickFora);
        return () => document.removeEventListener("mousedown", handleClickFora);
    }, []);

    // --- APLICA O VALOR E FECHA A LISTA ---
    function selecionar(valor) {
        onChange(valor);
        setAberto(false);
    }

    return (
        <div className="select" ref={ref}>
            {/* --- BOTAO GATILHO QUE ABRE/FECHA A LISTA --- */}
            <button
                type="button"
                className={`select__trigger ${!selecionado ? "select__trigger--placeholder" : ""}`}
                onClick={() => setAberto((v) => !v)}
            >
                {selecionado ? selecionado.label : placeholder}
                <span className={`select__seta ${aberto ? "select__seta--aberta" : ""}`}>▾</span>
            </button>

            {/* --- INPUT OCULTO PARA MANTER A VALIDACAO NATIVA --- */}
            {required && (
                <input
                    className="select__hidden-input"
                    tabIndex={-1}
                    value={value || ""}
                    onChange={() => {}}
                    required
                />
            )}

            {/* --- LISTA DE OPCOES CUSTOMIZADA --- */}
            {aberto && (
                <ul className="select__opcoes">
                    {itens.map((item) => (
                        <li
                            key={item.value}
                            className={`select__opcao ${item.value === value ? "select__opcao--ativa" : ""}`}
                            onClick={() => selecionar(item.value)}
                        >
                            {item.label}
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}
