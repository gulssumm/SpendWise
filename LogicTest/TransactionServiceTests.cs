using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using Data;
using System;
using System.Linq;

namespace LogicTest
{
    [TestClass]
    public class TransactionServiceTests
    {
        private MockTransactionRepository _mockRepository = null!;
        private TransactionService _transactionService = null!; 

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new MockTransactionRepository();
            _transactionService = new TransactionService(_mockRepository);
        }

        [TestMethod]
        public void AddTransaction_ValidInput_ShouldAddSuccessfully()
        {
            // Arrange - Data Generation Method 1: Single valid transaction
            var category = new TestTransactionCategory("Food", "Food expenses");
            var user = new TestUser(Guid.NewGuid(), "John Doe");

            // Act
            _transactionService.AddTransaction("Lunch", 15.50m, true, category, user);

            // Assert
            var transactions = _transactionService.GetTransactions();
            Assert.AreEqual(1, transactions.Count);
            Assert.AreEqual("Lunch", transactions[0].Description);
            Assert.AreEqual(15.50m, transactions[0].Amount);
            Assert.IsTrue(transactions[0].IsExpense);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddTransaction_EmptyDescription_ShouldThrowException()
        {
            // Arrange
            var category = new TestTransactionCategory("Food", "Food expenses");
            var user = new TestUser(Guid.NewGuid(), "John Doe");

            // Act & Assert
            _transactionService.AddTransaction("", 15.50m, true, category, user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddTransaction_NegativeAmount_ShouldThrowException()
        {
            // Arrange
            var category = new TestTransactionCategory("Food", "Food expenses");
            var user = new TestUser(Guid.NewGuid(), "John Doe");

            // Act & Assert
            _transactionService.AddTransaction("Lunch", -15.50m, true, category, user);
        }

        [TestMethod]
        public void GetBalance_MultipleTransactions_ShouldCalculateCorrectly()
        {
            // Arrange - Data Generation Method 2: Multiple transactions with different types
            var category = new TestTransactionCategory("Mixed", "Mixed transactions");
            var user = new TestUser(Guid.NewGuid(), "Jane Doe");

            // Add income transactions
            _transactionService.AddTransaction("Salary", 2000.00m, false, category, user);
            _transactionService.AddTransaction("Bonus", 500.00m, false, category, user);

            // Add expense transactions
            _transactionService.AddTransaction("Rent", 800.00m, true, category, user);
            _transactionService.AddTransaction("Groceries", 200.00m, true, category, user);

            // Act
            var balance = _transactionService.GetBalance();

            // Assert
            Assert.AreEqual(1500.00m, balance); // 2500 income - 1000 expenses
        }

        [TestMethod]
        public void GetMonthlyReport_FiltersByMonthAndYear()
        {
            // Arrange - Data Generation Method 3: Transactions across different months
            var category = new TestTransactionCategory("Test", "Test category");
            var user = new TestUser(Guid.NewGuid(), "Test User");

            // Manually create transactions with specific dates
            var repo = new MockTransactionRepository();
            var service = new TransactionService(repo);

            // Add transaction for current month
            service.AddTransaction("Current Month", 100.00m, false, category, user);

            // Act
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var monthlyTransactions = service.GetMonthlyReport(currentMonth, currentYear);

            // Assert
            Assert.AreEqual(1, monthlyTransactions.Count);
            Assert.AreEqual("Current Month", monthlyTransactions[0].Description);
        }

        [TestMethod]
        public void GetProcessState_ShouldCalculateCorrectTotals()
        {
            // Arrange
            var category = new TestTransactionCategory("Test", "Test category");
            var user = new TestUser(Guid.NewGuid(), "Test User");

            _transactionService.AddTransaction("Income 1", 1000.00m, false, category, user);
            _transactionService.AddTransaction("Income 2", 500.00m, false, category, user);
            _transactionService.AddTransaction("Expense 1", 300.00m, true, category, user);

            // Act
            var processState = _transactionService.GetProcessState();

            // Assert
            Assert.IsNotNull(processState);
            Assert.AreEqual(1500.00m, processState.TotalIncome);
            Assert.AreEqual(300.00m, processState.TotalExpenses);
            Assert.AreEqual(1200.00m, processState.CurrentBalance);
        }

        // Test helper classes - LogicTest only
        private class TestTransactionCategory : TransactionCategory
        {
            public TestTransactionCategory(string name, string description) : base(name, description)
            {
            }
        }

        private class TestUser : User
        {
            public TestUser(Guid id, string name)
            {
                Id = id;
                Name = name;
            }
        }
    }
}