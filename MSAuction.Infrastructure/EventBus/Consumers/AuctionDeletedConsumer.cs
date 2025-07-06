using MongoDB.Driver;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;
using MSAuction.Commons.DTOs;
using MSAuction.Infrastructure.EventBus.Events;

namespace MSAuction.Infrastructure.EventBus.Consumers;

public class AuctionDeletedConsumer
{
    private readonly IMongoCollection<MongoAuction> _auctionCollection;

    public AuctionDeletedConsumer(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("Subastas2025");
        _auctionCollection = database.GetCollection<MongoAuction>("Auctions");
    }

    public void StartListening()
    {
        Console.WriteLine("pase1");
        var factory = new ConnectionFactory() { HostName = "localhost" };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "auction_deleted_queue", durable: false, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("pase2");
            var deletedEvent = JsonConvert.DeserializeObject<AuctionDeletedEvent>(message);

            // Verificar que la subasta exista en Mongo
            var filter = Builders<MongoAuction>.Filter.Eq(a => a.AuctionId, deletedEvent.AuctionId);

            // Actualizar el estado a "eliminada"
            var update = Builders<MongoAuction>.Update.Set(a => a.Status, "eliminada");

            // Ejecutar la actualización en MongoDB
            var result = await _auctionCollection.UpdateOneAsync(filter, update);

            // Puedes agregar un log para verificar cuántos documentos fueron modificados
            Console.WriteLine($"Subasta {deletedEvent.AuctionId} marcada como 'eliminada'. Resultados modificados: {result.ModifiedCount}");
        };

        channel.BasicConsume(queue: "auction_deleted_queue", autoAck: true, consumer: consumer);

        // Mantener la aplicación escuchando indefinidamente
        Console.ReadLine();
    }
}