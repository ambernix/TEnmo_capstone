using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenmoServer.DAO;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoServerTests;
using TenmoServer.Models;

namespace TenmoServer.DAO.Tests
{
    [TestClass()]
    public class TransferSqlDaoTests : TenmoDaoTests
    {
        private TransferSqlDao transferSqlDao;
        [TestInitialize]
        public override void Setup()
        {
            transferSqlDao = new TransferSqlDao(ConnectionString);
            base.Setup();
        }

        [DataTestMethod()]
        [DataRow(11, 13, "500")]
        [DataRow(13, 11, "5.50")]
        [DataRow(11, 13, "0.01")]
        public void MakeTransferTest_ShouldSucceed(int fromId, int toId, string stringAmount)
        {
            decimal transferAmount = decimal.Parse(stringAmount);
            int output = transferSqlDao.MakeTransfer(fromId, toId, transferAmount);
            Assert.IsTrue(output > 0);
        }
        [DataTestMethod()]
        [DataRow(11, 13, "5000")]
        [DataRow(13, 13, "5.50")]
        [DataRow(11, 15, "0.01")]
        public void MakeTransferTest_ShouldFail(int fromId, int toId, string stringAmount)
        {
            decimal transferAmount = decimal.Parse(stringAmount);
            int output = transferSqlDao.MakeTransfer(fromId, toId, transferAmount);
            Assert.AreEqual(0, output);
        }

        [TestMethod()]
        public void GetTransfersTest()
        {
            string searchName = "test";
            Assert.AreEqual(2, transferSqlDao.GetTransfers(searchName).Count);
            Assert.AreEqual(2, transferSqlDao.GetTransfers(searchName, 1).Count);
            Assert.AreEqual(0, transferSqlDao.GetTransfers(searchName+"ament").Count);
        }

        [DataTestMethod()]
        [DataRow("test", 21, false)]
        [DataRow("test2", 22, false)]
        [DataRow("test2", 23, true)]
        [DataRow("test", 24, true)]
        public void GetTransferTest_ShouldSucceed(string searchName, int transferId, bool isPending)
        {
            Assert.AreEqual(transferId, transferSqlDao.GetTransfer(searchName, transferId, isPending).TransferId);
        }
        [DataTestMethod()]
        [DataRow("testable", 21, false)]
        [DataRow("test2", 55, false)]
        [DataRow("test", 23, true)]
        public void GetTransferTest_ShouldFail(string searchName, int transferId, bool isPending)
        {
            Assert.IsNull(transferSqlDao.GetTransfer(searchName, transferId, isPending));
        }

        [DataTestMethod()]
        [DataRow(13, 11, "50")]
        [DataRow(12, 13, "50")]
        public void MakeRequestTransfer_ShouldSucceed(int fromId, int toId, string stringAmount)
        {
            decimal transferAmount = decimal.Parse(stringAmount);
            int output = transferSqlDao.MakeRequestTransfer(fromId, toId, transferAmount);
            Assert.IsTrue(output > 0);
        }

        [DataTestMethod()]
        [DataRow(15, 11, "50")]
        [DataRow(12, 12, "50")]
        public void MakeRequestTransfer_ShouldFail(int fromId, int toId, string stringAmount)
        {
            decimal transferAmount = decimal.Parse(stringAmount);
            int output = transferSqlDao.MakeRequestTransfer(fromId, toId, transferAmount);
            Assert.AreEqual(0, output);
        }

        [TestMethod()]
        public void ApproveTransferTest()
        {
            Transfer transfer = transferSqlDao.GetTransfer("test2", 23, true);
            Assert.IsTrue(transferSqlDao.ActionTransfer(transfer,1));
        }

        [TestMethod()]
        public void RejectTransferTest()
        {
            Transfer transfer = transferSqlDao.GetTransfer("test", 24, true);
            Assert.IsTrue(transferSqlDao.ActionTransfer(transfer, 2));
        }
    }
}