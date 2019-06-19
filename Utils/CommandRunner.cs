using System;
using System.Windows.Input;

namespace GameLauncher.Utils
{
    public class CommandRunner: ICommand
    {
        private readonly Action<object> _TargetExecuteMethod;
        private readonly Func<bool> _TargetCanExecuteMethod;

        public CommandRunner(Action<object> executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public CommandRunner(Action<object> executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {

            if (_TargetCanExecuteMethod != null)
            {
                return _TargetCanExecuteMethod();
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            _TargetExecuteMethod?.Invoke(parameter);
        }
    }
}
