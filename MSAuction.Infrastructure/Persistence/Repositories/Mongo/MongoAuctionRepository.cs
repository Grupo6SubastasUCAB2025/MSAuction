using MSAuction.Interfaces.Persistence.Repositories.Mongo;
using MSAuction.Commons.DTOs;
using MongoDB.Driver;

namespace MSAuction.Infrastructure.Persistence.Repositories.Mongo;

public class MongoAuctionRepository : IMongoAuctionRepository
{
    private readonly IMongoCollection<MongoAuction> _collection;

    public MongoAuctionRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("Subastas2025"); // Nombre de tu base MongoDB
        _collection = database.GetCollection<MongoAuction>("Auctions");
    }

    public async Task AddAsync(MongoAuction auction)
    {
        await _collection.InsertOneAsync(auction);
    }

    public async Task UpdateAsync(MongoAuction auction)
    {
        var filter = Builders<MongoAuction>.Filter.Eq(a => a.AuctionId, auction.AuctionId);
        await _collection.ReplaceOneAsync(filter, auction);
    }

    public async Task DeleteAsync(int auctionId)
    {
        var filter = Builders<MongoAuction>.Filter.Eq(a => a.AuctionId, auctionId);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<MongoAuction> GetByIdAsync(int auctionId)
    {
        var filter = Builders<MongoAuction>.Filter.Eq(a => a.AuctionId, auctionId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}