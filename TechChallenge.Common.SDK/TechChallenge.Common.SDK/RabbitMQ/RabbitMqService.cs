using MassTransit;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using System.Text;

namespace TechChallenge.Common.SDK.RabbitMQ;
public class RabbitMqService : IRabbitMqService
{
    private readonly RabbitMQSetting _rabbitMqSetting;
    private readonly ConnectionFactory _factory;
    private readonly AsyncPolicy _retryPolicy;

    public RabbitMqService(RabbitMQSetting rabbitMqSetting)
    {
        _rabbitMqSetting = rabbitMqSetting;
        _factory = new ConnectionFactory
        {
            HostName = _rabbitMqSetting.HostName,
            UserName = _rabbitMqSetting.UserName,
            Password = _rabbitMqSetting.Password
        };

        _retryPolicy = Policy
            .Handle<Exception>()
            .Or<PublishException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public async Task PublishMessageAsync(string message, string queueName, CancellationToken cancellationToken)
    {
        using var connection = await _factory.CreateConnectionAsync(cancellationToken);
        using var channel = await connection.CreateChannelAsync();
        await _retryPolicy.ExecuteAsync(async () =>
        {
            await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);
            var props = new BasicProperties();
            props.ContentType = "text/plain";

            await channel.BasicPublishAsync(exchange: "", routingKey: queueName, mandatory: false, basicProperties: props, body: body, cancellationToken: cancellationToken);

        });
    }

    public async Task<string?> ReceiveMessageAsync(string message, string queueName, CancellationToken cancellationToken)
    {
        using var connection = await _factory.CreateConnectionAsync(cancellationToken);
        using var channel = await connection.CreateChannelAsync();
        await _retryPolicy.ExecuteAsync(async () =>
        {
            await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var result = await channel.BasicGetAsync(message, true, cancellationToken);
            return result != null ? Encoding.UTF8.GetString(result.Body.ToArray()) : null;
        });

        return null;
    }
}
