using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.EndUser
{
    public class ConsoleUser : IUserInput
    {
        public string GetInput()
        {
            return Console.ReadLine();
        }
    }
}
