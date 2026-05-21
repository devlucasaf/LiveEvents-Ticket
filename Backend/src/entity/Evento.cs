using System.ComponentModel.DataAnnotations;

namespace Backend.src.Entity
{
    public class Evento
    {
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        [Required, MaxLength(50)]
        public string Categoria { get; set; } = string.Empty; 

        [Required, MaxLength(200)]
        public string Local { get; set; } = string.Empty;

        [MaxLength(120)]
        public string?                      Cidade      { get; set; }
        public DateTime                     DataHora    { get; set; }
        public string?                      ImagemUrl   { get; set; }
        public bool                         Ativo       { get; set; } = true;
        public DateTime                     CriadoEm    { get; set; } = DateTime.UtcNow;
        public ICollection<SetorIngresso>   Setores     { get; set; } = new List<SetorIngresso>();
    }
}