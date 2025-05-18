using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendWise.Data.Models;
using SpendWise.Logic.Interfaces;
using SpendWise.Presentation.Models;
using SpendWise.Presentation.ViewModels;
using System;
using System.Collections.Generic;

namespace TestProject1.ViewModelTests
{
    [TestClass]
    public class MainViewModelTests
    {
        private MockTransactionModel _mockTransactionModel;
        private MainViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            // Create mock model
            _mockTransactionModel = new MockTransactionModel();

            // Create the view model with the mock
            _viewModel = new MainViewModel(_mockTransactionModel);
        }

        // Your existing test methods...

        // Mock implementation of TransactionModel for testing
        private class MockTransactionService : ITransactionService
        {
            // Implement required interface methods with minimal functionality
            public void AddTransaction(string description, decimal amount, bool isExpense, TransactionCategoryModel category, UserModel user)
            {
                // No-op for tests
            }

            public decimal GetBalance()
            {
                return 0;
            }

            public List<FinancialTransactionModel> GetMonthlyReport(int month, int year)
            {
                return new List<FinancialTransactionModel>();
            }

            public ProcessState GetProcessState()
            {
                return new TransactionProcessState();
            }

            public List<FinancialTransactionModel> GetTransactions()
            {
                return new List<FinancialTransactionModel>();
            }

            public void LoadTransactions()
            {
                // No-op for tests
            }

            public void SaveTransactions()
            {
                // No-op for tests
            }
        }

        private class MockTransactionModel : TransactionModel
        {
            public Action LoadTransactionsAction { get; set; }

            public MockTransactionModel() : base(new MockTransactionService())
            {
                // Pass a mock service to the base constructor instead of null
            }

            public override List<FinancialTransactionModel> GetAllTransactions()
            {
                return TestDataGenerator.GenerateMultipleTransactionModels(3);
            }

            public override decimal GetBalance()
            {
                return 1000m;
            }
        }
    }
}