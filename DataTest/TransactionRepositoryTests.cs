using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataTest
{
    [TestClass]
    public class TransactionRepositoryTests
    {
        private MockTransactionRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new MockTransactionRepository();
        }

        [TestMethod]
        public void GetTransactions_ReturnsTransactions()
        {
            // Act
            var transactions = _repository.GetTransactions();

            // Assert
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions.Count > 0);
        }

        [TestMethod]
        public void AddTransaction_ValidTransaction_AddsSuccessfully()
        {
            // Arrange
            var transaction = new FinancialTransaction("Test", 100m, true, "Food", DateTime.Today);

            // Act
            _repository.AddTransaction(transaction);
            var transactions = _repository.GetTransactions();

            // Assert
            Assert.IsTrue(transactions.Any(t => t.Description == "Test"));
        }

        [TestMethod]
        public void GetTransactionsByCategory_ValidCategory_ReturnsFilteredResults()
        {
            // Act
            var foodTransactions = _repository.GetTransactionsByCategory("Food");

            // Assert
            Assert.IsNotNull(foodTransactions);
            Assert.IsTrue(foodTransactions.All(t => t.Category == "Food"));
        }

        [TestMethod]
        public void DeleteTransaction_ExistingId_RemovesTransaction()
        {
            // Arrange
            var transactions = _repository.GetTransactions();
            if (transactions.Count > 0)
            {
                var firstTransactionId = transactions[0].Id;

                // Act
                _repository.DeleteTransaction(firstTransactionId);
                var updatedTransactions = _repository.GetTransactions();

                // Assert
                Assert.IsFalse(updatedTransactions.Any(t => t.Id == firstTransactionId));
            }
        }

        [TestMethod]
        public void TestDataGenerationMethod1_MockRepository_GeneratesValidData()
        {
            // Data Generation Method 1
            var mockRepo = new MockTransactionRepository();
            var transactions = mockRepo.GetTransactions();
            var users = mockRepo.GetUsers();
            var categories = mockRepo.GetCategories();

            // Assert
            Assert.IsTrue(transactions.Count > 0, "MockRepository should generate transactions");
            Assert.IsTrue(users.Count > 0, "MockRepository should generate users");
            Assert.IsTrue(categories.Count > 0, "MockRepository should generate categories");
            Assert.IsTrue(transactions.All(t => t.Amount > 0), "All transactions should have positive amounts");
        }

        [TestMethod]
        public void TestDataGenerationMethod2_TestDataGenerator_GeneratesValidData()
        {
            // Data Generation Method 2
            var generatedTransactions = TestDataGenerator.GenerateRandomTransactions(5);
            var generatedUsers = TestDataGenerator.GenerateTestUsers(3);
            var generatedCategories = TestDataGenerator.GenerateTestCategories();

            // Assert
            Assert.AreEqual(5, generatedTransactions.Count, "Should generate exactly 5 transactions");
            Assert.AreEqual(3, generatedUsers.Count, "Should generate exactly 3 users");
            Assert.IsTrue(generatedCategories.Count > 0, "Should generate categories");
            Assert.IsTrue(generatedTransactions.All(t => t.Amount > 0), "All generated transactions should have positive amounts");
            Assert.IsTrue(generatedTransactions.All(t => !string.IsNullOrEmpty(t.Description)), "All transactions should have descriptions");
        }
    }
}