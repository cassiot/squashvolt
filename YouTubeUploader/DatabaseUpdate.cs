using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeUploader
{
    public class DatabaseUpdate
    {
        IMongoDatabase db;

        public DatabaseUpdate()
        {
            var mongo = new MongoClient("mongodb+srv://cvt:Vaquinha.54@squashvoltcluster-vwgma.azure.mongodb.net/squashvoltdb?retryWrites=true&w=majority&connectTimeoutMS=30000&socketTimeoutMS=30000&waitQueueTimeoutMS=30000&maxIdleTimeMS=45000");
            db = mongo.GetDatabase("SquashVoltDB");
        }

        public string CreateMatch(FileDetail fileDetail)
        {
            var matchesCollection = db.GetCollection<Match>("Matches");

            var matchFound = matchesCollection.Find(m => m.Title == fileDetail.Title).SingleOrDefault();

            if (matchFound != null)
                return matchFound.Id;

            var match = new Match()
            {
                Title = fileDetail.Title,
                Player1 = fileDetail.Player1,
                Player2 = fileDetail.Player2,
                Tournment = fileDetail.Tournment,
                Year = fileDetail.Year
            };

            matchesCollection.InsertOne(match);

            return match.Id;
        }

        public void CreateVideo(FileDetail fileDetail, string matchId, string youtubeId)
        {
            var videosCollection = db.GetCollection<Video>("Videos");

            var videoExists = videosCollection.Find(v => v.YouTubeId == youtubeId).Any();

            if (videoExists)
                return;

            var video = new Video()
            {
                Index = fileDetail.Index,
                MatchId = matchId,
                RefereeDecision = fileDetail.RefereeDecision,
                YouTubeId = youtubeId
            };

            videosCollection.InsertOne(video);
        }

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

        public class Video
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            public string MatchId { get; set; }
            public int RefereeDecision { get; set; }
            public int Index { get; set; }
            public string YouTubeId { get; set; }
        }
    }
}
