using Microsoft.VisualStudio.TestTools.UnitTesting;
using Presentation.ViewModel;
using Presentation.Model;
using Logic;
using Data;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace PresentationTest
{
    /// <summary>
    /// Tests for the MVVM structure - Tests ViewModel independently with dependency injection
    /// FIXED: Uses composition instead of inheritance for proper layer separation
    /// </summary>
    [TestClass]
    public class MainViewModelTests
    {
        private MainViewModel _viewModel;
        private TestFinancialDataModel _mockModel;

        [TestInitialize]
        public void TestInitialize()
        {
            // Dependency injection for testing - ViewModel depends on Model
            _mockModel = new TestFinancialDataModel();
            _viewModel = new MainViewModel(_mockModel);
        }

        [TestMethod]
        public async Task ViewModel_LoadsTransactionsOnInitialization()
        {
            // Wait for async initialization
            await Task.Delay(100);

            // Assert - verify data is loaded through Model layer
            Assert.IsTrue(_viewModel.Transactions.Count > 0, "Should load transactions on initialization");
            Assert.IsTrue(_viewModel.Users.Count > 0, "Should load users on initialization");
            Assert.IsTrue(_viewModel.Categories.Count > 0, "Should load categories on initialization");
        }

        [TestMethod]
        public async Task ViewModel_CalculatesBalanceCorrectly()
        {
            await Task.Delay(200);

            // Test ViewModel business logic for UI display
            _viewModel.Transactions.Clear();

            Assert.AreEqual(0, _viewModel.Transactions.Count, "Transactions should be cleared");

            // Add test data using mock objects (avoiding direct Data layer references)
            _viewModel.Transactions.Add(new MockTransaction { Amount = 1000m, IsExpense = false, Description = "Income" });
            _viewModel.Transactions.Add(new MockTransaction { Amount = 300m, IsExpense = true, Description = "Expense" });

            Assert.AreEqual(2, _viewModel.Transactions.Count, "Should have exactly 2 test transactions");

            // Assert calculated properties
            Assert.AreEqual(700m, _viewModel.TotalBalance, "Should calculate balance correctly: 1000 - 300 = 700");
            Assert.AreEqual(300m, _viewModel.TotalExpenses, "Should calculate expenses correctly");
            Assert.AreEqual(1000m, _viewModel.TotalIncome, "Should calculate income correctly");
        }

        [TestMethod]
        public void ViewModel_MasterDetailPattern_SelectedTransactionWorks()
        {
            // Test Master-Detail pattern requirement
            var testTransaction = new MockTransaction { Id = 1, Description = "Test Transaction", Amount = 100m, IsExpense = true, Category = "Test" };
            _viewModel.Transactions.Add(testTransaction);

            // Act - select transaction (Detail view)
            _viewModel.SelectedTransaction = testTransaction;

            // Assert - Master-Detail binding works
            Assert.AreEqual(testTransaction, _viewModel.SelectedTransaction, "Master-Detail pattern should work");
        }

        [TestMethod]
        public void ViewModel_AddTransactionCommand_CanExecuteValidation()
        {
            // Test command validation logic

            // Test with invalid data
            _viewModel.NewDescription = "";
            _viewModel.NewAmount = 0;
            _viewModel.NewCategory = "";
            Assert.IsFalse(_viewModel.AddTransactionCommand.CanExecute(null), "Should not execute with invalid data");

            // Test with valid data
            _viewModel.NewDescription = "Test Transaction";
            _viewModel.NewAmount = 100;
            _viewModel.NewCategory = "Test Category";
            Assert.IsTrue(_viewModel.AddTransactionCommand.CanExecute(null), "Should execute with valid data");
        }

        [TestMethod]
        public void ViewModel_DeleteTransactionCommand_CanExecuteValidation()
        {
            // Test command enablement based on selection

            // No selection - command should be disabled
            Assert.IsFalse(_viewModel.DeleteTransactionCommand.CanExecute(null), "Should not execute without selection");

            // With selection - command should be enabled
            _viewModel.SelectedTransaction = new MockTransaction { Id = 1, Description = "Test" };
            Assert.IsTrue(_viewModel.DeleteTransactionCommand.CanExecute(null), "Should execute with selection");
        }

        [TestMethod]
        public async Task ViewModel_UsesOnlyModelLayerAPI()
        {
            // Test that ViewModel uses only Model layer, not Logic layer directly
            // This verifies the proper MVVM architecture: View → ViewModel → Model → Logic

            // Act - trigger data loading
            _viewModel.LoadDataCommand.Execute(null);
            await Task.Delay(100);

            // Assert - verify mock model was called (demonstrates proper layering)
            Assert.IsTrue(_mockModel.GetTransactionsAsyncCalled, "Should call Model layer API");
            Assert.IsTrue(_mockModel.GetUsersAsyncCalled, "Should call Model layer API");
            Assert.IsTrue(_mockModel.GetCategoriesAsyncCalled, "Should call Model layer API");
        }

        [TestMethod]
        public void ViewModel_PropertyChanged_FiresCorrectly()
        {
            // Test INotifyPropertyChanged implementation for data binding
            bool propertyChangedFired = false;
            _viewModel.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(_viewModel.NewDescription))
                    propertyChangedFired = true;
            };

            // Act - change property
            _viewModel.NewDescription = "Test Description";

            // Assert - PropertyChanged event should fire for UI data binding
            Assert.IsTrue(propertyChangedFired, "PropertyChanged should fire for data binding");
        }

        [TestMethod]
        public void RelayCommand_ExecutesActionsCorrectly()
        {
            // Test RelayCommand implementation
            bool actionExecuted = false;
            var command = new RelayCommand(() => actionExecuted = true);

            // Act
            command.Execute(null);

            // Assert
            Assert.IsTrue(actionExecuted, "Command should execute the action");
        }

        [TestMethod]
        public void RelayCommand_CanExecuteWorksCorrectly()
        {
            // Test command validation
            bool canExecuteResult = false;
            var command = new RelayCommand(() => { }, () => canExecuteResult);

            // Test disabled state
            Assert.IsFalse(command.CanExecute(null), "CanExecute should return false");

            // Test enabled state
            canExecuteResult = true;
            Assert.IsTrue(command.CanExecute(null), "CanExecute should return true");
        }

        [TestMethod]
        public async Task ViewModel_DependencyInjection_WorksForTesting()
        {
            // Test requirement: "View Model use dependency injection for testing purpose"

            // Create alternative mock for different test scenario
            var alternativeMock = new TestFinancialDataModel();
            var alternativeViewModel = new MainViewModel(alternativeMock);

            // Act
            alternativeViewModel.LoadDataCommand.Execute(null);
            await Task.Delay(50);

            // Assert - verify dependency injection enables independent testing
            Assert.IsTrue(alternativeMock.GetTransactionsAsyncCalled, "Dependency injection should work for testing");
            Assert.AreNotEqual(_viewModel, alternativeViewModel, "Should create independent ViewModel instances");
        }

        [TestMethod]
        public async Task ViewModel_AddTransaction_CallsModelLayer()
        {
            // Test that ViewModel properly delegates to Model layer

            // Arrange
            _viewModel.NewDescription = "Test Add Transaction";
            _viewModel.NewAmount = 250m;
            _viewModel.NewIsExpense = true;
            _viewModel.NewCategory = "Test Category";
            _viewModel.NewDate = DateTime.Today;

            // Act
            await Task.Run(() => _viewModel.AddTransactionCommand.Execute(null));
            await Task.Delay(100); // Allow async operation to complete

            // Assert
            Assert.IsTrue(_mockModel.AddTransactionAsyncCalled, "Should call Model layer to add transaction");
        }

        [TestMethod]
        public async Task ViewModel_DeleteTransaction_CallsModelLayer()
        {
            // Test delete operation through Model layer

            // Arrange
            var transactionToDelete = new MockTransaction { Id = 99, Description = "To Delete", Amount = 50m };
            _viewModel.Transactions.Add(transactionToDelete);
            _viewModel.SelectedTransaction = transactionToDelete;

            // Act
            await Task.Run(() => _viewModel.DeleteTransactionCommand.Execute(null));
            await Task.Delay(100); // Allow async operation to complete

            // Assert
            Assert.IsTrue(_mockModel.DeleteTransactionAsyncCalled, "Should call Model layer to delete transaction");
        }
    }

    /// <summary>
    /// Test Model for independent ViewModel testing
    /// FIXED: Uses override instead of new keyword for proper method overriding
    /// </summary>
    public class TestFinancialDataModel : FinancialDataModel
    {
        private readonly List<IFinancialTransaction> _transactions;
        private readonly List<IUser> _users;
        private readonly List<ITransactionCategory> _categories;

        // Properties to verify method calls (demonstrates layer independence)
        public bool GetTransactionsAsyncCalled { get; private set; }
        public bool GetUsersAsyncCalled { get; private set; }
        public bool GetCategoriesAsyncCalled { get; private set; }
        public bool AddTransactionAsyncCalled { get; private set; }
        public bool DeleteTransactionAsyncCalled { get; private set; }

        public TestFinancialDataModel() : base(new TestTransactionService())
        {
            _transactions = new List<IFinancialTransaction>();
            _users = new List<IUser>();
            _categories = new List<ITransactionCategory>();

            // Initialize test data
            var testUser = new User { Id = Guid.NewGuid(), Name = "Test User" };
            _users.Add(testUser);

            _categories.Add(new TransactionCategory("Food", "Food expenses") { Id = 1 });
            _categories.Add(new TransactionCategory("Transport", "Transport expenses") { Id = 2 });

            _transactions.Add(new FinancialTransaction("Test Transaction", 75m, true, "Food", DateTime.Today) { Id = 1, UserId = testUser.Id });
            _transactions.Add(new FinancialTransaction("Test Income", 1500m, false, "Salary", DateTime.Today) { Id = 2, UserId = testUser.Id });
        }

        public override async Task<List<IFinancialTransaction>> GetTransactionsAsync()
        {
            GetTransactionsAsyncCalled = true;
            return await Task.FromResult(_transactions);
        }

        public override async Task<List<IUser>> GetUsersAsync()
        {
            GetUsersAsyncCalled = true;
            return await Task.FromResult(_users);
        }

        public override async Task<List<ITransactionCategory>> GetCategoriesAsync()
        {
            GetCategoriesAsyncCalled = true;
            return await Task.FromResult(_categories);
        }

        public override async Task<IFinancialTransaction> AddTransactionAsync(string description, decimal amount, bool isExpense, string category, DateTime date)
        {
            AddTransactionAsyncCalled = true;
            var transaction = new FinancialTransaction(description, amount, isExpense, category, date)
            {
                Id = _transactions.Count + 1,
                UserId = _users.FirstOrDefault()?.Id ?? Guid.Empty
            };
            _transactions.Add(transaction);
            return await Task.FromResult(transaction);
        }

        public override async Task DeleteTransactionAsync(int id)
        {
            DeleteTransactionAsyncCalled = true;
            _transactions.RemoveAll(t => t.Id == id);
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Minimal test service for FinancialDataModel base constructor
    /// This provides the ITransactionService dependency needed by the base class
    /// </summary>
    public class TestTransactionService : ITransactionService
    {
        // Minimal implementations required by base constructor
        public async Task<List<IFinancialTransaction>> GetTransactionsAsync() => await Task.FromResult(new List<IFinancialTransaction>());
        public async Task<List<IUser>> GetUsersAsync() => await Task.FromResult(new List<IUser>());
        public async Task<List<ITransactionCategory>> GetCategoriesAsync() => await Task.FromResult(new List<ITransactionCategory>());
        public async Task AddTransactionAsync(IFinancialTransaction transaction) => await Task.CompletedTask;
        public async Task DeleteTransactionAsync(int id) => await Task.CompletedTask;

        // Other required interface methods with minimal implementations
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

    /// <summary>
    /// Mock transaction for testing without direct Data layer dependencies in ViewModel tests
    /// </summary>
    public class MockTransaction
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public bool IsExpense { get; set; }
        public string Category { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Today;
        public Guid? UserId { get; set; }
    }
}