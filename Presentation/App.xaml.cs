using System.Configuration;
using System.Windows;
using Data;
using Logic;

namespace Presentation
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Get connection string from App.config
            var connectionString = ConfigurationManager.ConnectionStrings["FinancialAppConnection"]?.ConnectionString
                ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FinancialApp;Integrated Security=True";

            // Create dependencies
            ITransactionRepository repository = new TransactionRepository(connectionString);
            ITransactionService transactionService = new TransactionService(repository);

            // Create main window with dependencies
            var mainWindow = new MainWindow();
            var mainViewModel = new MainViewModel(transactionService);
            mainWindow.DataContext = mainViewModel;

            // Show main window
            mainWindow.Show();
        }
    }
}