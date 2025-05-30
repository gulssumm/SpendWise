using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Data;
using System;
using System.Linq;

namespace DataTest
{
    [TestClass]
    public class TransactionRepositoryTests
    {
        private FinancialDbContext _context;
        private TransactionRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<FinancialDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FinancialDbContext(options);
            _repository = new TransactionRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
            _repository?.Dispose();
        }

        [TestMethod]
        public void AddTransaction_ShouldStoreInMemory()
        {
            // Arrange - Data Generation Method 1: Single transaction
            var transaction = new TestFinancialTransaction("Test Transaction", 100.50m, false, "Income", DateTime.Now);

            // Act
            _repository.AddTransaction(transaction);

            // Assert
            var storedTransactions = _repository.GetTransactions();
            Assert.AreEqual(1, storedTransactions.Count);
            Assert.AreEqual("Test Transaction", storedTransactions[0].Description);
            Assert.AreEqual(100.50m, storedTransactions[0].Amount);
        }

        [TestMethod]
        public void AddEvent_ShouldStoreCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventObj = new TestUserEvent(userId, "Test event occurred");

            // Act
            _repository.AddEvent(eventObj);

            // Assert
            var storedEvents = _repository.GetEvents();
            Assert.AreEqual(1, storedEvents.Count);
            Assert.AreEqual("Test event occurred", storedEvents[0].Description);
            Assert.AreEqual(userId, storedEvents[0].UserId);
        }

        [TestMethod]
        public void LoadTransactions_ShouldLoadStoredTransactions()
        {
            // Arrange - Data Generation Method 2: Multiple transactions
            var transactions = new[]
            {
                new TestFinancialTransaction("Transaction 1", 50.00m, true, "Expense", DateTime.Now),
                new TestFinancialTransaction("Transaction 2", 25.00m, false, "Income", DateTime.Now.AddDays(-1)),
                new TestFinancialTransaction("Transaction 3", 75.00m, true, "Expense", DateTime.Now.AddDays(-2))
            };

            foreach (var transaction in transactions)
            {
                _repository.AddTransaction(transaction);
            }

            // Act
            var loadedTransactions = _repository.LoadTransactions();

            // Assert
            Assert.AreEqual(3, loadedTransactions.Count);
            Assert.IsTrue(loadedTransactions.Any(t => t.Description == "Transaction 1"));
            Assert.IsTrue(loadedTransactions.Any(t => t.Description == "Transaction 2"));
            Assert.IsTrue(loadedTransactions.Any(t => t.Description == "Transaction 3"));
        }

        [TestMethod]
        public void GetTransactions_ShouldFilterAndOrder()
        {
            // Arrange - Data Generation Method 3: Time-based data generation
            var oldTransaction = new TestFinancialTransaction("Old Transaction", 100.00m, false, "Income", DateTime.Now.AddDays(-5));
            var newTransaction = new TestFinancialTransaction("New Transaction", 200.00m, false, "Income", DateTime.Now);

            _repository.AddTransaction(oldTransaction);
            _repository.AddTransaction(newTransaction);

            // Act
            var transactions = _repository.GetTransactions();

            // Assert
            Assert.AreEqual(2, transactions.Count);
            // Should be ordered by date descending (newest first)
            Assert.AreEqual("New Transaction", transactions[0].Description);
            Assert.AreEqual("Old Transaction", transactions[1].Description);
        }

        [TestMethod]
        public void SaveTransactions_ShouldReplaceExistingTransactions()
        {
            // Arrange - Add initial transaction
            var initialTransaction = new TestFinancialTransaction("Initial", 50.00m, true, "Expense", DateTime.Now);
            _repository.AddTransaction(initialTransaction);

            // Prepare new transactions list
            var newTransactions = new FinancialTransaction[]
            {
                new TestFinancialTransaction("New 1", 100.00m, false, "Income", DateTime.Now),
                new TestFinancialTransaction("New 2", 150.00m, true, "Expense", DateTime.Now)
            };

            // Act
            _repository.SaveTransactions(newTransactions.ToList());

            // Assert
            var savedTransactions = _repository.LoadTransactions();
            Assert.AreEqual(2, savedTransactions.Count);
            Assert.IsTrue(savedTransactions.Any(t => t.Description == "New 1"));
            Assert.IsTrue(savedTransactions.Any(t => t.Description == "New 2"));
            Assert.IsFalse(savedTransactions.Any(t => t.Description == "Initial"));
        }

        // Test helper classes
        private class TestFinancialTransaction : FinancialTransaction
        {
            public TestFinancialTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
                : base(description, amount, isExpense, category, date)
            {
            }
        }

        private class TestUserEvent : UserEvent
        {
            public TestUserEvent(Guid userId, string description)
                : base(userId, description)
            {
            }
        }
    }
}