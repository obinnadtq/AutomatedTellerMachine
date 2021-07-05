using System;
using System.Collections.Generic;
using System.IO;
using ATM.Model;

namespace ATM
{
    public class Logic
    {
       // method to verify login of admin

        public bool VerifyLogin(Admin admin)
        {
            Data adminData = new Data();
            return adminData.IsAdminInFile(admin);
        }

        // method to verify if username is in file

        public int IsUserActive(string user)
        {
            Data data = new Data();
            return data.IsUserActive(user);
        }

        // method to verify login of customer

        public bool VerifyLogin(Customer customer)
        {
            Data customerData = new Data();
            return customerData.CanLogin(customer);
        }

        // Method to check if Username is valid or not (Username can only contain A-Z, a-z & 0-9)

        public bool IsValidUsername(string s)
        {
            foreach(char c in s)
            {
                if((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        // Method to check if Pin is valid or not (Pin is 5-digit & can only contain 0-9)

        public bool IsPinValid(string pin)
        {
            if(pin.Length != 5)
            {
                return false;
            }

            foreach(char c in pin)
            {
                if(c >= '0' && c <= '9')
                {
                    continue;
                }
                else
                {
                    return false;
                } 
            }
            return true;
        }
        

        // Disables an account

        public void DisableAccount(string userName)
        {
            Data data = new Data();
            Customer customer = data.GetCustomer(userName);
            customer.Status = "Disabled";

            data.UpdateInFile(customer);
        }

        // creates an account

        public void  CreateAccount()
        {
            // TODO: Authentication for account creation.
            Data data = new Data();
            Customer customer = new Customer();
            Console.WriteLine("-----Creating New Account-------");
            Console.WriteLine("Please enter User Details");

            Console.Write("Username: ");
            string userName = Console.ReadLine();
            customer.Username = userName;
                

            Console.WriteLine("5-digit pin: ");
            customer.Pin = Console.ReadLine();

            Console.WriteLine("Holder's name: ");
            customer.Name = Console.ReadLine();

            Console.WriteLine("Account Type (Current/Savings): ");
            customer.AccountType = Console.ReadLine();

            Console.WriteLine("Starting balance: ");
            customer.Balance = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Status (Active/Disabled): ");
            customer.Status = Console.ReadLine();

            customer.AccountNo = data.getLastAccountNumber() + 1;

            data.AddToFile(customer);

            Console.WriteLine($"Account Successfully Created – the account number assigned is: {customer.AccountNo}");
        }

        public void DeleteAccount()
        {
            int accNo = getAccNum();

            Data data = new Data();
            Customer customer = new Customer();

            if(data.IsAccountInFile(accNo, out customer))
            {
                Console.Write($"You wish to delete the account held by {customer.Name}.\n" +
                    "If this information is correct please re-enter the account number: ");
                try
                {
                    int tempAccNo = Convert.ToInt32(Console.ReadLine());

                    if(tempAccNo == accNo)
                    {
                        data.DeleteCustomer(customer);
                        Console.WriteLine("Account deleted successfully");
                        return;
                    }

                    else
                    {
                        Console.WriteLine("No account was deleted");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine($"Account number {accNo} does not exist!");
            }
        }

        public void UpdateAccount()
        {
            int accNo = getAccNum();

            Data data = new Data();
            Customer customer = new Customer();

            if(data.IsAccountInFile(accNo, out customer))
            {
                PrintAccountDetails(customer);
                Console.WriteLine("\nPlease enter in the fields you wish to update (leave blank otherwise):\n");

            getUsername:
                {
                    string userName = GetUserName();
                    customer.Username = userName;

                    if (data.IsUsernameInFile(customer.Username))
                    {
                        Console.WriteLine("Username already exists! Enter again!");
                        goto getUsername;
                    }

                }

                string pin = GetPin();

                string name = GetName();
                if(name != null)
                {
                    customer.Name = name;
                }

                string status = GetStatus();
                if(status != null)
                {
                    customer.Status = status;
                }

                data.UpdateInFile(customer);
                Console.WriteLine($"Account # {customer.AccountNo} has been successfully been updated.");
            }
            else
            {
                Console.WriteLine($"Account number {accNo} does not exist!");
                return;
            }
        }

        // method to search account info

        public void SearchAccount()
        {
            Customer customer = new Customer();

            Console.WriteLine("----SEARCH MENU----\n");
            Console.WriteLine("Please enter in the fields you wish to include in search (leave blank otherwise): \n");

            string accNum = string.Empty;

        getAccountNumber:
            {
                Console.WriteLine("Account Number: ");
                accNum = Console.ReadLine();
                if (!String.IsNullOrEmpty(accNum))
                {
                    try
                    {
                        customer.AccountNo = Convert.ToInt32(accNum);
                    }

                    catch (Exception)
                    {
                        Console.WriteLine("Invalid input! Enter a number.");
                        goto getAccountNumber;
                    }
                }
            }

            customer.Username = GetUserName();

            string name = GetName();
            customer.Name = name;

            string type = GetAccountType();
            customer.AccountType = type;

            Data data = new Data();
            List<Customer> list = data.ReadFile<Customer>("/Users/obinnaisiwekpeni/Desktop/customers.txt");

            List<Customer> outList = new List<Customer>();

            if(list.Count > 0)
            {
                foreach(Customer c in list)
                {
                    if(customer.AccountNo == c.AccountNo &&
                        customer.AccountType == c.AccountType &&
                        customer.Username == c.Username &&
                        customer.Name == c.Name)
                    {
                        outList.Add(c);
                    }
                }
            }

            Console.WriteLine("This is your search result");
            if(outList.Count > 0)
            {
                Console.WriteLine("Account No".PadRight(12)
                    + "Username".PadRight(10)
                    + "Holder's Name".PadRight(15)
                    + "Account Type".PadRight(9));

                foreach(Customer c1 in outList)
                {
                    Console.WriteLine(Convert.ToString(c1.AccountNo).PadRight(12)
                        + c1.Username.PadRight(10)
                        + c1.Name.PadRight(15)
                        + c1.AccountType.PadRight(9));
                }
            }

            else
            {
                Console.WriteLine("No data found matching the details");
            }


        }


        // get account number from user to be used in delete account and updating account
        public int getAccNum()
        {
            int accNo;
        getAccNo:
            {
                Console.Write("Account number: ");
                try
                {
                    accNo = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid Input! Please try again.");
                    goto getAccNo;
                }
            }
            return accNo;
        }
        public void PrintAccountDetails(Customer account)
        {
            Console.WriteLine($"Account # {account.AccountNo}\n" +
                $"Type: {account.AccountType}\n" +
                $"Holder: {account.Name}\n" + "" +
                $"Balance: {account.Balance}\n" +
                $"Status: {account.Status}");
        }

        public string GetUserName()
        {
        getUsername:
            {
                Console.Write("Username: ");
                string un = Console.ReadLine();

                // Checks if Username is valid or not (Username can only contain A-Z, a-z & 0-9)
                if (!IsValidUsername(un) || un == " ")
                {
                    Console.WriteLine("Enter valid Username (Username can only contain A-Z, a-z & 0-9)");
                    goto getUsername;
                }
                // if username is valid or empty
                else
                {
                    return un;
                }
            }
        }

        public string GetPin()
        {
        getPin:
            {
                Console.Write("5-digit Pin: ");
                string pin = Console.ReadLine();

                // Checks if Pin is valid or not (Pin is 5-digit & can only contain 0-9)
                if (!string.IsNullOrEmpty(pin) && !IsPinValid(pin))
                {
                    Console.WriteLine("Enter valid Pin (Pin is 5-digit & can only contain 0-9)");
                    goto getPin;
                }
                // if pin is valid, changes its value else do nothing
                else if (IsPinValid(pin))
                {
                    return pin;
                }
                return null;
            }
        }
        public string GetName()
        {
            Console.Write("Holder's Name: ");
            string name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name) || !string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
            return null;
        }

        public string GetStatus()
        {
        getStatus:
            {
                Console.Write("Status (Active/Disabled): ");
                string status = Console.ReadLine();

                // Checks if Status is valid
                if (status != "" && !(status == "Active" || status == "Disabled"))
                {
                    Console.WriteLine("Wrong Input. Enter \"Active\" & \"Disabled\"");
                    goto getStatus;
                }
                // changing status if entered valid
                else if (status == "Active" || status == "Disabled")
                {
                    return status;
                }
                return null;
            }
        }

        public string GetAccountType()
        {
        getAcccountType:
            {
                Console.WriteLine("Account Type (Savings/Current): ");
                string type = Console.ReadLine();

                if(!string.IsNullOrEmpty(type) && !(type == "Savings" || type == "Current"))
                {
                    Console.WriteLine("Wrong Input. Enter either Savings or Current");
                    goto getAcccountType;
                }

                return type;
            }
               
        }

        // CUSTOMER LOGIC BEGINS HERE

        // Method to withdraw cash

        public void WithdrawCash(string userName)
        {
            Console.WriteLine("---Withdraw cash---");
            List<int> cashOptions = new List<int>(new int[] { 500, 1000, 2000, 5000, 10000, 20000 });
            Console.WriteLine($"1---{cashOptions[0]}\n" +
                                    $"2---{cashOptions[1]}\n" +
                                    $"3---{cashOptions[2]}\n" +
                                    $"4---{cashOptions[3]}\n" +
                                    $"5---{cashOptions[4]}\n" +
                                    $"6---{cashOptions[5]}\n" +
                                    $"7---{cashOptions[6]}\n");
            Console.WriteLine("Select one of the denominations");

            string op = Console.ReadLine();

            if (op == "1" || op == "2" || op == "3" || op == "4" || op == "5" || op == "6" || op == "7")
            {
                Data data = new Data();
                int opt = Convert.ToInt32(op);
                Console.WriteLine($"Are you sure you want to withdraw {cashOptions[opt - 1]} Naira (Y/N)?");

                if (Console.ReadLine() == "Y" || Console.ReadLine() == "y")
                {
                    Customer customer = data.GetCustomer(userName);
                    int totalAmount = data.TodaysTransactionAmount(customer.AccountNo);
                    if (customer != null && customer.Balance > cashOptions[opt - 1])
                    {
                        data.DeductBalance(customer, cashOptions[opt - 1]);
                        Console.WriteLine("Cash successfully withdrawn");


                        // recording the transaction
                        Transaction transaction = MakeTransaction(customer, cashOptions[opt - 1], "Cash Withdrawal");

                        Console.WriteLine("Do you wish to print a receipt (Y/N)?");

                        if (Console.ReadLine() == "Y" || Console.ReadLine() == "y")
                        {
                            PrintReceipt(transaction, "Withdrawn");
                        }

                    }

                    else
                    {
                        Console.WriteLine("Insufficient Balance. Transaction Failed!");
                    }

                }

            }
        }

        // method to transfer cash

        public void TransferCash(string userName)
        {
            Data data = new Data();

            Customer sender = new Customer();

            sender = data.GetCustomer(userName);

        getAmount:
            {
                Console.WriteLine("Transfer cash");
                Console.WriteLine("Enter amount in multiple of 500");
                try
                {
                    int amount = Convert.ToInt32(Console.ReadLine());

                    if(amount % 500 == 0)
                    {
                        if(amount <= sender.Balance)
                        {
                        getAccNo:
                            {
                                Console.WriteLine("Enter the account number you want to transfer to:");
                                try
                                {
                                    int accNo = Convert.ToInt32(Console.ReadLine());
                                    Customer receiver = new Customer();
                                    if(data.IsAccountInFile(accNo, out receiver))
                                    {
                                        Console.WriteLine($"You wish to transfer {amount} Naira to {receiver.Name}. Press Y to continue");

                                        if(Console.ReadLine() == "y" || Console.ReadLine() == "Y")
                                        {
                                            data.DeductBalance(sender, amount);

                                            data.AddAmount(receiver, amount);

                                            Console.WriteLine("Transaction confirmed");

                                            Transaction transaction = MakeTransaction(sender, amount, "Cash Transfer");

                                            Transaction transaction1 = MakeTransaction(receiver, amount, "Cash Transfer");

                                            Console.WriteLine("Do you want a receipt (Y/N)?");

                                            if(Console.ReadLine() == "Y" || Console.ReadLine() == "y")
                                            {
                                                PrintReceipt(transaction, "Amount Transferred");
                                            }
                                        }
                                    }

                                    else
                                    {
                                        Console.WriteLine("Given account does not exist");
                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Invalid Input. Try again");
                                    goto getAccNo;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Insufficient funds");
                        }
                    }

                    else
                    {
                        Console.WriteLine("Invalid Input. Try again");
                        goto getAmount;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid Input. Try again");
                    goto getAmount;

                }
            }


        }

        public void DepositCash(string userName)
        {
            Data data = new Data();

            Customer customer = new Customer();
            customer = data.GetCustomer(userName);
        getAmount:
            {
                Console.WriteLine("----- Deposit Cash -----\n");
                Console.Write("Enter amount to deposit: ");
                try
                {
                    int amount = Convert.ToInt32(Console.ReadLine());
                    // Add amount to the account
                    data.AddAmount(customer, amount);
                    Console.WriteLine("Cash Deposited Successfully.");

                    // Making and recording transaction to file for Sender
                    Transaction transaction = MakeTransaction(customer, amount, "Cash Deposit");

                    // Asking if user wants a receipt
                    Console.Write("Do you wish to print a receipt(Y/N)? ");
                    string y = Console.ReadLine();
                    if (y == "Y" || y == "y")
                    {
                        // printing receipt
                        PrintReceipt(transaction, "Amount Deposited");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid Input. Try again!");
                    goto getAmount;
                }
            }
        }

        // method to make a transaction and record in the file
        public Transaction MakeTransaction(Customer c, int amount, string type)
        {
            Transaction transaction = new Transaction();
            transaction.AccountNo = c.AccountNo;
            transaction.Username = c.Username;
            transaction.HoldersName = c.Name;
            transaction.TransactionType = type;
            transaction.TransactionAmount = amount;
            DateTime date = DateTime.Now;
            transaction.Date = date.ToString("dd/mm/yyyy");
            transaction.Balance = c.Balance;

            // appending to transactions.txt
            Data data = new Data();
            data.AddToFile<Transaction>(transaction);
            return transaction;
        }

        public void PrintReceipt(Transaction transaction, string t)
        {
            Console.WriteLine($"Account No: {transaction.AccountNo}");
            Console.WriteLine($"Date: {transaction.Date}");
            Console.WriteLine($"{t}: {transaction.TransactionAmount}");
            Console.WriteLine($"Balance: {transaction.Balance}");
        }

    }

}
