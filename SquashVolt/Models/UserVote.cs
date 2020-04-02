using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquashVolt.Models
{
    public class UserVote
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("matchId")]
        public string MatchId { get; set; }
        
        [BsonElement("shotNumber")]
        public int ShotNumber { get; set; }
        
        [BsonElement("userDecision")]
        public int UserDecision { get; set; }
        
        [BsonElement("date")]
        public DateTime Date { get; set; }
    }
}
