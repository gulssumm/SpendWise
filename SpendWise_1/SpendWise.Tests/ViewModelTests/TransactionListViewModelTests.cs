using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendWise.Data.Models;
using SpendWise.Logic.Interfaces;
using SpendWise.Presentation.Models;
using SpendWise.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestProject1.ViewModelTests
{
    [TestClass]
    public class TransactionListViewModelTests
    {
        private MockTransactionModel _mockTransactionModel;
        private MockMainViewModel _mockMainViewModel;
        private TransactionListViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            // Create mocks
            _mockTransactionModel = new MockTransactionModel();
            _mockMainViewModel = new MockMainViewModel();

            // Create the view model
            _viewModel = new TransactionListViewModel(_mockTransactionModel, _mockMainViewModel);
        }

        [TestMethod]
        public void Constructor_ShouldLoadTransactions()
        {
            // Assert
            Assert.IsNotNull(_viewModel.Transactions);
            Assert.AreEqual(3, _viewModel.Transactions.Count);
        }

        [TestMethod]
        public void SelectedTransaction_WhenSet_ShouldCallMainViewModelTransactionSelected()
        {
            // Arrange
            var transaction = TestDataGenerator.GenerateTransactionModel("Test", 100, true, "Test", DateTime.Now);
            FinancialTransactionModel selectedTransaction = null;
            _mockMainViewModel.TransactionSelectedAction = (t) => selectedTransaction = t;

            // Act
            _viewModel.SelectedTransaction = transaction;

            // Assert
            Assert.AreEqual(transaction, selectedTransaction);
        }

        [TestMethod]
        public void AddNewTransaction_ShouldCallMainViewModelCreateNewTransaction()
        {
            // Arrange
            bool createNewTransactionCalled = false;
            _mockMainViewModel.CreateNewTransactionAction = () => createNewTransactionCalled = true;

            // Act
            _viewModel.AddNewTransactionCommand.Execute(null);

            // Assert
            Assert.IsTrue(createNewTransactionCalled);
        }

        [TestMethod]
        public void RefreshCommand_ShouldReloadTransactions()
        {
            // Arrange
            bool getTransactionsCalled = false;
            _mockTransactionModel.GetAllTransactionsAction = () =>
            {
                getTransactionsCalled = true;
                return TestDataGenerator.GenerateMultipleTransactionModels(5);
            };

            // Act
            _viewModel.RefreshCommand.Execute(null);

            // Assert
            Assert.IsTrue(getTransactionsCalled);
            Assert.AreEqual(5, _viewModel.Transactions.Count);
        }

        [TestMethod]
        public void LoadTransactions_ShouldUpdateCurrentBalance()
        {
            // Arrange
            _mockTransactionModel.GetBalanceValue = 750m;

            // Act
            _viewModel.LoadTransactions();

            // Assert
            Assert.AreEqual(750m, _viewModel.CurrentBalance);
        }

        [TestMethod]
        public void ClearSelection_ShouldSetSelectedTransactionToNull()
        {
            // Arrange
            _viewModel.SelectedTransaction = TestDataGenerator.GenerateTransactionModel("Test", 100, true, "Test", DateTime.Now);

            // Act
            _viewModel.ClearSelection();

            // Assert
            Assert.IsNull(_viewModel.SelectedTransaction);
        }

        // Mock implementation of ITransactionService for testing
        private class MockTransactionService : ITransactionService
        {
            public void AddTransaction(string description, decimal amount, bool isExpense, TransactionCategoryModel category, UserModel user)
            {
                // No implementation needed for these tests
            }

            public decimal GetBalance()
            {
                return 0m;
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
                // No implementation needed
            }

            public void SaveTransactions()
            {
                // No implementation needed
            }
        }

        // Mock implementation of TransactionModel for testing
        private class MockTransactionModel : TransactionModel
        {
            public Func<List<FinancialTransactionModel>> GetAllTransactionsAction { get; set; }
            public decimal GetBalanceValue { get; set; } = 1000m;

            public MockTransactionModel() : base(new MockTransactionService())
            {
                // Pass a mock service instead of null
            }

            public override List<FinancialTransactionModel> GetAllTransactions()
            {
                if (GetAllTransactionsAction != null)
                    return GetAllTransactionsAction();

                return TestDataGenerator.GenerateMultipleTransactionModels(3);
            }

            public override decimal GetBalance()
            {
                return GetBalanceValue;
            }
        }

        // Mock implementation of MainViewModel for testing
        private class MockMainViewModel : MainViewModel
        {
            public Action<FinancialTransactionModel> TransactionSelectedAction { get; set; }
            public Action CreateNewTransactionAction { get; set; }

            public MockMainViewModel() : base(new MockTransactionModel())
            {
                // Pass a mocked model instead of null
            }

            public override void TransactionSelected(FinancialTransactionModel transaction)
            {
                TransactionSelectedAction?.Invoke(transaction);
            }

            public override void CreateNewTransaction()
            {
                CreateNewTransactionAction?.Invoke();
            }
        }
    }
}