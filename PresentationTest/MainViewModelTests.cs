using Microsoft.VisualStudio.TestTools.UnitTesting;
using Presentation;
using Logic;
using Data;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace PresentationTest
{
    [TestClass]
    public class MainViewModelTests
    {
        private MainViewModel _viewModel;
        private PresentationTestMockService _mockService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Dependency injection for testing - using mock service
            _mockService = new PresentationTestMockService();
            _viewModel = new MainViewModel(_mockService);
        }

        [TestMethod]
        public async Task ViewModel_LoadsTransactionsOnInitialization()
        {
            // Wait a moment for async initialization
            await Task.Delay(100);

            // Assert
            Assert.IsTrue(_viewModel.Transactions.Count > 0, "Should load transactions on initialization");
            Assert.IsTrue(_viewModel.Users.Count > 0, "Should load users on initialization");
            Assert.IsTrue(_viewModel.Categories.Count > 0, "Should load categories on initialization");
        }

        [TestMethod]
        public void ViewModel_CalculatesBalanceCorrectly()
        {
            // Arrange - Add test data
            _viewModel.Transactions.Clear();
            _viewModel.Transactions.Add(new FinancialTransaction("Income", 1000m, false, "Salary", DateTime.Today));
            _viewModel.Transactions.Add(new FinancialTransaction("Expense", 300m, true, "Food", DateTime.Today));

            // Assert
            Assert.AreEqual(700m, _viewModel.TotalBalance);
            Assert.AreEqual(300m, _viewModel.TotalExpenses);
            Assert.AreEqual(1000m, _viewModel.TotalIncome);
        }

        [TestMethod]
        public void ViewModel_MasterDetailPattern_SelectedTransactionWorks()
        {
            // Arrange
            var transaction = new FinancialTransaction("Test", 100m, true, "Test", DateTime.Today);
            _viewModel.Transactions.Add(transaction);

            // Act
            _viewModel.SelectedTransaction = transaction;

            // Assert
            Assert.AreEqual(transaction, _viewModel.SelectedTransaction);
        }

        [TestMethod]
        public void ViewModel_AddTransactionCommand_CanExecuteValidation()
        {
            // Arrange - Invalid data
            _viewModel.NewDescription = "";
            _viewModel.NewAmount = 0;
            _viewModel.NewCategory = "";

            // Assert
            Assert.IsFalse(_viewModel.AddTransactionCommand.CanExecute(null));

            // Arrange - Valid data
            _viewModel.NewDescription = "Test";
            _viewModel.NewAmount = 100;
            _viewModel.NewCategory = "Test";

            // Assert
            Assert.IsTrue(_viewModel.AddTransactionCommand.CanExecute(null));
        }

        [TestMethod]
        public void ViewModel_DeleteTransactionCommand_CanExecuteValidation()
        {
            // Assert - No selection
            Assert.IsFalse(_viewModel.DeleteTransactionCommand.CanExecute(null));

            // Arrange - With selection
            _viewModel.SelectedTransaction = new FinancialTransaction("Test", 100m, true, "Test", DateTime.Today);

            // Assert
            Assert.IsTrue(_viewModel.DeleteTransactionCommand.CanExecute(null));
        }

        [TestMethod]
        public async Task ViewModel_UsesAbstractLogicLayerAPI()
        {
            // Act - Call methods
            _viewModel.LoadDataCommand.Execute(null);

            // Wait for async operation to complete
            await Task.Delay(100);

            // Assert - Verify
            Assert.IsTrue(_mockService.GetTransactionsAsyncCalled, "Should call abstract service API");
            Assert.IsTrue(_mockService.GetUsersAsyncCalled, "Should call abstract service API");
            Assert.IsTrue(_mockService.GetCategoriesAsyncCalled, "Should call abstract service API");
        }

        [TestMethod]
        public void ViewModel_PropertyChanged_FiresCorrectly()
        {
            // Arrange
            bool propertyChangedFired = false;
            _viewModel.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(_viewModel.NewDescription))
                    propertyChangedFired = true;
            };

            // Act
            _viewModel.NewDescription = "Test";

            // Assert
            Assert.IsTrue(propertyChangedFired, "PropertyChanged should fire for data binding");
        }
    }

    // Mock service for Presentation layer testing - demonstrates independence
    public class PresentationTestMockService : ITransactionService
    {
        private readonly List<IFinancialTransaction> _transactions = new List<IFinancialTransaction>();
        private readonly List<IUser> _users = new List<IUser>();
        private readonly List<ITransactionCategory> _categories = new List<ITransactionCategory>();

        // Properties to verify method calls
        public bool GetTransactionsAsyncCalled { get; private set; }
        public bool GetUsersAsyncCalled { get; private set; }
        public bool GetCategoriesAsyncCalled { get; private set; }

        public PresentationTestMockService()
        {
            // Test data for Presentation layer
            var user = new User { Id = Guid.NewGuid(), Name = "Presentation Test User" };
            _users.Add(user);

            _categories.Add(new TransactionCategory("Food", "Food expenses") { Id = 1 });
            _categories.Add(new TransactionCategory("Transport", "Transport expenses") { Id = 2 });

            _transactions.Add(new FinancialTransaction("Presentation Test", 50m, true, "Food", DateTime.Today) { UserId = user.Id });
            _transactions.Add(new FinancialTransaction("Presentation Income", 1000m, false, "Salary", DateTime.Today) { UserId = user.Id });
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsAsync()
        {
            GetTransactionsAsyncCalled = true;
            return await Task.FromResult(_transactions);
        }

        public async Task<List<IUser>> GetUsersAsync()
        {
            GetUsersAsyncCalled = true;
            return await Task.FromResult(_users);
        }

        public async Task<List<ITransactionCategory>> GetCategoriesAsync()
        {
            GetCategoriesAsyncCalled = true;
            return await Task.FromResult(_categories);
        }

        public async Task AddTransactionAsync(IFinancialTransaction transaction)
        {
            _transactions.Add(transaction);
            await Task.CompletedTask;
        }

        public async Task DeleteTransactionAsync(int id)
        {
            _transactions.RemoveAll(t => t.Id == id);
            await Task.CompletedTask;
        }

        // Other required interface methods (simplified for testing)
        public async Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId) => await Task.FromResult(new List<IFinancialTransaction>());
        public async Task<List<IFinancialTransaction>> GetTransactionsByCategory(string category) => await Task.FromResult(new List<IFinancialTransaction>());
        public async Task<List<IFinancialTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate) => await Task.FromResult(new List<IFinancialTransaction>());
        public async Task UpdateTransactionAsync(IFinancialTransaction transaction) => await Task.CompletedTask;
        public async Task<decimal> CalculateBalanceAsync() => await Task.FromResult(0m);
        public async Task<decimal> CalculateBalanceByUserAsync(Guid userId) => await Task.FromResult(0m);
        public async Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync() => await Task.FromResult(new Dictionary<string, decimal>());
        public async Task<List<IFinancialTransaction>> GetRecentTransactionsAsync(int count = 10) => await Task.FromResult(new List<IFinancialTransaction>());
        public async Task<IUser> GetUserAsync(Guid id) => await Task.FromResult((IUser)null);
        public async Task AddUserAsync(IUser user) => await Task.CompletedTask;
        public async Task UpdateUserAsync(IUser user) => await Task.CompletedTask;
        public async Task DeleteUserAsync(Guid id) => await Task.CompletedTask;
        public async Task AddCategoryAsync(ITransactionCategory category) => await Task.CompletedTask;
        public async Task UpdateCategoryAsync(ITransactionCategory category) => await Task.CompletedTask;
        public async Task DeleteCategoryAsync(int id) => await Task.CompletedTask;
        public async Task<List<IEvent>> GetEventsAsync() => await Task.FromResult(new List<IEvent>());
        public async Task<List<IEvent>> GetEventsByUserAsync(Guid userId) => await Task.FromResult(new List<IEvent>());
        public async Task AddEventAsync(IEvent e) => await Task.CompletedTask;
    }
}