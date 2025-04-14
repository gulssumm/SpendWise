using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using SpendWise.Logic;
using SpendWise.Data.Interfaces;
using SpendWise.Data.Models;
using SpendWise.Logic.Interfaces;
using SpendWise.Logic.Services;

namespace TestProject1.Tests
{
    [TestClass]
    public class TransactionTests
    {
        private ITransactionService _transactionService = null!;
        private ITransactionRepository _transactionRepository = null!;

        [TestInitialize]
        public void Setup()
        {
            var services = new ServiceCollection();

            // Register dependencies
            services.AddScoped<ITransactionRepository, InMemoryTransactionRepository>();
            services.AddScoped<ITransactionService, TransactionService>();

            var serviceProvider = services.BuildServiceProvider();

            _transactionService = serviceProvider.GetRequiredService<ITransactionService>();
            _transactionRepository = serviceProvider.GetRequiredService<ITransactionRepository>();
        }

        // Test: Business Logic Layer
        [TestMethod]
        public void AddTransaction_ShouldStoreTransaction()
        {
            var user = TestDataGenerator.GenerateTestUser();
            var item = TestDataGenerator.GenerateTestCatalogItem();

            _transactionService.AddTransaction("Buy milk", 10.0m, true, item, user);

            var transactions = _transactionRepository.GetTransactions().ToList();

            Assert.AreEqual(1, transactions.Count);
            Assert.AreEqual("Buy milk", transactions[0].Description);
            Assert.AreEqual(10.0m, transactions[0].Amount);
            Assert.IsTrue(transactions[0].IsExpense);
        }

        [TestMethod]
        public void GetBalance_ShouldReturnCorrectTotal()
        {
            var user = TestDataGenerator.GenerateTestUser();
            var item = TestDataGenerator.GenerateTestCatalogItem();

            _transactionService.AddTransaction("Water bill", 30.0m, true, item, user);
            _transactionService.AddTransaction("Salary", 1000.0m, false, item, user);

            var balance = _transactionService.GetBalance();

            Assert.AreEqual(970.0m, balance);
        }

        [TestMethod]
        public void InMemoryTransactionRepository_ShouldStoreAndReturnTransactions()
        {
            var repo = new InMemoryTransactionRepository();
            var transaction = TestDataGenerator.GenerateTransaction("Test Transaction", 50m, false, "Salary", DateTime.Now);

            repo.AddTransaction(transaction);

            var stored = repo.GetTransactions().ToList();

            Assert.AreEqual(1, stored.Count);
            Assert.AreEqual("Test Transaction", stored[0].Description);
            Assert.AreEqual(50m, stored[0].Amount);
            Assert.IsFalse(stored[0].IsExpense);
            Assert.AreEqual("Salary", stored[0].Category);
        }

        [TestMethod]
        public void AddEvent_ShouldStoreEvent()
        {
            var repo = new InMemoryTransactionRepository();
            var testEvent = TestDataGenerator.GenerateTestEvent();

            repo.AddEvent(testEvent);

            var storedEvents = repo.GetEvents();

            Assert.AreEqual(1, storedEvents.Count);
            Assert.AreEqual("Test Event", storedEvents[0].Description);
        }


    }

    // Dummy In-Memory Repository for Data Layer Testing
    public class InMemoryTransactionRepository : ITransactionRepository
    {
        private readonly List<FinancialTransaction> _transactions = new();
        private readonly List<Event> _events = new();

        public void AddTransaction(FinancialTransaction transaction)
        {
            _transactions.Add(transaction);
        }

        public List<FinancialTransaction> GetTransactions()
        {
            return _transactions;
        }

        public void AddEvent(Event e)
        {
            _events.Add(e);
        }

        public List<Event> GetEvents()
        {
            return _events;
        }

        public void SaveTransactions(List<FinancialTransaction> transactions)
        {
            // For testing, just overwrite the current list
            _transactions.Clear();
            _transactions.AddRange(transactions);
        }

        public List<FinancialTransaction> LoadTransactions()
        {
            return new List<FinancialTransaction>(_transactions);
        }
    }


    public static class TestDataGenerator
    {
        public static User GenerateTestUser()
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User"
            };
        }

        public static CatalogItem GenerateTestCatalogItem()
        {
            return new CatalogItem
            {
                Id = 1,
                Name = "Test Category"
            };
        }

        public static Event GenerateTestEvent()
        {
            return new Event
            {
                UserId = Guid.NewGuid(),
                Description = "Test Event",
                Timestamp = DateTime.Now
            };
        }


        public static FinancialTransaction GenerateTransaction(
            string description = "Sample Transaction",
            decimal amount = 10.0m,
            bool isExpense = true,
            string category = "General",
            DateTime? date = null)
        {
            return new FinancialTransaction(
                description,
                amount,
                isExpense,
                category,
                date ?? DateTime.Now
            );
        }

        public static List<FinancialTransaction> GenerateMultipleTransactions(int count)
        {
            var transactions = new List<FinancialTransaction>();
            for (int i = 0; i < count; i++)
            {
                transactions.Add(GenerateTransaction(
                    $"Transaction {i + 1}",
                    amount: 5 * (i + 1),
                    isExpense: i % 2 == 0,
                    category: i % 2 == 0 ? "Food" : "Income",
                    date: DateTime.Today.AddDays(-i)
                ));
            }

            return transactions;
        }
    }
}
