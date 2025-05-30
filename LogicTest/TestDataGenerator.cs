using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using Data;

namespace LogicTest
{
    public static class TestDataGenerator
    {
        public static ConcreteUser GenerateTestUser()
        {
            return new ConcreteUser(Guid.NewGuid(), "Test User");
        }

        public static ConcreteTransactionCategory GenerateTestTransactionCategory()
        {
            return new ConcreteTransactionCategory("Test Category", "Sample category");
        }

        public static ConcreteFinancialTransaction GenerateTransaction(string description, decimal amount, bool isExpense, string category, DateTime? date = null)
        {
            return new ConcreteFinancialTransaction(description, amount, isExpense, category, date ?? DateTime.Now);
        }

        public static ConcreteUserEvent GenerateTestEvent()
        {
            return new ConcreteUserEvent(Guid.NewGuid(), "Test Event");
        }

        public static List<ConcreteFinancialTransaction> GenerateMultipleTransactions(int count)
        {
            var list = new List<ConcreteFinancialTransaction>();
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
    }
}