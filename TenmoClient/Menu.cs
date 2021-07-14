using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TenmoClient.Models;
using TenmoClient.EndUser;

namespace TenmoClient
{
    public class Menu
    {
        private static readonly AccountService accountService = new AccountService();
        private static readonly ConsoleServiceOutput consoleOutput = new ConsoleServiceOutput();
        private readonly ConsoleServiceInput consoleInput;
        private readonly ApiUser user;
        public Menu() //strictly for testing!
        {
            user = new ApiUser();
            consoleInput = new ConsoleServiceInput(new ConsoleUser());
        }
        public Menu(ApiUser _user, IUserInput input)
        {
            user = _user;
            consoleInput = new ConsoleServiceInput(input);
        }
        public void MenuSelection()
        {

            int menuSelection = -1;
            while (menuSelection != 7)
            {
                menuSelection = consoleInput.PromptForMainMenu();
                string message = MenuSelect(menuSelection);
                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine(message);
                }
            }
        }
        public string MenuSelect(int menuSelection)
        {
            string output = "";
            if (menuSelection == 1)
            {
                //print all accounts with balance
                consoleOutput.PrintMenu("ID,Balance", MenuSelect1());
            }
            else if (menuSelection == 2)
            {
                //print all completed Transfers
                consoleOutput.PrintTransferDetails(MenuSelect2());
            }
            else if (menuSelection == 3)
            {
                //print pending Request
                consoleOutput.PrintMessage(MenuSelect3());
            }
            else if (menuSelection == 4)
            {
                //create new send Transfer
                consoleOutput.PrintMenu("ID,Balance", MenuSelect1()); //print all accounts with balance
                consoleOutput.PrintMessage(MenuSelect4());
            }
            else if (menuSelection == 5)
            {
                //create new Request Transfer
                consoleOutput.PrintMenu("ID,Balance", MenuSelect1()); //print all accounts with balance
                consoleOutput.PrintMessage(MenuSelect5());
            }
            else if (menuSelection == 6)
            {
                //Add Account to current User
                consoleOutput.PrintMessage(MenuSelect6());
            }
            else if (menuSelection == 7)
            {
                //LogOut
                Console.WriteLine("");
            }
            else if (menuSelection == 0)
            {
                //Exits
                Console.WriteLine("Goodbye!");
                Environment.Exit(0);
            }
            else
            {
                //Invalid integer
                output = "Invalid selection.";
            }
            return output;
        }
        public IList<string> MenuSelect1() //view current balance
        {
            IList<string> balanceStringList = new List<string>();
            IList<ApiAccount> accounts = accountService.GetAccounts(user.UserId);
            foreach (ApiAccount account in accounts)
            {
                balanceStringList.Add(account.ToString());
            }
            return balanceStringList;
        }
        public string MenuSelect2() //print all completed Transfers
        {
            IList<ApiTransfer> transfers = accountService.GetTransfers(user.Username);
            List<string> transferStringList = TransfersToStrings(transfers);
            consoleOutput.PrintMenu("ID,From,To,Amount", transferStringList);
            //get Transfer ID
            string output = "";
            int transferId = consoleInput.PromptForTransferID("view details");
            if (transferId != 0)
            {
                //output = accountService.GetTransfer(user.Username, transferId).ToStringDetails();
                output = FindTransfer(transferId, transfers)?.ToStringDetails();
            }
            return output;
        }
        public string MenuSelect3() //print pending Request Transfers
        {
            IList<ApiTransfer> transfers = accountService.GetTransfers(user.Username, "pending");
            List<string> transferStringList = TransfersToStrings(transfers);
            consoleOutput.PrintMenu("ID,From,To,Amount", transferStringList);
            int transferId = consoleInput.PromptForTransferID("change approval");
            if (transferId != 0)
            {
                ApiTransfer updatedTransfer = FindTransfer(transferId, transfers);
                string output = "";
                output = updatedTransfer?.ToStringDetails();
                consoleOutput.PrintTransferDetails(output);
                if (!String.IsNullOrEmpty(output))
                {
                    int actionId = consoleInput.PromptForApproval();

                    if (actionId != 0) //Approve or Reject based on actionId
                    {
                        if (actionId == 1)
                        {
                            consoleOutput.PrintMenu("ID,Balance", MenuSelect1());
                            int fromAccountId = consoleInput.PromptForAccount();
                            updatedTransfer.AccountFrom = fromAccountId;
                        }
                        if (accountService.ActionTransfer(updatedTransfer, actionId))
                        {
                            string action = (actionId == 1) ? "Approved" : "Rejected";
                            return $"Transfer Successfully {action}";
                        }
                    }
                }
            }
            return "";
        }
        public string MenuSelect4() //create new send Transfer
        {
            int fromAccountId = consoleInput.PromptForAccount();
            IList<string> usersStringList = new List<string>();
            IList<ApiUser> users = accountService.GetUsers();
            foreach (ApiUser user in users)
            {
                usersStringList.Add(user.ToString());
            }
            consoleOutput.PrintMenu("ID,Name", usersStringList);
            int toUserId = consoleInput.PromptForUserID("sending to");

            decimal transferAmount = consoleInput.PromptForTransferAmount();

            // checking to make sure that all values are valid before sending request to server
            if (toUserId != 0 && fromAccountId != 0 && transferAmount != 0)
            {
                int transferId = accountService.Transfer(fromAccountId, toUserId, transferAmount, "Send");
                if (transferId > 0)
                {
                    return $"Transfer Success! Transfer ID number is : {transferId}";
                }
                else
                {
                    return "Transfer declined.";
                }
            }
            return "";
        }
        public string MenuSelect5() //create new request transfer
        {
            int toAccountId = consoleInput.PromptForAccount();
            IList<string> usersStringList = new List<string>();
            IList<ApiUser> users = accountService.GetUsers();
            foreach (ApiUser user in users)
            {
                usersStringList.Add(user.ToString());
            }
            consoleOutput.PrintMenu("ID,Name", usersStringList);
            int fromUserId = consoleInput.PromptForUserID("requesting from");

            decimal transferAmount = consoleInput.PromptForTransferAmount();

            // checking to make sure that all values are valid before sending request to server
            if (fromUserId != 0 && toAccountId != 0 && transferAmount != 0)
            {
                int transferId = accountService.Transfer(toAccountId, fromUserId, transferAmount, "Request");
                if (transferId > 0)
                {
                    return $"Transfer requested! Transfer ID number is : {transferId}";
                }
                else
                {
                    return "Transfer request not posted.";
                }
            }
            return "";
        }
        public string MenuSelect6() //Add Account to current User
        {
            int accountId = accountService.AddAccount(user.UserId);
            if (accountId != 0)
            {
                return $"Account {accountId} successfully created!";
            }
            else
            {
                return "Could not create account.";
            }
        }
        private List<string> TransfersToStrings(IList<ApiTransfer> transfers)
        {
            if (transfers != null)
            {
                List<string> sList = new List<string>();
                foreach (ApiTransfer transfer in transfers)
                {
                    sList.Add(transfer.ToString());
                }
                return sList;
            }
            else { return null; }  
        }

        private ApiTransfer FindTransfer(int transferId, IList<ApiTransfer> transfers)
        {
            ApiTransfer transfer = null;
            if (transfers != null)
            {
                transfer = transfers.FirstOrDefault<ApiTransfer>(t => t.TransferId == transferId);
            }
            return transfer;
        }
    }

}
