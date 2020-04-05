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
using System.Dynamic;
using System.Collections.Generic;

namespace SquashVolt
{
    public class GetMatch
    {
        private readonly IMongoDatabase db;

        public GetMatch(MongoClient mongo)
        {
            db = mongo.GetDatabase("SquashVoltDB");
        }

        [FunctionName("GetMatch")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "matches/{id}")] HttpRequest req, string id, ILogger log)
        {
            var match = await db.GetCollection<Match>("Matches").Find(v => v.Id == id).SingleOrDefaultAsync();

            var votes = await db.GetCollection<UserVote>("UserVotes").Find(uv => uv.MatchId == match.Id).ToListAsync();

            var dic = new Dictionary<int, int[]>();

            foreach (var vote in votes)
            {
                if (dic.ContainsKey(vote.ShotNumber) == false)
                    dic.Add(vote.ShotNumber, new int[3]);

                dic[vote.ShotNumber][vote.UserDecision]++;
            }

            var ret = new GetMatchDTO()
            {
                Id = match.Id,
                YouTubeId = match.YouTubeId,
                Shots = new List<GetMatchDTO.Shot>()
            };

            foreach (var shot in match.Shots)
            {
                var shotDTO = new GetMatchDTO.Shot()
                {
                    Number = shot.Number,
                    Time = shot.Time,
                };

                var totalVotes = dic.ContainsKey(shot.Number) ? dic[shot.Number][0] + dic[shot.Number][1] + dic[shot.Number][2] : 0;

                if (totalVotes > 0)
                {
                    shotDTO.TotalVotes = totalVotes;

                    shotDTO.LetVotes = dic[shot.Number][0];
                    shotDTO.StrokeVotes = dic[shot.Number][1];
                    shotDTO.NoLetVotes = dic[shot.Number][2];

                    shotDTO.LetPercentual = dic[shot.Number][0] / totalVotes;
                    shotDTO.StrokePercentual = dic[shot.Number][1] / totalVotes;
                    shotDTO.NoLetPercentual = dic[shot.Number][2] / totalVotes;
                }

                ret.Shots.Add(shotDTO);
            }

            return new OkObjectResult(ret);
        }

        public class GetMatchDTO
        {
            public string Id { get; set; }

            public string YouTubeId { get; set; }

            public IList<Shot> Shots { get; set; }

            public class Shot
            {
                public int Number { get; set; }

                public decimal Time { get; set; }

                public int LetVotes { get; set; }

                public int LetPercentual { get; set; }

                public int StrokeVotes { get; set; }

                public int StrokePercentual { get; set; }

                public int NoLetVotes { get; set; }

                public int NoLetPercentual { get; set; }

                public int TotalVotes { get; set; }
            }
        }
    }

    public class GetMatches
    {
        private readonly IMongoDatabase db;

        public GetMatches(MongoClient mongo)
        {
            db = mongo.GetDatabase("SquashVoltDB");
        }

        [FunctionName("GetMatches")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "matches")] HttpRequest req, ILogger log)
        {
            var matches = await db.GetCollection<Match>("Matches").Find(v => true).ToListAsync();

            var matchesDTO = matches.Select(m => new GetMatchesDTO 
            {
                Id = m.Id,
                YouTubeId = m.YouTubeId,
                IsFullMatch = m.IsFullMatch
            });

            return new OkObjectResult(matchesDTO);
        }

        public class GetMatchesDTO
        {
            public string Id { get; set; }

            public string YouTubeId { get; set; }

            public bool IsFullMatch { get; set; }
        }
    }
}
