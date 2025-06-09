using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using Data;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace LogicTest
{
    [TestClass]
    public class TransactionServiceTests
    {
        private ITransactionService _service;
        private LogicTestMockRepository _mockRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new LogicTestMockRepository();
            _service = new TransactionService(_mockRepository);
        }

        [TestMethod]
        public async Task GetTransactionsAsync_ReturnsAllTransactions()
        {
            // Act
            var transactions = await _service.GetTransactionsAsync();

            // Assert
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions.Count > 0);
        }

        [TestMethod]
        public async Task AddTransactionAsync_ValidTransaction_AddsSuccessfully()
        {
            // Arrange
            var transaction = new FinancialTransaction("Test transaction", 100m, true, "Test", DateTime.Today);

            // Act
            await _service.AddTransactionAsync(transaction);
            var transactions = await _service.GetTransactionsAsync();

            // Assert
            Assert.IsTrue(transactions.Any(t => t.Description == "Test transaction"));
        }

        [TestMethod]
        public async Task CalculateBalanceAsync_ReturnsCorrectBalance()
        {
            // Act
            var balance = await _service.CalculateBalanceAsync();

            // Assert
            Assert.IsTrue(balance != 0); // Should have some balance from test data
        }

        [TestMethod]
        public async Task GetExpensesByCategoryAsync_ReturnsGroupedExpenses()
        {
            // Act
            var expenses = await _service.GetExpensesByCategoryAsync();

            // Assert
            Assert.IsNotNull(expenses);
            Assert.IsTrue(expenses.Count > 0);
        }

        [TestMethod]
        public async Task LogicLayer_TestDataGenerationMethod1_PreDefinedData()
        {
            // Generation Method 1: Pre-defined test data in constructor
            var service = new TransactionService(new LogicTestMockRepository());
            var transactions = await service.GetTransactionsAsync();

            // Assert
            Assert.IsTrue(transactions.Count >= 2, "Should have pre-defined test data");
            Assert.IsTrue(transactions.Any(t => t.Description == "Grocery"), "Should contain predefined grocery transaction");
        }

        [TestMethod]
        public async Task LogicLayer_TestDataGenerationMethod2_DynamicData()
        {
            // Generation Method 2: Dynamic test data generation
            var dynamicRepo = new LogicTestMockRepository();

            // Add dynamically generated transactions
            for (int i = 0; i < 3; i++)
            {
                var transaction = new FinancialTransaction($"Dynamic Transaction {i}", (i + 1) * 50m, i % 2 == 0, "Test", DateTime.Today.AddDays(-i));
                dynamicRepo.AddTransaction(transaction);
            }

            var service = new TransactionService(dynamicRepo);
            var transactions = await service.GetTransactionsAsync();

            // Assert
            Assert.IsTrue(transactions.Count >= 5, "Should have both predefined and dynamic data"); // 2 predefined + 3 dynamic
            Assert.IsTrue(transactions.Any(t => t.Description.Contains("Dynamic")), "Should contain dynamically generated transactions");
        }

        [TestMethod]
        public async Task GetTransactionsByUserAsync_ReturnsUserTransactions()
        {
            // Arrange
            var transactions = await _service.GetTransactionsAsync();
            var firstTransaction = transactions.First();
            var userId = firstTransaction.UserId ?? Guid.Empty; // Handle nullable Guid

            // Act
            var userTransactions = await _service.GetTransactionsByUserAsync(userId);

            // Assert
            Assert.IsNotNull(userTransactions);
            Assert.IsTrue(userTransactions.All(t => t.UserId == userId));
        }

        [TestMethod]
        public async Task GetTransactionsByCategory_ReturnsFilteredTransactions()
        {
            // Act
            var foodTransactions = await _service.GetTransactionsByCategory("Food");

            // Assert
            Assert.IsNotNull(foodTransactions);
            Assert.IsTrue(foodTransactions.All(t => t.Category == "Food"));
        }

        [TestMethod]
        public async Task DeleteTransactionAsync_RemovesTransaction()
        {
            // Arrange
            var transactions = await _service.GetTransactionsAsync();
            var initialCount = transactions.Count;
            var transactionToDelete = transactions.First();

            // Act
            await _service.DeleteTransactionAsync(transactionToDelete.Id);
            var remainingTransactions = await _service.GetTransactionsAsync();

            // Assert
            Assert.AreEqual(initialCount - 1, remainingTransactions.Count);
            Assert.IsFalse(remainingTransactions.Any(t => t.Id == transactionToDelete.Id));
        }

        [TestMethod]
        public async Task CalculateBalanceByUserAsync_ReturnsCorrectUserBalance()
        {
            // Arrange
            var transactions = await _service.GetTransactionsAsync();
            var userId = transactions.First().UserId ?? Guid.Empty; // Handle nullable Guid

            // Act
            var balance = await _service.CalculateBalanceByUserAsync(userId);

            // Assert
            Assert.IsTrue(balance != 0); // Should have some balance for the user
        }

        [TestMethod]
        public async Task GetRecentTransactionsAsync_ReturnsLimitedResults()
        {
            // Act
            var recentTransactions = await _service.GetRecentTransactionsAsync(1);

            // Assert
            Assert.IsNotNull(recentTransactions);
            Assert.AreEqual(1, recentTransactions.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddTransactionAsync_NullTransaction_ThrowsException()
        {
            // Act & Assert
            await _service.AddTransactionAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddTransactionAsync_InvalidTransaction_ThrowsException()
        {
            // Arrange
            var invalidTransaction = new FinancialTransaction("", -100m, true, "", DateTime.Today);

            // Act & Assert
            await _service.AddTransactionAsync(invalidTransaction);
        }
    }

    // Local mock repository for LogicTest - demonstrates independence from DataTest
    public class LogicTestMockRepository : ITransactionRepository
    {
        private readonly List<FinancialTransaction> _transactions = new List<FinancialTransaction>();
        private readonly List<User> _users = new List<User>();
        private readonly List<Event> _events = new List<Event>();
        private readonly List<TransactionCategory> _categories = new List<TransactionCategory>();
        private int _nextTransactionId = 1;

        public LogicTestMockRepository()
        {
            // Generation Method 1: Pre-defined test data
            var user1 = new User { Id = Guid.NewGuid(), Name = "Logic Test User" };
            _users.Add(user1);

            var category = new TransactionCategory("Food", "Food expenses") { Id = 1 };
            _categories.Add(category);

            _transactions.AddRange(new[]
            {
                new FinancialTransaction("Grocery", 50.00m, true, "Food", DateTime.Today)
                    { Id = _nextTransactionId++, UserId = user1.Id },
                new FinancialTransaction("Salary", 2000.00m, false, "Income", DateTime.Today)
                    { Id = _nextTransactionId++, UserId = user1.Id }
            });

            // Add a test event
            _events.Add(new Event
            {
                Id = 1,
                UserId = user1.Id,
                Description = "Test Event",
                Timestamp = DateTime.Today
            });
        }

        // Transaction methods returning interface types
        public List<IFinancialTransaction> GetTransactions()
        {
            return _transactions.Where(t => t.Amount > 0).Cast<IFinancialTransaction>().ToList();
        }

        public List<IFinancialTransaction> GetTransactionsByCategory(string category)
        {
            return _transactions.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).Cast<IFinancialTransaction>().ToList();
        }

        public List<IFinancialTransaction> GetTransactionsByUser(Guid userId)
        {
            return _transactions.Where(t => t.UserId == userId).Cast<IFinancialTransaction>().ToList();
        }

        public List<IFinancialTransaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _transactions.Where(t => t.Date >= startDate && t.Date <= endDate).Cast<IFinancialTransaction>().ToList();
        }

        public void AddTransaction(IFinancialTransaction transaction)
        {
            var concreteTransaction = transaction as FinancialTransaction ??
                new FinancialTransaction(transaction.Description, transaction.Amount, transaction.IsExpense, transaction.Category, transaction.Date)
                {
                    Id = _nextTransactionId++,
                    UserId = transaction.UserId ?? Guid.Empty // Handle nullable Guid
                };

            if (concreteTransaction.Id == 0)
                concreteTransaction.Id = _nextTransactionId++;

            _transactions.Add(concreteTransaction);
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
                existing.UserId = transaction.UserId ?? Guid.Empty; // Handle nullable Guid
            }
        }

        public void DeleteTransaction(int id)
        {
            _transactions.RemoveAll(t => t.Id == id);
        }

        // Async transaction methods
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

        // User methods returning interface types
        public List<IUser> GetUsers()
        {
            return _users.Cast<IUser>().ToList();
        }

        public IUser GetUser(Guid id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public void AddUser(IUser user)
        {
            var concreteUser = user as User ?? new User { Id = user.Id, Name = user.Name };
            _users.Add(concreteUser);
        }

        public void UpdateUser(IUser user)
        {
            var existing = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null) existing.Name = user.Name;
        }

        public void DeleteUser(Guid id)
        {
            _users.RemoveAll(u => u.Id == id);
        }

        // Async user methods
        public async Task<List<IUser>> GetUsersAsync()
        {
            return await Task.FromResult(GetUsers());
        }

        public async Task<IUser> GetUserAsync(Guid id)
        {
            return await Task.FromResult(GetUser(id));
        }

        public async Task AddUserAsync(IUser user)
        {
            await Task.Run(() => AddUser(user));
        }

        public async Task UpdateUserAsync(IUser user)
        {
            await Task.Run(() => UpdateUser(user));
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await Task.Run(() => DeleteUser(id));
        }

        // Category methods returning interface types
        public List<ITransactionCategory> GetCategories()
        {
            return _categories.Cast<ITransactionCategory>().ToList();
        }

        public void AddCategory(ITransactionCategory category)
        {
            var concreteCategory = category as TransactionCategory ??
                new TransactionCategory(category.Name, category.Description) { Id = category.Id };
            _categories.Add(concreteCategory);
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

        public void DeleteCategory(int id)
        {
            _categories.RemoveAll(c => c.Id == id);
        }

        // Async category methods
        public async Task<List<ITransactionCategory>> GetCategoriesAsync()
        {
            return await Task.FromResult(GetCategories());
        }

        public async Task AddCategoryAsync(ITransactionCategory category)
        {
            await Task.Run(() => AddCategory(category));
        }

        public async Task UpdateCategoryAsync(ITransactionCategory category)
        {
            await Task.Run(() => UpdateCategory(category));
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await Task.Run(() => DeleteCategory(id));
        }

        // Event methods returning interface types
        public List<IEvent> GetEvents()
        {
            return _events.Cast<IEvent>().ToList();
        }

        public List<IEvent> GetEventsByUser(Guid userId)
        {
            return _events.Where(e => e.UserId == userId).Cast<IEvent>().ToList();
        }

        public void AddEvent(IEvent e)
        {
            var concreteEvent = e as Event ?? new Event
            {
                Id = e.Id,
                Description = e.Description,
                Timestamp = e.Timestamp,
                UserId = e.UserId
            };
            _events.Add(concreteEvent);
        }

        // Async event methods
        public async Task<List<IEvent>> GetEventsAsync()
        {
            return await Task.FromResult(GetEvents());
        }

        public async Task<List<IEvent>> GetEventsByUserAsync(Guid userId)
        {
            return await Task.FromResult(GetEventsByUser(userId));
        }

        public async Task AddEventAsync(IEvent e)
        {
            await Task.Run(() => AddEvent(e));
        }

        // Legacy methods for backward compatibility
        public List<FinancialTransaction> LoadTransactions() => _transactions.Where(t => t.Amount > 0).ToList();
        public void SaveTransactions(List<FinancialTransaction> transactions)
        {
            _transactions.Clear();
            _transactions.AddRange(transactions);
        }
    }
}