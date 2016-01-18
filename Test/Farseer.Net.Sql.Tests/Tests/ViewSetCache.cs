using System;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.Tests
{
    [TestClass]
    public class ViewSetCache
    {
        [TestMethod]
        public void Insert_Normal()
        {
        }
        [TestMethod]
        public void Insert_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void AddUp_Normal()
        {
        }
        [TestMethod]
        public void AddUp_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Delete_Normal()
        {
        }
        [TestMethod]
        public void Delete_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void GetValue_Normal()
        {
        }
        [TestMethod]
        public void GetValue_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Statistics_Normal()
        {
        }
        [TestMethod]
        public void Statistics_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToEntity_Normal()
        {
        }
        [TestMethod]
        public void ToEntity_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToList_Normal()
        {
        }
        [TestMethod]
        public void ToList_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToSelectList_Normal()
        {
        }
        [TestMethod]
        public void ToSelectList_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Update_Normal()
        {
        }
        [TestMethod]
        public void Update_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }
    }
}