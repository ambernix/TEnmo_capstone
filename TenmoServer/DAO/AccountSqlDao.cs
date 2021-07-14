using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDao : IAccountDao
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public AccountSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Account GetAccount(int accountId)
        {
            Account returnAccount = null;
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance FROM accounts WHERE account_id = @account_id", conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnAccount = GetAccountFromReader(reader);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return returnAccount;
        }
        public int GetDefaultAccountId(int userId)
        {
            int accountId = 0;
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                // Selecting "top 1" allows money to be deposited into an account when sent from another user
                // without additional input from this user
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 account_id FROM accounts WHERE user_id = @user_id", conn);
                cmd.Parameters.AddWithValue("@user_id", userId);
                accountId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return accountId;
        }



        public IList<Account> GetAccounts(int userId)
        {
            List<Account> returnList = new List<Account>();

            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance FROM accounts WHERE user_id = @user_id", conn);
                cmd.Parameters.AddWithValue("@user_id", userId);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    returnList.Add(GetAccountFromReader(reader));
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return returnList;
        }

        public Account CreateAccount(int userId)
        {
            Account newAccount = null;
            int accountId = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO accounts (user_id, balance) OUTPUT INSERTED.account_id VALUES (@user_id, @start_balance);", conn);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    cmd.Parameters.AddWithValue("@start_balance", startingBalance); // each account starts with 1,000 "bucks"
                    accountId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                newAccount = GetAccount(accountId);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return newAccount;
        }


        private Account GetAccountFromReader(SqlDataReader reader)
        {
            Account a = new Account()
            {
                AccountId = Convert.ToInt32(reader["account_id"]),
                UserId = Convert.ToInt32(reader["user_id"]),
                Balance = Convert.ToDecimal(reader["balance"])
            };

            return a;
        }
    }
}
