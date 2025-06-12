using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    /// <summary>
    /// Command implementation for MVVM pattern
    /// Handles both synchronous and asynchronous command execution
    /// </summary>
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