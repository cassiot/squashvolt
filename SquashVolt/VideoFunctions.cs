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

    //public class GetAllVideosFromMatch
    //{
    //    private readonly IMongoDatabase db;

    //    public GetAllVideosFromMatch(MongoClient mongo)
    //    {
    //        db = mongo.GetDatabase("SquashVoltDB");
    //    }

    //    [FunctionName("Get")]
    //    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "videos")] HttpRequest req, ILogger log)
    //    {
    //        var matchesQuery = await db.GetCollection<Match>("Matches").FindAsync(v => true);
    //        var matches = await matchesQuery.ToListAsync();

    //        var rng = new Random();
    //        var match = matches.ElementAt(rng.Next(matches.Count()));

    //        var videos = await db.GetCollection<Video>("Videos").Find(v => v.MatchId == match.Id).SortBy(v=> v.Index).ToListAsync();

    //        return (ActionResult)new OkObjectResult(videos);
    //    }
    //}
}
