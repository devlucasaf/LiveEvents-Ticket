using LiveEventsTicket.Backend.Modules.Evento.Dto;
using LiveEventsTicket.Backend.Modules.Evento.Service;
using LiveEventsTicket.Backend.Modules.Ingresso.Dto;
using LiveEventsTicket.Backend.Modules.Ingresso.Service;
using LiveEventsTicket.Backend.Modules.Relatorio.Dto;
using LiveEventsTicket.Backend.Modules.Relatorio.Service;
using LiveEventsTicket.Backend.Modules.Usuario.Dto;
using LiveEventsTicket.Backend.Modules.Usuario.Repository;

namespace LiveEventsTicket.Backend.Modules.Admin.Service;

public class AdminService
{
    private readonly EventoService      _eventoService;
    private readonly IngressoService    _ingressoService;
    private readonly RelatorioService   _relatorioService;
    private readonly IUsuarioRepository _usuarioRepository;

    // --- CONSTRUTOR COM INJEÇÃO DE DEPENDÊNCIA ---
    public AdminService(
        EventoService eventoService,
        IngressoService ingressoService,
        RelatorioService relatorioService,
        IUsuarioRepository usuarioRepository)
    {
        _eventoService = eventoService;
        _ingressoService = ingressoService;
        _relatorioService = relatorioService;
        _usuarioRepository = usuarioRepository;
    }

    // --- CRIAR EVENTO ---
    public Task<EventoResumoDto> CriarEventoAsync(EventoCriarDto dto, CancellationToken cancellationToken = default)
    {
        return _eventoService.CriarAsync(dto, cancellationToken);
    }

    // --- CRIAR INGRESSO ---
    public Task<IngressoDisponivelDto> CriarIngressoAsync(IngressoCriarDto dto, CancellationToken cancellationToken = default)
    {
        return _ingressoService.CriarAsync(dto, cancellationToken);
    }

    // --- GERAR RELATÓRIO DE VENDAS ---
    public Task<RelatorioVendasDto> RelatorioVendasAsync(CancellationToken cancellationToken = default)
    {
        return _relatorioService.GerarVendasAsync(cancellationToken);
    }

    // --- LISTAR TODOS OS USUÁRIOS E MAPEAR PARA DTO ---
    public async Task<List<UsuarioRespostaDto>> ListarUsuariosAsync(CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarioRepository.ListarTodosAsync(cancellationToken);
        return usuarios.Select(u => new UsuarioRespostaDto
        {
            Id = u.Id,
            Nome = u.Nome,
            Sobrenome = u.Sobrenome,
            Email = u.Email,
            Telefone = u.Telefone,
            Cpf = u.Cpf,
            Role = u.Role
        }).ToList();
    }

    // --- ALTERAR ROLE DE UM USUÁRIO ---
    public async Task<UsuarioRespostaDto> AlterarRoleAsync(int usuarioId, string novaRole, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.BuscarPorIdAsync(usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        usuario.Role = novaRole;
        await _usuarioRepository.AtualizarAsync(usuario, cancellationToken);

        return new UsuarioRespostaDto
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
}
