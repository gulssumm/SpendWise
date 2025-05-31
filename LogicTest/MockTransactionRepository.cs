using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicTest
{
    // Mock implementation for testing Logic layer without Data layer dependency
    public class MockTransactionRepository : ITransactionRepository
    {
        private readonly List<FinancialTransaction> _transactions = new();
        private readonly List<Event> _events = new();

        // Async implementations
        public async Task AddTransactionAsync(FinancialTransaction transaction)
        {
            await Task.Run(() => AddTransaction(transaction));
        }

        public async Task<List<FinancialTransaction>> GetTransactionsAsync()
        {
            return await Task.FromResult(GetTransactions());
        }

        public async Task<FinancialTransaction?> GetTransactionByIdAsync(int id)
        {
            return await Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id));
        }

        public async Task UpdateTransactionAsync(FinancialTransaction transaction)
        {
            await Task.Run(() =>
            {
                var existing = _transactions.FirstOrDefault(t => t.Id == transaction.Id);
                if (existing != null)
                {
                    var index = _transactions.IndexOf(existing);
                    _transactions[index] = transaction;
                }
            });
        }

        public async Task DeleteTransactionAsync(int id)
        {
            await Task.Run(() =>
            {
                var transaction = _transactions.FirstOrDefault(t => t.Id == id);
                if (transaction != null)
                {
                    _transactions.Remove(transaction);
                }
            });
        }

        public async Task AddEventAsync(Event e)
        {
            await Task.Run(() => AddEvent(e));
        }

        public async Task<List<Event>> GetEventsAsync()
        {
            return await Task.FromResult(GetEvents());
        }

        public async Task SaveTransactionsAsync(List<FinancialTransaction> transactions)
        {
            await Task.Run(() => SaveTransactions(transactions));
        }

        public async Task<List<FinancialTransaction>> LoadTransactionsAsync()
        {
            return await Task.FromResult(LoadTransactions());
        }

        // Synchronous implementations
        public void AddTransaction(FinancialTransaction transaction)
        {
            var testTransaction = new TestFinancialTransaction(
                transaction.Description,
                transaction.Amount,
                transaction.IsExpense,
                transaction.Category,
                transaction.Date)
            {
                Id = _transactions.Count + 1
            };

            _transactions.Add(testTransaction);
        }

        public List<FinancialTransaction> GetTransactions()
        {
            return _transactions.ToList();
        }

        public void AddEvent(Event e)
        {
            var testEvent = new TestUserEvent(e.UserId, e.Description)
            {
                Id = _events.Count + 1,
                Timestamp = e.Timestamp
            };

            _events.Add(testEvent);
        }

        public List<Event> GetEvents()
        {
            return _events.ToList();
        }

        public void SaveTransactions(List<FinancialTransaction> transactions)
        {
            _transactions.Clear();
            foreach (var transaction in transactions)
            {
                AddTransaction(transaction);
            }
        }

        public List<FinancialTransaction> LoadTransactions()
        {
            return _transactions.ToList();
        }

        // Test implementations - only for LogicTest
        private class TestFinancialTransaction : FinancialTransaction
        {
            public TestFinancialTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
                : base(description, amount, isExpense, category, date)
            {
            }
        }

        private class TestUserEvent : UserEvent
        {
            public TestUserEvent(Guid userId, string description)
                : base(userId, description)
            {
            }
        }
    }
}