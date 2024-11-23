// Entities/Message.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace API.Entities
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Content { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
