using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;
using System.Collections.Generic;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserDao userDao;
        private readonly IAccountDao accountDao;
        public UserController(IUserDao _userDao, IAccountDao _accountDao)
        {
            userDao = _userDao;
            accountDao = _accountDao;
        }

        [HttpPost("{userId}")]
        public ActionResult<int> CreateAccount(int userId)
        {
            int accountId = 0;
            accountId = accountDao.CreateAccount(userId).AccountId;
            if (accountId != 0)
            {
                return Ok(accountId);
            }
            else
            {
                return NotFound("Account could not be Created");
            }
        }

        // Exists so that users can transfer to other people's accounts without exposing account ids.
        [HttpGet("{userId}/default")]
        public ActionResult<int> GetDefaultAccount(int userId)
        {

                int accountId = accountDao.GetDefaultAccountId(userId);
                if (accountId != 0)
                {
                    return Ok(accountId);
                }
                else
                {
                    return NotFound("No accounts associated with user id.");
                }
        }

        // returns account numbers and balances for all accounts for the user
        [HttpGet("{userId}")]
        public ActionResult<IList<Account>> GetAccounts(int userId)
        {
            if (IsCorrectUser(userId))
            {
                IList<Account> accounts = accountDao.GetAccounts(userId);
                if (accounts.Count > 0)
                {
                    return Ok(accounts);
                }
                else
                {
                    return NotFound("No accounts associated with user.");
                }
            }
            return Forbid();
        }

        // gives list of all users with their user ids except the one making the request
        [HttpGet]
        public ActionResult<IList<User>> GetUsers()
        {
                IList<User> users = userDao.GetUsers(User.Identity.Name);
                if (users.Count > 0)
                {
                    return Ok(users);
                }
                else
                {
                    return NotFound("No users found.");
                }
        }

        // Validates that correct user is making the request for the information.
        private bool IsCorrectUser(int id)
        {
            return userDao.GetUser(User.Identity.Name).UserId == id;
        }
    }
}
