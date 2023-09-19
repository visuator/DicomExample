

using FellowOakDicom;

using RabbitMQ.Client;

using System.Text;
using System.Text.Json;

namespace DicomExample.Services
{
    public class StudyService : IStudyService
    {
        // Да, я знаю про существование MassTransit, EasyNetQ и прочих. Базовый клиент выбран для того, чтобы показать владение типичным функционалом Rabbit-а.
        private readonly IModel _channel;
        // Это один из вариантов реализации - паттерн стратегия. Второй вариант реализации - шаблонный метод - т.е abstract (virtual) метод, объявленный в StudyService, который переопределяется "конкретным" подтипом... например ParseDataset. 
        // Не, ну можно конечно натянуть публикацию события в микросервис парсера, а потом в микросервис-exporer данных, но это уже дичь)))
        // p.s по большей части можно вообще без стратегии обойтись, но даже так навскидку рано или поздно понадобится не только Uid
        private readonly IStudyReaderStrategy _studyReader;
        private readonly ILogger<StudyService> _logger;

        public StudyService(IModel channel, IStudyReaderStrategy studyReader, ILogger<StudyService> logger)
        {
            _channel = channel;
            _studyReader = studyReader;
            _logger = logger;
        }

        public async Task Save(Stream stream, CancellationToken token = default)
        {
            // Fanout т.к игнорирует router-key, он не нужОн
            _channel.ExchangeDeclare("saver", ExchangeType.Fanout, true, false);
            _channel.QueueDeclare("saver_queue", true, false, false);
            _channel.QueueBind("saver_queue", "saver", string.Empty);
            // с dicom не знакомился вообще - за использование либы прошу понять и простить :D
            var file = await DicomFile.OpenAsync(stream);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_studyReader.GetValues(file.Dataset)));
            _channel.BasicPublish(exchange: "saver", routingKey: string.Empty, body: body);
            _logger.LogInformation("Message published at: {0}", DateTime.UtcNow);
        }
    }
}
