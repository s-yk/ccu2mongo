using CcuCore;
using CommandLine;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ccu2mongo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async opt => { 
                    var results = await new TypeOnlyAnalyzer().AnalyzeToDict(opt.SolutionPath);
                    var client = GetClient();
                    var db = client.GetDatabase("ccu");
                    var collection = db.GetCollection<BsonDocument>("ccu");

                    var models = collection.BulkWrite(results.Select(r => new InsertOneModel<BsonDocument>(new BsonDocument(r))).ToList(),
                        new BulkWriteOptions { IsOrdered = true });
                });
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            Environment.Exit(0);
        }

        static MongoClient GetClient()
        {
            var host = Environment.GetEnvironmentVariable("CCU2MONGO_MONGO_HOST");
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new Exception("Empty: CCU2MONGO_MONGO_HOST");
            }

            var port = Environment.GetEnvironmentVariable("CCU2MONGO_MONGO_PORT");
            if (string.IsNullOrWhiteSpace(port))
            {
                throw new Exception("Empty: CCU2MONGO_MONGO_PORT");
            }

            var user = Environment.GetEnvironmentVariable("CCU2MONGO_MONGO_USER");
            if (string.IsNullOrWhiteSpace(user))
            {
                throw new Exception("Empty: CCU2MONGO_MONGO_USER");
            }

            var pwd = Environment.GetEnvironmentVariable("CCU2MONGO_MONGO_PASSWORD");
            if (string.IsNullOrWhiteSpace(pwd))
            {
                throw new Exception("Empty: CCU2MONGO_MONGO_PASSWORD");
            }

            return new MongoClient($"mongodb://{user}:{pwd}@{host}:{port}/ccu");
        }
    }
}
