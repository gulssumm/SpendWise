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
    public class TransactionDetailViewModelTests
    {
        private MockTransactionModel _mockTransactionModel;
        private MockMainViewModel _mockMainViewModel;
        private TransactionDetailViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            // Create mocks
            _mockTransactionModel = new MockTransactionModel();
            _mockMainViewModel = new MockMainViewModel(_mockTransactionModel);
        }

        [TestMethod]
        public void Constructor_WithExistingTransaction_ShouldInitializePropertiesFromTransaction()
        {
            // Arrange
            var transaction = TestDataGenerator.GenerateTransactionModel(
                "Existing Transaction", 150.5m, true, "Test Category", DateTime.Now);

            // Act
            _viewModel = new TransactionDetailViewModel(_mockTransactionModel, _mockMainViewModel, transaction);

            // Assert
            Assert.AreEqual("Existing Transaction", _viewModel.Description);
            Assert.AreEqual(150.5m, _viewModel.Amount);
            Assert.IsTrue(_viewModel.IsExpense);
            Assert.AreEqual("Test Category", _viewModel.Category);
        }

        [TestMethod]
        public void Constructor_WithNoTransaction_ShouldInitializeWithDefaultValues()
        {
            // Act
            _viewModel = new TransactionDetailViewModel(_mockTransactionModel, _mockMainViewModel);

            // Assert
            Assert.AreEqual(string.Empty, _viewModel.Description);
            Assert.AreEqual(0m, _viewModel.Amount);
            Assert.IsTrue(_viewModel.IsExpense);
            Assert.AreEqual(string.Empty, _viewModel.Category);
        }

        [TestMethod]
        public void SaveCommand_WithValidData_ShouldBeEnabled()
        {
            // Arrange
            _viewModel = new TransactionDetailViewModel(_mockTransactionModel, _mockMainViewModel);
            _viewModel.Description = "Valid Description";
            _viewModel.Amount = 100m;
            _viewModel.Category = "Food";

            // Act & Assert
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [TestMethod]
        public void SaveCommand_WithInvalidData_ShouldBeDisabled()
        {
            // Arrange
            _viewModel = new TransactionDetailViewModel(_mockTransactionModel, _mockMainViewModel);

            // Missing description
            _viewModel.Description = "";
            _viewModel.Amount = 100m;
            _viewModel.Category = "Food";
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));

            // Missing category
            _viewModel.Description = "Test";
            _viewModel.Amount = 100m;
            _viewModel.Category = "";
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));

            // Invalid amount
            _viewModel.Description = "Test";
            _viewModel.Amount = 0m;
            _viewModel.Category = "Food";
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [TestMethod]
        public void SaveCommand_ForNewTransaction_ShouldAddTransaction()
        {
            // Arrange
            FinancialTransactionModel addedTransaction = null;
            _mockTransactionModel.AddTransactionAction = (desc, amt, isExp, cat, user) =>
            {
                addedTransaction = new FinancialTransactionModel(desc, amt, isExp, cat.Name, DateTime.Now);
            };

            _viewModel = new TransactionDetailViewModel(_mockTransactionModel, _mockMainViewModel);
            _viewModel.Description = "New Transaction";
            _viewModel.Amount = 75.5m;
            _viewModel.IsExpense = false;
            _viewModel.Category = "Income";

            // Act
            _viewModel.SaveCommand.Execute(null);

            // Assert
            Assert.IsNotNull(addedTransaction);
            Assert.AreEqual("New Transaction", addedTransaction.Description);
            Assert.AreEqual(75.5m, addedTransaction.Amount);
            Assert.IsFalse(addedTransaction.IsExpense);
            Assert.AreEqual("Income", addedTransaction.Category);
        }

        [TestMethod]
        public void SaveCommand_ShouldCallRefreshTransactions()
        {
            // Arrange
            bool refreshTransactionsCalled = false;
            _mockMainViewModel.RefreshTransactionsAction = () => refreshTransactionsCalled = true;

            _viewModel = new TransactionDetailViewModel(_mockTransactionModel, _mockMainViewModel);
            _viewModel.Description = "Test Transaction";
            _viewModel.Amount = 100m;
            _viewModel.Category = "Test";

            // Act
            _viewModel.SaveCommand.Execute(null);

            // Assert
            Assert.IsTrue(refreshTransactionsCalled);
        }

        [TestMethod]
        public void CancelCommand_ShouldCallClearTransactionSelection()
        {
            // Arrange
            bool clearTransactionSelectionCalled = false;
            _mockMainViewModel.ClearTransactionSelectionAction = () => clearTransactionSelectionCalled = true;

            _viewModel = new TransactionDetailViewModel(_mockTransactionModel, _mockMainViewModel);

            // Act
            _viewModel.CancelCommand.Execute(null);

            // Assert
            Assert.IsTrue(clearTransactionSelectionCalled);
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
            // Corrected to match your actual method signature
            public Action<string, decimal, bool, TransactionCategoryModel, UserModel> AddTransactionAction { get; set; }
            public Action SaveTransactionsAction { get; set; }

            public MockTransactionModel() : base(new MockTransactionService())
            {
                // Pass a mock service instead of null
            }

            // Updated to match your actual signature
            public override void AddTransaction(string description, decimal amount, bool isExpense,
                              TransactionCategoryModel category, UserModel user)
            {
                AddTransactionAction?.Invoke(description, amount, isExpense, category, user);
            }

            public override void SaveTransactions()
            {
                SaveTransactionsAction?.Invoke();
            }

            public override List<FinancialTransactionModel> GetAllTransactions()
            {
                return new List<FinancialTransactionModel>();
            }
        }

        // Mock implementation of MainViewModel for testing
        private class MockMainViewModel : MainViewModel
        {
            public Action RefreshTransactionsAction { get; set; }
            public Action ClearTransactionSelectionAction { get; set; }

            public MockMainViewModel(TransactionModel model) : base(model)
            {
                // Pass the mocked model instead of null
            }

            public override void RefreshTransactions()
            {
                RefreshTransactionsAction?.Invoke();
            }

            public override void ClearTransactionSelection()
            {
                ClearTransactionSelectionAction?.Invoke();
            }
        }
    }
}