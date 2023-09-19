using DicomSaver.Services;

using Microsoft.EntityFrameworkCore;

using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddSingleton(x =>
{
    var connectionString = builder.Configuration.GetSection("RabbitMq").Get<AmqpTcpEndpoint>(); // ������ � ����� ���� ������ � ���������, �� ��������� ��� ������������ ���������, �� ����� � ��� ���.
    // string.IsNullOrEmpty - throw new StartupException, �����-���� � ����� ������������ �� ���� ����� ������. �� ��� ���� - ������ ��� ����.
    var factory = new ConnectionFactory() { DispatchConsumersAsync = true };
    var connection = factory.CreateConnection(new AmqpTcpEndpoint[] { connectionString });
    return connection.CreateModel();
});
// (provider, opt) => { ... }
builder.Services.AddDbContext<DicomDbContext>(opt =>
{
    var connection = builder.Configuration.GetConnectionString("Database");
    opt.UseNpgsql(connection);
    // ��� �� ������, ���� ���������. ���� ��� - ������ �� ����� provider � �������� ���� ����
    opt.AddInterceptors(new EntityInterceptor());
});
builder.Services.AddScoped<IDicomStorage, DicomStorage>();
// ������� �� ������
builder.Services.AddHostedService<DbInitHostedService>();
builder.Services.AddHostedService<EventHostedService>();

var app = builder.Build();
app.Run();
