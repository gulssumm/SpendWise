using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class DataFactory : IDataFactory
    {
        private readonly Random _random = new Random();
        private readonly string[] _categories = { "Food", "Transport", "Entertainment", "Bills", "Shopping", "Healthcare" };
        private readonly string[] _firstNames = { "John", "Jane", "Bob", "Alice", "Charlie", "Diana" };
        private readonly string[] _lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia" };

        public IFinancialTransaction CreateTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
        {
            return new FinancialTransaction(description, amount, isExpense, category, date);
        }

        public IUser CreateUser(Guid id, string name)
        {
            return new User { Id = id, Name = name };
        }

        public IEvent CreateEvent(Guid userId, string description)
        {
            return new Event
            {
                UserId = userId,
                Description = description,
                Timestamp = DateTime.Now
            };
        }

        public ITransactionCategory CreateCategory(string name, string description)
        {
            return new TransactionCategory(name, description);
        }

        // Random data generation
        public List<IFinancialTransaction> GenerateSampleTransactions(int count)
        {
            var transactions = new List<IFinancialTransaction>();
            var startDate = DateTime.Now.AddDays(-365);

            for (int i = 0; i < count; i++)
            {
                var isExpense = _random.Next(2) == 0;
                var amount = (decimal)(_random.Next(10, 1000) + _random.NextDouble());
                var category = _categories[_random.Next(_categories.Length)]; // Fixed: removed asterisks
                var date = startDate.AddDays(_random.Next(365));
                var description = $"{(isExpense ? "Payment" : "Income")} - {category} #{i + 1}";

                transactions.Add(CreateTransaction(description, amount, isExpense, category, date));
            }

            return transactions;
        }

        public List<IUser> GenerateSampleUsers(int count)
        {
            return Enumerable.Range(0, count)
                .Select(i => CreateUser(
                    Guid.NewGuid(),
                    $"{_firstNames[_random.Next(_firstNames.Length)]} {_lastNames[_random.Next(_lastNames.Length)]}"
                ))
                .ToList();
        }
    }
}