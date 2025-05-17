using System;
using System.Windows.Input;
using SpendWise.Data.Models;
using SpendWise.Presentation.Commands;
using SpendWise.Presentation.Models;

namespace SpendWise.Presentation.ViewModels
{
    /// <summary>
    /// ViewModel for the transaction details (detail view).
    /// Manages editing or creating individual transactions.
    /// </summary>
    public class TransactionDetailViewModel : ViewModelBase
    {
        private readonly TransactionModel _model;
        private readonly MainViewModel _mainViewModel;
        private FinancialTransactionModel _transaction;
        private string _description;
        private decimal _amount;
        private bool _isExpense;
        private string _category;
        private DateTime _date;
        private bool _isNewTransaction;

        public TransactionDetailViewModel(TransactionModel model, MainViewModel mainViewModel, FinancialTransactionModel transaction = null)
        {
            _model = model;
            _mainViewModel = mainViewModel;
            _isNewTransaction = transaction == null;

            // Initialize commands
            SaveCommand = new RelayCommand(_ => SaveTransaction(), _ => CanSaveTransaction());
            CancelCommand = new RelayCommand(_ => CancelEdit());

            if (transaction != null)
            {
                // Edit existing transaction
                Transaction = transaction;
            }
            else
            {
                // Create new transaction
                _transaction = new FinancialTransactionModel("", 0, true, "", DateTime.Now);
                Description = "";
                Amount = 0;
                IsExpense = true;
                Category = "";
                Date = DateTime.Now;
            }
        }

        /// <summary>
        /// The transaction being edited or created
        /// </summary>
        public FinancialTransactionModel Transaction
        {
            get => _transaction;
            set
            {
                if (SetProperty(ref _transaction, value) && value != null)
                {
                    Description = value.Description;
                    Amount = value.Amount;
                    IsExpense = value.IsExpense;
                    Category = value.Category;
                    Date = value.Date;
                }
            }
        }

        /// <summary>
        /// Transaction description
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Transaction amount
        /// </summary>
        public decimal Amount
        {
            get => _amount;
            set
            {
                SetProperty(ref _amount, value);
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Whether the transaction is an expense (true) or income (false)
        /// </summary>
        public bool IsExpense
        {
            get => _isExpense;
            set => SetProperty(ref _isExpense, value);
        }

        /// <summary>
        /// Transaction category
        /// </summary>
        public string Category
        {
            get => _category;
            set
            {
                SetProperty(ref _category, value);
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Transaction date
        /// </summary>
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        /// <summary>
        /// Command to save the transaction
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Command to cancel editing
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Determines if the transaction can be saved
        /// </summary>
        private bool CanSaveTransaction()
        {
            return !string.IsNullOrEmpty(Description) &&
                   Amount > 0 &&
                   !string.IsNullOrEmpty(Category);
        }

        /// <summary>
        /// Saves the current transaction
        /// </summary>
        private void SaveTransaction()
        {
            // Update transaction with the current values
            _transaction.Description = Description;
            _transaction.Amount = Amount;
            _transaction.IsExpense = IsExpense;
            _transaction.Category = Category;
            _transaction.Date = Date;

            // If this is a new transaction, add it
            if (_isNewTransaction)
            {
                // For simplicity, we're assuming we have a default user and category
                // In a real app, you'd have user management and category selection
                var user = new UserModel { Id = Guid.NewGuid(), Name = "Current User" };
                var category = new TransactionCategoryModel(Category, "");

                _model.AddTransaction(Description, Amount, IsExpense, category, user);
            }
            else
            {
                // For existing transactions, save changes
                _model.SaveTransactions();
            }

            // Refresh the transaction list
            _mainViewModel.RefreshTransactions();

            // Clear the detail view
            _mainViewModel.ClearTransactionSelection();
        }

        /// <summary>
        /// Cancels editing the current transaction
        /// </summary>
        private void CancelEdit()
        {
            // Just clear the selection
            _mainViewModel.ClearTransactionSelection();
        }
    }
}