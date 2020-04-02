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
    //public static class Function1
    //{
    //    [FunctionName("Function1")]
    //    public static async Task<IActionResult> Run(
    //        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    //        ILogger log)
    //    {
    //        log.LogInformation("C# HTTP trigger function processed a request.");

    //        string name = req.Query["name"];

    //        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    //        dynamic data = JsonConvert.DeserializeObject(requestBody);
    //        name = name ?? data?.name;

    //        return name != null
    //            ? (ActionResult)new OkObjectResult($"Hello, {name}")
    //            : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
    //    }
    //}

    //public class GetVideoById
    //{
    //    private readonly IMongoDatabase db;

    //    public GetVideoById(MongoClient mongo)
    //    {
    //        db = mongo.GetDatabase("SquashVoltDB");
    //    }

    //    [FunctionName("GetById")]
    //    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "videos/{id}")] HttpRequest req, string id, ILogger log)
    //    {
    //        var video = db.GetCollection<Video>("Videos").Find(v => v.Id == id).SingleOrDefault();

    //        return (ActionResult)new OkObjectResult(video);
    //    }
    //}

    public class GetMatch
    {
        private readonly IMongoDatabase db;

        public GetMatch(MongoClient mongo)
        {
            db = mongo.GetDatabase("SquashVoltDB");
        }

        [FunctionName("GetMatch")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "matches")] HttpRequest req, ILogger log)
        {
            var matches = await db.GetCollection<Match>("Matches").Find(v => true).ToListAsync();

            var rng = new Random();
            var match = matches.ElementAt(rng.Next(matches.Count()));

            var votes = await db.GetCollection<UserVote>("UserVotes").Find(uv => uv.MatchId == match.Id).ToListAsync();

            var dic = new Dictionary<int, int[]>();

            foreach (var vote in votes)
            {
                if (dic.ContainsKey(vote.ShotNumber) == false)
                    dic.Add(vote.ShotNumber, new int[3]);

                dic[vote.ShotNumber][vote.UserDecision]++;
            }

            var ret = new MatchDTO()
            {
                Id = match.Id,
                YouTubeId = match.YouTubeId,
                Shots = new List<MatchDTO.Shot>()
            };

            foreach (var shot in match.Shots)
            {
                var shotDTO = new MatchDTO.Shot()
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

        public class MatchDTO
        {
            public string Id { get; set; }

            public string YouTubeId { get; set; }

            public IList<Shot> Shots { get; set; }

            public class Shot
            {
                public int Number { get; set; }

                public int Time { get; set; }

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
}
