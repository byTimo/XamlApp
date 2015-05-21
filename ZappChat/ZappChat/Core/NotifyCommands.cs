using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ZappChat.Core
{
    public class ShowWindowCommand : ICommand
    {
        public void Execute(object parameter)
        {
            App.ShowCurrentWindow();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
    public class CloseApplicationCommand : ICommand
    {
        public void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}

