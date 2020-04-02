using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver;
using SquashVolt.Models;
using System.Linq;

namespace SquashVolt
{
    public class PostUserVote
    {
        private readonly IMongoDatabase db;

        public PostUserVote(MongoClient mongo)
        {
            db = mongo.GetDatabase("SquashVoltDB");
        }

        [FunctionName("Vote")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/vote")] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var voteDTO = JsonConvert.DeserializeObject<UserVoteDTO>(requestBody);

            var vote = new UserVote()
            {
                Date = DateTime.UtcNow,
                UserDecision = voteDTO.UserDecision,
                MatchId = voteDTO.MatchId,
                ShotNumber = voteDTO.ShotNumber
            };

            var userVoteCollection = db.GetCollection<UserVote>("UserVotes");
            userVoteCollection.InsertOne(vote);

            return new OkResult();
        }

        public class UserVoteDTO
        {
            public int UserDecision { get; set; }
            public string MatchId { get; set; }
            public int ShotNumber { get; set; }
        }
    }
}
