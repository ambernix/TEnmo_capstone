using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;
using System.Collections.Generic;
using System.Linq;


namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : Controller
    {
        private readonly ITransferDao transferDao;
        public TransferController(ITransferDao _transferDao)
        {
            transferDao = _transferDao;
        }

        // handles transfers that are both sending money and approving a request for money
        [HttpPost("{username}")]
        public ActionResult<int> Transfer(string username, Transfer transfer)
        {
            int transferId = 0;
            if (IsCorrectUser(username))
            {
                if (transfer.TransferType == "Request")
                {
                    transferId = transferDao.MakeRequestTransfer(transfer.AccountFrom, transfer.AccountTo, transfer.Amount);
                }
                else if (transfer.TransferType == "Send")
                {
                    transferId = transferDao.MakeTransfer(transfer.AccountFrom, transfer.AccountTo, transfer.Amount);
                }

                if (transferId != 0)
                {
                    return Ok(transferId);
                }
                else
                {
                    return NotFound("Transaction declined.");
                }
            }
            return Forbid();
        }

        // returns all past approved transfers for all accounts for the user
        [HttpGet("{username}")]
        public ActionResult<IList<Transfer>> GetTransfers(string username)
        {
            if (IsCorrectUser(username))
            {
                IList<Transfer> transfers = transferDao.GetTransfers(username);
                if (transfers.Count > 0)
                {
                    return Ok(transfers);
                }
                else
                {
                    return NotFound("No transfers associated with user.");
                }
            }
            return Forbid();
        }

        // returns all pending transfers for all accounts for the user
        [HttpGet("{username}/pending")]
        public ActionResult<IList<Transfer>> GetPendingTransfers(string username)
        {
            if (IsCorrectUser(username))
            {
                IList<Transfer> transfers = transferDao.GetTransfers(username, 1);
                if (transfers.Count > 0)
                {
                    transfers = transfers.Where(t => t.UsernameFrom == username).ToList();
                }
                if (transfers.Count > 0)
                {
                    return Ok(transfers);
                }
                else
                {
                    return NotFound("No transfers associated with user.");
                }
            }
            return Forbid();
        }

        // returns details on an individual transfer for the user
        [HttpGet("{username}/{transferId}")]
        public ActionResult<Transfer> GetTransfer(string username, int transferId)
        {
            if (IsCorrectUser(username))
            {
                Transfer transfer = transferDao.GetTransfer(username, transferId);
                if (transfer != null)
                {
                    return Ok(transfer);
                }
                else
                {
                    return NotFound("No transfers associated with this id.");
                }
            }
            return Forbid();
        }

        // returns details on an individual pending transfer for the user
        [HttpGet("{username}/{transferId}/pending")]
        public ActionResult<Transfer> GetPendingTransfer(string username, int transferId)
        {
            if (IsCorrectUser(username))
            {
                Transfer transfer = transferDao.GetTransfer(username, transferId, true);
                if (transfer != null)
                {
                    return Ok(transfer);
                }
                else
                {
                    return NotFound("No transfers associated with this id.");
                }
            }
            return Forbid();
        }

        // depending on action id, approves or rejects a pending transfer
        [HttpPut("{username}/{actionId}")]
        public ActionResult ActionTransfer(Transfer transfer, int actionId)
        {
            bool result;
            if (IsCorrectUser(transfer.UsernameFrom))
            {
                result = transferDao.ActionTransfer(transfer, actionId);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return NotFound("Transfer status not changed.");
                }
            }
            return Forbid();
        }

        // Validates that correct user is making the request for the information.
        private bool IsCorrectUser(string username)
        {
            return User.Identity.Name == username;
        }
    }
}
