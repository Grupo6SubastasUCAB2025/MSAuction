using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using MSAuction.Commons.DTOs;
using MSAuction.Infrastructure.EventBus.Events;

namespace MSAuction.Infrastructure.EventBus.Consumers;

public class AuctionCreatedBackgroundService : BackgroundService
    {
        private readonly IMongoCollection<MongoAuction> _collection;
        private IModel? _channel;
        private IConnection? _connection;

        public AuctionCreatedBackgroundService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Subastas2025");
            _collection = database.GetCollection<MongoAuction>("Auctions");

            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "auction_created_queue", durable: false, exclusive: false, autoDelete: false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var createdEvent = JsonConvert.DeserializeObject<AuctionCreatedEvent>(message);

                if (createdEvent != null)
                {
                    var mongoAuction = new MongoAuction
                    {
                        AuctionId = createdEvent.AuctionId,
                        ProductId = createdEvent.ProductId,
                        Title = createdEvent.Title,
                        Description = createdEvent.Description,
                        ReservePrice = createdEvent.ReservePrice,
                        StartDate = createdEvent.StartDate,
                        EndDate = createdEvent.EndDate,
                        Status = createdEvent.Status,
                        Type = createdEvent.Type,
                        UserId = createdEvent.UserId
                    };

                    await _collection.InsertOneAsync(mongoAuction);
                }
            };

            _channel.BasicConsume(queue: "auction_created_queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }