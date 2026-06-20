using PontoVenda.Backend.Infra.Security;
using PontoVenda.Backend.Modules.PontoVenda.Dto;
using PontoVenda.Backend.Modules.PontoVenda.Model;
using PontoVenda.Backend.Modules.PontoVenda.Repository;

namespace PontoVenda.Backend.Modules.PontoVenda.Service;

public class OperadorService
{
    private readonly IOperadorRepository _repository;
    private readonly JwtTokenService _jwtTokenService;

    // --- INJEÇÃO DO REPOSITORY E DO SERVIÇO DE JWT ---
    public OperadorService(IOperadorRepository repository, JwtTokenService jwtTokenService)
    {
        _repository = repository;
        _jwtTokenService = jwtTokenService;
    }

    // --- AUTENTICAR OPERADOR E GERAR TOKEN ---
    public async Task<TokenRespostaDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var operador = await _repository.BuscarPorLoginAsync(dto.Login, cancellationToken)
            ?? throw new InvalidOperationException("Login ou senha inválidos.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Senha, operador.SenhaHash))
        {
            throw new InvalidOperationException("Login ou senha inválidos.");
        }

        return new TokenRespostaDto
        {
            Token = _jwtTokenService.GenerateToken(operador),
            Nome = operador.Nome,
            Role = operador.Role
        };
    }
}
