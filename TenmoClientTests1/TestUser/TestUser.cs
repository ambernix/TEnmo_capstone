using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClientTests1.TestUser
{
    public class TestUser : TenmoClient.Models.ApiUser
    {
        public TestUser()
        {
            UserId = 1;
            Username = "test";
            Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6InRlc3QiLCJuYmYiOjE2MjU2MDUxMzgsImV4cCI6MTYyNjIwOTkzOCwiaWF0IjoxNjI1NjA1MTM4fQ.mdD5tDKO9ibbwTWDokW8HrAuBWa0CnkPakLpJEnj9VQ";
            TenmoClient.UserService.SetLogin(this);
        }
    }
}
