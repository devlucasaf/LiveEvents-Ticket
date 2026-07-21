import { useEffect, useRef, useState } from "react";
import "../styles/select-custom.css";

// --- DROPDOWN CUSTOMIZADO ---
export default function SelectCustom({ value, onChange, options = [], placeholder = "Selecione...", disabled = false }) {
    const [aberto, setAberto] = useState(false);
    const raizRef = useRef(null);

    const selecionada = options.find((op) => op.value === value);

    // --- FECHA O DROPDOWN AO CLICAR FORA DELE ---
    useEffect(() => {
        function aoClicarFora(e) {
            if (raizRef.current && !raizRef.current.contains(e.target)) {
                setAberto(false);
            }
        }
        document.addEventListener("mousedown", aoClicarFora);
        return () => document.removeEventListener("mousedown", aoClicarFora);
    }, []);

    // --- FECHA COM A TECLA ESC ---
    useEffect(() => {
        function aoTeclar(e) {
            if (e.key === "Escape") {
                setAberto(false);
            }
        }
        document.addEventListener("keydown", aoTeclar);
        return () => document.removeEventListener("keydown", aoTeclar);
    }, []);

    // --- SELECIONA UMA OPCAO E FECHA O MENU ---
    function selecionar(op) {
        onChange(op.value);
        setAberto(false);
    }

    return (
        <div className={`select-custom ${disabled ? "select-custom--disabled" : ""}`} ref={raizRef}>
            {/* --- BOTAO QUE ABRE/FECHA O MENU --- */}
            <button
                type="button"
                className={`select-custom__trigger ${aberto ? "select-custom__trigger--aberto" : ""}`}
                onClick={() => !disabled && setAberto((a) => !a)}
                disabled={disabled}
            >
                <span className={selecionada ? "" : "select-custom__placeholder"}>
                    {selecionada ? selecionada.label : placeholder}
                </span>
                <svg
                    className={`select-custom__seta ${aberto ? "select-custom__seta--aberta" : ""}`}
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
                    strokeLinecap="round" strokeLinejoin="round"
                >
                    <polyline points="6 9 12 15 18 9" />
                </svg>
            </button>

            {/* --- LISTA DE OPÇÕES --- */}
            {aberto && (
                <ul className="select-custom__menu" role="listbox">
                    {options.map((op) => (
                        <li
                            key={op.value}
                            role="option"
                            aria-selected={op.value === value}
                            className={`select-custom__opcao ${op.value === value ? "select-custom__opcao--ativa" : ""}`}
                            onClick={() => selecionar(op)}
                        >
                            {op.label}
                            {op.value === value && (
                                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                                    <polyline points="20 6 9 17 4 12" />
                                </svg>
                            )}
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}
