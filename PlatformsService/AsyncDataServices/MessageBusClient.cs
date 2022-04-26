using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
  private readonly IConfiguration _configutation;
  private readonly ILogger<MessageBusClient> _logger;
  private readonly IConnection _connection;
  private readonly IModel _channel;

  public MessageBusClient(IConfiguration configutation,
    ILogger<MessageBusClient> logger)
  {
    _logger = logger;
    _configutation = configutation;

    var factory = new ConnectionFactory()
    {
      HostName = _configutation["RabbitMQHost"],
      Port = int.Parse(_configutation["RabbitMQPort"])
    };

    try
    {
      _connection = factory.CreateConnection();
      _channel = _connection.CreateModel();
      _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
      _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

      _logger.LogInformation("--> Connected to MessageBus");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "--> Could not connect to the Message Bus.");
    }
  }

  private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
  {
    _logger.LogInformation("--> RabbitMQ connection has shutdown.");
  }

  private void SendMessage(string message)
  {
    if (_connection.IsOpen)
    {
      _logger.LogInformation("--> RabbitMQ Connection open, sending message...");
      var body = Encoding.UTF8.GetBytes(message);
      _channel.BasicPublish(
        exchange: "trigger",
        routingKey: "",
        basicProperties: null,
        body: body);

      _logger.LogInformation($"--> We have sent {message}");
    }
    else
    {
      _logger.LogError("--> RabbitMQ connection not open, not sending");
    }
  }

  public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
  {
    var message = JsonSerializer.Serialize(platformPublishedDto);
    SendMessage(message);
  }

  public void Dispose()
  {
    if (_channel.IsOpen)
    {
      _channel.Close();
      _connection.Close();
    }
  }
}
