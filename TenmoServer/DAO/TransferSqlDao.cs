using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
        private readonly string connectionString;
        public TransferSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public int MakeTransfer(int fromId, int toId, decimal transferAmount)
        {
            int transferId = 0;
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                // Inside transaction completes the account number changes. Before committing checks that
                // the account the money is coming from did not become overdrawn in the transaction.
                string query =
                    "BEGIN TRANSACTION " +
                    "INSERT INTO transfers(transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                    "VALUES(2, 2, @from_id, @to_id, @amount) " +
                    "UPDATE accounts SET balance = balance - @amount WHERE account_id = @from_id " +
                    "UPDATE accounts SET balance = balance + @amount WHERE account_id = @to_id " +
                    "IF((SELECT balance FROM accounts WHERE account_id = @from_id) < 0) " +
                    "BEGIN " +
                    "ROLLBACK " +
                    "END " +
                    "ELSE " +
                    "BEGIN " +
                    "SELECT SCOPE_IDENTITY() " +
                    "COMMIT " +
                    "END";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@from_id", fromId); //ID references account
                cmd.Parameters.AddWithValue("@to_id", toId); //ID references account
                cmd.Parameters.AddWithValue("@amount", transferAmount);

                transferId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return transferId;
        }

        public IList<Transfer> GetTransfers(string username, int transferStatusId = 2) 
        {
            List<Transfer> returnList = new List<Transfer>();
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = "SELECT t.transfer_id, t.account_from, t.account_to, t.amount, " +
                    "ts.transfer_status_desc, tt.transfer_type_desc, ufr.username AS user_from, uto.username AS user_to " +
                    "FROM transfers t " +
                    "JOIN transfer_statuses ts ON ts.transfer_status_id = t.transfer_status_id " +
                    "JOIN transfer_types tt ON tt.transfer_type_id = t.transfer_type_id " +
                    "JOIN accounts afr ON afr.account_id = t.account_from " +
                    "JOIN accounts ato ON ato.account_id = t.account_to " +
                    "JOIN users ufr ON ufr.user_id = afr.user_id " +
                    "JOIN users uto ON uto.user_id = ato.user_id " +
                    // checks that you only get transfers that are from or to the user and that are approved
                    "WHERE (ufr.username = @username OR uto.username = @username) AND t.transfer_status_id = @transfer_status_id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@transfer_status_id", transferStatusId);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    returnList.Add(GetTransferFromReader(reader));
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return returnList;
        }

        // works for both approved and pending transfers
        public Transfer GetTransfer(string username, int transferId, bool isPending = false)
        {
            Transfer returnTransfer = null;
            string query = "SELECT t.transfer_id, t.account_from, t.account_to, t.amount, " +
            "ts.transfer_status_desc, tt.transfer_type_desc, ufr.username AS user_from, uto.username AS user_to " +
            "FROM transfers t " +
            "JOIN transfer_statuses ts ON ts.transfer_status_id = t.transfer_status_id " +
            "JOIN transfer_types tt ON tt.transfer_type_id = t.transfer_type_id " +
            "JOIN accounts afr ON afr.account_id = t.account_from " +
            "JOIN accounts ato ON ato.account_id = t.account_to " +
            "JOIN users ufr ON ufr.user_id = afr.user_id " +
            "JOIN users uto ON uto.user_id = ato.user_id ";
            if (isPending)
            {
                // checks that the transfer you get is from the user and is pending
                query += "WHERE ufr.username = @username AND t.transfer_status_id = 1 AND t.transfer_id = @transfer_id";
            }
            else
            {
                // checks that the transfer you get is from the user
                query += "WHERE (ufr.username = @username OR uto.username = @username) AND t.transfer_id = @transfer_id";
            }
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open(); 

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@transfer_id", transferId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnTransfer = (GetTransferFromReader(reader));
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return returnTransfer;
        }

        public int MakeRequestTransfer(int fromId, int toId, decimal transferAmount)
        {
            int transferId = 0;
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = "INSERT INTO transfers(transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                "VALUES(1, 1, @from_id, @to_id, @amount)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@from_id", fromId); //ID references account
                cmd.Parameters.AddWithValue("@to_id", toId); //ID references account
                cmd.Parameters.AddWithValue("@amount", transferAmount);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("SELECT @@IDENTITY", conn);

                transferId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return transferId;
        }
        public bool ActionTransfer(Transfer transfer, int actionId)
        {
            if (actionId == 1)
            {
                return ApproveTransfer(transfer);
            }
            else if (actionId == 2)
            {
                return RejectTransfer(transfer.AccountFrom, transfer.TransferId);
            }
            else
            {
                return false;
            }
        }

        public bool ApproveTransfer(Transfer transfer)
        {
            int outputId = 0;
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                // Inside transaction completes the account number changes. Before committing checks that
                // the account the money is coming from did not become overdrawn in the transaction.
                string query =
                    "BEGIN TRANSACTION " +
                    "UPDATE accounts SET balance = balance - @amount WHERE account_id = @from_account " +
                    "UPDATE accounts SET balance = balance + @amount WHERE account_id = @to_account " +
                    // Transfer is already created when request made, so simply need to change from pending to approved
                    "UPDATE transfers SET transfer_status_id = 2 WHERE transfer_id = @transfer_id " +
                    "IF((SELECT balance FROM accounts WHERE account_id = @from_account) < 0) " +
                    "BEGIN " +
                    "ROLLBACK " +
                    "END " +
                    "ELSE " +
                    "BEGIN " +
                    "SELECT @transfer_id " +
                    "COMMIT " +
                    "END";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", transfer.UsernameFrom);
                cmd.Parameters.AddWithValue("@transfer_id", transfer.TransferId);
                cmd.Parameters.AddWithValue("@amount", transfer.Amount);
                cmd.Parameters.AddWithValue("@from_account", transfer.AccountFrom);
                cmd.Parameters.AddWithValue("@to_account", transfer.AccountTo);
                outputId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return transfer.TransferId == outputId;
        }


        public bool RejectTransfer(int accountFrom, int transferId)
        {
            int outputId = 0;
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                string query = "IF (@account_from = (SELECT account_from FROM transfers WHERE transfer_id = @transfer_id)) " +
                // Only change that needs to happen is that the transfer status is set to rejected. 
                // Currently no functionality to view rejected transfers.
                                "BEGIN " +
                                "UPDATE transfers SET transfer_status_id = 3 WHERE transfer_id = @transfer_id " +
                                "SELECT @transfer_id " +
                                "END";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account_from", accountFrom);
                cmd.Parameters.AddWithValue("@transfer_id", transferId);
                outputId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return outputId == transferId;
        }

        private Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer t = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                AccountFrom = Convert.ToInt32(reader["account_from"]),
                AccountTo = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"]),
                TransferStatus = Convert.ToString(reader["transfer_status_desc"]),
                TransferType = Convert.ToString(reader["transfer_type_desc"]),
                UsernameFrom = Convert.ToString(reader["user_from"]),
                UsernameTo = Convert.ToString(reader["user_to"])
            };

            return t;
        }
    }
}