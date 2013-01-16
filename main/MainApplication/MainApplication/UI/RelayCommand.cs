﻿using System;
using System.Windows.Input;

namespace AppDirect.WindowsClient.UI
{    
    ///<summary>
    /// Represents A Command That Can Be Called Directly By The View
    ///</summary>
    public class RelayCommand<T> : ICommand
    {
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
    }
}