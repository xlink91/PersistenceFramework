using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson.Serialization.Attributes;
using PersistenceFramework.Attributes;
using PersistenceFramework.Exceptions;
using PersistenceFramework.Util;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PersistenceFramework.DbContextTest.UnitTest
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void GetIdLE_PersistenceIdAttributeWithoutId_IdName()
        {
            VersionWithoutId version = new VersionWithoutId() { Name = "versionWithoutId", Descpription = "D" };
            var expression = DynamicLambdaBuilder.GetIdLE(version, typeof(PersistenceIdAttribute));
            var binaryExpression = (BinaryExpression)expression.Body;
            var memberExpression = (MemberExpression)binaryExpression.Left;

            Assert.AreEqual("Name", memberExpression.Member.Name);
        }

        [TestMethod]
        public void GetIdLE_PersistenceIdAttributeWithId_IdName()
        {
            VersionWithId version = new VersionWithId() { Name = "versionWithId", Descpription = "D" };
            var expression = DynamicLambdaBuilder.GetIdLE(version, typeof(PersistenceIdAttribute));
            var binaryExpression = (BinaryExpression)expression.Body;
            var memberExpression = (MemberExpression)binaryExpression.Left;

            Assert.AreEqual("Name", memberExpression.Member.Name);
        }


        [TestMethod]
        public void GetIdLE_PersistenceIdAttributeWithDuplicateId_DuplicatedIdException()
        {
            Assert.ThrowsException<DuplicateIdException>(() =>
            {
                VersionWithDuplicateId version = new VersionWithDuplicateId() { Name = "versionWithId", Descpription = "D" };
                var expression = DynamicLambdaBuilder.GetIdLE(version, typeof(PersistenceIdAttribute));
            });
        }

        [TestMethod]
        public void GetIdLE_PersistenceIdAttributeWithoutIdAttribute_DuplicatedIdException()
        {
            VersionWithoutIdAttribute version = new VersionWithoutIdAttribute() { Name = "versionWithId", Descpription = "D" };
            var expression = DynamicLambdaBuilder.GetIdLE(version, typeof(PersistenceIdAttribute));
            var binaryExpression = (BinaryExpression)expression.Body;
            var memberExpression = (MemberExpression)binaryExpression.Left;

            Assert.AreEqual("Id", memberExpression.Member.Name);
        }

        [TestMethod]
        public void CreateFilterForCollection_TakeAFilterExpression_And_Return_ABooleanExpression()
        {
            IEnumerable<InfoEntity> infoEntityList
                = Enumerable
                    .Range(1, 10)
                    .Select(x => new InfoEntity
                    {
                        Lastname = $"__lastname{x}"
                    })
                    .ToList();
            
            var expression1 = DynamicLambdaBuilder
                                .CreateFilterForCollection<InfoEntity, string>(
                                    x => x.Lastname, 
                                    infoEntityList.ElementAt(0)
                                );

            Assert.AreEqual(((expression1.Body as BinaryExpression).Right as ConstantExpression).Value, "__lastname1");

            var expression2 = DynamicLambdaBuilder
                                .CreateFilterForCollection<InfoEntity, string>(
                                    x => x.Lastname, 
                                    infoEntityList.ElementAt(3)
                                );

            Assert.AreEqual(((expression2.Body as BinaryExpression).Right as ConstantExpression).Value, "__lastname4");

            var expression3 = DynamicLambdaBuilder
                                .CreateFilterForCollection<InfoEntity, string>(
                                    x => x.Lastname, 
                                    infoEntityList.ElementAt(6)
                                );

            Assert.AreEqual(((expression3.Body as BinaryExpression).Right as ConstantExpression).Value, "__lastname7");
        }
    }

    internal class VersionWithoutId
    {
        [PersistenceId]
        [BsonId]
        public string Name { get; set; }
        public string Descpription { get; set; }
    }

    internal class VersionWithId
    {
        public int Id { get; set; }
        [PersistenceId]
        public string Name { get; set; }
        public string Descpription { get; set; }
    }

    internal class VersionWithDuplicateId
    {
        public int Id { get; set; }
        [PersistenceId]
        public string Name { get; set; }
        [PersistenceId]
        public string Descpription { get; set; }
    }

    internal class VersionWithoutIdAttribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Descpription { get; set; }
    }
}
