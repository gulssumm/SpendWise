using System.Windows;

namespace Presentation
{
    // View in MVVM pattern
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set up ViewModel with dependency injection
            var viewModel = new MainViewModel();
            DataContext = viewModel;
        }
    }
}