namespace Backend.src.Entity;

public class Ingresso
{
        public int              Id                  { get; set; }
        public int              SetorIngressoId     { get; set; }
        public SetorIngresso?   Setor               { get; set; }
        public int?             PedidoId            { get; set; }
        public Pedido?          Pedido              { get; set; }
        public string           Codigo              { get; set; } = Guid.NewGuid().ToString("N");
        public string?          QrCodeBase64        { get; set; }
        public string           Status              { get; set; } = "DISPONIVEL"; 
        public string?          Assento             { get; set; }
        public DateTime         CriadoEm            { get; set; } = DateTime.UtcNow;
}