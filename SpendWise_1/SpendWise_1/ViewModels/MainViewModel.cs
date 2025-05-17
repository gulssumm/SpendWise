using SpendWise.Data.Models;
using SpendWise.Presentation.Models;

namespace SpendWise.Presentation.ViewModels
{
    /// <summary>
    /// Main ViewModel that coordinates interaction between master and detail views.
    /// Acts as the primary DataContext for the main window.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private TransactionListViewModel _transactionListViewModel;
        private TransactionDetailViewModel _detailViewModel;
        private readonly TransactionModel _transactionModel;

        public MainViewModel(TransactionModel transactionModel)
        {
            _transactionModel = transactionModel;

            // Create the transaction list view model
            TransactionListViewModel = new TransactionListViewModel(transactionModel, this);
        }

        /// <summary>
        /// ViewModel for the transaction list (master view)
        /// </summary>
        public TransactionListViewModel TransactionListViewModel
        {
            get => _transactionListViewModel;
            set => SetProperty(ref _transactionListViewModel, value);
        }

        /// <summary>
        /// ViewModel for the transaction details (detail view)
        /// </summary>
        public TransactionDetailViewModel DetailViewModel
        {
            get => _detailViewModel;
            set => SetProperty(ref _detailViewModel, value);
        }

        /// <summary>
        /// Called when a transaction is selected in the list
        /// </summary>
        public void TransactionSelected(FinancialTransactionModel transaction)
        {
            DetailViewModel = new TransactionDetailViewModel(_transactionModel, this, transaction);
        }

        /// <summary>
        /// Called to create a new transaction
        /// </summary>
        public void CreateNewTransaction()
        {
            DetailViewModel = new TransactionDetailViewModel(_transactionModel, this);
        }

        /// <summary>
        /// Refreshes the transaction list data
        /// </summary>
        public void RefreshTransactions()
        {
            TransactionListViewModel.LoadTransactions();
        }

        /// <summary>
        /// Clears the currently selected transaction
        /// </summary>
        public void ClearTransactionSelection()
        {
            DetailViewModel = null;
            TransactionListViewModel.ClearSelection();
        }
    }
}