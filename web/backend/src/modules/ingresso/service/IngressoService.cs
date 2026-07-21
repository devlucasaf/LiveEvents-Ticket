using LiveEventsTicket.Backend.Modules.Ingresso.Dto;
using LiveEventsTicket.Backend.Modules.Ingresso.Model;
using LiveEventsTicket.Backend.Modules.Ingresso.Repository;
using IngressoEntity = LiveEventsTicket.Backend.Modules.Ingresso.Model.Ingresso;

namespace LiveEventsTicket.Backend.Modules.Ingresso.Service;

public class IngressoService
{
    private readonly IIngressoRepository _repository;

    // --- INJEÇÃO DE DEPENDÊNCIA DO REPOSITÓRIO ---
    public IngressoService(IIngressoRepository repository)
    {
        _repository = repository;
    }

    // --- LISTAR OS 5 SETORES OFICIAIS DO EVENTO COM PRECOS INTEIRA/MEIA/SOCIAL ---
    public async Task<List<IngressoDisponivelDto>> ListarPorEventoAsync(int eventoId, CancellationToken cancellationToken = default)
    {
        var ingressos = await GarantirSetoresPadraoAsync(eventoId, cancellationToken);

        var resultado = new List<IngressoDisponivelDto>();
        foreach (var setor in CatalogoIngresso.Setores)
        {
            var ingresso = ingressos.FirstOrDefault(i =>
                string.Equals(i.Setor, setor.Nome, StringComparison.OrdinalIgnoreCase));

            if (ingresso is null)
            {
                continue;
            }

            resultado.Add(new IngressoDisponivelDto
            {
                Id = ingresso.Id,
                EventoId = ingresso.EventoId,
                SetorCodigo = setor.Codigo,
                Setor = ingresso.Setor,
                Preco = ingresso.Preco,
                PrecoMeia = CatalogoIngresso.CalcularPreco(ingresso.Preco, CatalogoIngresso.ModalidadeMeia),
                PrecoSocial = CatalogoIngresso.CalcularPreco(ingresso.Preco, CatalogoIngresso.ModalidadeSocial),
                QuantidadeDisponivel = ingresso.QuantidadeDisponivel
            });
        }

        return resultado;
    }

    // --- RETORNA O CATALOGO DE MODALIDADES ---
    public ModalidadesDto ObterModalidades()
    {
        return new ModalidadesDto
        {
            MeiaFator = CatalogoIngresso.MeiaFator,
            SocialAcrescimo = CatalogoIngresso.SocialAcrescimo,
            MeiaSubtipos = CatalogoIngresso.MeiaSubtipos
                .Select(s => new MeiaSubtipoDto
                {
                    Codigo = s.Codigo,
                    Nome = s.Nome,
                    Campos = CatalogoIngresso.CamposDocumento(s.Codigo)
                        .Select(c => new DocumentoCampoDto
                        {
                            Chave = c.Chave,
                            Rotulo = c.Rotulo,
                            Tipo = c.Tipo,
                            Obrigatorio = c.Obrigatorio
                        })
                        .ToList()
                })
                .ToList()
        };
    }

    // --- CRIA OS SETORES CANONICOS QUE AINDA NAO EXISTEM NO EVENTO ---
    private async Task<List<IngressoEntity>> GarantirSetoresPadraoAsync(int eventoId, CancellationToken cancellationToken)
    {
        var existentes = await _repository.ListarPorEventoAsync(eventoId, cancellationToken);

        var faltantes = CatalogoIngresso.Setores
            .Where(setor => !existentes.Any(i =>
                string.Equals(i.Setor, setor.Nome, StringComparison.OrdinalIgnoreCase)))
            .Select(setor => new IngressoEntity
            {
                EventoId = eventoId,
                Setor = setor.Nome,
                Preco = setor.PrecoPadrao,
                QuantidadeDisponivel = 500
            })
            .ToList();

        if (faltantes.Count > 0)
        {
            await _repository.AdicionarVariosAsync(faltantes, cancellationToken);
            existentes = await _repository.ListarPorEventoAsync(eventoId, cancellationToken);
        }

        return existentes;
    }

    // --- CRIAR NOVO INGRESSO A PARTIR DO DTO ---
    public async Task<IngressoDisponivelDto> CriarAsync(IngressoCriarDto dto, CancellationToken cancellationToken = default)
    {
        var ingresso = new IngressoEntity
        {
            EventoId = dto.EventoId,
            Setor = dto.Setor,
            Preco = dto.Preco,
            QuantidadeDisponivel = dto.QuantidadeDisponivel
        };

        await _repository.AdicionarAsync(ingresso, cancellationToken);

        return new IngressoDisponivelDto
        {
            Id = ingresso.Id,
            EventoId = ingresso.EventoId,
            Setor = ingresso.Setor,
            Preco = ingresso.Preco,
            PrecoMeia = CatalogoIngresso.CalcularPreco(ingresso.Preco, CatalogoIngresso.ModalidadeMeia),
            PrecoSocial = CatalogoIngresso.CalcularPreco(ingresso.Preco, CatalogoIngresso.ModalidadeSocial),
            QuantidadeDisponivel = ingresso.QuantidadeDisponivel
        };
    }
}
