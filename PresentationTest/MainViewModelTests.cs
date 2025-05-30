using Microsoft.VisualStudio.TestTools.UnitTesting;
using Presentation;
using Logic;

namespace PresentationTest
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void MainViewModel_Initialize_ShouldSetupCorrectly()
        {
            // Arrange & Act - dependency injection for testing
            var mockService = new MockTransactionService();
            var viewModel = new MainViewModel(mockService);

            // Assert
            Assert.IsNotNull(viewModel.Transactions);
            Assert.IsNotNull(viewModel.AddTransactionCommand);
        }

        [TestMethod]
        public void SelectedTransaction_WhenSet_ShouldUpdateDetails()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var transaction = new TestTransaction("Test", 100m, false, "Income", System.DateTime.Now);

            // Act
            viewModel.SelectedTransaction = transaction;

            // Assert
            Assert.AreEqual(transaction, viewModel.SelectedTransaction);
            Assert.IsTrue(viewModel.TransactionDetails.Contains("Test"));
        }

        // Mock service for testing
        private class MockTransactionService : ITransactionService
        {
            public void AddTransaction(string description, decimal amount, bool isExpense, Data.TransactionCategory category, Data.User user) { }
            public List<Data.FinancialTransaction> GetTransactions() => new List<Data.FinancialTransaction>();
            public decimal GetBalance() => 0m;
            public Data.ProcessState GetProcessState() => null;
            public void SaveTransactions() { }
            public void LoadTransactions() { }
            public List<Data.FinancialTransaction> GetMonthlyReport(int month, int year) => new List<Data.FinancialTransaction>();
        }

        private class TestTransaction : Data.FinancialTransaction
        {
            public TestTransaction(string description, decimal amount, bool isExpense, string category, System.DateTime date)
                : base(description, amount, isExpense, category, date) { }
        }
    }
}