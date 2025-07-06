using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using MSAuction.Infrastructure.EventBus.Events;
using MSAuction.Commons.DTOs;
    

namespace MSAuction.Infrastructure.EventBus.Consumers;

public class AuctionUpdatedConsumer
{
    private readonly IMongoCollection<MongoAuction> _auctionCollection;

    public AuctionUpdatedConsumer(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("Subastas2025"); // Nombre de tu base MongoDB
        _auctionCollection = database.GetCollection<MongoAuction>("Auctions");
        Console.WriteLine("Mongo");
    }

    public void StartListening()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "auction_updated_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var auctionUpdatedEvent = JsonConvert.DeserializeObject<AuctionUpdatedEvent>(message);

                // Actualizar el evento en MongoDB
                var updateDefinition = Builders<MongoAuction>.Update
                    .Set(a => a.Title, auctionUpdatedEvent.Title)
                    .Set(a => a.InitialPrice, auctionUpdatedEvent.InitialPrice)
                    .Set(a => a.EndDate, auctionUpdatedEvent.EndDate)
                    .Set(a => a.Status, auctionUpdatedEvent.Status);

                var filter = Builders<MongoAuction>.Filter.Eq(a => a.AuctionId, auctionUpdatedEvent.AuctionId);
                await _auctionCollection.UpdateOneAsync(filter, updateDefinition);
            };

            channel.BasicConsume(queue: "auction_updated_queue", autoAck: true, consumer: consumer);

            // Para mantener la aplicación escuchando indefinidamente
            Console.ReadLine();
        }
    }
}