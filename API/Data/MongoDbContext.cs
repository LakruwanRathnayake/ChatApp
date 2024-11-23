using MongoDB.Driver;
using API.Entities;
using Microsoft.Extensions.Configuration;

namespace API.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _database = client.GetDatabase("datingappDB");
        }

        // Collection for Users
        public IMongoCollection<AppUser> Users => _database.GetCollection<AppUser>("Users");

        // Collection for Counters
        public IMongoCollection<Counter> Counters => _database.GetCollection<Counter>("Counters");


        public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");

        // Method to get the next auto-incremented value for the Id
        public int GetNextSequenceValue(string collectionName)
        {
            var filter = Builders<Counter>.Filter.Eq(c => c.Id, collectionName);
            var update = Builders<Counter>.Update.Inc(c => c.SequenceValue, 1);
            var options = new FindOneAndUpdateOptions<Counter>
            {
                ReturnDocument = ReturnDocument.After, // Return the updated document after incrementing
                IsUpsert = true // Create the counter document if it doesn't exist
            };

            var counter = _database.GetCollection<Counter>("Counters")
                                   .FindOneAndUpdate(filter, update, options);

            return counter?.SequenceValue ?? 1;
        }
    }
}
