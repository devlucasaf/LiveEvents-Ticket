using LiveEventsTicket.Backend.Infra.Security;
using LiveEventsTicket.Backend.Modules.Usuario.Dto;
using LiveEventsTicket.Backend.Modules.Usuario.Repository;
using UsuarioEntity = LiveEventsTicket.Backend.Modules.Usuario.Model.Usuario;

namespace LiveEventsTicket.Backend.Modules.Usuario.Service;

public class UsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly JwtTokenService _jwtTokenService;

    public UsuarioService(IUsuarioRepository repository, JwtTokenService jwtTokenService)
    {
        _repository = repository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<UsuarioRespostaDto> RegistrarAsync(UsuarioRegistroDto dto, CancellationToken cancellationToken = default)
    {
        var existente = await _repository.BuscarPorEmailAsync(dto.Email, cancellationToken);
        if (existente is not null)
        {
            throw new InvalidOperationException("E-mail já cadastrado.");
        }

        var usuario = new UsuarioEntity
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
        };

        await _repository.AdicionarAsync(usuario, cancellationToken);
        return Map(usuario);
    }

    public async Task<TokenRespostaDto> LoginAsync(UsuarioLoginDto dto, CancellationToken cancellationToken = default)
    {
        var usuario = await _repository.BuscarPorEmailAsync(dto.Email, cancellationToken)
            ?? throw new InvalidOperationException("Usuário ou senha inválidos.");

        var senhaValida = BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash);
        if (!senhaValida)
        {
            throw new InvalidOperationException("Usuário ou senha inválidos.");
        }

        return new TokenRespostaDto
        {
            Token = _jwtTokenService.GenerateToken(usuario),
            Usuario = Map(usuario)
        };
    }

    public async Task<UsuarioRespostaDto> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var usuario = await _repository.BuscarPorIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        return Map(usuario);
    }

    private static UsuarioRespostaDto Map(UsuarioEntity usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        Role = usuario.Role
    };
}
