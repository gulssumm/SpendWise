using System;
using System.Windows;
using SpendWise.Data.Models;
using SpendWise.Logic.Interfaces;
using SpendWise.Logic.Services;
using SpendWise.Presentation.Models;
using SpendWise.Presentation.ViewModels;

namespace SpendWise.Presentation.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupViewModel();
        }

        /// <summary>
        /// Sets up the ViewModel as DataContext for the window
        /// </summary>
        private void SetupViewModel()
        {
            try
            {
                // Create repository
                ITransactionRepository repository = new TransactionRepository();

                // Create service with repository
                ITransactionService service = new TransactionService(repository);

                // Create model with service
                TransactionModel model = new TransactionModel(service);

                // Create main ViewModel
                MainViewModel viewModel = new MainViewModel(model);

                // Set as DataContext
                this.DataContext = viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}",
                                "Initialization Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}