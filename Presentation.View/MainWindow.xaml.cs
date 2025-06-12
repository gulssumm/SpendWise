using System.Windows;

namespace Presentation.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Pure View - no business logic, only UI code
    /// DataContext is set by App.xaml.cs following MVVM pattern
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // DataContext will be set by App.xaml.cs
            // No code-behind logic - pure MVVM pattern
        }
    }
}