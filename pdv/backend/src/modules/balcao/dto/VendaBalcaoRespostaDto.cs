namespace PontoVenda.Backend.Modules.Balcao.Dto;

public class VendaBalcaoRespostaDto
{
    public string   CodigoTicket    { get; set; } = string.Empty;
    public int      PedidoId        { get; set; }
    public string   ClienteNome     { get; set; } = string.Empty;
    public string   ClienteEmail    { get; set; } = string.Empty;
    public string   EventoTitulo    { get; set; } = string.Empty;
    public string   EventoLocal     { get; set; } = string.Empty;
    public DateTime EventoData      { get; set; }
    public string   Setor           { get; set; } = string.Empty;
    public string   TipoEntrada     { get; set; } = string.Empty;
    public int      Quantidade      { get; set; }
    public decimal  ValorTotal      { get; set; }
    public DateTime DataVenda       { get; set; }
    public string?  QrCodeBase64    { get; set; }

    // --- INFORMA SE UMA NOVA CONTA FOI CRIADA PARA O CLIENTE ---
    public bool     ContaCriada     { get; set; }
    public string?  SenhaInicial    { get; set; }
}
