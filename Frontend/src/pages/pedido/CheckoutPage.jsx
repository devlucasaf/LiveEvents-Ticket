import { useState } from 'react';
import { pedidoService } from '../../services/pedidoService';
import '../../styles/checkout.css';

export default function CheckoutPage() {
    const [tipo, setTipo] = useState('PIX');
    const [numeroCartao, setNumeroCartao] = useState('');
    const [resultado, setResultado] = useState(null);
    const [erro, setErro] = useState('');

    async function confirmarCompra() {
        const item = JSON.parse(localStorage.getItem('checkoutItem') || '{}');
        if (!item.ingressoId) {
            setErro('Nenhum ingresso selecionado.');
            return;
        }
        setErro('');
        try {
            const data = await pedidoService.checkout({
                itens: [item],
                pagamento: {
                    tipo,
                    numeroCartao: tipo === 'CARTAO' ? numeroCartao : null
                }
            });
            setResultado(data);
        } catch (e) {
            setErro(e.message);
        }
    }

  return (
    <div className="checkout-page">
      <div className="checkout-page__card">
        <h2>Finalizar compra</h2>

        <div className="checkout-page__field">
          <label>Forma de pagamento</label>
          <select className="checkout-page__select" value={tipo} onChange={(e) => setTipo(e.target.value)}>
            <option value="PIX">PIX</option>
            <option value="CARTAO">Cartão de crédito</option>
          </select>
        </div>

        {tipo === 'CARTAO' && (
          <div className="checkout-page__field">
            <label>Número do cartão</label>
            <input
              className="checkout-page__input"
              value={numeroCartao}
              onChange={(e) => setNumeroCartao(e.target.value)}
              placeholder="0000 0000 0000 0000"
            />
          </div>
        )}

        <button className="checkout-page__btn" onClick={confirmarCompra}>Confirmar compra</button>
        {erro && <p className="error">{erro}</p>}

        {resultado && (
          <div className="checkout-page__result">
            <h3>Pedido #{resultado.id}</h3>
            <p>Status: {resultado.status}</p>
            <p>Pagamento: {resultado.pagamentoStatus}</p>
            {resultado.codigoPix && <p>Código PIX: {resultado.codigoPix}</p>}
            {resultado.qrCodeBase64 && (
              <img src={`data:image/png;base64,${resultado.qrCodeBase64}`} alt="QR Code do ingresso" />
            )}
          </div>
        )}
      </div>
    </div>
  );
}
