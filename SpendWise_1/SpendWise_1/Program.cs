using System;
using System.Windows;
using SpendWise.Presentation.Views;

namespace SpendWise.Presentation
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // The STAThread attribute is required for WPF applications

            try
            {
                // Create a new WPF application
                var application = new Application();

                // Add startup handler to create and show the main window
                application.Startup += (s, e) =>
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                };

                // Start the WPF application
                application.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting application: {ex.Message}\n\n{ex.StackTrace}",
                                "Startup Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}