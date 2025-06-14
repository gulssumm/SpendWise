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
        public async Task LogicLayer_TestDataGenerationMethod1_PreDefinedData()
        {
            // Generation Method 1: Predefined mock data
            var service = new TransactionService(new LogicTestMockRepository());
            var transactions = await service.GetTransactionsAsync();

            // Assert
            Assert.IsTrue(transactions.Count >= 2, "Should have pre-defined test data");
            Assert.IsTrue(transactions.Any(t => t.Description == "Grocery"), "Should contain predefined grocery transaction");
        }

        [TestMethod]
        public async Task LogicLayer_TestDataGenerationMethod2_DynamicData()
        {
            // Generation Method 2: Dynamic data generation
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
            Assert.IsTrue(transactions.Count >= 5, "Should have both predefined and dynamic data");
            Assert.IsTrue(transactions.Any(t => t.Description.Contains("Dynamic")), "Should contain dynamically generated transactions");
        }

        [TestMethod]
        public async Task LogicLayer_UsesOnlyDataLayerAPI()
        {
            // Test that Logic layer uses only Data layer interface
            var mockRepository = new LogicTestMockRepository();
            var service = new TransactionService(mockRepository);

            // Act - Call service methods
            await service.GetTransactionsAsync();
            await service.GetUsersAsync();
            await service.GetCategoriesAsync();

            // Assert - Verify Data layer API was called
            Assert.IsTrue(mockRepository.GetTransactionsAsyncCalled, "Should call Data layer GetTransactionsAsync");
            Assert.IsTrue(mockRepository.GetUsersAsyncCalled, "Should call Data layer GetUsersAsync");
            Assert.IsTrue(mockRepository.GetCategoriesAsyncCalled, "Should call Data layer GetCategoriesAsync");
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

    // Mock repository for Logic layer testing (Fixed for .NET Framework 4.7.2)
    public class LogicTestMockRepository : ITransactionRepository
    {
        private readonly List<FinancialTransaction> _transactions = new List<FinancialTransaction>();
        private readonly List<User> _users = new List<User>();
        private readonly List<Event> _events = new List<Event>();
        private readonly List<TransactionCategory> _categories = new List<TransactionCategory>();
        private int _nextTransactionId = 1;

        // Properties to verify method calls
        public bool GetTransactionsAsyncCalled { get; private set; }
        public bool GetUsersAsyncCalled { get; private set; }
        public bool GetCategoriesAsyncCalled { get; private set; }

        public LogicTestMockRepository()
        {
            // Initialize test data
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
        }

        // Implement interface with tracking
        public async Task<List<IFinancialTransaction>> GetTransactionsAsync()
        {
            GetTransactionsAsyncCalled = true;
            return await Task.FromResult(_transactions.Cast<IFinancialTransaction>().ToList());
        }

        public async Task<List<IUser>> GetUsersAsync()
        {
            GetUsersAsyncCalled = true;
            return await Task.FromResult(_users.Cast<IUser>().ToList());
        }

        public async Task<List<ITransactionCategory>> GetCategoriesAsync()
        {
            GetCategoriesAsyncCalled = true;
            return await Task.FromResult(_categories.Cast<ITransactionCategory>().ToList());
        }

        public void AddTransaction(IFinancialTransaction transaction)
        {
            var concrete = transaction as FinancialTransaction ??
                new FinancialTransaction(transaction.Description, transaction.Amount, transaction.IsExpense, transaction.Category, transaction.Date)
                {
                    UserId = transaction.UserId ?? Guid.Empty
                };

            if (concrete.Id == 0)
                concrete.Id = _nextTransactionId++;

            _transactions.Add(concrete);
        }

        // Implement all other required interface methods with minimal implementations
        public List<IFinancialTransaction> GetTransactions() => _transactions.Cast<IFinancialTransaction>().ToList();
        public List<IFinancialTransaction> GetTransactionsByUser(Guid userId) => _transactions.Where(t => t.UserId == userId).Cast<IFinancialTransaction>().ToList();
        public List<IFinancialTransaction> GetTransactionsByCategory(string category) => _transactions.Where(t => t.Category == category).Cast<IFinancialTransaction>().ToList();
        public List<IFinancialTransaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate) => _transactions.Where(t => t.Date >= startDate && t.Date <= endDate).Cast<IFinancialTransaction>().ToList();
        public void UpdateTransaction(IFinancialTransaction transaction) { /* implementation */ }
        public void DeleteTransaction(int id) => _transactions.RemoveAll(t => t.Id == id);

        public async Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId) => await Task.FromResult(GetTransactionsByUser(userId));
        public async Task<List<IFinancialTransaction>> GetTransactionsByCategoryAsync(string category) => await Task.FromResult(GetTransactionsByCategory(category));
        public async Task<List<IFinancialTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate) => await Task.FromResult(GetTransactionsByDateRange(startDate, endDate));
        public async Task AddTransactionAsync(IFinancialTransaction transaction) { AddTransaction(transaction); await Task.CompletedTask; }
        public async Task UpdateTransactionAsync(IFinancialTransaction transaction) { UpdateTransaction(transaction); await Task.CompletedTask; }
        public async Task DeleteTransactionAsync(int id) { DeleteTransaction(id); await Task.CompletedTask; }

        // User operations
        public List<IUser> GetUsers() => _users.Cast<IUser>().ToList();
        public IUser GetUser(Guid id) => _users.FirstOrDefault(u => u.Id == id);
        public void AddUser(IUser user) { /* implementation */ }
        public void UpdateUser(IUser user) { /* implementation */ }
        public void DeleteUser(Guid id) { /* implementation */ }
        public async Task<IUser> GetUserAsync(Guid id) => await Task.FromResult(GetUser(id));
        public async Task AddUserAsync(IUser user) { AddUser(user); await Task.CompletedTask; }
        public async Task UpdateUserAsync(IUser user) { UpdateUser(user); await Task.CompletedTask; }
        public async Task DeleteUserAsync(Guid id) { DeleteUser(id); await Task.CompletedTask; }

        // Event operations
        public List<IEvent> GetEvents() => _events.Cast<IEvent>().ToList();
        public List<IEvent> GetEventsByUser(Guid userId) => _events.Where(e => e.UserId == userId).Cast<IEvent>().ToList();
        public void AddEvent(IEvent e) { /* implementation */ }
        public async Task<List<IEvent>> GetEventsAsync() => await Task.FromResult(GetEvents());
        public async Task<List<IEvent>> GetEventsByUserAsync(Guid userId) => await Task.FromResult(GetEventsByUser(userId));
        public async Task AddEventAsync(IEvent e) { AddEvent(e); await Task.CompletedTask; }

        // Category operations
        public List<ITransactionCategory> GetCategories() => _categories.Cast<ITransactionCategory>().ToList();
        public void AddCategory(ITransactionCategory category) { /* implementation */ }
        public void UpdateCategory(ITransactionCategory category) { /* implementation */ }
        public void DeleteCategory(int id) { /* implementation */ }
        public async Task AddCategoryAsync(ITransactionCategory category) { AddCategory(category); await Task.CompletedTask; }
        public async Task UpdateCategoryAsync(ITransactionCategory category) { UpdateCategory(category); await Task.CompletedTask; }
        public async Task DeleteCategoryAsync(int id) { DeleteCategory(id); await Task.CompletedTask; }
    }
}