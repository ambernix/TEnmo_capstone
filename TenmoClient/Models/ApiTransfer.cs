using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    class ApiTransfer
    {
        public int TransferId { get; set; }
        public string UsernameFrom { get; set; }
        public string UsernameTo { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
        public override string ToString()
        {
            return $"{TransferId},{UsernameFrom},{UsernameTo},{Amount}";
        }
        public string ToStringDetails()
        {
            return $"{TransferId},{UsernameFrom},{UsernameTo},{TransferType},{TransferStatus},{Amount}";
        }

    }
}
