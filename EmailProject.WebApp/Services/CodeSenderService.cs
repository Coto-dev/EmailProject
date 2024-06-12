using System.Text;
using System.Text.Json;
using EmailProject.WebApp.DTOs;
using Microsoft.Extensions.Options;

namespace EmailProject.WebApp.Services;
using RabbitMQ.Client;

public interface ICodeSenderService {
    void SendCode(RabbitMqMessageDto dto);
}

public class CodeSenderService : ICodeSenderService {
    private readonly IOptions<RabbitMqConfiguration> _configuration;

    public CodeSenderService(
        IOptions<RabbitMqConfiguration> configuration)
    {
        _configuration = configuration;
    }

    public void SendCode(RabbitMqMessageDto dto)
    {
        var factory = new ConnectionFactory() { HostName = _configuration.Value.HostName };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _configuration.Value.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var message = JsonSerializer.Serialize(dto);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
            routingKey: _configuration.Value.QueueName,
            basicProperties: null,
            body: body);

    }
}