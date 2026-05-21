using System.ComponentModel.DataAnnotations;

namespace Backend.src.Entity
{
    public class SetorIngresso
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public Evento? Evento { get; set; }

        [Required, MaxLength(80)]
        public string Nome { get; set; } = string.Empty; 
        public decimal Preco { get; set; }
        public int QuantidadeTotal { get; set; }
        public int QuantidadeDisponivel { get; set; }        
        public ICollection<Ingresso> Ingressos { get; set; } = new List<Ingresso>();
    }
}