using Microsoft.EntityFrameworkCore;

namespace DicomSaver.Services
{
    public class DbInitHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _factory;

        public DbInitHostedService(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task StartAsync(CancellationToken token)
        {
            await using var scope = _factory.CreateAsyncScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitHostedService>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<DicomDbContext>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(token);
            // Не могу точно сказать что там по времени будет. Типо GetPendingMigrations просто читает записи из таблицы, по логике быстрее, чем каждый раз их применять.
            if (pendingMigrations.Any())
                await dbContext.Database.MigrateAsync(token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
