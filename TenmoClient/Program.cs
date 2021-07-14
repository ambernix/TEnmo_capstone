using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleServiceInput consoleService = new ConsoleServiceInput(new EndUser.ConsoleUser());
        private static readonly AuthService authService = new AuthService();

        static void Main()
        {
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option (0 to exit): ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {

                    LoginUser loginUser = consoleService.PromptForLogin();
                    ApiUser user = authService.Login(loginUser);
                    if (user != null)
                    {
                        UserService.SetLogin(user);
                        StartMenu(user);
                    }
                    else
                    {
                        loginRegister = -1;
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered;

                    LoginUser registerUser = consoleService.PromptForLogin();
                    int confirm = consoleService.PromptForConfirmation();
                    if (confirm == 1)
                    {
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                        }
                    }
                    loginRegister = -1; //reset outer loop to allow choice for login/register
                }
                else if (loginRegister == 0)
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
        }
        private static void StartMenu(ApiUser user)
        {
            Menu menu = new Menu(user, new EndUser.ConsoleUser());
            menu.MenuSelection();
            
            UserService.SetLogin(new ApiUser()); //wipe out previous login info
            Program.Run(); //return to entry point
        }
    }
}
