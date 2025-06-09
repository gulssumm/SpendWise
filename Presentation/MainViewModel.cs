using System;
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
        private IFinancialTransaction _selectedTransaction;
        private string _newDescription = "";
        private decimal _newAmount;
        private bool _newIsExpense = true;
        private string _newCategory = "";
        private DateTime _newDate = DateTime.Today;

        // Constructor for dependency injection
        public MainViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));

            // Initialize collections using interface types
            Transactions = new ObservableCollection<IFinancialTransaction>();
            Users = new ObservableCollection<IUser>();
            Categories = new ObservableCollection<ITransactionCategory>();

            // Initialize commands
            AddTransactionCommand = new RelayCommand(async () => await AddTransactionAsync(), CanAddTransaction);
            DeleteTransactionCommand = new RelayCommand(async () => await DeleteTransactionAsync(), () => SelectedTransaction != null);
            LoadDataCommand = new RelayCommand(async () => await LoadDataAsync());

            // Load initial data
            _ = LoadDataAsync();
        }

        // Master-Detail pattern
        public ObservableCollection<IFinancialTransaction> Transactions { get; }
        public ObservableCollection<IUser> Users { get; }
        public ObservableCollection<ITransactionCategory> Categories { get; }

        // Selected transaction for detail view
        public IFinancialTransaction SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                _selectedTransaction = value;
                OnPropertyChanged(nameof(SelectedTransaction));
                ((RelayCommand)DeleteTransactionCommand).RaiseCanExecuteChanged();
            }
        }

        // Properties for adding new transactions
        public string NewDescription
        {
            get => _newDescription;
            set
            {
                _newDescription = value;
                OnPropertyChanged(nameof(NewDescription));
                ((RelayCommand)AddTransactionCommand).RaiseCanExecuteChanged();
            }
        }

        public decimal NewAmount
        {
            get => _newAmount;
            set
            {
                _newAmount = value;
                OnPropertyChanged(nameof(NewAmount));
                ((RelayCommand)AddTransactionCommand).RaiseCanExecuteChanged();
            }
        }

        public bool NewIsExpense
        {
            get => _newIsExpense;
            set
            {
                _newIsExpense = value;
                OnPropertyChanged(nameof(NewIsExpense));
            }
        }

        public string NewCategory
        {
            get => _newCategory;
            set
            {
                _newCategory = value;
                OnPropertyChanged(nameof(NewCategory));
                ((RelayCommand)AddTransactionCommand).RaiseCanExecuteChanged();
            }
        }

        public DateTime NewDate
        {
            get => _newDate;
            set
            {
                _newDate = value;
                OnPropertyChanged(nameof(NewDate));
            }
        }

        // Commands
        public ICommand AddTransactionCommand { get; }
        public ICommand DeleteTransactionCommand { get; }
        public ICommand LoadDataCommand { get; }

        // Calculated properties
        public decimal TotalBalance => Transactions?.Sum(t => t.IsExpense ? -t.Amount : t.Amount) ?? 0;
        public decimal TotalExpenses => Transactions?.Where(t => t.IsExpense).Sum(t => t.Amount) ?? 0;
        public decimal TotalIncome => Transactions?.Where(t => !t.IsExpense).Sum(t => t.Amount) ?? 0;

        // Methods
        private async Task LoadDataAsync()
        {
            try
            {
                // Load transactions using abstract Logic layer API
                var transactions = await _transactionService.GetTransactionsAsync();
                Transactions.Clear();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }

                // Load users
                var users = await _transactionService.GetUsersAsync();
                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }

                // Load categories
                var categories = await _transactionService.GetCategoriesAsync();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                OnPropertyChanged(nameof(TotalBalance));
                OnPropertyChanged(nameof(TotalExpenses));
                OnPropertyChanged(nameof(TotalIncome));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private async Task AddTransactionAsync()
        {
            try
            {
                // New transaction using concrete type
                var newTransaction = new FinancialTransaction(NewDescription, NewAmount, NewIsExpense, NewCategory, NewDate);

                await _transactionService.AddTransactionAsync(newTransaction);

                // Add to collection
                Transactions.Add(newTransaction);

                // Clear form
                ClearForm();

                // Update calculated properties
                OnPropertyChanged(nameof(TotalBalance));
                OnPropertyChanged(nameof(TotalExpenses));
                OnPropertyChanged(nameof(TotalIncome));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding transaction: {ex.Message}");
            }
        }

        private async Task DeleteTransactionAsync()
        {
            if (SelectedTransaction == null) return;

            try
            {
                await _transactionService.DeleteTransactionAsync(SelectedTransaction.Id);
                Transactions.Remove(SelectedTransaction);
                SelectedTransaction = null;

                OnPropertyChanged(nameof(TotalBalance));
                OnPropertyChanged(nameof(TotalExpenses));
                OnPropertyChanged(nameof(TotalIncome));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting transaction: {ex.Message}");
            }
        }

        private bool CanAddTransaction()
        {
            return !string.IsNullOrWhiteSpace(NewDescription) &&
                   NewAmount > 0 &&
                   !string.IsNullOrWhiteSpace(NewCategory);
        }

        private void ClearForm()
        {
            NewDescription = "";
            NewAmount = 0;
            NewIsExpense = true;
            NewCategory = "";
            NewDate = DateTime.Today;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _executeAsync;
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public RelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public async void Execute(object parameter)
        {
            if (_executeAsync != null)
                await _executeAsync();
            else
                _execute?.Invoke();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}