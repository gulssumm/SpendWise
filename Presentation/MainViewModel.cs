using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Logic;
using Data;

namespace Presentation
{
    // MVVM ViewModel - uses only abstract Logic layer API
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ITransactionService _transactionService;
        private FinancialTransaction _selectedTransaction;

        public MainViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
            Transactions = new ObservableCollection<FinancialTransaction>();
            AddTransactionCommand = new RelayCommand(AddTransaction);
            LoadTransactions();
        }

        // Parameterless constructor for testing with dependency injection
        public MainViewModel()
        {
            _transactionService = new TransactionService();
            Transactions = new ObservableCollection<FinancialTransaction>();
            AddTransactionCommand = new RelayCommand(AddTransaction);
        }

        // Master list - for Master-Detail pattern
        public ObservableCollection<FinancialTransaction> Transactions { get; set; }

        // Detail selection - for Master-Detail pattern
        public FinancialTransaction SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                _selectedTransaction = value;
                OnPropertyChanged(nameof(SelectedTransaction));
                OnPropertyChanged(nameof(TransactionDetails));
            }
        }

        // Detail view properties
        public string TransactionDetails =>
            SelectedTransaction != null
                ? $"Description: {SelectedTransaction.Description}\n" +
                  $"Amount: ${SelectedTransaction.Amount}\n" +
                  $"Type: {(SelectedTransaction.IsExpense ? "Expense" : "Income")}\n" +
                  $"Category: {SelectedTransaction.Category}\n" +
                  $"Date: {SelectedTransaction.Date:dd/MM/yyyy}"
                : "Select a transaction to view details";

        public decimal Balance => _transactionService.GetBalance();

        // Commands for MVVM
        public ICommand AddTransactionCommand { get; }

        private void AddTransaction()
        {
            try
            {
                var category = new TestCategory("Food", "Food expenses");
                var user = new TestUser(Guid.NewGuid(), "Test User");

                _transactionService.AddTransaction(
                    "Sample Transaction",
                    50.00m,
                    true,
                    category,
                    user);

                LoadTransactions();
                OnPropertyChanged(nameof(Balance));
            }
            catch (Exception ex)
            {
                // Handle error (in real app, show message to user)
                System.Diagnostics.Debug.WriteLine($"Error adding transaction: {ex.Message}");
            }
        }

        private void LoadTransactions()
        {
            var transactions = _transactionService.GetTransactions();
            Transactions.Clear();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Helper classes for ViewModel testing
        private class TestCategory : TransactionCategory
        {
            public TestCategory(string name, string description) : base(name, description) { }
        }

        private class TestUser : User
        {
            public TestUser(Guid id, string name)
            {
                Id = id;
                Name = name;
            }
        }
    }

    // Simple command implementation for MVVM
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();
    }
}