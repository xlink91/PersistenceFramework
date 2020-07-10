using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using PersistenceFramework.Contract;
using PersistenceFramework.Entities.NoSQL.Mongo.MongoBaseEntityDefinition;
using PersistenceFramework.Exceptions;
using PersistenceFramework.Mock.NoSQL.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistenceFramework.DbContextTest.UnitTest
{
    [TestClass]
    public class MockMongoDbContextOughtTo
    {
        private IDbContext DbMock;

        [TestMethod]
        public void GetList_NotICollectionExist_Exception()
        {
            DbMock = new MongoDbContext();
            NotDeclaredEntityException exception = Assert.ThrowsException<NotDeclaredEntityException>(() => (DbMock as MongoDbContext).GetList(typeof(NotDeclareEntity)));
            Assert.AreEqual(exception.Message, "NotDeclareEntity are not declare in context as ICollection<NotDeclareEntity>");
            Assert.AreEqual(exception.InnerException.Message, "You most declare the entity as private ICollection<> property");
        }

        [TestMethod]
        public void GetList_ICollectionExist_Null()
        {
            DbMock = new MongoDbContext();
            IEnumerable<NullDeclareEntity> nullDeclareEntity = (IEnumerable<NullDeclareEntity>)(DbMock as MongoDbContext).GetList(typeof(NullDeclareEntity));
            Assert.IsNull(nullDeclareEntity);
        }

        [TestMethod]
        public void GetEntity_GetEntityFuncLambda_Null()
        {
            DbMock = new MongoDbContext();
            InfoEntity infoEntity = DbMock.GetEntity<InfoEntity>(x => x.Id == ObjectId.Parse("5c89c3b00000000012341234")).FirstOrDefault();
            Assert.IsNull(infoEntity);
        }

        [TestMethod]
        public void GetEntity_GetEntityFuncLambda_RealEntity()
        {
            DbMock = new MongoDbContext(false);
            InfoEntity infoEntity00 = new InfoEntity
            {
                Id = ObjectId.Parse("5c89c3b00000000000000000"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client00@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };
            InfoEntity infoEntity01 = new InfoEntity
            {
                Id = ObjectId.Parse("5c89c3212000000000000000"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client01@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };
            DbMock.Add(infoEntity00);
            DbMock.Add(infoEntity01);

            InfoEntity infoEntity = DbMock.GetEntity<InfoEntity>(x => x.Email == "client01@google.com" && x.Name == "cName").FirstOrDefault();
            Assert.AreEqual(infoEntity.Id, ObjectId.Parse("5c89c3212000000000000000"));
        }

        [TestMethod]
        public void AddEntity_Entity_Success()
        {
            DbMock = new MongoDbContext();
            InfoEntity infoEntity = new InfoEntity
            {
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };
            Enumerable.Range(0, 4).ToList().ForEach(x => DbMock.Add(infoEntity));
            IEnumerable<InfoEntity> populatedMyEntitites = DbMock.GetEntity<InfoEntity>(x => true).ToList();
            Assert.AreEqual(populatedMyEntitites.Count(), 4);
        }

        [TestMethod]
        public void AddEntityList_Entity_Success()
        {
            DbMock = new MongoDbContext();
            DbMock.Add<InfoEntity>(Enumerable.Range(0, 4).Select(x => new InfoEntity
            {
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            }).ToList());
            IEnumerable<InfoEntity> populatedMyEntitites = DbMock.GetEntity<InfoEntity>(x => true).ToList();
            Assert.AreEqual(populatedMyEntitites.Count(), 4);
        }

        [TestMethod]
        public void DeleteEntity_Entity_Success()
        {
            DbMock = new MongoDbContext(false);
            InfoEntity infoEntity00 = new InfoEntity
            {
                Id = ObjectId.Parse("5c89c3b00000000000000000"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client00@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };
            InfoEntity infoEntity01 = new InfoEntity
            {
                Id = ObjectId.Parse("5c89c3212000000000000000"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client01@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };
            DbMock.Add(infoEntity00);
            DbMock.Add(infoEntity01);

            #region Remove Element
            DbMock.Remove(infoEntity00);
            #endregion

            #region Assert condition
            InfoEntity[] infoEntities = DbMock.GetEntity<InfoEntity>(x => true).ToArray();
            Assert.AreEqual(infoEntities.Length, 1);
            Assert.AreEqual(infoEntities[0].Email, "client01@google.com");
            #endregion
        }

        [TestMethod]
        public void DeleteEntityUsingExpression_Entity_Success()
        {
            DbMock = new MongoDbContext(false);
            InfoEntity infoEntity00 = new InfoEntity
            {
                Id = ObjectId.Parse("5c89c3b00000000000000000"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client00@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };
            InfoEntity infoEntity01 = new InfoEntity
            {
                Id = ObjectId.Parse("5c89c3212000000000000000"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client01@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };
            InfoEntity infoEntity02 = new InfoEntity
            {
                Id = ObjectId.Parse("5ed6af61b7f0c68829022ebc"),
                Guid = Guid.Parse("EBDB974B-291B-460A-9F43-871734958676"),
                Email = "client02@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };
            DbMock.Add(infoEntity00);
            DbMock.Add(infoEntity01);
            DbMock.Add(infoEntity02);

            #region Remove Element
            DbMock.Remove<InfoEntity>(x => x.Guid == Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"));
            #endregion

            #region Assert condition
            InfoEntity[] infoEntities = DbMock.GetEntity<InfoEntity>(x => true).ToArray();
            Assert.AreEqual(infoEntities.Length, 1);
            Assert.AreEqual(infoEntities[0].Email, "client02@google.com");
            #endregion
        }

        [TestMethod]
        public void UpdateEntity_Entity_Success()
        {
            DbMock = new MongoDbContext(false);
            InfoEntity infoEntity = new InfoEntity
            {
                Id = ObjectId.Parse("5c89c3b00000000000000000"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client00@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };

            InfoEntity infoEntityUpdated = new InfoEntity
            {
                Id = ObjectId.Parse("5c89c3b00000000000000000"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "dev@microsoft.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };

            DbMock.Add(infoEntity);

            #region Update element
            DbMock.Update(infoEntityUpdated);
            #endregion

            #region Assert Condition
            InfoEntity retrieveInfoEntity = DbMock.GetEntity<InfoEntity>(x => true).FirstOrDefault();
            Assert.AreEqual(retrieveInfoEntity.Email, "dev@microsoft.com");
            #endregion
        }

        [TestMethod]
        public void UpdateEntityWithLambda_Entity_Success()
        {
            DbMock = new MongoDbContext(false);
            InfoEntity infoEntity0 = new InfoEntity
            {
                Id = ObjectId.Parse("5c89c3b00000000000000000"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client00@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };

            InfoEntity infoEntity1 = new InfoEntity
            {
                Id = ObjectId.Parse("5ed6d96899609a85c65afac4"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client01@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };

            InfoEntity infoEntity2 = new InfoEntity
            {
                Id = ObjectId.Parse("5ed6d96dfdeac5b2c020f33c"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client02@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };

            InfoEntity infoEntity3 = new InfoEntity
            {
                Id = ObjectId.Parse("5ed6d971ee98f12b87cec7fa"),
                Guid = Guid.Parse("3ABCC35E-D5EF-4514-B913-6D501C301AC0"),
                Email = "client04@google.com",
                Name = "cName",
                Lastname = "cLastname",
                Phone = "+589 93 956 487"
            };

            DbMock.Add(infoEntity0);
            DbMock.Add(infoEntity1);
            DbMock.Add(infoEntity2);

            #region Update element
            DbMock.Update(x => x.Email == "client00@google.com", infoEntity3);
            #endregion

            #region Assert Condition
            InfoEntity retrieveRemovedInfoEntity = 
                DbMock.GetEntity<InfoEntity>(x => x.Email == "client00@google.com").
                FirstOrDefault();
            InfoEntity retrieveInfoEntity = DbMock.GetEntity<InfoEntity>(x => x.Email == "client04@google.com").
                FirstOrDefault();
            Assert.IsNull(retrieveRemovedInfoEntity);
            Assert.AreEqual("client04@google.com", retrieveInfoEntity.Email);
            #endregion
        }
    }

    internal class NotDeclareEntity : MongoDbBaseEntityDefinition
    {
        public string Name { get; set; }
        public string Guid { get; set; }
    }

    internal class NullDeclareEntity : MongoDbBaseEntityDefinition
    {

    }

    internal class InfoEntity : MongoDbBaseEntityDefinition
    {
        public string Msg { get; set; }
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
    }

    internal class MongoDbContext : MockMongoDbContext
    {
        private ICollection<InfoEntity> InfoEntity { get; set; } = new List<InfoEntity>();
        private ICollection<NullDeclareEntity> NullDeclareEntity { get; set; }

        public MongoDbContext(bool autogeneratedId = true) : base(autogeneratedId)
        {
        }

        public new IEnumerable<object> GetList(Type type)
        {
            return base.GetList(type);
        }
    }
}
