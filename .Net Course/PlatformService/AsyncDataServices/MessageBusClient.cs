using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataService;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _config;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration config)
    {
        _config = config;
        var factory = new ConnectionFactory() { HostName = _config["RabbitMQHost"], Port = int.Parse(_config["RabbitMQPort"])};

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_Connection_ConnectionShutdown;

            Console.WriteLine("--> Connected to message bus");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the message bus {ex.Message}");
        }
    }
    public void PublishNewPlatform(PlatformPublishDto platformPublishDto)
    {
        var message = JsonSerializer.Serialize(platformPublishDto);

        if(_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ connection open, sending message...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ connection is closed, not sending");
        }
    }

    // Because we may reuse the logik off sending a message, we make this into a method for reuse. 
    // We may need it for other publish methods.
    // This method needs to be generic
    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        // We declared this [ _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout); ]
        // which means we created a exchange with the name "trigger".
        // Because we are using fanout exchange we dont care about the routingkey, so that satys empty because we still need to supply it.
        _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);

        Console.WriteLine($"--> We have sent {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("MessageBus Disposed");
        if(_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }

    private void RabbitMQ_Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine($"--> RabbitMQ connection shutdown");
    }
}
