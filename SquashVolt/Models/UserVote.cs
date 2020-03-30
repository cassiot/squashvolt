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
        public string VideoId { get; set; }
        public string UserId { get; set; }
        public int RefereeDecision { get; set; }
        public int UserDecision { get; set; }
        public DateTime Date { get; set; }
    }
}
