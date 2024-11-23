using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    public class AppUser
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty; // Correct Property Name

        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }

        // Additional fields
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Country { get; set; }
        public string? Gender { get; set; }
        public byte[]? ProfilePhoto { get; set; }
    }
}
