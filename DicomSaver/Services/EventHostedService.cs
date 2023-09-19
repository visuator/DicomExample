using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Threading.Channels;

namespace DicomSaver.Services
{
    public class EventHostedService : IHostedService
    {
        // Ну вообще это рекомендованная практика, но иногда(!) можно обойтись IServiceProvider
        // Был такой вопрос в практике, что это анти-паттерн service locator. Но тут - оправданно, как и многое другое в отношении паттернов
        // :)
        private readonly IServiceScopeFactory _factory;

        public EventHostedService(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task StartAsync(CancellationToken token)
        {
            // IAsyncDisposable
            await using var scope = _factory.CreateAsyncScope();
            var channel = scope.ServiceProvider.GetRequiredService<IModel>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<EventHostedService>>();
            // Да-да, знаю, код повторяется. Ну просто лень было, не спорю.
            // Ну и порядок запуска решает - как бы тут один вариант - через хелфчек из докера.
            // Fanout т.к игнорирует router-key, он не нужОн
            channel.ExchangeDeclare("saver", ExchangeType.Fanout, true, false);
            channel.QueueDeclare("saver_queue", true, false, false);
            channel.QueueBind("saver_queue", "saver", string.Empty);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Registered += async (s, e) =>
            { logger.LogInformation("Consumer registered"); };
            consumer.Received += Handle;
            channel.BasicConsume("saver_queue", false, consumer);
        }

        private async Task Handle(object sender, BasicDeliverEventArgs e)
        {
            // Да, разумеется нужно проверять тип переданного мессаджа. Вообще, я бы для этого дела выделил очередь (отдельную). Ну либо routeKey.
            if (sender is not AsyncEventingBasicConsumer c) return;
            await using var scope = _factory.CreateAsyncScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<EventHostedService>>();
            var storage = scope.ServiceProvider.GetRequiredService<IDicomStorage>();

            try
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                await storage.SaveFromJson(message);
            }
            // Вообще я не сторонник внутреннего try catch, лучше через пайплайн. Но не в этом случае.
            catch { logger.LogError("Message revoked at: {0}", DateTime.UtcNow); c.Model.BasicNack(e.DeliveryTag, multiple: false, requeue: true); }
            finally { logger.LogInformation("Message handled at: {0}", DateTime.UtcNow); c.Model.BasicAck(e.DeliveryTag, multiple: false); }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }
    }
}
