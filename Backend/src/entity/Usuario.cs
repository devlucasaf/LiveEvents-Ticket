namespace Backend.src.entity;

public class Usuario
{
    public int Id { get; set; }
    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;
    [Required, MaxLength(160)]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string SenhaHash { get; set; } = string.Empty;
    [MaxLength(14)]
    public string? Cpf { get; set; }
    public string Perfil { get; set; } = "CLIENTE"; // CLIENTE | ADMIN
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}