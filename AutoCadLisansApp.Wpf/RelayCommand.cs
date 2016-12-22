using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaterialDesignDemo
{
    public class RelayCommand : ICommand
    {
        #region Fields WW
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        private Action<object> showDialog;
        private object p;
        private Action<int> showDialog1;
        #endregion // Fields 
        #region Constructors 
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; _canExecute = canExecute;
        }

        public RelayCommand(Action<object> showDialog, object p)
        {
            this.showDialog = showDialog;
            this.p = p;
        }

        public RelayCommand(Action<int> showDialog1, Func<object, bool> p)
        {
            this.showDialog1 = showDialog1;
            this.p = p;
        }
        #endregion // Constructors 
        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter) { _execute(parameter); }
        #endregion // ICommand Members 
    }
}
