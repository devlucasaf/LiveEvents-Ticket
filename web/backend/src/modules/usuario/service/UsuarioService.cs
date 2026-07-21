using LiveEventsTicket.Backend.Infra.Security;
using LiveEventsTicket.Backend.Modules.Usuario.Dto;
using LiveEventsTicket.Backend.Modules.Usuario.Repository;
using UsuarioEntity = LiveEventsTicket.Backend.Modules.Usuario.Model.Usuario;

namespace LiveEventsTicket.Backend.Modules.Usuario.Service;

public class UsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly JwtTokenService    _jwtTokenService;

    public UsuarioService(IUsuarioRepository repository, JwtTokenService jwtTokenService)
    {
        _repository = repository;
        _jwtTokenService = jwtTokenService;
    }

    // --- REGISTRAR NOVO USUÁRIO ---
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
            Sobrenome = dto.Sobrenome,
            Email = dto.Email,
            Cpf = dto.Cpf,
            Telefone = dto.Telefone,
            DataNascimento = dto.DataNascimento,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Role = dto.Email.EndsWith("@liveevents.com", StringComparison.OrdinalIgnoreCase) ? "ADMIN" : "CLIENTE"
        };

        await _repository.AdicionarAsync(usuario, cancellationToken);
        return Map(usuario);
    }

    // --- LOGIN DO USUÁRIO ---
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

    // --- BUSCAR USUÁRIO POR ID ---
    public async Task<UsuarioRespostaDto> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var usuario = await _repository.BuscarPorIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        return Map(usuario);
    }

    // --- ATUALIZAR PERFIL DO USUÁRIO ---
    public async Task<UsuarioRespostaDto> AtualizarAsync(int id, UsuarioAtualizarDto dto, CancellationToken cancellationToken = default)
    {
        var usuario = await _repository.BuscarPorIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        if (!string.IsNullOrWhiteSpace(dto.Nome))
        {
            usuario.Nome = dto.Nome;
        }

        if (!string.IsNullOrWhiteSpace(dto.Sobrenome))
        {
            usuario.Sobrenome = dto.Sobrenome;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != usuario.Email)
        {
            var existente = await _repository.BuscarPorEmailAsync(dto.Email, cancellationToken);
            if (existente is not null)
            {
                throw new InvalidOperationException("E-mail já cadastrado.");
            }
            usuario.Email = dto.Email;
        }

        if (!string.IsNullOrWhiteSpace(dto.Telefone))
        {
            usuario.Telefone = dto.Telefone;
        }

        if (!string.IsNullOrWhiteSpace(dto.Cpf))
        {
            usuario.Cpf = dto.Cpf;
        }

        // --- TROCA DE SENHA ---
        if (!string.IsNullOrWhiteSpace(dto.NovaSenha))
        {
            if (string.IsNullOrWhiteSpace(dto.SenhaAtual) || !BCrypt.Net.BCrypt.Verify(dto.SenhaAtual, usuario.SenhaHash))
            {
                throw new InvalidOperationException("Senha atual incorreta.");
            }
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
        }

        await _repository.AtualizarAsync(usuario, cancellationToken);
        return Map(usuario);
    }

    // --- MAPEAMENTO DE ENTIDADE PARA DTO DE RESPOSTA ---
    private static UsuarioRespostaDto Map(UsuarioEntity usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Sobrenome = usuario.Sobrenome,
        Email = usuario.Email,
        Telefone = usuario.Telefone,
        Cpf = usuario.Cpf,
        Role = usuario.Role
    };
}
