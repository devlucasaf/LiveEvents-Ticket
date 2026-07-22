using LiveEventsTicket.Backend.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LiveEventsTicket.Backend.Infra.Config;

public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    // --- CHAMADO NA VERSAO SINCRONA DE SaveChanges ---
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AplicarAuditoria(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    // --- CHAMADO NA VERSAO ASSINCRONA DE SaveChangesAsync ---
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AplicarAuditoria(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    // --- PERCORRE O CHANGE TRACKER E AJUSTA CreatedAt / UpdatedAt DE CADA AuditEntity ---
    private static void AplicarAuditoria(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var agora = DateTime.UtcNow;

        // --- SÓ CONSIDERA ENTIDADES QUE HERDAM DE AuditEntity ---
        IEnumerable<EntityEntry<AuditEntity>> entradas = context.ChangeTracker
            .Entries<AuditEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entrada in entradas)
        {
            if (entrada.State == EntityState.Added)
            {
                entrada.Entity.CreatedAt = agora;
                entrada.Entity.UpdatedAt = null;
                continue;
            }

            entrada.Property(nameof(AuditEntity.CreatedAt)).IsModified = false;
            entrada.Entity.UpdatedAt = agora;
        }
    }
}
