using DicomExample.Services;

using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IStudyService, StudyService>(); // ќп€ть-таки. “ут скоуп определ€етс€ в контексте HttpContext.Request, потому Transient - излишне, Singleton - неправильно.
builder.Services.AddSingleton(x =>
{
    var connectionString = builder.Configuration.GetSection("RabbitMq").Get<AmqpTcpEndpoint>(); // ќбычно € такие вещи выношу в константы, но поскольку это единственное вхождение, то можно и без нее.
    // string.IsNullOrEmpty - throw new StartupException, оп€ть-таки в цел€х демонстрации не стал этого делать. Ќу или логи - смотр€ как надо.
    var factory = new ConnectionFactory() { DispatchConsumersAsync = true };
    var connection = factory.CreateConnection(new AmqpTcpEndpoint[] { connectionString });
    return connection.CreateModel();
});
// » да. ≈стественно, тут может быть каталог стратегий (она же фабрика), но поскольку никаких других реализаций нет - не посчитал зазорным написать вот так.
// Singleton потому что остальные тупо не нужны. «десь нет состо€ни€.
builder.Services.AddSingleton<IStudyReaderStrategy, BasicStudyReaderStrategy>();
// «а версионизацию шарю, мне просто было лень :D
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
