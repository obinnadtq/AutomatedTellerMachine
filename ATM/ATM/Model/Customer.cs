using System;
namespace ATM.Model
{
    public class Customer
    {
        public string Username { get; set; }
        public string Pin { get; set; }
        public string Name { get; set; }
        public string AccountType { get; set; }
        public int Balance { get; set; }
        public string Status { get; set; }
        public int AccountNo { get; set; }
    }
}
