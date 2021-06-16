using System;
using ATM.Model;

namespace ATM
{
    public class Logic
    {
       // method to verify login of admin

        public bool VerifyLogin(Admin admin)
        {
            Data adminData = new Data();
            return adminData.IsInFile(admin);
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
        // Encryption Method
        // For alphabets we swap A with Z, B with Y and so on.
        // A B C D E F G H I J K L M N O P Q R S T U V W X Y Z
        // Z Y X W V U T S R Q P O N M L K J I H G F E D C B A
        // For Number we have
        // 0123456789
        // 9876543210

        public string EncryptionDecryption(string userName)
        {
            string output = "";
            foreach(char c in userName)
            {
                if(c >= 'A' && c <= 'Z')
                {
                    output += Convert.ToChar(('Z' - (c - 'A')));
                }
                else if(c >= 'a' && c <= 'z')
                {
                    output += Convert.ToChar(('z' - (c - 'a')));
                }
                else if (c >= '0' && c <= '9')
                {
                    output += Convert.ToChar(9 - char.GetNumericValue(c));
                }
            }
            return output;
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

        public void CreateAccount()
        {
            Data data = new Data();
            Customer customer = new Customer();
            Console.WriteLine("-----Creating New Account-------");
            Console.WriteLine("Please enter User Details");

        getUsername:
            {
                Console.Write("Username: ");
                string userName = Console.ReadLine();

                if(userName=="" || !IsValidUsername(userName))
                {
                    Console.WriteLine("Enter valid Username (Username can only contain A-Z, a-z & 0-9)");
                    goto getUsername;
                }

                customer.Username = EncryptionDecryption(userName);

                if (data.IsUsernameInFile(userName))
                {
                    Console.WriteLine("Username already exists!! Enter again.");
                    goto getUsername;
                }
            }

        getPin:
            {
                Console.WriteLine("5-digit pin: ");
                string pin = Console.ReadLine();

                if (!IsPinValid(pin))
                {
                    Console.WriteLine("Enter valid Pin (Pin is 5-digit & can only contain 0-9)");
                    goto getPin;
                }

                customer.Pin = EncryptionDecryption(pin);
            }

            Console.WriteLine("Holder's name: ");
            string name = Console.ReadLine();
            if(name != ""|| name != " ")
            {
                customer.Name = name;
            }

        getAccountType:
            {
                Console.WriteLine("Account Type (Current/Savings): ");
                customer.AccountType = Console.ReadLine();

                if (!(customer.AccountType == "Savings" || customer.AccountType == "Current"))
                {
                    Console.WriteLine("Wrong Input. Enter \"Savings\" & \"Current\"");
                    goto getAccountType;

                }
            }
        // get starting balance
        getBalance:
            {
                try
                {
                    Console.WriteLine("Starting balance: ");
                    customer.Balance = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.WriteLine("Wrong Input. Enter numbers only.");
                    goto getBalance;
                }
            }
        getStatus:
            {
                Console.WriteLine("Status (Active/Disabled): ");
                customer.Status = Console.ReadLine();

                if (!(customer.Status == "Active" || customer.Status == "Disabled"))
                {
                    Console.WriteLine("Wrong Input. Enter \"Active\" & \"Disabled\"");
                    goto getStatus;
                }
            }

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
                catch (Exception)
                {
                    Console.WriteLine("No account was deleted");
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
                PrintAccontDetails(customer);
                Console.WriteLine("\nPlease enter in the fields you wish to update (leave blank otherwise):\n");

            getUsername:
                {
                    string userName = getUsername();
                    customer.Username = EncryptionDecryption(userName);

                    if (data.IsUsernameInFile(customer.Username))
                    {
                        Console.WriteLine("Username already exists! Enter again!");
                        goto getUsername;
                    }

                }

                string pin = getPin();
                if (!string.IsNullOrEmpty(pin))
                {
                    customer.Pin = EncryptionDecryption(pin);
                }

                string name = getName();
                if(name != null)
                {
                    customer.Name = name;
                }

                string status = getStatus();
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

        // TODO: Search account, View Reports, customer logic

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
        public void PrintAccontDetails(Customer account)
        {
            Console.WriteLine($"Account # {account.AccountNo}\n" +
                $"Type: {account.AccountType}\n" +
                $"Holder: {account.Name}\n" + "" +
                $"Balance: {account.Balance}\n" +
                $"Status: {account.Status}");
        }

        public string getUsername()
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

        public string getPin()
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
        public string getName()
        {
            Console.Write("Holder's Name: ");
            string name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name) || !string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
            return null;
        }

        public string getStatus()
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

    }

}
