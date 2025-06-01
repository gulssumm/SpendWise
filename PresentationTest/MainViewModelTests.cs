using Microsoft.VisualStudio.TestTools.UnitTesting;
using Presentation;
using Logic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PresentationTest
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void MainViewModel_WithDependencyInjection_ShouldInitializeCorrectly()
        {
            // Arrange and Act - Test dependency injection pattern
            var mockService = new MockTransactionService();
            var viewModel = new MainViewModel(mockService);

            // Assert
            Assert.IsNotNull(viewModel.Transactions, "Transactions collection should be initialized");
            Assert.IsNotNull(viewModel.AddTransactionCommand, "AddTransactionCommand should be initialized");
            Assert.AreEqual(0, viewModel.Transactions.Count, "Should start with empty transactions");
        }

        [TestMethod]
        public void MainViewModel_ParameterlessConstructor_ShouldWork()
        {
            // Arrange & Act - Test parameterless constructor
            var viewModel = new MainViewModel();

            // Assert
            Assert.IsNotNull(viewModel.Transactions, "Transactions should be initialized");
            Assert.IsNotNull(viewModel.AddTransactionCommand, "Command should be initialized");
        }

        [TestMethod]
        public void SelectedTransaction_WhenSet_ShouldTriggerPropertyChanged()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var transaction = new TestTransaction("Test", 100m, false, "Income", DateTime.Now);
            bool propertyChangedTriggered = false;

            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(viewModel.SelectedTransaction) ||
                    e.PropertyName == nameof(viewModel.TransactionDetails))
                    propertyChangedTriggered = true;
            };

            // Act
            viewModel.SelectedTransaction = transaction;

            // Assert
            Assert.AreEqual(transaction, viewModel.SelectedTransaction);
            Assert.IsTrue(propertyChangedTriggered, "PropertyChanged should be triggered");
            Assert.IsTrue(viewModel.TransactionDetails.Contains("Test"), "Details should contain transaction description");
        }

        [TestMethod]
        public void TransactionDetails_WhenNoSelection_ShouldShowDefaultMessage()
        {
            // Arrange
            var viewModel = new MainViewModel();

            // Act
            var details = viewModel.TransactionDetails;

            // Assert
            Assert.AreEqual("Select a transaction to view details", details);
        }

        [TestMethod]
        public void TransactionDetails_WhenTransactionSelected_ShouldShowDetails()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var transaction = new TestTransaction("Grocery Shopping", 75.50m, true, "Food", DateTime.Now);

            // Act
            viewModel.SelectedTransaction = transaction;

            // Assert
            var details = viewModel.TransactionDetails;
            Assert.IsTrue(details.Contains("Grocery Shopping"), "Should contain description");
            Assert.IsTrue(details.Contains("75.50"), "Should contain amount");
            Assert.IsTrue(details.Contains("Expense"), "Should show expense type");
            Assert.IsTrue(details.Contains("Food"), "Should contain category");
        }

        [TestMethod]
        public void AddTransactionCommand_ShouldBeExecutable()
        {
            // Arrange
            var viewModel = new MainViewModel();

            // Act & Assert
            Assert.IsTrue(viewModel.AddTransactionCommand.CanExecute(null), "Command should be executable");
        }

        [TestMethod]
        public void Balance_ShouldReturnServiceBalance()
        {
            // Arrange
            var mockService = new MockTransactionService();
            var viewModel = new MainViewModel(mockService);

            // Act
            var balance = viewModel.Balance;

            // Assert
            Assert.AreEqual(0m, balance, "Should return mock service balance");
        }

        // Mock service for testing - implements dependency injection pattern
        private class MockTransactionService : ITransactionService
        {
            // Async implementations
            public async Task AddTransactionAsync(string description, decimal amount, bool isExpense, Data.TransactionCategory category, Data.User user)
            {
                await Task.CompletedTask; // Mock implementation
            }

            public async Task<List<Data.FinancialTransaction>> GetTransactionsAsync()
            {
                return await Task.FromResult(new List<Data.FinancialTransaction>());
            }

            public async Task UpdateTransactionAsync(int id, string description, decimal amount, bool isExpense, string category)
            {
                await Task.CompletedTask; // Mock implementation
            }

            public async Task DeleteTransactionAsync(int id)
            {
                await Task.CompletedTask; // Mock implementation
            }

            public async Task<decimal> GetBalanceAsync()
            {
                return await Task.FromResult(0m);
            }

            public async Task<Data.ProcessState?> GetProcessStateAsync()
            {
                return await Task.FromResult<Data.ProcessState?>(null);
            }

            public async Task SaveTransactionsAsync()
            {
                await Task.CompletedTask; // Mock implementation
            }

            public async Task LoadTransactionsAsync()
            {
                await Task.CompletedTask; // Mock implementation
            }

            public async Task<List<Data.FinancialTransaction>> GetMonthlyReportAsync(int month, int year)
            {
                return await Task.FromResult(new List<Data.FinancialTransaction>());
            }

            // Synchronous implementations for compatibility
            public void AddTransaction(string description, decimal amount, bool isExpense, Data.TransactionCategory category, Data.User user)
            {
                // Mock implementation
            }

            public List<Data.FinancialTransaction> GetTransactions() => new List<Data.FinancialTransaction>();
            public decimal GetBalance() => 0m;
            public Data.ProcessState? GetProcessState() => null;
            public void SaveTransactions() { }
            public void LoadTransactions() { }
            public List<Data.FinancialTransaction> GetMonthlyReport(int month, int year) => new List<Data.FinancialTransaction>();
        }

        // Test helper class
        private class TestTransaction : Data.FinancialTransaction
        {
            public TestTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
                : base(description, amount, isExpense, category, date) { }
        }
    }
}