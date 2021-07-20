using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ATM.Model;
using Newtonsoft.Json;

namespace ATM
{
    public class Data
    {
        // appends an object to file in Json Format
        public void AddToFile<T>(T obj)
        {
            string jsonOutput = System.Text.Json.JsonSerializer.Serialize(obj);

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
            string jsonOutput = System.Text.Json.JsonSerializer.Serialize(list[0]);

            if (list[0] is Admin)
            {
                File.WriteAllText("admins.txt", jsonOutput + Environment.NewLine);
            }
            else if (list[0] is Customer)
            {
                File.WriteAllText("customers.txt", jsonOutput + Environment.NewLine);
            }

            for(int i = 1; i < list.Count; i++)
            {
                AddToFile(list[i]);
            }
        }

        // returns a list of objects from file

        public List<T> ReadFile<T>(string fileName, T obj)
        {
            List<T> list = new List<T>();
            string FilePath = Path.Combine(Environment.CurrentDirectory, fileName); ;
            if (File.Exists(fileName))
            { 
                StreamReader sr = new StreamReader(FilePath);

                string line = String.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(System.Text.Json.JsonSerializer.Deserialize<T>(line));
                }
                sr.Close();

                return list;
            }

            else
            {
                string data = System.Text.Json.JsonSerializer.Serialize(obj);
                File.WriteAllText(fileName, data + Environment.NewLine);
                StreamReader sr = new StreamReader(FilePath);

                string line = String.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(System.Text.Json.JsonSerializer.Deserialize<T>(line));
                }
                sr.Close();

                return list;
            }
            
        }

        // deletes a customer object from file

        public void DeleteCustomer(Customer customer)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt", customer);

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
            List<Customer> list = ReadFile<Customer>("customers.txt", customer);

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

        public bool IsAdminInFile(Admin user)
        {
             List<Admin> list = ReadFile<Admin>("admins.txt", user);

             foreach (Admin admin in list)
             {
                if (admin.Username == user.Username && admin.Pin == user.Pin)
                {
                   return true;
                }
             }

                return false;
            
        }

        // check if an user is active

        public int IsUserActive(string user)
        {
            Customer customer1 = new Customer();
            List<Customer> list = ReadFile<Customer>("customers.txt", customer1);

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
            List<Customer> list = ReadFile<Customer>("customers.txt", customer);

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
            Customer c = new Customer();
            List<Customer> list = ReadFile<Customer>("customers.txt", c);

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
            Customer c = new Customer();
            List<Customer> list = ReadFile<Customer>("customers.txt", c);

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
            Customer c = new Customer();
            List<Customer> list = ReadFile<Customer>("customers.txt", c);

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
            Customer c = new Customer();
            List<Customer> list = ReadFile<Customer>("customers.txt", c);

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
            if(File.Exists(Path.Combine(Environment.CurrentDirectory, "customers.txt")))
            {
                Customer c = new Customer();
                List<Customer> list = ReadFile<Customer>("customers.txt", c);

                if (list.Count > 0)
                {
                    Customer customer = list[list.Count - 1];

                    return customer.AccountNo;
                }

                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
            

            
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

        // total amount of money customer has withdrawn

        public int TodaysTransactionAmount(int accNo)
        {
            Transaction c = new Transaction();
            List<Transaction> list = ReadFile<Transaction>("customers.txt", c);

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
