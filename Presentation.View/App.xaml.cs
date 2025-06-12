using System.Windows;
using Presentation.ViewModel;
using Presentation.Model;

namespace Presentation.View
{
    /// <summary>
    /// View layer - Pure WPF application
    /// CANNOT reference Data or Logic layers directly!
    /// Only references Presentation.ViewModel and Presentation.Model
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Use factory to create Model with all dependencies
            // This avoids View layer having direct references to Data/Logic
            var model = DependencyFactory.CreateFinancialDataModel();

            // Create ViewModel with Model dependency
            var mainViewModel = new MainViewModel(model);

            // Create main window and set DataContext
            var mainWindow = new MainWindow();
            mainWindow.DataContext = mainViewModel;

            // Show main window
            mainWindow.Show();
        }
    }
}