using System;
using System.Collections.Generic;
using Data;

namespace DataTest
{
    public class TestDataGenerator
    {
        public static User GenerateTestUser()
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User"
            };
        }

        public static TransactionCategory GenerateTestTransactionCategory()
        {
            return new TransactionCategory("Test Category", "Sample category");
        }

        public static FinancialTransaction GenerateTransaction(string description, decimal amount, bool isExpense, string category, DateTime? date = null)
        {
            return new FinancialTransaction(description, amount, isExpense, category, date ?? DateTime.Now);
        }

        public static Event GenerateTestEvent()
        {
            return new UserEvent(Guid.NewGuid(), "Test Event");
        }

        public static List<FinancialTransaction> GenerateMultipleTransactions(int count)
        {
            var list = new List<FinancialTransaction>();
            for (int i = 0; i < count; i++)
            {
                list.Add(GenerateTransaction(
                    $"Transaction {i + 1}",
                    amount: (i + 1) * 10,
                    isExpense: i % 2 == 0,
                    category: i % 2 == 0 ? "Food" : "Income",
                    date: DateTime.Now.AddDays(-i)
                ));
            }
            return list;
        }

        [TestMethod]
        public void AddTransaction_ShouldUpdateTransactionCount_UsingInlineCreation()
        {
            var state = new TransactionProcessState();

            var t1 = new FinancialTransaction("Inline T1", 100.0m, true, "Misc", DateTime.Now);
            var t2 = new FinancialTransaction("Inline T2", 150.0m, false, "Income", DateTime.Now);

            state.Transactions.Add(t1);
            state.Transactions.Add(t2);

            Assert.AreEqual(2, state.Transactions.Count);
        }

    }
}
