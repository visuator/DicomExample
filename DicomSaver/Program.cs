using DicomSaver.Services;

using Microsoft.EntityFrameworkCore;

using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddSingleton(x =>
{
    var connectionString = builder.Configuration.GetSection("RabbitMq").Get<AmqpTcpEndpoint>(); // ќбычно € такие вещи выношу в константы, но поскольку это единственное вхождение, то можно и без нее.
    // string.IsNullOrEmpty - throw new StartupException, оп€ть-таки в цел€х демонстрации не стал этого делать. Ќу или логи - смотр€ как надо.
    var factory = new ConnectionFactory() { DispatchConsumersAsync = true };
    var connection = factory.CreateConnection(new AmqpTcpEndpoint[] { connectionString });
    return connection.CreateModel();
});
// (provider, opt) => { ... }
builder.Services.AddDbContext<DicomDbContext>(opt =>
{
    var connection = builder.Configuration.GetConnectionString("Database");
    opt.UseNpgsql(connection);
    // Ёто не сервис, ниче страшного. ≈сли что - вызвал бы через provider в комменте чуть выше
    opt.AddInterceptors(new EntityInterceptor());
});
builder.Services.AddScoped<IDicomStorage, DicomStorage>();
// ѕор€док не мен€ть
builder.Services.AddHostedService<DbInitHostedService>();
builder.Services.AddHostedService<EventHostedService>();

var app = builder.Build();
app.Run();
