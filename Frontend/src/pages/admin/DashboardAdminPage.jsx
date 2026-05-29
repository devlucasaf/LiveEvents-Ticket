import { useEffect, useState } from 'react';
import { pedidoService } from '../../services/pedidoService';
import '../../styles/admin.css';

export default function DashboardAdminPage() {
    const [relatorio, setRelatorio] = useState(null);
    const [erro, setErro] = useState('');

    useEffect(() => {
        pedidoService
        .relatorioVendas()
        .then(setRelatorio)
        .catch((e) => setErro(e.message));
    }, []);

    return (
        <section className="admin-page">
            <h2>Dashboard Administrativo</h2>
            {erro && <p className="error">{erro}</p>}

            {relatorio && (
                <>
                <div className="admin-page__stats">
                    <div className="admin-page__stat-card">
                        <div className="admin-page__stat-label">Total de pedidos</div>
                        <div className="admin-page__stat-value">{relatorio.totalPedidos}</div>
                    </div>
                    <div className="admin-page__stat-card">
                        <div className="admin-page__stat-label">Receita total</div>
                        <div className="admin-page__stat-value">R$ {relatorio.receitaTotal.toFixed(2)}</div>
                    </div>
                </div>

                <h3 style={{ marginBottom: '1rem', fontSize: '1rem', fontWeight: 600 }}>Performance por evento</h3>
                <table className="admin-page__table">
                    <thead>
                        <tr>
                            <th>Evento</th>
                            <th>Ingressos vendidos</th>
                            <th>Receita</th>
                        </tr>
                    </thead>
                    <tbody>
                        {relatorio.eventos.map((item) => (
                            <tr key={item.evento}>
                                <td>{item.evento}</td>
                                <td>{item.quantidadeIngressosVendidos}</td>
                                <td>R$ {item.receita.toFixed(2)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                </>
            )}
        </section>
    );
}
