using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly FinancialDbContext _context;
        private readonly bool _disposeContext;

        public TransactionRepository(FinancialDbContext context)
        {
            _context = context;
            _disposeContext = false;
            _context.Database.EnsureCreated();
        }

        public TransactionRepository()
        {
            _context = new FinancialDbContext();
            _disposeContext = true;
            _context.Database.EnsureCreated();
        }

        // Async CRUD Operations
        public async Task AddTransactionAsync(FinancialTransaction transaction)
        {
            var concreteTransaction = new ConcreteFinancialTransaction(
                transaction.Description,
                transaction.Amount,
                transaction.IsExpense,
                transaction.Category,
                transaction.Date);

            await _context.Transactions.AddAsync(concreteTransaction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FinancialTransaction>> GetTransactionsAsync()
        {
            return await _context.Transactions
                .Where(t => t.Amount > 0)
                .OrderByDescending(t => t.Date)
                .Cast<FinancialTransaction>()
                .ToListAsync();
        }

        public async Task<FinancialTransaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions
                .Where(t => t.Id == id)
                .Cast<FinancialTransaction>()
                .FirstOrDefaultAsync();
        }

        public async Task UpdateTransactionAsync(FinancialTransaction transaction)
        {
            var existing = await _context.Transactions.FindAsync(transaction.Id);
            if (existing != null)
            {
                existing.Description = transaction.Description;
                existing.Amount = transaction.Amount;
                existing.IsExpense = transaction.IsExpense;
                existing.Category = transaction.Category;
                existing.Date = transaction.Date;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddEventAsync(Event e)
        {
            var concreteEvent = new ConcreteUserEvent(e.UserId, e.Description)
            {
                Timestamp = e.Timestamp
            };

            await _context.Events.AddAsync(concreteEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Event>> GetEventsAsync()
        {
            var query = from e in _context.Events
                        where e.Timestamp >= DateTime.Today.AddDays(-30)
                        orderby e.Timestamp descending
                        select e;

            return await query.Cast<Event>().ToListAsync();
        }

        public async Task SaveTransactionsAsync(List<FinancialTransaction> transactions)
        {
            var existingTransactions = await _context.Transactions.ToListAsync();
            _context.Transactions.RemoveRange(existingTransactions);

            foreach (var transaction in transactions)
            {
                var concreteTransaction = new ConcreteFinancialTransaction(
                    transaction.Description,
                    transaction.Amount,
                    transaction.IsExpense,
                    transaction.Category,
                    transaction.Date);

                await _context.Transactions.AddAsync(concreteTransaction);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<FinancialTransaction>> LoadTransactionsAsync()
        {
            return await _context.Transactions
                .AsNoTracking()
                .Cast<FinancialTransaction>()
                .ToListAsync();
        }

        // Synchronous versions for compatibility
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