using System;
using System.Collections.Generic;
using SpendWise.Data;
using SpendWise.Data.Models;

public static class TestDataGenerator
{
    // User
    public static User GenerateTestUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User"
        };
    }

    public static UserModel GenerateTestUserModel()
    {
        return new UserModel
        {
            Id = Guid.NewGuid(),
            Name = "Test User"
        };
    }

    // Transaction Category
    public static TransactionCategoryModel GenerateTestTransactionCategoryModel()
    {
        return new TransactionCategoryModel("Test Category", "Sample category");
    }

    // Event
    public static SpendWise.Data.Models.UserEventModel GenerateTestEventModel()
    {
        return new SpendWise.Data.Models.UserEventModel(Guid.NewGuid(), "Test Event");
    }

    // Transactions
    public static FinancialTransaction GenerateTransactionEntity(string description, decimal amount, bool isExpense, string category, DateTime? date = null)
    {
        return new FinancialTransaction(description, amount, isExpense, category, date ?? DateTime.Now);
    }

    public static FinancialTransactionModel GenerateTransactionModel(string description, decimal amount, bool isExpense, string category, DateTime? date = null)
    {
        return new FinancialTransactionModel(description, amount, isExpense, category, date ?? DateTime.Now);
    }

    // Multiple transactions
    public static List<FinancialTransaction> GenerateMultipleTransactionEntities(int count)
    {
        var list = new List<FinancialTransaction>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateTransactionEntity(
                $"Transaction {i + 1}",
                (i + 1) * 10,
                isExpense: i % 2 == 0,
                category: i % 2 == 0 ? "Food" : "Income",
                DateTime.Now.AddDays(-i)
            ));
        }
        return list;
    }

    public static List<FinancialTransactionModel> GenerateMultipleTransactionModels(int count)
    {
        var list = new List<FinancialTransactionModel>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateTransactionModel(
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