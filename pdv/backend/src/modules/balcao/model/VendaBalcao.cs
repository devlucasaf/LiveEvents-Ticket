using PontoVenda.Backend.Entity;

namespace PontoVenda.Backend.Modules.Balcao.Model;

public class VendaBalcao : AuditEntity
{
    public Guid     Id                      { get; set; } = Guid.NewGuid();
    public int      OperadorId              { get; set; }
    public string   OperadorNome            { get; set; } = string.Empty;
    public int      ClienteWebId            { get; set; }
    public string   ClienteNome             { get; set; } = string.Empty;
    public string   ClienteSobrenome        { get; set; } = string.Empty;
    public string   ClienteEmail            { get; set; } = string.Empty;
    public string   ClienteCpf              { get; set; } = string.Empty;
    public string   ClienteTelefone         { get; set; } = string.Empty;
    public DateTime? ClienteDataNascimento  { get; set; }
    public string?  Cep                     { get; set; }
    public string?  Logradouro              { get; set; }
    public string?  Numero                  { get; set; }
    public string?  Complemento             { get; set; }
    public string?  Bairro                  { get; set; }
    public string?  Cidade                  { get; set; }
    public string?  Estado                  { get; set; }
    public int      PedidoWebId             { get; set; }
    public int      EventoId                { get; set; }
    public string   EventoTitulo            { get; set; } = string.Empty;
    public int      IngressoId              { get; set; }
    public string   Setor                   { get; set; } = string.Empty;
    public string   TipoEntrada             { get; set; } = "INTEIRA";
    public string   FormaPagamento          { get; set; } = "CREDITO";
    public int      Quantidade              { get; set; }
    public decimal  ValorUnitario           { get; set; }
    public decimal  ValorTotal              { get; set; }
    public string   CodigoTicket            { get; set; } = string.Empty;
    public DateTime DataVenda               { get; set; } = DateTime.UtcNow;
    public string?  AcompanhantesJson       { get; set; }

    public string?  Subtipo                 { get; set; }

    public string?  DocumentosJson          { get; set; }
}
