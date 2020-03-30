using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: WebJobsStartup(typeof(SquashVolt.Startup))]

namespace SquashVolt
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            var config = (IConfiguration)builder.Services.First(d => d.ServiceType == typeof(IConfiguration)).ImplementationInstance;

            builder.Services.AddSingleton((s) =>
            {
                //var client = new MongoClient(config["MONGO_CONNECTION_STRING"]);

                var client = new MongoClient("mongodb+srv://cvt:Vaquinha.54@squashvoltcluster-vwgma.azure.mongodb.net/squashvoltdb?retryWrites=true&w=majority&connectTimeoutMS=30000&socketTimeoutMS=30000&waitQueueTimeoutMS=30000&maxIdleTimeMS=45000");
                //var database = client.GetDatabase("test");

                return client;
            });
        }
    }
}
