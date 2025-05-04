using System;
using System.Collections.Generic;
using System.Globalization;
using SpendWise.Logic.Interfaces; // Import logic layer
using SpendWise.Data;
using SpendWise.Logic;

class Program
{
    static ITransactionRepository repository = new TransactionRepository();
    static ITransactionService transactionService = new TransactionService(repository);

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=================================");
            Console.WriteLine("   PERSONAL FINANCE TRACKER   ");
            Console.WriteLine("=================================");
            Console.WriteLine("1  Add Transaction");
            Console.WriteLine("2  View Transactions");
            Console.WriteLine("3  Check Balance");
            Console.WriteLine("4  View Monthly Report");
            Console.WriteLine("5  Exit");
            Console.WriteLine("=================================");
            Console.Write(" Choose an option: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddTransaction();
                    break;
                case "2":
                    ViewTransactions();
                    break;
                case "3":
                    CheckBalance();
                    break;
                case "4":
                    ViewMonthlyReport();
                    break;
                case "5":
                    Console.WriteLine("\n Exiting... Your data is saved. Have a great day! 🚀");
                    return;
                default:
                    Console.WriteLine("\n Invalid option. Please try again.");
                    Pause();
                    break;
            }
        }
    }

    static void AddTransaction()
    {
        Console.Clear();
        Console.WriteLine("ADD TRANSACTION");
        Console.Write("Enter description: ");
        string description = Console.ReadLine();

        Console.Write("Enter amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("\n Invalid amount. Please enter a positive number.");
            Pause();
            return;
        }

        Console.Write("Select category (Food, Bills, Salary, Other): ");
        string categoryInput = Console.ReadLine();
        // Convert to CatalogItem
        var category = new TransactionCategory(categoryInput, $"Auto-generated for {categoryInput}");


        Console.Write("Is it an expense? (yes/no): ");
        bool isExpense = Console.ReadLine()?.Trim().ToLower() == "yes";

        // Mock User
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "DefaultUser" 
        };


        transactionService.AddTransaction(description, amount, isExpense, category, user);
        Console.WriteLine("\n Transaction added successfully!");
        Pause();
    }

    static void ViewTransactions()
    {
        Console.Clear();
        Console.WriteLine("TRANSACTION HISTORY");
        var transactions = transactionService.GetTransactions();

        if (transactions.Count == 0)
        {
            Console.WriteLine("\n No transactions found.");
        }
        else
        {
            Console.WriteLine("===================================================================");
            Console.WriteLine(" Date        | Description  | Type      | Category  | Amount");
            Console.WriteLine("===================================================================");
            foreach (var t in transactions)
            {
                string type = t.IsExpense ? "Expense" : "Income ";
                Console.WriteLine($" {t.Date:yyyy-MM-dd} | {t.Description,-12} | {type,-8} | {t.Category,-8} | ${t.Amount:F2}");
            }
            Console.WriteLine("===================================================================");
        }
        Pause();
    }

    static void CheckBalance()
    {
        Console.Clear();
        decimal balance = transactionService.GetBalance();
        Console.WriteLine("ACCOUNT BALANCE");
        Console.WriteLine("=========================");
        Console.WriteLine($"Current Balance: ${balance:F2}");
        Console.WriteLine("=========================");
        Pause();
    }

    static void ViewMonthlyReport()
    {
        Console.Clear();
        Console.Write("Enter month (MM): ");
        if (!int.TryParse(Console.ReadLine(), out int month))
        {
            Console.WriteLine("\n❌ Invalid month format.");
            Pause();
            return;
        }

        Console.Write("Enter year (YYYY): ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("\n Invalid year format.");
            Pause();
            return;
        }

        var report = transactionService.GetMonthlyReport(month, year);
        if (report.Count == 0)
        {
            Console.WriteLine("\n No transactions found for this month.");
        }
        else
        {
            decimal income = report.Where(t => !t.IsExpense).Sum(t => t.Amount);
            decimal expenses = report.Where(t => t.IsExpense).Sum(t => t.Amount);
            decimal balance = income - expenses;

            Console.WriteLine("=================================");
            Console.WriteLine($"Monthly Report: {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year}");
            Console.WriteLine("=================================");
            Console.WriteLine($" Total Income  : ${income:F2}");
            Console.WriteLine($" Total Expenses: ${expenses:F2}");
            Console.WriteLine($" Final Balance : ${balance:F2}");
            Console.WriteLine("=================================");
        }
        Pause();
    }

    static void Pause()
    {
        Console.WriteLine("\n Press any key to return to the menu...");
        Console.ReadKey();
    }
}
