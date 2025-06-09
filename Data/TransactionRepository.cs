using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly string _connectionString;

        public TransactionRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        #region Synchronous Transaction Operations

        public List<IFinancialTransaction> GetTransactions()
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var transactions = from t in context.Transactions
                                   where t.Amount > 0
                                   orderby t.Date descending
                                   select t;
                return transactions.Cast<IFinancialTransaction>().ToList();
            }
        }

        public List<IFinancialTransaction> GetTransactionsByUser(Guid userId)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                return context.Transactions
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.Date)
                    .Cast<IFinancialTransaction>()
                    .ToList();
            }
        }

        public List<IFinancialTransaction> GetTransactionsByCategory(string category)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var transactions = from t in context.Transactions
                                   where t.Category.Equals(category, StringComparison.OrdinalIgnoreCase)
                                   orderby t.Date descending
                                   select t;
                return transactions.Cast<IFinancialTransaction>().ToList();
            }
        }

        public List<IFinancialTransaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                return context.Transactions
                    .Where(t => t.Date >= startDate && t.Date <= endDate)
                    .OrderByDescending(t => t.Date)
                    .Cast<IFinancialTransaction>()
                    .ToList();
            }
        }

        public void AddTransaction(IFinancialTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            using (var context = new FinancialDataContext(_connectionString))
            {
                var concreteTransaction = transaction as FinancialTransaction;
                if (concreteTransaction == null)
                {
                    // Create a new concrete instance from the interface
                    concreteTransaction = new FinancialTransaction(
                        transaction.Description,
                        transaction.Amount,
                        transaction.IsExpense,
                        transaction.Category,
                        transaction.Date)
                    {
                        UserId = transaction.UserId
                    };
                }
                context.Transactions.InsertOnSubmit(concreteTransaction);
                context.SubmitChanges();
                transaction.Id = concreteTransaction.Id; // Update the interface with generated ID
            }
        }

        public void UpdateTransaction(IFinancialTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            using (var context = new FinancialDataContext(_connectionString))
            {
                var existing = context.Transactions.FirstOrDefault(t => t.Id == transaction.Id);
                if (existing != null)
                {
                    existing.Description = transaction.Description;
                    existing.Amount = transaction.Amount;
                    existing.IsExpense = transaction.IsExpense;
                    existing.Category = transaction.Category;
                    existing.Date = transaction.Date;
                    existing.UserId = transaction.UserId;
                    context.SubmitChanges();
                }
            }
        }

        public void DeleteTransaction(int id)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var transaction = context.Transactions.FirstOrDefault(t => t.Id == id);
                if (transaction != null)
                {
                    context.Transactions.DeleteOnSubmit(transaction);
                    context.SubmitChanges();
                }
            }
        }

        #endregion

        #region Asynchronous Transaction Operations

        public async Task<List<IFinancialTransaction>> GetTransactionsAsync()
        {
            return await Task.Run(() => GetTransactions());
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId)
        {
            return await Task.Run(() => GetTransactionsByUser(userId));
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByCategoryAsync(string category)
        {
            return await Task.Run(() => GetTransactionsByCategory(category));
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.Run(() => GetTransactionsByDateRange(startDate, endDate));
        }

        public async Task AddTransactionAsync(IFinancialTransaction transaction)
        {
            await Task.Run(() => AddTransaction(transaction));
        }

        public async Task UpdateTransactionAsync(IFinancialTransaction transaction)
        {
            await Task.Run(() => UpdateTransaction(transaction));
        }

        public async Task DeleteTransactionAsync(int id)
        {
            await Task.Run(() => DeleteTransaction(id));
        }

        #endregion

        #region User Operations

        public List<IUser> GetUsers()
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var users = from u in context.Users orderby u.Name select u;
                return users.Cast<IUser>().ToList();
            }
        }

        public IUser GetUser(Guid id)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                return context.Users.FirstOrDefault(u => u.Id == id);
            }
        }

        public void AddUser(IUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            using (var context = new FinancialDataContext(_connectionString))
            {
                var concreteUser = user as User ?? new User { Id = user.Id, Name = user.Name };
                context.Users.InsertOnSubmit(concreteUser);
                context.SubmitChanges();
            }
        }

        public void UpdateUser(IUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            using (var context = new FinancialDataContext(_connectionString))
            {
                var existing = context.Users.FirstOrDefault(u => u.Id == user.Id);
                if (existing != null)
                {
                    existing.Name = user.Name;
                    context.SubmitChanges();
                }
            }
        }

        public void DeleteUser(Guid id)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var user = context.Users.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    context.Users.DeleteOnSubmit(user);
                    context.SubmitChanges();
                }
            }
        }

        // Async versions for WPF
        public async Task<List<IUser>> GetUsersAsync() => await Task.Run(() => GetUsers());
        public async Task<IUser> GetUserAsync(Guid id) => await Task.Run(() => GetUser(id));
        public async Task AddUserAsync(IUser user) => await Task.Run(() => AddUser(user));
        public async Task UpdateUserAsync(IUser user) => await Task.Run(() => UpdateUser(user));
        public async Task DeleteUserAsync(Guid id) => await Task.Run(() => DeleteUser(id));

        #endregion

        #region Event Operations

        public List<IEvent> GetEvents()
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var events = from e in context.Events
                             where e.Timestamp >= DateTime.Today.AddDays(-30)
                             orderby e.Timestamp descending
                             select e;
                return events.Cast<IEvent>().ToList();
            }
        }

        public List<IEvent> GetEventsByUser(Guid userId)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                return context.Events
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.Timestamp)
                    .Cast<IEvent>()
                    .ToList();
            }
        }

        public void AddEvent(IEvent e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            using (var context = new FinancialDataContext(_connectionString))
            {
                var concreteEvent = e as Event ?? new Event
                {
                    UserId = e.UserId,
                    Description = e.Description,
                    Timestamp = e.Timestamp
                };
                context.Events.InsertOnSubmit(concreteEvent);
                context.SubmitChanges();
                e.Id = concreteEvent.Id;
            }
        }

        // Async versions for WPF
        public async Task<List<IEvent>> GetEventsAsync() => await Task.Run(() => GetEvents());
        public async Task<List<IEvent>> GetEventsByUserAsync(Guid userId) => await Task.Run(() => GetEventsByUser(userId));
        public async Task AddEventAsync(IEvent e) => await Task.Run(() => AddEvent(e));

        #endregion

        #region Category Operations

        public List<ITransactionCategory> GetCategories()
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var categories = from c in context.TransactionCategories
                                 orderby c.Name
                                 select c;
                return categories.Cast<ITransactionCategory>().ToList();
            }
        }

        public void AddCategory(ITransactionCategory category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            using (var context = new FinancialDataContext(_connectionString))
            {
                var concreteCategory = category as TransactionCategory ??
                    new TransactionCategory(category.Name, category.Description);
                context.TransactionCategories.InsertOnSubmit(concreteCategory);
                context.SubmitChanges();
                category.Id = concreteCategory.Id;
            }
        }

        public void UpdateCategory(ITransactionCategory category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            using (var context = new FinancialDataContext(_connectionString))
            {
                var existing = context.TransactionCategories.FirstOrDefault(c => c.Id == category.Id);
                if (existing != null)
                {
                    existing.Name = category.Name;
                    existing.Description = category.Description;
                    context.SubmitChanges();
                }
            }
        }

        public void DeleteCategory(int id)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var category = context.TransactionCategories.FirstOrDefault(c => c.Id == id);
                if (category != null)
                {
                    context.TransactionCategories.DeleteOnSubmit(category);
                    context.SubmitChanges();
                }
            }
        }

        // Async versions for WPF
        public async Task<List<ITransactionCategory>> GetCategoriesAsync() => await Task.Run(() => GetCategories());
        public async Task AddCategoryAsync(ITransactionCategory category) => await Task.Run(() => AddCategory(category));
        public async Task UpdateCategoryAsync(ITransactionCategory category) => await Task.Run(() => UpdateCategory(category));
        public async Task DeleteCategoryAsync(int id) => await Task.Run(() => DeleteCategory(id));

        #endregion

        #region Additional LINQ Examples (Method and Query Syntax)

        // Query syntax example
        public List<IFinancialTransaction> GetTopExpensesByCategory(string category, int top)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var query = from t in context.Transactions
                            where t.IsExpense && t.Category == category
                            orderby t.Amount descending
                            select t;

                return query.Take(top).Cast<IFinancialTransaction>().ToList();
            }
        }

        // Method syntax example
        public decimal GetAverageTransactionAmount(bool isExpense)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                return context.Transactions
                    .Where(t => t.IsExpense == isExpense)
                    .Select(t => t.Amount)
                    .DefaultIfEmpty(0)
                    .Average();
            }
        }

        // Combined query and method syntax
        public Dictionary<string, decimal> GetMonthlyTotals(int year)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var query = from t in context.Transactions
                            where t.Date.Year == year
                            group t by t.Date.Month into g
                            select new
                            {
                                Month = g.Key,
                                Total = g.Sum(x => x.IsExpense ? -x.Amount : x.Amount)
                            };

                return query.ToDictionary(x => $"{year}-{x.Month:00}", x => x.Total);
            }
        }

        #endregion
    }
}