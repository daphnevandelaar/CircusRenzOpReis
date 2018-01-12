using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DataLayer
{
    public class AnimalDB
    {
        static private IMongoCollection<AnimalBSON> AnimalCollection()
        {
            var client = new MongoClient("mongodb://daphnevandelaar:Daphne199$@circusrenz-shard-00-00-l9drs.mongodb.net:27017,circusrenz-shard-00-01-l9drs.mongodb.net:27017,circusrenz-shard-00-02-l9drs.mongodb.net:27017/test?ssl=true&replicaSet=CircusRenz-shard-0&authSource=admin");
            IMongoDatabase db = client.GetDatabase("CircusRenz");

            return db.GetCollection<AnimalBSON>("Animal");
        }

        public IQueryable GetAll()
        {
            return AnimalCollection().AsQueryable();
        }

        public void Insert(AnimalBSON animal)
        {
            AnimalCollection().InsertOneAsync(animal);
        }
    }
}
