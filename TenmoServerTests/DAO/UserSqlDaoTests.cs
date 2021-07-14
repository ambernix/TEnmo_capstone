using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenmoServer.DAO;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoServerTests;

namespace TenmoServer.DAO.Tests
{
    [TestClass()]
    public class UserSqlDaoTests : TenmoDaoTests
    {
        private UserSqlDao userSqlDao;

        [TestInitialize]
        public override void Setup()
        {
            userSqlDao = new UserSqlDao(ConnectionString);
            base.Setup();
        }

        [DataTestMethod]
        [DataRow("test")]
        [DataRow("test2")]
        public void GetUserFindsUser(string searchUser)
        {
            Assert.AreEqual(searchUser, userSqlDao.GetUser(searchUser).Username);

        }

        [TestMethod]
        public void GetUserShouldNotFindUser()
        {
            Assert.AreEqual(null, userSqlDao.GetUser("test3"));
        }

        [TestMethod]
        public void GetUsersFindsAllButCurrentUser()
        {
            string searchUser = "test";
            Assert.AreEqual("test2", userSqlDao.GetUsers(searchUser)[0].Username);
            Assert.AreEqual(1, userSqlDao.GetUsers(searchUser).Count);
        }

        [TestMethod]
        public void AddUserReturnsNewUser()
        {
            string addUserName = "test3";
            string addUserPassword = "test3";
            Assert.AreEqual(addUserName, userSqlDao.AddUser(addUserName, addUserPassword).Username);
            Assert.AreEqual(addUserName, userSqlDao.GetUser("test3").Username);
        }
    }
}