using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;
using TenmoServer.DAO;

namespace TenmoServerTests
{
    [TestClass]
    public class TenmoDaoTests
    {
        protected string ConnectionString { get; } = "Server=.\\SQLEXPRESS;Database=tenmo;Trusted_Connection=True;";

        /// <summary>
        /// The transaction for each test.
        /// </summary>
        private TransactionScope transaction;

        [TestInitialize]
        public virtual void Setup()
        {
            transaction = new TransactionScope();
            // Get the SQL script to run
            string sql = File.ReadAllText("test-data.sql");

            // Execute the script
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
        
        [TestCleanup]
        public virtual void Cleanup()
        {
            // Roll back the transaction
            transaction.Dispose();
        }
    }
}

