using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    public class Counter
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = string.Empty;

        public int SequenceValue { get; set; } // This will hold the counter value
    }
}
