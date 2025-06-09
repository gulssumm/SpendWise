using Data;
using System;
using System.Collections.Generic;

namespace LogicTest
{
    public static class TestDataGenerator
    {
        public static User GenerateTestUser() =>
            new TestUser(Guid.NewGuid(), "Test User");

        public static TransactionCategory GenerateTestTransactionCategory() =>
            new TestTransactionCategory("Test Category", "Sample category");

        public static FinancialTransaction GenerateTransaction(string description, decimal amount, bool isExpense, string category, DateTime? date = null) =>
            new TestFinancialTransaction(description, amount, isExpense, category, date ?? DateTime.Now);

        public static Event GenerateTestEvent() =>
            new TestUserEvent(Guid.NewGuid(), "Test Event");

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

        // Internal test classes
        private class TestUser : User
        {
            public TestUser(Guid id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        private class TestTransactionCategory : TransactionCategory
        {
            public TestTransactionCategory(string name, string description)
                : base(name, description) { }
        }

        private class TestFinancialTransaction : FinancialTransaction
        {
            public TestFinancialTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
                : base(description, amount, isExpense, category, date) { }
        }

        private class TestUserEvent : UserEvent
        {
            public TestUserEvent(Guid userId, string description)
                : base(userId, description) { }
        }
    }
}
