using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public static class IngressoPdfBuilder
{
    // --- GERA O PDF FINAL DO INGRESSO/COMPROVANTE ---
    public static byte[] Gerar(IngressoPdfDados dados)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(26);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken3));

                page.Header().Column(coluna =>
                {
                    coluna.Item().Text("LiveEvents Ticket").FontSize(22).SemiBold().FontColor(Colors.Red.Darken2);
                    coluna.Item().Text($"Ingresso digital - Pedido #{dados.PedidoId}").FontSize(12).FontColor(Colors.Grey.Darken1);
                    coluna.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingVertical(10).Column(coluna =>
                {
                    coluna.Spacing(10);

                    coluna.Item().Text("Dados do comprador").SemiBold().FontSize(13);
                    coluna.Item().Text($"Nome: {dados.NomeComprador}");
                    coluna.Item().Text($"CPF: {dados.DocumentoComprador}");
                    coluna.Item().Text($"E-mail: {dados.EmailComprador}");
                    coluna.Item().Text($"Endereço: {dados.EnderecoComprador}");
                    coluna.Item().Text($"Data da compra: {dados.DataCompra}");
                    coluna.Item().Text($"Valor total: {dados.ValorTotalFormatado}").SemiBold();

                    coluna.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    coluna.Item().Text("Ingressos").SemiBold().FontSize(13);

                    foreach (var item in dados.Itens)
                    {
                        coluna.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(card =>
                        {
                            card.Spacing(3);
                            card.Item().Text(item.EventoTitulo).SemiBold().FontSize(12);
                            card.Item().Text($"Data: {item.EventoData}");
                            card.Item().Text($"Local: {item.EventoLocal}");
                            card.Item().Text($"Setor: {item.Setor}");
                            card.Item().Text($"Quantidade: {item.Quantidade}");
                            card.Item().Text($"Valor unitário: {item.ValorUnitarioFormatado}");
                            card.Item().Text($"Subtotal: {item.SubtotalFormatado}").SemiBold();
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(dados.QrCodeBase64))
                    {
                        try
                        {
                            var qrBytes = Convert.FromBase64String(dados.QrCodeBase64);

                            coluna.Item().PaddingTop(10).AlignCenter().Column(qr =>
                            {
                                qr.Item().Text("Validação do ingresso").SemiBold();
                                qr.Item().Text("Apresente este QR Code na entrada do evento").FontSize(9).FontColor(Colors.Grey.Darken1);
                                qr.Item().PaddingTop(4).Width(120).Height(120).Image(qrBytes);
                            });
                        }
                        catch
                        {
                            // --- IGNORA QR CODE INVALIDO E SEGUE COM O PDF ---
                        }
                    }
                });

                page.Footer().AlignCenter().Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
            });
        }).GeneratePdf();
    }
}
