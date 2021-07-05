using System;
using ATM.Model;

namespace ATM
{
    public class View
    {
        public void LoginScreen()
        {
            Console.WriteLine("-----Welcome to ATM-----\n\n" +
                "Login as: \n" +
                "1-----Administrator\n" +
                "2-----Customer\n\n" +
                "Enter 1 or 2:");

            try
            {
            getUser:
                {
                    string user = Console.ReadLine();

                    if(user == "1" || user == "2")
                    {
                        switch (user)
                        {
                            case "1":
                                Console.WriteLine("----Administrator Login----\n" +
                                    "Please Enter your username & 5-digit Pin");
                                Admin admin = new Admin();
                                bool isSignedIn = false;
                                Logic logic = new Logic();
                                while (!isSignedIn)
                                {
                                    // reading and storing username
                                    Console.WriteLine("Username:");
                                    admin.Username = Console.ReadLine();


                                    Console.WriteLine("Pin:");
                                    admin.Pin = Console.ReadLine();


                                    //verify login

                                    if (logic.VerifyLogin(admin))
                                    {
                                        Console.WriteLine("\n---Logged in as Administrator---\n");
                                        isSignedIn = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Wrong username/pin. Try again");
                                    }
                                }
                                AdminScreen();
                                break;

                            case "2":
                                Console.WriteLine("----Customer Login----\n" +
                                   "Please Enter your username & 5-digit Pin");
                                Customer customer = new Customer();
                                Logic logic1 = new Logic();

                            getUsername:
                                {
                                    Console.WriteLine("Username: ");
                                    customer.Username = Console.ReadLine();


                                    if (logic1.IsValidUsername(customer.Username))
                                    {
                                        if(logic1.IsUserActive(customer.Username) == 1)
                                        {
                                            int wrong = 0;

                                        getPin:
                                            {
                                                Console.WriteLine("5 digit pin");
                                                customer.Pin = Console.ReadLine();


                                                if (logic1.VerifyLogin(customer))
                                                {
                                                    Console.WriteLine("\n---Logged in as Customer---\n");
                                                    CustomerScreen(customer.Username);
                                                }
                                                else
                                                {
                                                    wrong++;
                                                    if(wrong < 3)
                                                    {
                                                        Console.WriteLine("Wrong pin. Try again");
                                                        goto getPin;
                                                    }
                                                    else if(wrong == 3)
                                                    {
                                                        logic1.DisableAccount(customer.Username);
                                                        Console.WriteLine("Wrong pin input 3 times. Account disabled");
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        else if(logic1.IsUserActive(customer.Username) == 2)
                                        {
                                            Console.WriteLine("Your account is disabled. Contact the administrator");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Username does not exit! Try again");
                                            goto getUsername;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid input. Enter again");
                                    }
                                }
                                break;
                        }
                    }

                    else
                    {
                        Console.WriteLine("Wrong input. Please try again");
                        goto getUser;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void AdminScreen()
        {
        adminScreen:
            {
                Console.Clear();
                Console.WriteLine("----Admin Menu----\n");
                Console.WriteLine("1----Create New Account\n" +
                    "2----Delete Existing Account\n" +
                    "3----Update Account Information\n" +
                    "4----Search for Account\n" +
                    "5----Exit");

                try
                {
                getAdminOption:
                    {
                        string option = Console.ReadLine();

                        if(option == "1"||option == "2" || option == "3" || option == "4" || option == "5")
                        {
                            Logic logic = new Logic();
                            switch (option)
                            {
                                case "1":
                                    logic.CreateAccount();
                                    break;
                                case "2":
                                    logic.DeleteAccount();
                                    break;
                                case "3":
                                    logic.UpdateAccount();
                                    break;
                                case "4":
                                    logic.SearchAccount();
                                    break;
                                case "5":
                                    System.Environment.Exit(0);
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input, please try again");
                            goto getAdminOption;
                        }
                    }

                    Console.WriteLine("\nDo you wish to continue(Y/N):");
                    string wish = Console.ReadLine();
                    if(wish == "y" || wish == "Y")
                    {
                        goto adminScreen;
                    }
                    else
                    {
                        System.Environment.Exit(0);
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                }
            }
        }

        public void CustomerScreen(string username)
        {
        customerScreen:
            {
                Console.Clear();
                Console.WriteLine("----Customer Menu----\n");
                Console.WriteLine("1----Withdraw Cash\n" +
                    "2----Cash Transfer\n" +
                    "3----Deposit cash\n" +
                    "4----Exit");

                try
                {
                getCustomerOption:
                    {
                        string option = Console.ReadLine();

                        if (option == "1" || option == "2" || option == "3" || option == "4")
                        {
                            Logic logic = new Logic();
                            switch (option)
                            {
                                case "1":
                                    logic.WithdrawCash(username);
                                    break;
                                case "2":
                                    logic.TransferCash(username);
                                    break;
                                case "3":
                                    logic.DepositCash(username);
                                    break;
                                case "4":
                                    System.Environment.Exit(0);
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input, please try again");
                            goto getCustomerOption;
                        }
                    }

                    Console.WriteLine("\nDo you wish to continue(Y/N):");
                    string wish = Console.ReadLine();
                    if (wish == "y" || wish == "Y")
                    {
                        goto customerScreen;
                    }
                    else
                    {
                        System.Environment.Exit(0);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Please try again");
                }
            }
        }


    }
}
