using DicomSaver.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DicomSaver.Services
{
    public class EntityInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            foreach(var e in eventData.Context!.ChangeTracker.Entries())
            {
                if (e.Entity is not BaseEntity be) continue;
                if (e.State == EntityState.Added)
                    be.CreatedAt = now;
                be.UpdatedAt = now;
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
