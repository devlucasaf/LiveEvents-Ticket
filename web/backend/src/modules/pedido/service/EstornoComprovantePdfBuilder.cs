using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public static class EstornoComprovantePdfBuilder
{
    // --- GERA O PDF DE COMPROVANTE DE ESTORNO DO PEDIDO ---
    public static byte[] Gerar(EstornoComprovantePdfDados dados)
    {
        // --- CRIA O DOCUMENTO E CONFIGURA A PÁGINA ---
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(26);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken3));

                // --- TÍTULO, IDENTIFICACAO DO PEDIDO E LINHA DIVISORIA ---
                page.Header().Column(coluna =>
                {
                    coluna.Item().Text("LiveEvents Ticket").FontSize(22).SemiBold().FontColor(Colors.Green.Darken2);
                    coluna.Item().Text($"Comprovante de estorno - Pedido #{dados.PedidoId}").FontSize(12).FontColor(Colors.Grey.Darken1);
                    coluna.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                // --- BLOCOS DE RESUMO, COMPRADOR, PEDIDO E MOTIVO ---
                page.Content().PaddingVertical(10).Column(coluna =>
                {
                    coluna.Spacing(8);

                    // --- RESUMO DO ESTORNO ---
                    coluna.Item().Text("Resumo do estorno").SemiBold().FontSize(13);
                    coluna.Item().Text($"Protocolo: {dados.ProtocoloEstorno}").SemiBold();
                    coluna.Item().Text($"Valor estornado: {dados.ValorEstornadoFormatado}").SemiBold();
                    coluna.Item().Text($"Data da solicitação: {FormatarDataHora(dados.DataSolicitacao)}");
                    coluna.Item().Text($"Data da aprovação: {FormatarDataHora(dados.DataAprovacao)}");
                    coluna.Item().Text($"Data do estorno: {FormatarDataHora(dados.DataEstorno)}");

                    // --- DADOS DO COMPRADOR ---
                    coluna.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    coluna.Item().Text("Dados do comprador").SemiBold().FontSize(13);
                    coluna.Item().Text($"Nome: {dados.NomeComprador}");
                    coluna.Item().Text($"CPF: {dados.DocumentoComprador}");
                    coluna.Item().Text($"E-mail: {dados.EmailComprador}");

                    // --- DADOS DO PEDIDO REEMBOLSADO ---
                    coluna.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    coluna.Item().Text("Dados do pedido reembolsado").SemiBold().FontSize(13);
                    coluna.Item().Text($"Evento: {dados.EventoTitulo}");
                    coluna.Item().Text($"Data do evento: {FormatarDataHora(dados.EventoData)}");
                    coluna.Item().Text($"Setor: {dados.Setor ?? "Não informado"}");
                    coluna.Item().Text($"Quantidade de ingressos: {dados.QuantidadeIngressos}");

                    // --- MOTIVO DO ESTORNO E REGRA APLICADA ---
                    coluna.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    coluna.Item().Text("Motivo do estorno").SemiBold().FontSize(13);
                    coluna.Item().Text($"Código: {dados.MotivoCodigo}");
                    coluna.Item().Text($"Descrição: {dados.MotivoDescricao}");
                    coluna.Item().Text($"Detalhamento: {dados.MotivoInformado ?? "Não informado"}");
                    coluna.Item().Text($"Regra aplicada: {dados.RegraAplicada ?? "Não informada"}");
                });

                page.Footer().AlignCenter().Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
            });
        }).GeneratePdf();
    }

    // --- FORMATA DATAS NO PADRÃO PT-BR PARA EXIBICAO NO COMPROVANTE ---
    private static string FormatarDataHora(DateTime? data)
    {
        if (!data.HasValue)
        {
            return "Não informado";
        }

        return data.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
    }
}
