// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Diagnostics;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

namespace TEMS.InventoryModel.util
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other
    /// objects by invoking delegates. The default return value for the CanExecute
    /// method is 'true'.
    /// Initially based on https://stackoverflow.com/questions/3531772/binding-button-click-to-a-method
    /// </summary>
    public class RelayCommand : NotifyPropertyChanged, ICommand
    {
        /// <summary>
        /// Only for subclasses which are responsible to ensure _execute is not null!
        /// </summary>
        protected RelayCommand() { }

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        // Note: these are not readonly so set in subclass constructor for non-static implementations
        protected Action<object> _execute;

        protected Predicate<object> _canExecute;

        /// <summary>
        /// Event to fire to notify when CanExecute has changed
        /// </summary>
        public event EventHandler CanExecuteChanged
#if true
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
#else // alternative that doesn't require CommandManager, see https://github.com/SimpleMvvm/SimpleMvvmToolkit.Express
            ;

        /// <summary>
        /// Method to fire CanExecuteChanged event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

#endif

        /// <summary>
        /// Indicates if requirements are met to allow execution to occur
        /// based on predicate constructed with
        /// </summary>
        /// <param name="parameters">optional parameters to pass to predicate</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameters)
        {
            return _canExecute == null ? true : _canExecute(parameters);
        }

        /// <summary>
        /// Perform the action constructed with, assume CanExcute is true
        /// </summary>
        /// <param name="parameters">optional parameters for the action</param>
        public void Execute(object parameters)
        {
            _execute(parameters);
        }
    }
}