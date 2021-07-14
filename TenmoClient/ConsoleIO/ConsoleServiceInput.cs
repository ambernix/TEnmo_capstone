using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.EndUser;

namespace TenmoClient
{
    public class ConsoleServiceInput
    {
        public readonly IUserInput input;
        public ConsoleServiceInput(IUserInput _input)
        {
            input = _input;
        }
        public int PromptForMainMenu()
        {
            Console.WriteLine("");
            Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
            Console.WriteLine("1: View your current balance");
            Console.WriteLine("2: View your past transfers");
            Console.WriteLine("3: View your pending requests");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Add Account");
            Console.WriteLine("7: Log in as different user");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
            Console.Write("Please choose an option: ");

            if (!int.TryParse(input.GetInput(), out int menuSelection))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
                return -1;
            }
            else
            {
                return menuSelection;
            }
        }
    /// <summary>
    /// Prompts for transfer ID to view, approve, or reject
    /// </summary>
    /// <param name="action">String to print in prompt. Expected values are "Approve" or "Reject" or "View"</param>
    /// <returns>ID of transfers to view, approve, or reject</returns>
    public int PromptForTransferID(string action)
        {
            Console.WriteLine("");
            Console.Write($"Please enter transfer ID to {action} (0 to cancel): ");
            if (!int.TryParse(input.GetInput(), out int actionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return actionId;
            }
        }
        public int PromptForApproval()
        {
            Console.WriteLine();
            Console.WriteLine("1: Approve");
            Console.WriteLine("2: Reject");
            Console.Write($"Please enter option (0 to cancel): ");
            if (!int.TryParse(input.GetInput(), out int actionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return actionId;
            }
        }
        public int PromptForAccount()
        {
            Console.WriteLine("");
            Console.Write("Enter account ID to transfer with (0 to cancel): ");
            if (!int.TryParse(input.GetInput(), out int accountId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return accountId;
            }
        }
        public int PromptForUserID(string action)
        {
            Console.WriteLine("");
            Console.Write($"Please enter user ID you are {action} (0 to cancel): ");
            if (!int.TryParse(input.GetInput(), out int userId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return userId;
            }
        }
        public decimal PromptForTransferAmount()
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer amount (0 to cancel): ");
            if (!decimal.TryParse(input.GetInput(), out decimal transferAmount))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return transferAmount;
            }
        }
        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            string username = input.GetInput();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }
        public int PromptForConfirmation()
        {
            Console.Write("Enter 1 to register user (0 to cancel): ");
            if (!int.TryParse(input.GetInput(), out int choice) || choice != 1)
            {
                Console.WriteLine("User not registered.");
                return 0;
            }
            else
            {
                return 1;
            }
        }
        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }
    }
}
