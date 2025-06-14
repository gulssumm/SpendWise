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

        #region Virtual Methods for Testing

        // All methods virtual for inheritance-based testing
        public virtual List<IFinancialTransaction> GetTransactions()
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

        public virtual List<IFinancialTransaction> GetTransactionsByUser(Guid userId)
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

        public virtual List<IFinancialTransaction> GetTransactionsByCategory(string category)
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

        public virtual List<IFinancialTransaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
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

        public virtual void AddTransaction(IFinancialTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            using (var context = new FinancialDataContext(_connectionString))
            {
                var concreteTransaction = transaction as FinancialTransaction;
                if (concreteTransaction == null)
                {
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
                transaction.Id = concreteTransaction.Id;
            }
        }

        public virtual void UpdateTransaction(IFinancialTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

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

        public virtual void DeleteTransaction(int id)
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

        public virtual List<IUser> GetUsers()
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var users = from u in context.Users orderby u.Name select u;
                return users.Cast<IUser>().ToList();
            }
        }

        public virtual List<ITransactionCategory> GetCategories()
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                var categories = from c in context.TransactionCategories
                                 orderby c.Name
                                 select c;
                return categories.Cast<ITransactionCategory>().ToList();
            }
        }

        #endregion

        #region Async Methods (Fixed)

        public virtual async Task<List<IFinancialTransaction>> GetTransactionsAsync()
        {
            return await Task.FromResult(GetTransactions());
        }

        public virtual async Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId)
        {
            return await Task.FromResult(GetTransactionsByUser(userId));
        }

        public virtual async Task<List<IFinancialTransaction>> GetTransactionsByCategoryAsync(string category)
        {
            return await Task.FromResult(GetTransactionsByCategory(category));
        }

        public virtual async Task<List<IFinancialTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(GetTransactionsByDateRange(startDate, endDate));
        }

        public virtual async Task AddTransactionAsync(IFinancialTransaction transaction)
        {
            AddTransaction(transaction);
            await Task.CompletedTask;
        }

        public virtual async Task UpdateTransactionAsync(IFinancialTransaction transaction)
        {
            UpdateTransaction(transaction);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteTransactionAsync(int id)
        {
            DeleteTransaction(id);
            await Task.CompletedTask;
        }

        public virtual async Task<List<IUser>> GetUsersAsync()
        {
            return await Task.FromResult(GetUsers());
        }

        public virtual async Task<List<ITransactionCategory>> GetCategoriesAsync()
        {
            return await Task.FromResult(GetCategories());
        }

        #endregion

        #region User Operations

        public virtual IUser GetUser(Guid id)
        {
            using (var context = new FinancialDataContext(_connectionString))
            {
                return context.Users.FirstOrDefault(u => u.Id == id);
            }
        }

        public virtual void AddUser(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            using (var context = new FinancialDataContext(_connectionString))
            {
                var concreteUser = user as User ?? new User { Id = user.Id, Name = user.Name };
                context.Users.InsertOnSubmit(concreteUser);
                context.SubmitChanges();
            }
        }

        public virtual void UpdateUser(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

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

        public virtual void DeleteUser(Guid id)
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

        public virtual async Task<IUser> GetUserAsync(Guid id)
        {
            return await Task.FromResult(GetUser(id));
        }

        public virtual async Task AddUserAsync(IUser user)
        {
            AddUser(user);
            await Task.CompletedTask;
        }

        public virtual async Task UpdateUserAsync(IUser user)
        {
            UpdateUser(user);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteUserAsync(Guid id)
        {
            DeleteUser(id);
            await Task.CompletedTask;
        }

        #endregion

        #region Event Operations

        public virtual List<IEvent> GetEvents()
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

        public virtual List<IEvent> GetEventsByUser(Guid userId)
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

        public virtual void AddEvent(IEvent e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

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

        public virtual async Task<List<IEvent>> GetEventsAsync()
        {
            return await Task.FromResult(GetEvents());
        }

        public virtual async Task<List<IEvent>> GetEventsByUserAsync(Guid userId)
        {
            return await Task.FromResult(GetEventsByUser(userId));
        }

        public virtual async Task AddEventAsync(IEvent e)
        {
            AddEvent(e);
            await Task.CompletedTask;
        }

        #endregion

        #region Category Operations

        public virtual void AddCategory(ITransactionCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            using (var context = new FinancialDataContext(_connectionString))
            {
                var concreteCategory = category as TransactionCategory ??
                    new TransactionCategory(category.Name, category.Description);
                context.TransactionCategories.InsertOnSubmit(concreteCategory);
                context.SubmitChanges();
                category.Id = concreteCategory.Id;
            }
        }

        public virtual void UpdateCategory(ITransactionCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

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

        public virtual void DeleteCategory(int id)
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

        public virtual async Task AddCategoryAsync(ITransactionCategory category)
        {
            AddCategory(category);
            await Task.CompletedTask;
        }

        public virtual async Task UpdateCategoryAsync(ITransactionCategory category)
        {
            UpdateCategory(category);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteCategoryAsync(int id)
        {
            DeleteCategory(id);
            await Task.CompletedTask;
        }

        #endregion
    }
}