using LiveEventsTicket.Backend.Infra.Config;
using LiveEventsTicket.Backend.Modules.Admin.Dto;
using LiveEventsTicket.Backend.Modules.Admin.Model;
using Microsoft.EntityFrameworkCore;

namespace LiveEventsTicket.Backend.Modules.Admin.Service;

public class FuncionarioService
{
    private readonly PdvDbContext _context;

    public FuncionarioService(PdvDbContext context)
    {
        _context = context;
    }

    // --- LISTAR TODOS OS FUNCIONARIOS ---
    public async Task<List<FuncionarioRespostaDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Funcionarios
            .OrderByDescending(f => f.Id)
            .Select(f => new FuncionarioRespostaDto
            {
                Id = f.Id,
                Nome = f.Nome,
                Login = f.Login,
                Role = f.Role,
                Ativo = f.Ativo,
                CreatedAt = f.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    // --- CADASTRAR UM NOVO FUNCIONARIO ---
    public async Task<FuncionarioRespostaDto> CriarAsync(FuncionarioCriarDto dto, CancellationToken cancellationToken = default)
    {
        var login = dto.Login.Trim().ToLowerInvariant();

        var jaExiste = await _context.Funcionarios.AnyAsync(f => f.Login == login, cancellationToken);
        if (jaExiste)
        {
            throw new InvalidOperationException("Já existe um funcionário com este login.");
        }

        var funcionario = new Funcionario
        {
            Nome = dto.Nome.Trim(),
            Login = login,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Role = NormalizarRole(dto.Role),
            Ativo = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Funcionarios.Add(funcionario);
        await _context.SaveChangesAsync(cancellationToken);

        return Mapear(funcionario);
    }

    // --- EDITAR NOME/ROLE E OPCIONALMENTE REDEFINIR A SENHA ---
    public async Task<FuncionarioRespostaDto> AtualizarAsync(int id, FuncionarioAtualizarDto dto, CancellationToken cancellationToken = default)
    {
        var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(f => f.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Funcionário não encontrado.");

        funcionario.Nome = dto.Nome.Trim();
        funcionario.Role = NormalizarRole(dto.Role);

        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            if (dto.Senha.Trim().Length < 6)
            {
                throw new InvalidOperationException("A senha deve ter ao menos 6 caracteres.");
            }
            funcionario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha.Trim());
        }

        funcionario.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Mapear(funcionario);
    }

    // --- ATIVAR/DESATIVAR UM FUNCIONARIO ---
    public async Task<FuncionarioRespostaDto> AlterarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default)
    {
        var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(f => f.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Funcionário não encontrado.");

        funcionario.Ativo = ativo;
        funcionario.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Mapear(funcionario);
    }

    // --- NORMALIZA A ROLE ---
    private static string NormalizarRole(string? role)
    {
        var normalizada = (role ?? "OPERADOR").Trim().ToUpperInvariant();
        return normalizada == "ADMIN" ? "ADMIN" : "OPERADOR";
    }

    // --- CONVERTE A ENTIDADE PARA O DTO DE RESPOSTA ---
    private static FuncionarioRespostaDto Mapear(Funcionario f) => new()
    {
        Id = f.Id,
        Nome = f.Nome,
        Login = f.Login,
        Role = f.Role,
        Ativo = f.Ativo,
        CreatedAt = f.CreatedAt
    };
}
