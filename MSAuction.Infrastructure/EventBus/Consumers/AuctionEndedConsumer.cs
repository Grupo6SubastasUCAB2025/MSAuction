using MongoDB.Driver;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using MSAuction.Commons.DTOs;
using MSAuction.Infrastructure.EventBus.Events;

namespace MSAuction.Infrastructure.EventBus.Consumers;

public class AuctionEndedConsumer
{
    private readonly IMongoCollection<MongoAuction> _collection;

    public AuctionEndedConsumer(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("Subastas2025");
        _collection = database.GetCollection<MongoAuction>("Auctions");
    }

    public void StartListening()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "auction_ended_queue", durable: false, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var endedEvent = JsonConvert.DeserializeObject<AuctionEndedEvent>(message);

            var filter = Builders<MongoAuction>.Filter.Eq(a => a.AuctionId, endedEvent.AuctionId);
            var update = Builders<MongoAuction>.Update.Set(a => a.Status, "finalizada");

            await _collection.UpdateOneAsync(filter, update);
        };

        channel.BasicConsume(queue: "auction_ended_queue", autoAck: true, consumer: consumer);
        Console.ReadLine();
    }
}