using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenmoServer.DAO;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoServerTests;

namespace TenmoServer.DAO.Tests
{
    [TestClass()]
    public class AccountSqlDaoTests : TenmoDaoTests
    {
        private AccountSqlDao accountSqlDao;
        [TestInitialize]
        public override void Setup()
        {
            accountSqlDao = new AccountSqlDao(ConnectionString);
            base.Setup();
        }
        [DataTestMethod()]
        [DataRow(11)]
        [DataRow(12)]
        [DataRow(13)]
        public void GetAccountReturnsAccount(int accountId)
        {
            Assert.AreEqual(accountId,accountSqlDao.GetAccount(accountId).AccountId);
        }
        [DataTestMethod()]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(1000)]
        public void GetAccountReturnsNull(int accountId)
        {
            Assert.AreEqual(null, accountSqlDao.GetAccount(accountId));
        }

        [DataTestMethod()]
        [DataRow(1,11)]
        [DataRow(2,13)]
        public void GetDefaultAccountIdReturnsId(int userId, int accountId)
        {
            Assert.AreEqual(accountId, accountSqlDao.GetDefaultAccountId(userId));
        }
        [DataTestMethod()]
        [DataRow(11)]
        [DataRow(-1)]
        [DataRow(25)]
        [DataRow(50675)]
        public void GetDefaultAccountIdReturns0(int userId)
        {
            Assert.AreEqual(0, accountSqlDao.GetDefaultAccountId(userId));
        }

        [DataTestMethod()]
        [DataRow(1,2)]
        [DataRow(2,1)]
        public void GetAccountsTest(int userId, int expectedCount)
        {
            Assert.AreEqual(expectedCount, accountSqlDao.GetAccounts(userId).Count);
        }
        [TestMethod]
        public void GetAccountsReturnsEmpty()
        {
            Assert.AreEqual(0, accountSqlDao.GetAccounts(255).Count);
        }

        [TestMethod()]
        public void CreateAccountTest()
        {
            int userId = 1;
            Assert.AreEqual(userId,accountSqlDao.CreateAccount(userId).UserId);
        }
        
    }
}