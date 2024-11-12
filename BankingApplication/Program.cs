using System;
using System.Collections.Generic;

namespace BankingApplication
{
    class Program
    {
        static List<User> users = new List<User>();
        static User loggedInUser = null;

        static void Main(string[] args)
        {
            while (true)
            {
                if (loggedInUser == null)
                {
                    ShowLoginMenu();
                }
                else
                {
                    ShowMainMenu();
                }
            }
        }

        static void ShowLoginMenu()
        {
            Console.WriteLine("\n1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Register();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("\n1. Open Account");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Withdraw");
            Console.WriteLine("4. View Statement");
            Console.WriteLine("5. Calculate Interest");
            Console.WriteLine("6. Check Balance");
            Console.WriteLine("7. Logout");
            Console.Write("Select an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    OpenAccount();
                    break;
                case "2":
                    Deposit();
                    break;
                case "3":
                    Withdraw();
                    break;
                case "4":
                    ViewStatement();
                    break;
                case "5":
                    CalculateInterest();
                    break;
                case "6":
                    CheckBalance();
                    break;
                case "7":
                    loggedInUser = null;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }

        static void Register()
        {
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Username and password cannot be empty.");
                return;
            }

            if (users.Exists(u => u.Username == username))
            {
                Console.WriteLine("Username already exists. Try a different one.");
                return;
            }

            users.Add(new User { Username = username, Password = password });
            Console.WriteLine("Registration successful.");
        }

        static void Login()
        {
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();

            var user = users.Find(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                Console.WriteLine("Invalid credentials. Try again.");
                return;
            }

            loggedInUser = user;
            Console.WriteLine("Login successful.");
        }

        static void OpenAccount()
        {
            Console.Write("Enter account holder's name: ");
            var name = Console.ReadLine();
            Console.Write("Enter account type (savings/checking): ");
            var type = Console.ReadLine().ToLower();

            if (type != "savings" && type != "checking")
            {
                Console.WriteLine("Invalid account type. Please enter 'savings' or 'checking'.");
                return;
            }

            Console.Write("Enter initial deposit amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var initialDeposit) || initialDeposit < 0)
            {
                Console.WriteLine("Invalid deposit amount. Please enter a positive number.");
                return;
            }

            var account = new Account
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AccountHolderName = name,
                AccountType = type,
                Balance = initialDeposit
            };

            loggedInUser.Accounts.Add(account);
            Console.WriteLine("Account opened successfully.");
        }

        static void Deposit()
        {
            var account = SelectAccount();
            if (account == null) return;

            Console.Write("Enter deposit amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                Console.WriteLine("Invalid deposit amount. Please enter a positive number.");
                return;
            }

            account.Balance += amount;
            account.Transactions.Add(new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                Type = "Deposit",
                Amount = amount
            });

            Console.WriteLine("Deposit successful.");
        }

        static void Withdraw()
        {
            var account = SelectAccount();
            if (account == null) return;

            Console.Write("Enter withdrawal amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                Console.WriteLine("Invalid withdrawal amount. Please enter a positive number.");
                return;
            }

            if (amount > account.Balance)
            {
                Console.WriteLine("Insufficient funds.");
                return;
            }

            account.Balance -= amount;
            account.Transactions.Add(new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                Type = "Withdrawal",
                Amount = amount
            });

            Console.WriteLine("Withdrawal successful.");
        }

        static void ViewStatement()
        {
            var account = SelectAccount();
            if (account == null) return;

            Console.WriteLine("Transaction History:");
            foreach (var transaction in account.Transactions)
            {
                Console.WriteLine($"{transaction.Date} - {transaction.Type} - {transaction.Amount}");
            }
        }

        static void CalculateInterest()
        {
            var account = SelectAccount();
            if (account == null) return;

            if (account.AccountType.ToLower() != "savings")
            {
                Console.WriteLine("Interest calculation is only applicable to savings accounts.");
                return;
            }

            var interestRate = 0.01m; // 1% monthly interest rate
            var interest = account.Balance * interestRate;
            account.Balance += interest;

            account.Transactions.Add(new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                Type = "Interest",
                Amount = interest
            });

            Console.WriteLine("Interest calculated and added to the balance.");
        }

        static void CheckBalance()
        {
            var account = SelectAccount();
            if (account == null) return;

            Console.WriteLine($"Current balance: {account.Balance}");
        }

        static Account SelectAccount()
        {
            Console.WriteLine("Select an account:");
            for (int i = 0; i < loggedInUser.Accounts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {loggedInUser.Accounts[i].AccountHolderName} - {loggedInUser.Accounts[i].AccountType}");
            }

            if (!int.TryParse(Console.ReadLine(), out var choice) || choice < 1 || choice > loggedInUser.Accounts.Count)
            {
                Console.WriteLine("Invalid choice. Try again.");
                return null;
            }

            return loggedInUser.Accounts[choice - 1];
        }
    }

    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Account> Accounts { get; set; } = new List<Account>();
    }

    class Account
    {
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    class Transaction
    {
        public string TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }
}
