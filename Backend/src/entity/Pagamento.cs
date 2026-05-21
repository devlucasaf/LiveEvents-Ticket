namespace Backend.src.Entity;

public class Pagamento
{
        public int          Id              { get; set; }
        public int          PedidoId        { get; set; }
        public Pedido?      Pedido          { get; set; }
        public string       Metodo          { get; set; } = string.Empty; 
        public string       Status          { get; set; } = "Pendente"; 
        public string?      CodigoPix       { get; set; }
        public string?      NumeroCartao    { get; set; }
        public DateTime     CriadoEm        { get; set; } = DateTime.UtcNow;
        public DateTime?    ConfirmadoEm    { get; set; }
}