using System.ComponentModel.DataAnnotations;

namespace Backend.src.Entity;

public class Pedido
{
        public int                      Id          { get; set; }
        public int                      UsuarioId   { get; set; }
        public Usuario?                 Usuario     { get; set; }
        public decimal                  Total       { get; set; }
        public string                   Status      { get; set; } = "PENDENTE"; 
        public DateTime                 CriadoEm    { get; set; } = DateTime.UtcNow;
        public DateTime?                PagoEm      { get; set; }
        public ICollection<Ingresso>    Ingressos   { get; set; } = new List<Ingresso>();
        public Pagamento?               Pagamento   { get; set; }
}