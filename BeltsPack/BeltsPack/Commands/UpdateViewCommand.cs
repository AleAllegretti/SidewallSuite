using BeltsPack.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace BeltsPack.Commands
{
    public class UpdateViewCommand : ICommand
    {
        private MainViewModel viewModel;

        public UpdateViewCommand(MainViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            
            if(parameter.ToString() == "Input")
            {
                viewModel.SelectedViewModel = new InputViewModel();
            }
            else if(parameter.ToString() == "Output")
            {
                viewModel.SelectedViewModel = new OutputViewModel();
            }
        }
    }
}
