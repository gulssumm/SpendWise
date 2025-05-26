using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Data;   // Adjust namespace if needed
using Logic;  // Adjust namespace if needed

namespace LogicTests
{
    // A simple fake repository to use in tests
    internal class FakeTransactionRepository : ITransactionRepository
    {
        private readonly List<FinancialTransaction> _transactions = new();

        public void AddTransaction(FinancialTransaction transaction)
        {
            _transactions.Add(transaction);
        }

        public List<FinancialTransaction> GetTransactions()
        {
            return new List<FinancialTransaction>(_transactions);
        }

        public void AddEvent(Event e) => throw new NotImplementedException();
        public List<Event> GetEvents() => throw new NotImplementedException();
        public void SaveTransactions(List<FinancialTransaction> transactions) => throw new NotImplementedException();
        public List<FinancialTransaction> LoadTransactions() => throw new NotImplementedException();
    }

    [TestClass]
    public class TransactionServiceTests
    {
        [TestMethod]
        public void AddTransaction_ShouldAddTransactionCorrectly()
        {
            // Arrange
            var repo = new FakeTransactionRepository();
            var service = new TransactionService(repo);

            var user = new User { Id = Guid.NewGuid(), Name = "Test User" };
            var category = new TransactionCategory("Food", "Food category");

            // Act
            service.AddTransaction(
                description: "Test transaction",
                amount: 50.0m,
                isExpense: true,
                category: category,
                user: user
            );

            // Assert
            var transactions = service.GetTransactions();
            Assert.AreEqual(1, transactions.Count);
            Assert.AreEqual("Test transaction", transactions[0].Description);
            Assert.AreEqual(50.0m, transactions[0].Amount);
        }
    }
}
