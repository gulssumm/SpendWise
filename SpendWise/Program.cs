using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

class Program
{
    static List<Transaction> transactions = new List<Transaction>();
    const string FilePath = "transactions.txt";

    static void Main()
    {
        LoadTransactions(); // Load saved transactions at startup

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
                    Console.WriteLine("\n✅ Exiting... Your data is saved. Have a great day! 🚀");
                    return;
                default:
                    Console.WriteLine("\n❌ Invalid option. Please try again.");
                    Pause();
                    break;
            }
        }
    }

    static void AddTransaction()
    {
        Console.Clear();
        Console.WriteLine("📌 ADD TRANSACTION");
        Console.Write("🔹 Enter description: ");
        string description = Console.ReadLine();

        Console.Write("🔹 Enter amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("\n❌ Invalid amount. Please enter a positive number.");
            Pause();
            return;
        }

        Console.Write("🔹 Select category (Food, Bills, Salary, Other): ");
        string category = Console.ReadLine();

        Console.Write("🔹 Is it an expense? (yes/no): ");
        bool isExpense = Console.ReadLine()?.Trim().ToLower() == "yes";

        transactions.Add(new Transaction(description, amount, isExpense, category, DateTime.Now));
        SaveTransactions();
        Console.WriteLine("\n✅ Transaction added successfully!");
        Pause();
    }

    static void ViewTransactions()
    {
        Console.Clear();
        Console.WriteLine("📜 TRANSACTION HISTORY");
        if (transactions.Count == 0)
        {
            Console.WriteLine("\n⚠️ No transactions found.");
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
        decimal balance = transactions.Sum(t => t.IsExpense ? -t.Amount : t.Amount);
        Console.WriteLine("💰 ACCOUNT BALANCE");
        Console.WriteLine("=========================");
        Console.WriteLine($"🔹 Current Balance: ${balance:F2}");
        Console.WriteLine("=========================");
        Pause();
    }

    static void ViewMonthlyReport()
    {
        Console.Clear();
        Console.Write("📆 Enter month (MM): ");
        string monthInput = Console.ReadLine();
        Console.Write("📆 Enter year (YYYY): ");
        string yearInput = Console.ReadLine();

        if (!int.TryParse(monthInput, out int month) || !int.TryParse(yearInput, out int year))
        {
            Console.WriteLine("\n❌ Invalid date format. Please enter a valid month and year.");
            Pause();
            return;
        }

        var filtered = transactions.Where(t => t.Date.Month == month && t.Date.Year == year).ToList();
        if (filtered.Count == 0)
        {
            Console.WriteLine("\n⚠️ No transactions found for this month.");
        }
        else
        {
            decimal income = filtered.Where(t => !t.IsExpense).Sum(t => t.Amount);
            decimal expenses = filtered.Where(t => t.IsExpense).Sum(t => t.Amount);
            decimal balance = income - expenses;

            Console.WriteLine("=================================");
            Console.WriteLine($" 📅 Monthly Report: {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year}");
            Console.WriteLine("=================================");
            Console.WriteLine($" Total Income  : ${income:F2}");
            Console.WriteLine($" Total Expenses: ${expenses:F2}");
            Console.WriteLine($" Final Balance : ${balance:F2}");
            Console.WriteLine("=================================");
        }
        Pause();
    }

    static void SaveTransactions()
    {
        using (StreamWriter writer = new StreamWriter(FilePath))
        {
            foreach (var t in transactions)
            {
                writer.WriteLine($"{t.Description}|{t.Amount}|{t.IsExpense}|{t.Category}|{t.Date}");
            }
        }
    }

    static void LoadTransactions()
    {
        if (!File.Exists(FilePath)) return;

        foreach (var line in File.ReadAllLines(FilePath))
        {
            var data = line.Split('|');
            if (data.Length == 5 && decimal.TryParse(data[1], out decimal amount) && DateTime.TryParse(data[4], out DateTime date))
            {
                transactions.Add(new Transaction(data[0], amount, bool.Parse(data[2]), data[3], date));
            }
        }
    }

    static void Pause()
    {
        Console.WriteLine("\n🔹 Press any key to return to the menu...");
        Console.ReadKey();
    }

    record Transaction(string Description, decimal Amount, bool IsExpense, string Category, DateTime Date);
}
