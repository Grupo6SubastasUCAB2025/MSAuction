using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

namespace MSAuction.Infrastructure.EventBus.Events;

public class AuctionEventPublisher
{
    public void PublishAuctionUpdatedEvent(AuctionUpdatedEvent auctionUpdatedEvent)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "auction_updated_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var json = JsonConvert.SerializeObject(auctionUpdatedEvent);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: "", routingKey: "auction_updated_queue", basicProperties: null, body: body);
    }
}