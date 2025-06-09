using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataTest
{
    public class MockTransactionRepository : ITransactionRepository
    {
        private readonly List<FinancialTransaction> _transactions = new List<FinancialTransaction>();
        private readonly List<User> _users = new List<User>();
        private readonly List<Event> _events = new List<Event>();
        private readonly List<TransactionCategory> _categories = new List<TransactionCategory>();
        private int _nextTransactionId = 1;
        private int _nextEventId = 1;
        private int _nextCategoryId = 1;

        public MockTransactionRepository()
        {
            GenerateTestData();
        }

        private void GenerateTestData()
        {
            var user1 = new User { Id = Guid.NewGuid(), Name = "John Doe" };
            var user2 = new User { Id = Guid.NewGuid(), Name = "Jane Smith" };
            _users.AddRange(new[] { user1, user2 });

            _categories.AddRange(new[]
            {
                new TransactionCategory("Food", "Food expenses") { Id = _nextCategoryId++ },
                new TransactionCategory("Transport", "Transport costs") { Id = _nextCategoryId++ }
            });

            _transactions.AddRange(new[]
            {
                new FinancialTransaction("Grocery shopping", 50.00m, true, "Food", DateTime.Today.AddDays(-1))
                    { Id = _nextTransactionId++, UserId = user1.Id },
                new FinancialTransaction("Salary payment", 3000.00m, false, "Income", DateTime.Today.AddDays(-7))
                    { Id = _nextTransactionId++, UserId = user1.Id },
                new FinancialTransaction("Bus ticket", 2.50m, true, "Transport", DateTime.Today)
                    { Id = _nextTransactionId++, UserId = user2.Id }
            });
        }

        #region Synchronous Transaction Operations
        public List<IFinancialTransaction> GetTransactions()
        {
            return _transactions.Where(t => t.Amount > 0)
                .OrderByDescending(t => t.Date)
                .Cast<IFinancialTransaction>()
                .ToList();
        }

        public List<IFinancialTransaction> GetTransactionsByUser(Guid userId)
        {
            return _transactions.Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Cast<IFinancialTransaction>()
                .ToList();
        }

        public List<IFinancialTransaction> GetTransactionsByCategory(string category)
        {
            return _transactions.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.Date)
                .Cast<IFinancialTransaction>()
                .ToList();
        }

        public List<IFinancialTransaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _transactions.Where(t => t.Date >= startDate && t.Date <= endDate)
                .OrderByDescending(t => t.Date)
                .Cast<IFinancialTransaction>()
                .ToList();
        }

        public void AddTransaction(IFinancialTransaction transaction)
        {
            var concrete = transaction as FinancialTransaction ??
                new FinancialTransaction(
                    transaction.Description,
                    transaction.Amount,
                    transaction.IsExpense,
                    transaction.Category,
                    transaction.Date)
                {
                    UserId = transaction.UserId
                };
            concrete.Id = _nextTransactionId++;
            _transactions.Add(concrete);
            transaction.Id = concrete.Id;
        }

        public void UpdateTransaction(IFinancialTransaction transaction)
        {
            var existing = _transactions.FirstOrDefault(t => t.Id == transaction.Id);
            if (existing != null)
            {
                existing.Description = transaction.Description;
                existing.Amount = transaction.Amount;
                existing.IsExpense = transaction.IsExpense;
                existing.Category = transaction.Category;
                existing.Date = transaction.Date;
                existing.UserId = transaction.UserId;
            }
        }

        public void DeleteTransaction(int id)
        {
            _transactions.RemoveAll(t => t.Id == id);
        }
        #endregion

        #region Asynchronous Transaction Operations
        public async Task<List<IFinancialTransaction>> GetTransactionsAsync()
        {
            return await Task.FromResult(GetTransactions());
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId)
        {
            return await Task.FromResult(GetTransactionsByUser(userId));
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByCategoryAsync(string category)
        {
            return await Task.FromResult(GetTransactionsByCategory(category));
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(GetTransactionsByDateRange(startDate, endDate));
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
        public List<IUser> GetUsers() => _users.OrderBy(u => u.Name).Cast<IUser>().ToList();

        public IUser GetUser(Guid id) => _users.FirstOrDefault(u => u.Id == id);

        public void AddUser(IUser user)
        {
            var concrete = user as User ?? new User { Id = user.Id, Name = user.Name };
            _users.Add(concrete);
        }

        public void UpdateUser(IUser user)
        {
            var existing = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null) existing.Name = user.Name;
        }

        public void DeleteUser(Guid id) => _users.RemoveAll(u => u.Id == id);

        public async Task<List<IUser>> GetUsersAsync() => await Task.FromResult(GetUsers());
        public async Task<IUser> GetUserAsync(Guid id) => await Task.FromResult(GetUser(id));
        public async Task AddUserAsync(IUser user) => await Task.Run(() => AddUser(user));
        public async Task UpdateUserAsync(IUser user) => await Task.Run(() => UpdateUser(user));
        public async Task DeleteUserAsync(Guid id) => await Task.Run(() => DeleteUser(id));
        #endregion

        #region Event Operations
        public List<IEvent> GetEvents() => _events.Cast<IEvent>().ToList();

        public List<IEvent> GetEventsByUser(Guid userId) =>
            _events.Where(e => e.UserId == userId).Cast<IEvent>().ToList();

        public void AddEvent(IEvent e)
        {
            var concrete = e as Event ?? new Event
            {
                UserId = e.UserId,
                Description = e.Description,
                Timestamp = e.Timestamp
            };
            concrete.Id = _nextEventId++;
            _events.Add(concrete);
            e.Id = concrete.Id;
        }

        public async Task<List<IEvent>> GetEventsAsync() => await Task.FromResult(GetEvents());
        public async Task<List<IEvent>> GetEventsByUserAsync(Guid userId) =>
            await Task.FromResult(GetEventsByUser(userId));
        public async Task AddEventAsync(IEvent e) => await Task.Run(() => AddEvent(e));
        #endregion

        #region Category Operations
        public List<ITransactionCategory> GetCategories() =>
            _categories.OrderBy(c => c.Name).Cast<ITransactionCategory>().ToList();

        public void AddCategory(ITransactionCategory category)
        {
            var concrete = category as TransactionCategory ??
                new TransactionCategory(category.Name, category.Description);
            concrete.Id = _nextCategoryId++;
            _categories.Add(concrete);
            category.Id = concrete.Id;
        }

        public void UpdateCategory(ITransactionCategory category)
        {
            var existing = _categories.FirstOrDefault(c => c.Id == category.Id);
            if (existing != null)
            {
                existing.Name = category.Name;
                existing.Description = category.Description;
            }
        }

        public void DeleteCategory(int id) => _categories.RemoveAll(c => c.Id == id);

        public async Task<List<ITransactionCategory>> GetCategoriesAsync() =>
            await Task.FromResult(GetCategories());
        public async Task AddCategoryAsync(ITransactionCategory category) =>
            await Task.Run(() => AddCategory(category));
        public async Task UpdateCategoryAsync(ITransactionCategory category) =>
            await Task.Run(() => UpdateCategory(category));
        public async Task DeleteCategoryAsync(int id) =>
            await Task.Run(() => DeleteCategory(id));
        #endregion
    }
}