namespace TechChallenge.Common.SDK.RabbitMQ;

public interface IRabbitMqService
{
    Task PublishMessageAsync(string message, string queueName, CancellationToken cancellationToken);
    Task<string?> ReceiveMessageAsync(string message, string queueName, CancellationToken cancellationToken);
}