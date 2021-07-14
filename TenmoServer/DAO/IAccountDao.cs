using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        Account CreateAccount(int userId);
        Account GetAccount(int accountId);
        IList<Account> GetAccounts(int userId);
        int GetDefaultAccountId(int userId);

        //List<Account> GetAccounts();
    }
}
