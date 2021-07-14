using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    // methods to take list style information from the server and print to console with appropriate headers
    class ConsoleServiceOutput 
    {
        public void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }
        public void PrintTransferDetails(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                string[] array = SplitString(input);
                if (array.Length == 6)
                {
                    Console.WriteLine($"Id: {array[0]}\nFrom: {array[1]}\nTo: {array[2]}\nType: {array[3]}\nStatus: {array[4]}\nAmount: {array[5]}\n");
                }
            }
            else
            {
                Console.WriteLine("No transfer found.");
            }
        }

        public void PrintTransfers(IList<string> inputs)
        {
            if (inputs != null)
            {
                foreach (string input in inputs)
                {
                    string[] array = SplitString(input);
                    Console.WriteLine("{0,-10}{1,-30}{2,-10}{3,-10}", array[0], array[1], array[2], $"{array[3]:c2}");
                }
            }
            else
            {
                Console.WriteLine("No transfers found.");
            }
        }

        public void PrintMenu(string header, IList<string> body)
        {
            Console.Clear();
            string[] array = SplitString(header);
            //printheader
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write("{0, -20}", array[i]);
            }
            int headerLength = array.Length * 20;
            Console.Write("\n");
            Console.WriteLine(new string('-', headerLength));
            if (body != null)
            {
                foreach (string line in body)
                {
                    string[] lineArray = SplitString(line);
                    for (int i = 0; i < lineArray.Length; i++)
                    {
                        Console.Write("{0, -20}", lineArray[i]);
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Nothing to Display.");
            }
        }
        private string[] SplitString(string input)
        {
            string[] array = input.Split(',');
            if (decimal.TryParse(array[^1], out decimal last))
            {
                array[^1] = $"{last:c2}";
            }
            return array;
        }
    }
}
