using Data;
using System;
using System.Collections.Generic;

namespace DataTest
{
    public static class TestDataGenerator
    {
        private static readonly Random _random = new Random();

        // Method for ProcessStateTests
        public static FinancialTransaction GenerateTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
        {
            return new FinancialTransaction(description, amount, isExpense, category, date);
        }

        public static List<FinancialTransaction> GenerateRandomTransactions(int count)
        {
            var transactions = new List<FinancialTransaction>();
            var categories = new[] { "Food", "Transport", "Entertainment", "Utilities", "Salary", "Freelance" };
            var descriptions = new[] { "Grocery shopping", "Gas station", "Movie tickets", "Electricity bill", "Monthly salary", "Project payment" };

            for (int i = 0; i < count; i++)
            {
                var isExpense = _random.NextDouble() > 0.3;
                var amount = (decimal)(_random.NextDouble() * 1000 + 10);
                var category = categories[_random.Next(categories.Length)];
                var description = descriptions[_random.Next(descriptions.Length)];
                var date = DateTime.Today.AddDays(-_random.Next(90));

                transactions.Add(new FinancialTransaction(description, amount, isExpense, category, date)
                {
                    Id = i + 1,
                    UserId = Guid.NewGuid()
                });
            }

            return transactions;
        }

        public static List<User> GenerateTestUsers(int count)
        {
            var users = new List<User>();
            var firstNames = new[] { "John", "Jane", "Bob", "Alice", "Charlie", "Diana" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia" };

            for (int i = 0; i < count; i++)
            {
                var firstName = firstNames[_random.Next(firstNames.Length)];
                var lastName = lastNames[_random.Next(lastNames.Length)];

                users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Name = $"{firstName} {lastName}"
                });
            }

            return users;
        }

        public static List<TransactionCategory> GenerateTestCategories()
        {
            return new List<TransactionCategory>
            {
                new TransactionCategory("Food", "Food and dining expenses") { Id = 1 },
                new TransactionCategory("Transport", "Transportation costs") { Id = 2 },
                new TransactionCategory("Entertainment", "Entertainment and leisure") { Id = 3 },
                new TransactionCategory("Utilities", "Utility bills and services") { Id = 4 },
                new TransactionCategory("Salary", "Monthly salary income") { Id = 5 },
                new TransactionCategory("Freelance", "Freelance project income") { Id = 6 }
            };
        }
    }
}