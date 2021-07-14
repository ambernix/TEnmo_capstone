using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    class ApiAccount
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public override string ToString()
        {
            return $"{this.AccountId},{this.Balance}";
        }
    }
}
