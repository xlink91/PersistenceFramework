# PersistenceFramework
The MongoDbContext need that every entity contains a ```public ObjectId Id { get; set; }``` property. 
You most declare each document collection like a ```private IMongoCollection<Entity> Entity { get; set; }``` property.

Persistence Framework example for MongoDb

    class Program
    {
        static void Main(string[] args)
        {
            CustomDbContext context = new CustomDbContext("MongoDatabaseName", "127.0.0.1:27017");

            for (int i = 0; i < 10; ++i)
                context.Add(new Info { Data = i.ToString() + " aa" });

            IEnumerable<Info> infos = context.GetEntity<Info>(x => true);
            foreach (var info in infos)
                Console.WriteLine(info.Id + " " + info.Data);
            Console.WriteLine(context.GetEntity<Info>(x => x.Data == "1 aa").FirstOrDefault().Data);
        }
    }
    
    public class CustomDbContext : MongoDbContext
    {
        public CustomDbContext(string databaseName, string url) : base(databaseName, url)
        {
        }

        private IMongoCollection<Info> Info { get; set; }
        private IMongoCollection<Data> Data { get; set; }
    }
    
    
    internal class Info
    {
        public ObjectId Id { get; set; }
        public string Data { get; set; }
    }
    
    internal class Data
    {
        public ObjectId Id { get; set; }
        public string Code { get; set; }
    }

   
