using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpendWise.Presentation.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels that implements INotifyPropertyChanged.
    /// Provides property change notification support for data binding.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Event that is raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. 
        /// If not specified, defaults to the calling property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets a property's backing field to the provided value and raises PropertyChanged if the value changed.
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="storage">Reference to the backing field</param>
        /// <param name="value">New value for the property</param>
        /// <param name="propertyName">Name of the property (auto-populated by compiler)</param>
        /// <returns>True if the value was changed, false if the new value equals the old value</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}