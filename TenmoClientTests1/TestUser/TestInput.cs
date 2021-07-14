using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.EndUser;

namespace TenmoClientTests1.TestUser
{
    class TestInput : IUserInput
    {
        public string output { get; set; }
        public TestInput(string input)
        {
            output = input;
        }
        public string GetInput()
        {
            return output;
        }

    }
}
