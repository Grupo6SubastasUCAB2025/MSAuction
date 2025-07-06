using MongoDB.Driver;
using MSAuction.Commons.DTOs;
using MSAuction.Infrastructure.EventBus.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;


namespace MSAuction.Infrastructure.EventBus.Consumers;

public class AuctionCreatedConsumer
{
    private readonly IMongoCollection<MongoAuction> _auctionCollection;

    public AuctionCreatedConsumer(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("Subastas2025");

        _auctionCollection = database.GetCollection<MongoAuction>("Auctions");
    }

    public void StartListening()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "auction_created_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var auctionCreatedEvent = JsonConvert.DeserializeObject<AuctionCreatedEvent>(message);

                // Inserta el evento en MongoDB
                var mongoAuction = new MongoAuction
                {
                    AuctionId = auctionCreatedEvent.AuctionId,
                    ProductId = auctionCreatedEvent.ProductId,
                    Title = auctionCreatedEvent.Title,
                    Description = auctionCreatedEvent.Description,
                    InitialPrice = auctionCreatedEvent.InitialPrice,
                    MinIncrement = auctionCreatedEvent.MinIncrement,
                    ReservePrice = auctionCreatedEvent.ReservePrice,
                    StartDate = auctionCreatedEvent.StartDate,
                    EndDate = auctionCreatedEvent.EndDate,
                    Status = auctionCreatedEvent.Status,
                    Conditions = auctionCreatedEvent.Conditions,
                    Type = auctionCreatedEvent.Type,
                    UserId = auctionCreatedEvent.UserId
                };

                await _auctionCollection.InsertOneAsync(mongoAuction);
            };

            channel.BasicConsume(queue: "auction_created_queue", autoAck: true, consumer: consumer);

            // Para mantener la aplicación escuchando indefinidamente
            Console.ReadLine();
        }
    }
}