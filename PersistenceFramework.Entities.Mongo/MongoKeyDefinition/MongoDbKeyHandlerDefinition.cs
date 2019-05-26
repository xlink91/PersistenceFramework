using MongoDB.Bson;
using PersistenceFramework.Entities.BaseEntityContract;

namespace PersistenceFramework.Entities.NoSQL.Mongo.MongoKeyDefinition
{
    public class MongoDbKeyHandlerDefinition : IKeyHandlerDefinition<ObjectId>
    {
        public string KeyValueToString(ObjectId KeyValue)
        {
            return KeyValue.ToString();
        }

        public ObjectId ParseValue(string StringKeyValue)
        {
            if (StringKeyValue == null)
                return ObjectId.GenerateNewId();
            return ObjectId.Parse(StringKeyValue);
        }
    }
}
