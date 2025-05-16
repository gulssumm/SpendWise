using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendWise.Logic;
using SpendWise.Logic.Interfaces;
using SpendWise.Data;
using SpendWise.Logic.Services;
using System.Collections.Generic;

[TestClass]
public class TransactionServiceTests
{
    private ITransactionService _transactionService;

    [TestInitialize]
    public void Setup()
    {
        var repository = new InMemoryTransactionRepositoryForTest();
        _transactionService = new TransactionService(repository);
    }

    [TestMethod]
    public void AddTransaction_ShouldStoreCorrectly()
    {
        var user = TestDataGenerator.GenerateTestUser();
        var category = TestDataGenerator.GenerateTestTransactionCategory();

        _transactionService.AddTransaction("Groceries", 50.0m, true, category, user);

        var transactions = _transactionService.GetTransactions();
        Assert.AreEqual(1, transactions.Count);
        Assert.AreEqual("Groceries", transactions[0].Description);
    }

    [TestMethod]
    public void GetBalance_ShouldReturnCorrectAmount()
    {
        var user = TestDataGenerator.GenerateTestUser();
        var category = TestDataGenerator.GenerateTestTransactionCategory();

        _transactionService.AddTransaction("Salary", 1000.0m, false, category, user);
        _transactionService.AddTransaction("Rent", 400.0m, true, category, user);

        var balance = _transactionService.GetBalance();
        Assert.AreEqual(600.0m, balance);
    }

    // In-memory repository implementation for testing
    private class InMemoryTransactionRepositoryForTest : ITransactionRepository
    {
        private readonly List<FinancialTransaction> _transactions = new List<FinancialTransaction>();
        private readonly List<Event> _events = new List<Event>();

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
            _events.Add(e);
        }

        public List<Event> GetEvents()
        {
            return new List<Event>(_events);
        }

        public void SaveTransactions(List<FinancialTransaction> transactions)
        {
            _transactions.Clear();
            _transactions.AddRange(transactions);
        }

        public List<FinancialTransaction> LoadTransactions()
        {
            return new List<FinancialTransaction>(_transactions);
        }
    }
}
