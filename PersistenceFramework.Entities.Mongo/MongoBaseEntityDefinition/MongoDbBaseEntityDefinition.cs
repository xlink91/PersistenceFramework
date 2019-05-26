using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PersistenceFramework.Entities.BaseEntityContract;
using PersistenceFramework.Entities.NoSQL.Mongo.MongoKeyDefinition;

namespace PersistenceFramework.Entities.NoSQL.Mongo.MongoBaseEntityDefinition
{
    public class MongoDbBaseEntityDefinition : IBaseEntity<MongoDbKeyHandlerDefinition, ObjectId>
    {
        private readonly MongoDbKeyHandlerDefinition KeyHandler = new MongoDbKeyHandlerDefinition();
        public ObjectId Id { get; set; }
        [BsonIgnore]
        public string KeyValue
        {
            get
            {
                return KeyHandler.KeyValueToString(Id);
            }
            set
            {
                Id = KeyHandler.ParseValue(value);
            }
        }
    }
}
