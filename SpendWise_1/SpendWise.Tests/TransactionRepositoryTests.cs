using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendWise.Data;
using SpendWise.Data.Models;
using System;
using System.Collections.Generic;

[TestClass]
public class TransactionRepositoryTests
{
    [TestMethod]
    public void AddTransaction_ShouldStoreInMemory()
    {
        // Arrange
        var repo = new TransactionRepository();  // Make sure this uses SpendWise.Data.Models.TransactionRepository
        var transaction = TestDataGenerator.GenerateTransactionModel("Book", 20.0m, true, "Education");

        // Act
        repo.AddTransaction(transaction);
        var stored = repo.GetTransactions();

        // Assert
        Assert.AreEqual(1, stored.Count);
        Assert.AreEqual("Book", stored[0].Description);
        Assert.AreEqual(20.0m, stored[0].Amount);
    }

    [TestMethod]
    public void AddEvent_ShouldStoreCorrectly()
    {
        // Arrange
        var repo = new TransactionRepository();
        var e = TestDataGenerator.GenerateTestEventModel();

        // Act
        repo.AddEvent(e);
        var storedEvents = repo.GetEvents();

        // Assert
        Assert.AreEqual(1, storedEvents.Count);
        Assert.AreEqual("Test Event", storedEvents[0].Description);
    }

    [TestMethod]
    public void SaveTransactions_ShouldSaveAllTransactions()
    {
        // Arrange
        var repo = new TransactionRepository();
        var transaction1 = TestDataGenerator.GenerateTransactionModel("Book", 20.0m, true, "Education");
        var transaction2 = TestDataGenerator.GenerateTransactionModel("Laptop", 1000.0m, true, "Electronics");
        repo.AddTransaction(transaction1);
        repo.AddTransaction(transaction2);

        // Act
        var newTransactions = new List<FinancialTransactionModel>
        {
            TestDataGenerator.GenerateTransactionModel("Phone", 500.0m, true, "Electronics")
        };
        repo.SaveTransactions(newTransactions);

        var stored = repo.GetTransactions();

        // Assert
        Assert.AreEqual(1, stored.Count);
        Assert.AreEqual("Phone", stored[0].Description);
    }

    [TestMethod]
    public void LoadTransactions_ShouldLoadStoredTransactions()
    {
        // Arrange
        var repo = new TransactionRepository();
        var transaction1 = TestDataGenerator.GenerateTransactionModel("Book", 20.0m, true, "Education");
        var transaction2 = TestDataGenerator.GenerateTransactionModel("Laptop", 1000.0m, true, "Electronics");
        repo.AddTransaction(transaction1);
        repo.AddTransaction(transaction2);

        // Act
        var stored = repo.LoadTransactions();

        // Assert
        Assert.AreEqual(2, stored.Count);
        Assert.AreEqual("Book", stored[0].Description);
        Assert.AreEqual("Laptop", stored[1].Description);
    }
}