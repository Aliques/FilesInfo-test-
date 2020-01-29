using System;
using System.Windows.Input;

namespace FilesInfo.Core
{
    public class RealayParametrizedCommand : ICommand
    {
        private Action<object> action;
        public event EventHandler CanExecuteChanged=(sender,e)=> { };

        public RealayParametrizedCommand(Action<object> action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }
    }
}
