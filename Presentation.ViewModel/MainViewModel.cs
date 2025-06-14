using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Presentation.Model;

namespace Presentation.ViewModel
{
    /// <summary>
    /// ViewModel layer - UI logic and data binding
    /// Only references Presentation.Model, cannot access Logic or Data directly
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FinancialDataModel _model;
        private object _selectedTransaction; // Using object to avoid Data layer reference
        private string _newDescription = "";
        private decimal _newAmount;
        private bool _newIsExpense = true;
        private string _newCategory = "";
        private DateTime _newDate = DateTime.Today;

        // Constructor for dependency injection
        public MainViewModel(FinancialDataModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));

            // Collections using object type to avoid Data layer references
            Transactions = new ObservableCollection<object>();
            Users = new ObservableCollection<object>();
            Categories = new ObservableCollection<object>();

            // Initialize commands
            AddTransactionCommand = new RelayCommand(async () => await AddTransactionAsync(), CanAddTransaction);
            DeleteTransactionCommand = new RelayCommand(async () => await DeleteTransactionAsync(), () => SelectedTransaction != null);
            LoadDataCommand = new RelayCommand(async () => await LoadDataAsync());

            // Using Task.Run to avoid blocking the constructor while still initiating the load
            _ = Task.Run(async () =>
            {
                try
                {
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in initial data load: {ex.Message}");
                }
            });
        }

        // Master-Detail pattern - Collections
        public ObservableCollection<object> Transactions { get; }
        public ObservableCollection<object> Users { get; }
        public ObservableCollection<object> Categories { get; }

        // Selected transaction for detail view (Master-Detail pattern)
        public object SelectedTransaction
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

        // Calculated properties for UI display
        public decimal TotalBalance
        {
            get
            {
                if (Transactions == null || !Transactions.Any()) return 0;

                decimal balance = 0;
                foreach (dynamic transaction in Transactions)
                {
                    balance += transaction.IsExpense ? -transaction.Amount : transaction.Amount;
                }
                return balance;
            }
        }

        public decimal TotalExpenses
        {
            get
            {
                if (Transactions == null || !Transactions.Any()) return 0;

                decimal expenses = 0;
                foreach (dynamic transaction in Transactions)
                {
                    if (transaction.IsExpense)
                        expenses += transaction.Amount;
                }
                return expenses;
            }
        }

        public decimal TotalIncome
        {
            get
            {
                if (Transactions == null || !Transactions.Any()) return 0;

                decimal income = 0;
                foreach (dynamic transaction in Transactions)
                {
                    if (!transaction.IsExpense)
                        income += transaction.Amount;
                }
                return income;
            }
        }

        // Methods
        private async Task LoadDataAsync()
        {
            try
            {
                // Load transactions through Model layer
                var transactions = await _model.GetTransactionsAsync();
                Transactions.Clear();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }

                var users = await _model.GetUsersAsync();
                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }

                var categories = await _model.GetCategoriesAsync();
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
                // Add transaction through Model layer (Model creates the concrete object)
                var newTransaction = await _model.AddTransactionAsync(NewDescription, NewAmount, NewIsExpense, NewCategory, NewDate);

                // Add to UI collection
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
                // Get ID using dynamic to avoid casting to specific Data types
                dynamic transaction = SelectedTransaction;
                int id = transaction.Id;

                await _model.DeleteTransactionAsync(id);
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

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task WaitForInitializationAsync()
        {
            // Give some time for the initial load to complete
            // Tests can call this method to ensure data is loaded
            await Task.Delay(50);

            // If collections are still empty, try loading again
            if (!Transactions.Any() && !Users.Any() && !Categories.Any())
            {
                await LoadDataAsync();
            }
        }
    }
}