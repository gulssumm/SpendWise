using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Transactions;
using SpendWise.Logic.Interfaces;
using SpendWise.Logic.Models;

namespace SpendWise.Logic.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly List<FinancialTransaction> transactions = new List<FinancialTransaction>();
        private readonly string FilePath = "transactions.txt"; 

        public void AddTransaction(string description, decimal amount, bool isExpense, string category)
        {
            transactions.Add(new FinancialTransaction(description, amount, isExpense, category, DateTime.Now));  // Updated
            SaveTransactions();
        }

        public List<FinancialTransaction> GetTransactions()  // Updated
        {
            return transactions;
        }

        public decimal GetBalance()
        {
            return transactions.Sum(t => t.IsExpense ? -t.Amount : t.Amount);
        }

        public List<FinancialTransaction> GetMonthlyReport(int month, int year)  // Updated
        {
            return transactions.Where(t => t.Date.Month == month && t.Date.Year == year).ToList();
        }

        public void LoadTransactions()
        {
            if (!File.Exists(FilePath)) return;

            foreach (var line in File.ReadAllLines(FilePath))
            {
                var data = line.Split('|');
                if (data.Length == 5 &&
                    !string.IsNullOrWhiteSpace(data[0]) &&
                    decimal.TryParse(data[1], out decimal amount) &&
                    bool.TryParse(data[2], out bool isExpense) &&
                    !string.IsNullOrWhiteSpace(data[3]) &&
                    DateTime.TryParse(data[4], out DateTime date))
                {
                    transactions.Add(new FinancialTransaction(data[0], amount, isExpense, data[3], date));  // Updated
                }
            }
        }

        public void SaveTransactions()
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                foreach (var transaction in transactions)
                {
                    writer.WriteLine($"{transaction.Description}|{transaction.Amount}|{transaction.IsExpense}|{transaction.Category}|{transaction.Date}");
                }
            }
        }

    }
}

