using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DataLayer
{
    public class AnimalBSON
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Type")]
        public bool Type { get; set; } //false herbivore true carnivore

        [BsonElement("Size")]
        public int Size { get; set; }


        MongoClient client = new MongoClient("mongodb://circusrenz-shard-00-00-l9drs.mongodb.net:27017");

        public void InsertAnimal()
        {
            
        }
    }
}
