using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendWise.Data;
using System;
using System.Collections.Generic;

[TestClass]
public class TransactionRepositoryTests
{
    [TestMethod]
    public void AddTransaction_ShouldStoreInMemory()
    {
        // Arrange
        var repo = new TransactionRepository();  // Using the actual repository class
        var transaction = TestDataGenerator.GenerateTransaction("Book", 20.0m, true, "Education");

        // Act
        repo.AddTransaction(transaction);
        var stored = repo.GetTransactions();

        // Assert
        Assert.AreEqual(1, stored.Count);  // Ensure one transaction is stored
        Assert.AreEqual("Book", stored[0].Description);  // Ensure transaction description matches
        Assert.AreEqual(20.0m, stored[0].Amount);  // Ensure transaction amount matches
    }

    [TestMethod]
    public void AddEvent_ShouldStoreCorrectly()
    {
        // Arrange
        var repo = new TransactionRepository();
        var e = TestDataGenerator.GenerateTestEvent();  // Assuming this creates an Event object

        // Act
        repo.AddEvent(e);
        var storedEvents = repo.GetEvents();

        // Assert
        Assert.AreEqual(1, storedEvents.Count);  // Ensure one event is stored
        Assert.AreEqual("Test Event", storedEvents[0].Description);  // Ensure event description matches
    }

    [TestMethod]
    public void SaveTransactions_ShouldSaveAllTransactions()
    {
        // Arrange
        var repo = new TransactionRepository();
        var transaction1 = TestDataGenerator.GenerateTransaction("Book", 20.0m, true, "Education");
        var transaction2 = TestDataGenerator.GenerateTransaction("Laptop", 1000.0m, true, "Electronics");
        repo.AddTransaction(transaction1);
        repo.AddTransaction(transaction2);

        // Act
        var newTransactions = new List<FinancialTransaction>
        {
            TestDataGenerator.GenerateTransaction("Phone", 500.0m, true, "Electronics")
        };
        repo.SaveTransactions(newTransactions);  // This should clear and add the new transactions

        var stored = repo.GetTransactions();

        // Assert
        Assert.AreEqual(1, stored.Count);  // Only one transaction should be stored
        Assert.AreEqual("Phone", stored[0].Description);  // Ensure that only the "Phone" transaction remains
    }

    [TestMethod]
    public void LoadTransactions_ShouldLoadStoredTransactions()
    {
        // Arrange
        var repo = new TransactionRepository();
        var transaction1 = TestDataGenerator.GenerateTransaction("Book", 20.0m, true, "Education");
        var transaction2 = TestDataGenerator.GenerateTransaction("Laptop", 1000.0m, true, "Electronics");
        repo.AddTransaction(transaction1);
        repo.AddTransaction(transaction2);

        // Act
        var stored = repo.LoadTransactions();

        // Assert
        Assert.AreEqual(2, stored.Count);  // Ensure both transactions are loaded
        Assert.AreEqual("Book", stored[0].Description);  // Ensure the description of the first transaction matches
        Assert.AreEqual("Laptop", stored[1].Description);  // Ensure the description of the second transaction matches
    }
}
