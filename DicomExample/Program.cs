using DicomExample.Services;

using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IStudyService, StudyService>(); // �����-����. ��� ����� ������������ � ��������� HttpContext.Request, ������ Transient - �������, Singleton - �����������.
builder.Services.AddSingleton(x =>
{
    var connectionString = builder.Configuration.GetSection("RabbitMq").Get<AmqpTcpEndpoint>(); // ������ � ����� ���� ������ � ���������, �� ��������� ��� ������������ ���������, �� ����� � ��� ���.
    // string.IsNullOrEmpty - throw new StartupException, �����-���� � ����� ������������ �� ���� ����� ������. �� ��� ���� - ������ ��� ����.
    var factory = new ConnectionFactory() { DispatchConsumersAsync = true };
    var connection = factory.CreateConnection(new AmqpTcpEndpoint[] { connectionString });
    return connection.CreateModel();
});
// � ��. �����������, ��� ����� ���� ������� ��������� (��� �� �������), �� ��������� ������� ������ ���������� ��� - �� �������� �������� �������� ��� ���.
// Singleton ������ ��� ��������� ���� �� �����. ����� ��� ���������.
builder.Services.AddSingleton<IStudyReaderStrategy, BasicStudyReaderStrategy>();
// �� ������������� ����, ��� ������ ���� ���� :D
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
