
// Services/MessageService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using MongoDB.Driver;

namespace API.Services
{
    public class MessageService
    {
        private readonly MongoDbContext _context;

        public MessageService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task SendMessageAsync(Message message)
        {
            await _context.Messages.InsertOneAsync(message);
        }

       public async Task<List<Message>> GetMessagesAsync(string userId, string recipientId)
        {
            var filter = Builders<Message>.Filter.Or(
                Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(m => m.SenderId, userId),
                    Builders<Message>.Filter.Eq(m => m.RecipientId, recipientId)
                ),
                Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(m => m.SenderId, recipientId),
                    Builders<Message>.Filter.Eq(m => m.RecipientId, userId)
                )
            );

            return await _context.Messages.Find(filter).SortBy(m => m.Timestamp).ToListAsync();
        }


       public async Task<List<AppUserUpdateDto>> GetPreviousConversationsAsync(string userId)
        {
            // Query the Messages collection for conversations involving the given userId
            var messages = await _context.Messages
                .Find(Builders<Message>.Filter.Or(
                    Builders<Message>.Filter.Eq(m => m.SenderId, userId),
                    Builders<Message>.Filter.Eq(m => m.RecipientId, userId)
                ))
                .ToListAsync();

            // Extract unique user IDs (opposite of userId in the conversation)
            var userIds = messages
                .Select(m => m.SenderId == userId ? m.RecipientId : m.SenderId) // Get the opposite user ID
                .Distinct() // Remove duplicates
                .ToList();

            // Convert userIds to integers (if they are valid)
            var userIdsInt = userIds
                .Where(id => int.TryParse(id, out _)) // Only include valid integers
                .Select(int.Parse)
                .ToList();

            // Query the Users collection to fetch details of unique users
            var users = await _context.Users
                .Find(Builders<AppUser>.Filter.In(u => u.Id, userIdsInt))
                .Project(u => new AppUserUpdateDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Country = u.Country,
                    Gender = u.Gender
                })
                .ToListAsync();

            return users;
        }


    }
}
