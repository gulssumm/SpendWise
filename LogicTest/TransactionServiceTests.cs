using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using Data;

namespace LogicTest
{
    [TestClass]
    public class TransactionServiceTests
    {
        // A simple fake repository implementing ITransactionRepository
        private class FakeTransactionRepository : ITransactionRepository
        {
            private readonly List<FinancialTransaction> _transactions = new();
            private readonly List<Event> _events = new();

            public void AddTransaction(FinancialTransaction transaction)
            {
                _transactions.Add(transaction);
            }

            public List<FinancialTransaction> GetTransactions()
            {
                return new List<FinancialTransaction>(_transactions);
            }

            public void AddEvent(Event e)
            {
                // Just store events, no exception
                _events.Add(e);
            }

            public List<Event> GetEvents()
            {
                return new List<Event>(_events);
            }

            public void SaveTransactions(List<FinancialTransaction> transactions)
            {
                // For test, do nothing
            }

            public List<FinancialTransaction> LoadTransactions()
            {
                return new List<FinancialTransaction>(_transactions);
            }
        }

        [TestMethod]
        public void AddTransaction_ShouldAddTransactionCorrectly()
        {
            // Arrange
            var repo = new FakeTransactionRepository();
            var service = new TransactionService(repo);

            var user = new ConcreteUser { Id = Guid.NewGuid(), Name = "Test User" };
            var category = new ConcreteTransactionCategory("Food", "Food category");

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
