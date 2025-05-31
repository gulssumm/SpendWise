using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Logic;
using Data;

namespace Presentation
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ITransactionService _transactionService;
        private FinancialTransaction? _selectedTransaction;
        private bool _isLoading;

        public MainViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
            Transactions = new ObservableCollection<FinancialTransaction>();
            InitializeCommands();
            _ = LoadTransactionsAsync();
        }

        public MainViewModel()
        {
            _transactionService = new TransactionService();
            Transactions = new ObservableCollection<FinancialTransaction>();
            InitializeCommands();
        }

        public ObservableCollection<FinancialTransaction> Transactions { get; set; }

        public FinancialTransaction? SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                _selectedTransaction = value;
                OnPropertyChanged(nameof(SelectedTransaction));
                OnPropertyChanged(nameof(TransactionDetails));
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanDelete));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public bool CanEdit => SelectedTransaction != null && !IsLoading;
        public bool CanDelete => SelectedTransaction != null && !IsLoading;

        public string TransactionDetails =>
            SelectedTransaction != null
                ? $"Description: {SelectedTransaction.Description}\n" +
                  $"Amount: ${SelectedTransaction.Amount}\n" +
                  $"Type: {(SelectedTransaction.IsExpense ? "Expense" : "Income")}\n" +
                  $"Category: {SelectedTransaction.Category}\n" +
                  $"Date: {SelectedTransaction.Date:dd/MM/yyyy}"
                : "Select a transaction to view details";

        public decimal Balance => _transactionService.GetBalance();

        public ICommand AddTransactionCommand { get; private set; } = null!;
        public ICommand EditTransactionCommand { get; private set; } = null!;
        public ICommand DeleteTransactionCommand { get; private set; } = null!;
        public ICommand SaveTransactionsCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            AddTransactionCommand = new AsyncRelayCommand(AddTransactionAsync);
            EditTransactionCommand = new AsyncRelayCommand(EditTransactionAsync, () => CanEdit);
            DeleteTransactionCommand = new AsyncRelayCommand(DeleteTransactionAsync, () => CanDelete);
            SaveTransactionsCommand = new AsyncRelayCommand(SaveTransactionsAsync);
            RefreshCommand = new AsyncRelayCommand(LoadTransactionsAsync);
        }

        private async Task AddTransactionAsync()
        {
            try
            {
                IsLoading = true;
                var category = new TestCategory("Food", "Food expenses");
                var user = new TestUser(Guid.NewGuid(), "Test User");

                await _transactionService.AddTransactionAsync(
                    "Sample Transaction",
                    50.00m,
                    true,
                    category,
                    user);

                await LoadTransactionsAsync();
                OnPropertyChanged(nameof(Balance));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding transaction: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task EditTransactionAsync()
        {
            if (SelectedTransaction == null) return;

            try
            {
                IsLoading = true;
                await _transactionService.UpdateTransactionAsync(
                    SelectedTransaction.Id,
                    SelectedTransaction.Description + " (Edited)",
                    SelectedTransaction.Amount,
                    SelectedTransaction.IsExpense,
                    SelectedTransaction.Category);

                await LoadTransactionsAsync();
                OnPropertyChanged(nameof(Balance));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error editing transaction: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteTransactionAsync()
        {
            if (SelectedTransaction == null) return;

            try
            {
                IsLoading = true;
                await _transactionService.DeleteTransactionAsync(SelectedTransaction.Id);
                await LoadTransactionsAsync();
                OnPropertyChanged(nameof(Balance));
                SelectedTransaction = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting transaction: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveTransactionsAsync()
        {
            try
            {
                IsLoading = true;
                await _transactionService.SaveTransactionsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving transactions: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadTransactionsAsync()
        {
            try
            {
                IsLoading = true;
                var transactions = await _transactionService.GetTransactionsAsync();
                Transactions.Clear();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading transactions: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);

        public async void Execute(object? parameter)
        {
            if (_isExecuting) return;

            _isExecuting = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            try
            {
                await _execute();
            }
            finally
            {
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}