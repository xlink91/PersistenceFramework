using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using PersistenceFramework.NoSQL.MongoDb.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PersistenceFramework.DbContextTest.Integration
{
    [Ignore]
    [TestClass]
    public class MongoDbContextTest
    {
        static readonly Random rnd = new Random();

        [TestMethod]
        public void Insert_1000_document()
        {
            CustomMongoDbContext db = new CustomMongoDbContext("PFrameworkTest", "localhost:27017");
            Info info = null;
            for (int i = 0; i < 1000; ++i)
            {
                byte[] buffer = new byte[1000];
                rnd.NextBytes(buffer);
                info = new Info(ObjectId.GenerateNewId(), Encoding.UTF8.GetString(buffer));
                db.Add(info);
            }
            IQueryable<Info> qr = db.GetEntity<Info>(x => true);

            long totalMemory = GC.GetTotalMemory(false);
            long totalMemoryEntity = Marshal.SizeOf<char>() * 1000;
            int cnt = qr.Count();
            long totalMemoryAfter = GC.GetTotalMemory(false);
            Assert.IsTrue(totalMemoryAfter >= totalMemory + 500 * totalMemoryEntity && cnt == 1000);
        }

        [TestMethod]
        public void Insert_1000_document_as_transaction()
        {
            CustomMongoDbContext db = new CustomMongoDbContext("PFrameworkTest", "localhost:27017");
            List<Info> infoList = new List<Info>();
            for (int i = 0; i < 1000; ++i)
            {
                byte[] buffer = new byte[1000];
                rnd.NextBytes(buffer);
                Info info = new Info(ObjectId.GenerateNewId(), Encoding.UTF8.GetString(buffer));
                infoList.Add(info);
            }

            db.AddAsTransaction(infoList);

            IQueryable<Info> qr = db.GetEntity<Info>(x => true);

            long totalMemory = GC.GetTotalMemory(false);
            long totalMemoryEntity = Marshal.SizeOf<char>() * 1000;
            int cnt = qr.Count();
            long totalMemoryAfter = GC.GetTotalMemory(false);
            Assert.IsTrue(totalMemoryAfter >= totalMemory + 500 * totalMemoryEntity && cnt == 1000);
        }

        [TestMethod]
        public void Update_1000_document_as_transaction()
        {
            CustomMongoDbContext db = new CustomMongoDbContext("PFrameworkTest", "localhost:27017");
            List<Info> qr = db.GetEntity<Info>(x => true)
                                                .ToList()
                                                .Select(x => new Info(x.Id, string.Concat(DateTime.Now.ToString(), " ", x.Data)))
                                                .ToList()
                                                ;
            db.UpdateAsTransaction(qr, x => x.Id);
        }
        
        [TestMethod]
        public void Remove_As_Transaction()
        {
            CustomMongoDbContext db = new CustomMongoDbContext("PFrameworkTest", "localhost:27017");
        }
    }

    internal class CustomMongoDbContext : MongoDbContext
    {
        public CustomMongoDbContext(string tableName, string connectionString)
            : base(tableName, connectionString)
        {
        }

        private IMongoCollection<Info> Info { get; set; }
    }

    internal class Info
    {
       public ObjectId Id { get; set; }

       public Info() { }

       public Info(ObjectId id, string Data)
       {
            this.Id = id;
            this.Data = Data;
       } 

       public string  Data { get; set; }
    }
}
