using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ATM.Model;

namespace ATM
{
    public class Data
    {
        // appends an object to file in Json Format
        public void AddToFile<T>(T obj)
        {
            string jsonOutput = JsonSerializer.Serialize(obj);

            if(obj is Admin)
            {
                File.AppendAllText("admins.txt", jsonOutput + Environment.NewLine);
            }
            else if (obj is Customer)
            {
                File.AppendAllText("customers.txt", jsonOutput + Environment.NewLine);
            }
            else if (obj is Transaction)
            {
                File.AppendAllText("transactions.txt", jsonOutput + Environment.NewLine);
            }
        }

        // clears last data and save new list to file in json format
        public void SaveToFile<T>(List<T> list)
        {
            string jsonOutput = JsonSerializer.Serialize(list[0]);

            if (list[0] is Admin)
            {
                File.WriteAllText("admins.txt", jsonOutput + Environment.NewLine);
            }
            else if (list[0] is Customer)
            {
                File.AppendAllText("customers.txt", jsonOutput + Environment.NewLine);
            }

            for(int i = 1; i < list.Count; i++)
            {
                AddToFile(list[i]);
            }
        }

        // returns a list of objects from file

        public List<T> ReadFile<T>(string fileName)
        {
            List<T> list = new List<T>();
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
            StreamReader streamReader = new StreamReader(filePath);

            string line = string.Empty;

            while((line = streamReader.ReadLine()) != null)
            {
                list.Add(JsonSerializer.Deserialize<T>(line));
            }
            streamReader.Close();

            return list;
        }

        // deletes a customer object from file

        public void DeleteCustomer(Customer customer)
        {
            List<Customer> list = ReadFile<Customer>("customer.txt");

            foreach(Customer item in list)
            {
                if(item.AccountNo == customer.AccountNo)
                {
                    list.Remove(item);
                    break;
                }
            }

            SaveToFile<Customer>(list);
        }

        // updates a customer object in file

        public void UpdateInFile(Customer customer)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");

            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].AccountNo == customer.AccountNo)
                {
                    list[i] = customer;
                }
            }
            SaveToFile<Customer>(list);
        }


        // check if an admin object is in file

        public bool IsInFile(Admin user)
        {
            List<Admin> list = ReadFile<Admin>("admins.txt");

            foreach(Admin admin in list)
            {
                if(admin.Username == user.Username && admin.Pin == user.Pin)
                {
                    return true;
                }
            }

            return false;
        }

        // check if an user is active

        public int IsUserActive(string user)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");

            foreach (Customer customer in list)
            {
                if (customer.Username == user && customer.Status == "Active")
                {
                    return 1;
                }
                else if (customer.Username == user && customer.Status == "Disabled")
                {
                    return 2;
                }
            }

            return 0;
        }

        //checks if a user can login

        public bool CanLogin(Customer customer)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");

            foreach(Customer user in list)
            {
                if(customer.Username == user.Username && customer.Pin == user.Pin && user.Status == "Active")
                {
                    return true;
                }
            }

            return false;
        }

        // checks if an account number is in file

        public bool IsAccountInFile(int accNo, out Customer customer)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");

            foreach(Customer user in list)
            {
                if(user.AccountNo == accNo)
                {
                    customer = user;
                    return true;
                }
            }
            customer = null;
            return false;
        }

        // checks if username is in file

        public bool IsUsernameInFile(string userName)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");

            foreach(Customer customer in list)
            {
                if(customer.Username == userName)
                {
                    return true;
                }
            }

            return false;
        }

        // returns an object if given a username

        public Customer GetCustomer(string userName)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");

            foreach(Customer customer in list)
            {
                if (customer.Username == userName)
                {
                    return customer;
                }
            }
            return null;
        }

        // returns an object if given accountNo

        public Customer GetCustomer(int accountNo)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");

            foreach (Customer customer in list)
            {
                if (customer.AccountNo == accountNo)
                {
                    return customer;
                }
            }
            return null;
        }

        // get last account number

        public int getLastAccountNumber()
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");

            if(list.Count > 0)
            {
                Customer customer = list[list.Count - 1];

                return customer.AccountNo;
            }

            return 0;
        }

        // deduct amount from balance of an account and update in file

        public void DeductBalance(Customer c, int amount)
        {
            c.Balance -= amount;
            UpdateInFile(c);
        }

        // add amount from balance of an account and update in file

        public void AddAmount(Customer c, int amount)
        {
            c.Balance += amount;
            UpdateInFile(c);
        }

        // withdraw total amount of money customer has withdrawn

        public int TodaysTransactionAmount(int accNo)
        {
            List<Transaction> list = ReadFile<Transaction>("customers.txt");

            int totalAmount = 0;

            foreach(Transaction t in list)
            {
                if(t.AccountNo == accNo)
                {
                    totalAmount += t.TransactionAmount;
                }
            }
            return totalAmount;
        }
    }
}
