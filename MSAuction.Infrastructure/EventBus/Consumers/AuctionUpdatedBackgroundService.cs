using System.Text;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MSAuction.Commons.DTOs;
using MSAuction.Infrastructure.EventBus.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MSAuction.Infrastructure.EventBus.Consumers;

public class AuctionUpdatedBackgroundService : BackgroundService
    {
        private readonly IMongoCollection<MongoAuction> _collection;
        private IModel? _channel;
        private IConnection? _connection;

        public AuctionUpdatedBackgroundService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Subastas2025");
            _collection = database.GetCollection<MongoAuction>("Auctions");

            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "auction_updated_queue", durable: false, exclusive: false, autoDelete: false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var updatedEvent = JsonConvert.DeserializeObject<AuctionUpdatedEvent>(message);

                if (updatedEvent != null)
                {
                    var filter = Builders<MongoAuction>.Filter.Eq(a => a.AuctionId, updatedEvent.AuctionId);

                    var update = Builders<MongoAuction>.Update
                        .Set(a => a.Title, updatedEvent.Title)
                        .Set(a=>a.ProductId, updatedEvent.ProductId)
                        .Set(a => a.Description, updatedEvent.Description)
                        .Set(a => a.InitialPrice, updatedEvent.InitialPrice)
                        .Set(a => a.MinIncrement, updatedEvent.MinIncrement)
                        .Set(a => a.ReservePrice, updatedEvent.ReservePrice);

                    await _collection.UpdateOneAsync(filter, update);
                }
            };

            _channel.BasicConsume(queue: "auction_updated_queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }