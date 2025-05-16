using System;
using System.Collections.Generic;
using SpendWise.Data;

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
                (i + 1) * 10,
                isExpense: i % 2 == 0,
                category: i % 2 == 0 ? "Food" : "Income",
                DateTime.Now.AddDays(-i)
            ));
        }
        return list;
    }
}
