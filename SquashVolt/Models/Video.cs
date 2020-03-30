using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquashVolt.Models
{
    public class Video
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Index { get; set; }

        public string MatchId { get; set; }
        public string YouTubeId { get; set; }

        /// <summary>
        /// 0 - Let
        /// 1 - Stroke
        /// 2 - No Let
        /// </summary>
        public int RefereeDecision { get; set; }
    }
}
