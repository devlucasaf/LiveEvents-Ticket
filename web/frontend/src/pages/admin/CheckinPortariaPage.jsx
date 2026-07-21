import { useEffect, useRef, useState } from "react";
import { checkinService } from "../../services/checkinService";
import "../../styles/checkin-portaria.css";

// --- TELA DE PORTARIA PARA VALIDAR INGRESSOS VIA QR CODE ---
export default function CheckinPortariaPage() {
    const usuario = JSON.parse(localStorage.getItem("usuario") || "{}");
    const [token, setToken] = useState("");
    const [erro, setErro] = useState("");
    const [resultado, setResultado] = useState(null);
    const [processando, setProcessando] = useState(false);
    const [cameraAtiva, setCameraAtiva] = useState(false);
    const [cameraSuportada, setCameraSuportada] = useState(true);

    const videoRef = useRef(null);
    const streamRef = useRef(null);
    const detectorRef = useRef(null);
    const frameRef = useRef(null);
    const bloqueioLeituraRef = useRef(false);

    // --- LIBERA RECURSOS DA CAMERA AO SAIR DA PAGINA ---
    useEffect(() => {
        return () => {
            pararCamera();
        };
    }, []);

    // --- VALIDA TOKEN MANUALMENTE OU VIA LEITURA DE CAMERA ---
    async function validarToken(tokenLido) {
        const valor = String(tokenLido || "").trim();
        if (!valor) {
            setErro("Informe ou escaneie um token válido.");
            return;
        }

        setErro("");
        setProcessando(true);

        try {
            const resposta = await checkinService.validar(valor);
            setResultado(resposta);
            setToken(valor);
        } catch (e) {
            setErro(e.message);
        } finally {
            setProcessando(false);
            bloqueioLeituraRef.current = false;
        }
    }

    // --- INICIA A LEITURA DE QR PELA CAMERA QUANDO DISPONIVEL ---
    async function iniciarCamera() {
        setErro("");

        if (!("BarcodeDetector" in window)) {
            setCameraSuportada(false);
            setErro("Leitura por câmera não suportada neste navegador. Use digitação manual do token.");
            return;
        }

        try {
            detectorRef.current = new window.BarcodeDetector({ formats: ["qr_code"] });
            const stream = await navigator.mediaDevices.getUserMedia({
                video: { facingMode: "environment" },
                audio: false
            });

            streamRef.current = stream;
            if (videoRef.current) {
                videoRef.current.srcObject = stream;
                await videoRef.current.play();
            }

            setCameraAtiva(true);
            loopLeitura();
        } catch {
            setErro("Não foi possível acessar a câmera para leitura do QR Code.");
            setCameraAtiva(false);
        }
    }

    // --- INTERROMPE CAMERA E LOOP DE LEITURA ---
    function pararCamera() {
        if (frameRef.current) {
            cancelAnimationFrame(frameRef.current);
            frameRef.current = null;
        }

        if (streamRef.current) {
            streamRef.current.getTracks().forEach((track) => track.stop());
            streamRef.current = null;
        }

        if (videoRef.current) {
            videoRef.current.srcObject = null;
        }

        setCameraAtiva(false);
    }

  // --- LE FRAMES DA CAMERA E VALIDA O PRIMEIRO QR ENCONTRADO ---
  async function loopLeitura() {
    if (!cameraAtiva || !videoRef.current || !detectorRef.current) {
      return;
    }

    try {
      const codigos = await detectorRef.current.detect(videoRef.current);
      if (codigos.length > 0 && !bloqueioLeituraRef.current) {
        bloqueioLeituraRef.current = true;
        const valor = codigos[0].rawValue || "";
        await validarToken(valor);
      }
    } catch {
      // --- IGNORA FALHAS INTERMITENTES DE LEITURA E SEGUE NO LOOP ---
    }

    frameRef.current = requestAnimationFrame(loopLeitura);
  }

  if (usuario.role !== "ADMIN") {
    return (
      <section className="checkin-page">
        <h2>Portaria - Check-in</h2>
        <p className="error">Acesso restrito ao administrador.</p>
      </section>
    );
  }

  return (
    <section className="checkin-page">
      <h2>Portaria - Check-in em tempo real</h2>
      <p className="checkin-page__subtitle">
        Escaneie o QR Code do ingresso ou informe o token manualmente.
      </p>

      <div className="checkin-page__card">
        <div className="checkin-page__actions">
          {!cameraAtiva ? (
            <button className="checkin-page__btn" onClick={iniciarCamera}>
              Iniciar câmera
            </button>
          ) : (
            <button className="checkin-page__btn checkin-page__btn--ghost" onClick={pararCamera}>
              Parar câmera
            </button>
          )}
        </div>

        {cameraSuportada && (
          <div className="checkin-page__camera">
            <video ref={videoRef} muted playsInline />
          </div>
        )}

        <div className="checkin-page__manual">
          <label>Token do ingresso</label>
          <input
            value={token}
            onChange={(e) => setToken(e.target.value)}
            placeholder="CHK-..."
          />
          <button
            className="checkin-page__btn"
            disabled={processando}
            onClick={() => validarToken(token)}
          >
            {processando ? "Validando..." : "Validar check-in"}
          </button>
        </div>

        {erro && <p className="error">{erro}</p>}

        {resultado && (
          <div className={`checkin-page__result ${resultado.permitido ? "checkin-page__result--ok" : "checkin-page__result--deny"}`}>
            <h3>{resultado.permitido ? "Entrada autorizada" : "Entrada negada"}</h3>
            <p>{resultado.mensagem}</p>
            <p><strong>Pedido:</strong> {resultado.pedidoId || "-"}</p>
            <p><strong>Evento:</strong> {resultado.eventoTitulo || "-"}</p>
            <p><strong>Setor:</strong> {resultado.setor || "-"}</p>
            <p><strong>Usos:</strong> {resultado.usosRealizados}/{resultado.quantidadeTotal}</p>
            <p><strong>Restantes:</strong> {resultado.usosRestantes}</p>
          </div>
        )}
      </div>
    </section>
  );
}
