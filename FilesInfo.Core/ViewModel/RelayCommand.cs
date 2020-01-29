using System;
using System.Windows.Input;

namespace FilesInfo.Core
{
    public class RelayCommand : ICommand
    {
        private Action action;
        public event EventHandler CanExecuteChanged =(sender,e)=> { };

        #region Constructor
        public RelayCommand(Action action)
        {
            this.action = action;
        }
        #endregion

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action();
        }
    }
}
