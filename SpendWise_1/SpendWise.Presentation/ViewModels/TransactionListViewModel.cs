using System.Collections.ObjectModel;
using System.Windows.Input;
using SpendWise.Data.Models;
using SpendWise.Presentation.Commands;
using SpendWise.Presentation.Models;

namespace SpendWise.Presentation.ViewModels
{
    /// <summary>
    /// ViewModel for the transaction list (master view).
    /// Manages the list of transactions, balance, and list-related commands.
    /// </summary>
    public class TransactionListViewModel : ViewModelBase
    {
        private readonly TransactionModel _model;
        private readonly MainViewModel _mainViewModel;
        private ObservableCollection<FinancialTransactionModel> _transactions;
        private FinancialTransactionModel _selectedTransaction;
        private decimal _currentBalance;

        public TransactionListViewModel(TransactionModel model, MainViewModel mainViewModel)
        {
            _model = model;
            _mainViewModel = mainViewModel;

            // Initialize commands
            AddNewTransactionCommand = new RelayCommand(_ => AddNewTransaction());
            RefreshCommand = new RelayCommand(_ => LoadTransactions());

            // Load initial data
            LoadTransactions();
        }

        /// <summary>
        /// Collection of transactions to display in the list
        /// </summary>
        public ObservableCollection<FinancialTransactionModel> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
        }

        /// <summary>
        /// Currently selected transaction
        /// </summary>
        public FinancialTransactionModel SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                if (SetProperty(ref _selectedTransaction, value) && value != null)
                {
                    _mainViewModel.TransactionSelected(value);
                }
            }
        }

        /// <summary>
        /// Current balance (total income - total expenses)
        /// </summary>
        public decimal CurrentBalance
        {
            get => _currentBalance;
            set => SetProperty(ref _currentBalance, value);
        }

        /// <summary>
        /// Command to add a new transaction
        /// </summary>
        public ICommand AddNewTransactionCommand { get; }

        /// <summary>
        /// Command to refresh the transaction list
        /// </summary>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// Loads or reloads the transaction list from the data source
        /// </summary>
        public void LoadTransactions()
        {
            var transactionList = _model.GetAllTransactions();
            Transactions = new ObservableCollection<FinancialTransactionModel>(transactionList);
            CurrentBalance = _model.GetBalance();
        }

        /// <summary>
        /// Initiates the process of adding a new transaction
        /// </summary>
        private void AddNewTransaction()
        {
            _mainViewModel.CreateNewTransaction();
        }

        /// <summary>
        /// Clears the current transaction selection
        /// </summary>
        public void ClearSelection()
        {
            SelectedTransaction = null;
        }
    }
}