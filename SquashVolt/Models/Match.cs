using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace SquashVolt.Models
{
    public class Match
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("youTubeId")]
        public string YouTubeId { get; set; }

        [BsonElement("shots")]
        public IList<Shot> Shots { get; set; }
    }

    public class Shot
    {
        [BsonElement("number")]
        public int Number { get; set; }
        
        [BsonElement("time")]
        public int Time { get; set; }
    }
}
