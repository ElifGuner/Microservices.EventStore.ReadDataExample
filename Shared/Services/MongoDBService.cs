using MongoDB.Driver;
using Shared.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class MongoDBService : IMongoDBService
    {
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            IMongoDatabase database = GetDatabase();
            return database.GetCollection<T>(collectionName);
        }

        public IMongoDatabase GetDatabase(string databaseName="ProductDB", string connectionString = "mongodb://localhost:27017")
        {
            MongoClient mongoClient = new(connectionString);
            return mongoClient.GetDatabase(databaseName);
        }
    }
}
