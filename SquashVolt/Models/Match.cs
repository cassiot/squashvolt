using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SquashVolt.Models
{
    public class Match
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string Tournment { get; set; }
        public int Year { get; set; }
    }
}
