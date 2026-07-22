using LiveEventsTicket.Backend.Modules.Pedido.Dto;

using PedidoEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

internal static class ReembolsoRegras
{
    // --- AVALIA ELEGIBILIDADE DE REEMBOLSO AUTOMATICO SEM APROVACAO MANUAL ---
    public static (bool elegivel, string? mensagem, string regra) AvaliarElegibilidade(
        PedidoEntity pedido,
        DateTime? dataEvento,
        DateTime agora)
    {
        var regra = RegrasReembolso.RegraAplicada;

        if (pedido.Status == StatusPedido.Reembolsado)
        {
            return (false, "Este pedido já foi reembolsado.", regra);
        }

        if (pedido.Status != StatusPedido.Pago)
        {
            return (false, "Apenas pedidos com pagamento aprovado podem ser reembolsados.", regra);
        }

        // --- JANELA DE ARREPENDIMENTO EXPIRADA ---
        var limiteArrependimento = pedido.DataCriacao.AddDays(RegrasReembolso.JanelaArrependimentoDias);
        if (agora > limiteArrependimento)
        {
            return (false, "Prazo de arrependimento expirado para este pedido.", regra);
        }

        if (!dataEvento.HasValue)
        {
            return (false, "Não foi possível validar a data do evento para este pedido.", regra);
        }

        if (dataEvento.Value <= agora.AddHours(RegrasReembolso.MinimoHorasAntesEvento))
        {
            return (false, "Reembolso indisponível para eventos com menos de 48 horas para início.", regra);
        }

        return (true, "Elegível para reembolso automático.", regra);
    }

    // --- NORMALIZA E VALIDA O MOTIVO INFORMADO NO PEDIDO DE REEMBOLSO ---
    public static MotivoReembolsoResolvido NormalizarMotivo(SolicitarReembolsoDto dto)
    {
        var codigo      = NormalizarCodigo(dto.MotivoCodigo);
        var detalhe     = string.IsNullOrWhiteSpace(dto.MotivoDetalhe) ? null : dto.MotivoDetalhe.Trim();
        var motivoLivre = string.IsNullOrWhiteSpace(dto.Motivo) ? null : dto.Motivo.Trim();

        if (string.IsNullOrWhiteSpace(codigo))
        {
            if (string.IsNullOrWhiteSpace(motivoLivre))
            {
                codigo = MotivoReembolso.Arrependimento;
            }
            else
            {
                codigo  = MotivoReembolso.Outro;
                detalhe = motivoLivre;
            }
        }

        if (!MotivoReembolso.Descricoes.TryGetValue(codigo, out var descricao))
        {
            var codigosValidos = string.Join(", ", MotivoReembolso.Descricoes.Keys.OrderBy(x => x));
            throw new InvalidOperationException($"Motivo de reembolso inválido. Use um dos códigos: {codigosValidos}.");
        }

        if (codigo == MotivoReembolso.Outro)
        {
            var detalheFinal = detalhe ?? motivoLivre;
            if (string.IsNullOrWhiteSpace(detalheFinal))
            {
                throw new InvalidOperationException("Para o motivo OUTRO, detalhe o motivo do estorno.");
            }

            detalhe = detalheFinal.Trim();
        }

        if (!string.IsNullOrWhiteSpace(detalhe) && detalhe.Length > RegrasReembolso.TamanhoMaximoDetalheMotivo)
        {
            throw new InvalidOperationException(
                $"O detalhe do motivo deve ter no máximo {RegrasReembolso.TamanhoMaximoDetalheMotivo} caracteres."
            );
        }

        // --- MONTA O TEXTO PERSISTIDO CONCATENANDO DESCRICAO E DETALHE ---
        var motivoPersistido = string.IsNullOrWhiteSpace(detalhe)
            ? descricao
            : $"{descricao} Detalhe: {detalhe}";

        return new MotivoReembolsoResolvido(codigo, descricao, detalhe, motivoPersistido);
    }

    // --- RETORNA DESCRICAO OFICIAL DO CODIGO OU A DESCRICAO DE "OUTRO" QUANDO DESCONHECIDO ---
    public static string DescricaoDoCodigo(string? codigo)
    {
        var codigoNormalizado = NormalizarCodigo(codigo);
        if (!string.IsNullOrWhiteSpace(codigoNormalizado)
            && MotivoReembolso.Descricoes.TryGetValue(codigoNormalizado, out var descricao))
        {
            return descricao;
        }

        return MotivoReembolso.Descricoes[MotivoReembolso.Outro];
    }

    // --- NORMALIZA CODIGO DE MOTIVO PARA O PADRAO ESPERADO ---
    private static string NormalizarCodigo(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return string.Empty;
        }

        return codigo.Trim().ToUpperInvariant().Replace('-', '_').Replace(' ', '_');
    }
}
