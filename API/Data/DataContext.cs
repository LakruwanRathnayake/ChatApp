using MongoDB.Driver;
using API.Entities;

namespace API.Data
{
    public class DataContext
    {
        private readonly IMongoDatabase _database;

        public DataContext(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            _database = client.GetDatabase("ChatDatingApp"); // Database name
        }

        public IMongoCollection<AppUser> Users => _database.GetCollection<AppUser>("Users");
    }
}
