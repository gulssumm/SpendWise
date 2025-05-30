using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly FinancialDbContext _context;
        private readonly bool _disposeContext;

        // Constructor with dependency injection
        public TransactionRepository(FinancialDbContext context)
        {
            _context = context;
            _disposeContext = false; // Don't dispose injected context
            _context.Database.EnsureCreated();
        }

        // Parameterless constructor - creates its own context
        public TransactionRepository()
        {
            _context = new FinancialDbContext();
            _disposeContext = true; // Dispose self-created context
            _context.Database.EnsureCreated();
        }

        public void AddTransaction(FinancialTransaction transaction)
        {
            var concreteTransaction = new ConcreteFinancialTransaction(
                transaction.Description,
                transaction.Amount,
                transaction.IsExpense,
                transaction.Category,
                transaction.Date);

            _context.Transactions.Add(concreteTransaction);
            _context.SaveChanges();
        }

        public List<FinancialTransaction> GetTransactions()
        {
            // LINQ Method Syntax - required by Task 2
            return _context.Transactions
                .Where(t => t.Amount > 0)
                .OrderByDescending(t => t.Date)
                .Cast<FinancialTransaction>()
                .ToList();
        }

        public void AddEvent(Event e)
        {
            var concreteEvent = new ConcreteUserEvent(e.UserId, e.Description)
            {
                Timestamp = e.Timestamp
            };

            _context.Events.Add(concreteEvent);
            _context.SaveChanges();
        }

        public List<Event> GetEvents()
        {
            // LINQ Query Syntax - required by Task 2
            var query = from e in _context.Events
                        where e.Timestamp >= DateTime.Today.AddDays(-30)
                        orderby e.Timestamp descending
                        select e;

            return query.Cast<Event>().ToList();
        }

        public void SaveTransactions(List<FinancialTransaction> transactions)
        {
            var existingTransactions = _context.Transactions.ToList();
            _context.Transactions.RemoveRange(existingTransactions);

            foreach (var transaction in transactions)
            {
                var concreteTransaction = new ConcreteFinancialTransaction(
                    transaction.Description,
                    transaction.Amount,
                    transaction.IsExpense,
                    transaction.Category,
                    transaction.Date);

                _context.Transactions.Add(concreteTransaction);
            }

            _context.SaveChanges();
        }

        public List<FinancialTransaction> LoadTransactions()
        {
            return _context.Transactions
                .AsNoTracking()
                .Cast<FinancialTransaction>()
                .ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _disposeContext)
            {
                _context?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}