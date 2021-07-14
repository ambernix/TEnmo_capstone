using System.Collections.Generic;
using TenmoServer.Models;


namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        public int MakeTransfer(int fromId, int toId, decimal transferAmount);
        public IList<Transfer> GetTransfers(string username, int transferStatusId = 2);
        public Transfer GetTransfer(string username, int transferId, bool isPending = false);
        public int MakeRequestTransfer(int fromId, int toId, decimal transferAmount);
        public bool ActionTransfer(Transfer transfer, int actionId);
        public bool ApproveTransfer(Transfer transfer);
        public bool RejectTransfer(int fromAccount, int transferId);
       
    }
}
