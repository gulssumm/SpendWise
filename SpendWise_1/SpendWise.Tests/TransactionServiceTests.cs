using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendWise.Logic;
using SpendWise.Logic.Interfaces;
using SpendWise.Data;
using SpendWise.Data.Models;
using SpendWise.Logic.Services;
using System.Collections.Generic;
using System;

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
        var user = TestDataGenerator.GenerateTestUserModel();
        var category = TestDataGenerator.GenerateTestTransactionCategoryModel();

        _transactionService.AddTransaction("Groceries", 50.0m, true, category, user);

        var transactions = _transactionService.GetTransactions();
        Assert.AreEqual(1, transactions.Count);
        Assert.AreEqual("Groceries", transactions[0].Description);
    }

    [TestMethod]
    public void GetBalance_ShouldReturnCorrectAmount()
    {
        var user = TestDataGenerator.GenerateTestUserModel();
        var category = TestDataGenerator.GenerateTestTransactionCategoryModel();

        _transactionService.AddTransaction("Salary", 1000.0m, false, category, user);
        _transactionService.AddTransaction("Rent", 400.0m, true, category, user);

        var balance = _transactionService.GetBalance();
        Assert.AreEqual(600.0m, balance);
    }

    // In-memory repository implementation for testing
    private class InMemoryTransactionRepositoryForTest : ITransactionRepository
    {
        private readonly List<FinancialTransactionModel> _transactions = new List<FinancialTransactionModel>();
        private readonly List<EventModel> _events = new List<EventModel>();

        public void AddTransaction(FinancialTransactionModel transaction)
        {
            _transactions.Add(transaction);
        }

        public List<FinancialTransactionModel> GetTransactions()
        {
            return new List<FinancialTransactionModel>(_transactions);
        }

        public void AddEvent(EventModel e)
        {
            _events.Add(e);
        }

        public List<EventModel> GetEvents()
        {
            return new List<EventModel>(_events);
        }

        public void SaveTransactions(List<FinancialTransactionModel> transactions)
        {
            _transactions.Clear();
            _transactions.AddRange(transactions);
        }

        public List<FinancialTransactionModel> LoadTransactions()
        {
            return new List<FinancialTransactionModel>(_transactions);
        }
    }
}