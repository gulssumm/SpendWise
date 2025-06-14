using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task GetTransactionsAsync_ReturnsTransactions()
        {
            // Act
            var transactions = await _repository.GetTransactionsAsync();

            // Assert
            Assert.IsNotNull(transactions);
            Assert.IsTrue(transactions.Count > 0);
        }

        [TestMethod]
        public async Task AddTransactionAsync_ValidTransaction_AddsSuccessfully()
        {
            // Arrange
            var initialCount = (await _repository.GetTransactionsAsync()).Count;

            // Act
            var testTransaction = new FinancialTransaction("Test Transaction", 100m, true, "Food", DateTime.Today);
            await _repository.AddTransactionAsync(testTransaction);
            var transactions = await _repository.GetTransactionsAsync();

            // Assert
            Assert.AreEqual(initialCount + 1, transactions.Count);
            Assert.IsTrue(transactions.Any(t => t.Description == "Test Transaction"));
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
        public async Task DeleteTransactionAsync_ExistingId_RemovesTransaction()
        {
            // Arrange
            var transactions = await _repository.GetTransactionsAsync();
            if (transactions.Count > 0)
            {
                var firstTransactionId = transactions[0].Id;
                var initialCount = transactions.Count;

                // Act
                await _repository.DeleteTransactionAsync(firstTransactionId);
                var updatedTransactions = await _repository.GetTransactionsAsync();

                // Assert
                Assert.AreEqual(initialCount - 1, updatedTransactions.Count);
                Assert.IsFalse(updatedTransactions.Any(t => t.Id == firstTransactionId));
            }
        }

        [TestMethod]
        public async Task TestDataGenerationMethod1_MockRepository_GeneratesValidData()
        {
            // Data Generation Method 1: Mock Repository with predefined data
            var mockRepo = new MockTransactionRepository();
            var transactions = await mockRepo.GetTransactionsAsync();
            var users = await mockRepo.GetUsersAsync();
            var categories = await mockRepo.GetCategoriesAsync();

            // Assert
            Assert.IsTrue(transactions.Count > 0, "MockRepository should generate transactions");
            Assert.IsTrue(users.Count > 0, "MockRepository should generate users");
            Assert.IsTrue(categories.Count > 0, "MockRepository should generate categories");
            Assert.IsTrue(transactions.All(t => t.Amount > 0), "All transactions should have positive amounts");
            Assert.IsTrue(transactions.Any(t => t.Description == "Grocery shopping"), "Should contain expected test data");
        }

        [TestMethod]
        public void TestDataGenerationMethod2_TestDataGenerator_GeneratesValidData()
        {
            // Data Generation Method 2: Random data generation
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

        [TestMethod]
        public async Task DataRepository_CRUD_Operations_WorkCorrectly()
        {
            // Test full CRUD cycle
            var initialCount = (await _repository.GetTransactionsAsync()).Count;

            // Create
            var testTransaction = new FinancialTransaction("CRUD Test", 150m, true, "Test", DateTime.Today);
            await _repository.AddTransactionAsync(testTransaction);
            var afterAdd = await _repository.GetTransactionsAsync();
            Assert.AreEqual(initialCount + 1, afterAdd.Count, "Create should increase count");

            // Read
            var addedTransaction = afterAdd.FirstOrDefault(t => t.Description == "CRUD Test");
            Assert.IsNotNull(addedTransaction, "Added transaction should be readable");

            // Update
            addedTransaction.Description = "CRUD Test Updated";
            await _repository.UpdateTransactionAsync(addedTransaction);
            var afterUpdate = await _repository.GetTransactionsAsync();
            var updatedTransaction = afterUpdate.FirstOrDefault(t => t.Id == addedTransaction.Id);
            Assert.AreEqual("CRUD Test Updated", updatedTransaction?.Description, "Update should modify description");

            // Delete
            await _repository.DeleteTransactionAsync(addedTransaction.Id);
            var afterDelete = await _repository.GetTransactionsAsync();
            Assert.AreEqual(initialCount, afterDelete.Count, "Delete should restore original count");
            Assert.IsFalse(afterDelete.Any(t => t.Id == addedTransaction.Id), "Deleted transaction should not exist");
        }
    }
}