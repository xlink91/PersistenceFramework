using System;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistenceFramework.NoSQL.MongoDb.Implementation;
using MongoDB.Driver;

namespace PersistenceFramework.DbContextTest.Integration
{
    [TestClass]
    public class MongoDbContextTest
    {
        [TestMethod]
        public void Insert_1000_document()
        {
            CustomMongoDbContext db = new CustomMongoDbContext("PFrameworkTest", "192.168.250.132:27017");
            //Info info = null; 
            //for(int i = 0; i < 1000; ++i)
            //{
            //    byte[] buffer = new byte[1000];
            //    info = new Info(Encoding.UTF8.GetString(buffer)); 
            //    db.Add(info); 
            //}
            IQueryable<Info> qr = db.GetEntity<Info>(x => true);
            long totalMemory = GC.GetTotalMemory(false);
            long totalMemoryEntity = Marshal.SizeOf<char>() * 1000;
            int cnt = qr.Count();
            long totalMemoryAfter = GC.GetTotalMemory(false);
            Assert.IsTrue(totalMemoryAfter >= totalMemory + 500 * totalMemoryEntity && cnt == 1000);
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
       public Info(string Data)
       {
            this.Data = Data;
       } 

       public string  Data { get; set; }
    }
}
