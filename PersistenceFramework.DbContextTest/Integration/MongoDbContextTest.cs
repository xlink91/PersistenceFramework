using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using PersistenceFramework.NoSQL.MongoDb.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        //[TestMethod]
        //public void Remove_As_Transaction()
        //{
        //    CustomMongoDbContext db = new CustomMongoDbContext("PFrameworkTest", "localhost:27017");
        //}

        //TODO: Make a put out this unit test from integration part, also make it usefull with some real constraint
        [TestMethod]
        public void CreateUpdateDefinition_Return_UpdateDefinition()
        {
            IList<(string op, (Expression<Func<Info, object>> property, object value) updateInfo)> operations =
                new List<(string op, (Expression<Func<Info, object>> property, object value) updateInfo)>();

            Expression<Func<Info, object>> property = x => x.Data;
            string dataValue = "changedValue";

            operations.Add((MongoDbUpdateDefinitions.SET, (property, dataValue)));
            UpdateDefinition<Info> updateDefinition = MongoDbContextUtils.CreateUpdateDefinition(operations);

            Assert.AreNotEqual(default, updateDefinition);
        }

        [TestMethod]
        public void UpdateDefinition_UpdateValue()
        {
            CustomMongoDbContext db = new CustomMongoDbContext("PFrameworkTest", "192.168.250.132:27017");

            IList<(string op, (Expression<Func<Info, object>> property, object value) updateInfo)> operations =
                new List<(string op, (Expression<Func<Info, object>> property, object value) updateInfo)>();

            Expression<Func<Info, object>> property = x => x.Data;
            string dataValue = "changedValue_1";

            operations.Add((MongoDbUpdateDefinitions.SET, (property, dataValue)));
            
            bool result = db.UpdateSetAsTransaction<Info>(x => x.Id == ObjectId.Parse("5f625c904d053f8d533a6ed0")  ||
                x.Id == ObjectId.Parse("5f625c904d053f8d533a6ed1"), operations);
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
