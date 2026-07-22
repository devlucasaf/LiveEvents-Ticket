using QRCoder;

using PedidoEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

internal static class PedidoHelpers
{
    // --- MANTEM APENAS DIGITOS DE UMA STRING ---
    public static string SomenteDigitos(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        return new string(valor.Where(char.IsDigit).ToArray());
    }

    // --- FORMATA DECIMAL NO PADRAO DE MOEDA BRL ---
    public static string FormatarMoeda(decimal valor)
    {
        return valor.ToString("C", new System.Globalization.CultureInfo("pt-BR"));
    }

    // --- NORMALIZA UM VALOR NUMERICO PARA UM INTERVALO PERMITIDO ---
    public static int NormalizarIntervalo(int valor, int minimo, int maximo)
    {
        return Math.Clamp(valor, minimo, maximo);
    }

    // --- GERA TOKEN UNICO DE CHECKIN COM O PREFIXO PADRAO ---
    public static string GerarTokenCheckin()
    {
        return $"{PrefixosToken.Checkin}-{Guid.NewGuid():N}";
    }

    // --- GERA TOKEN UNICO PARA COMPARTILHAMENTO PUBLICO ---
    public static string GerarTokenCompartilhamento()
    {
        return $"{PrefixosToken.Compartilhamento}-{Guid.NewGuid():N}";
    }

    // --- GERA CODIGO DE PROTOCOLO DE ESTORNO PARA PDF/RASTREIO ---
    public static string GerarProtocoloEstorno(int pedidoId, DateTime? dataEstorno)
    {
        if (!dataEstorno.HasValue)
        {
            return string.Empty;
        }

        return $"{PrefixosToken.Estorno}-{pedidoId:D6}-{dataEstorno.Value:yyyyMMddHHmmss}";
    }

    // --- GERA QR CODE EM BASE64 A PARTIR DE UM PAYLOAD DE TEXTO ---
    public static string GerarQrCodeBase64(string payload)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(data);
        var bytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(bytes);
    }

    // --- MONTA ENDERECO COMPLETO DO COMPRADOR EM UMA LINHA ---
    public static string MontarEnderecoComprador(PedidoEntity pedido)
    {
        var complemento = string.IsNullOrWhiteSpace(pedido.EnderecoComplemento)
            ? string.Empty
            : $" ({pedido.EnderecoComplemento})";

        return $"{pedido.EnderecoLogradouro}, {pedido.EnderecoNumero}{complemento} - {pedido.EnderecoBairro}, {pedido.EnderecoCidade}/{pedido.EnderecoEstado}, CEP {pedido.EnderecoCep}";
    }

    // --- AVALIA SE O LINK DE COMPARTILHAMENTO CONTINUA ATIVO ---
    public static bool CompartilhamentoAtivo(PedidoEntity pedido, DateTime agora)
    {
        if (string.IsNullOrWhiteSpace(pedido.CompartilhamentoToken))
        {
            return false;
        }

        if (pedido.CompartilhamentoRevogadoEm.HasValue)
        {
            return false;
        }

        if (!pedido.CompartilhamentoExpiraEm.HasValue || pedido.CompartilhamentoExpiraEm.Value <= agora)
        {
            return false;
        }

        if (pedido.CompartilhamentoMaxAcessos <= 0)
        {
            return false;
        }

        return pedido.CompartilhamentoAcessosRealizados < pedido.CompartilhamentoMaxAcessos;
    }
}
