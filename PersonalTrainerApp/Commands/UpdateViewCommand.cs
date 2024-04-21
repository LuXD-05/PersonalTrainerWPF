using PersonalTrainerApp.ViewModels;
using System;
using System.Windows.Input;

namespace PersonalTrainerApp.Commands
{
    public class UpdateViewCommand : ICommand
    {
        private MainViewModel _viewModel;

        public UpdateViewCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                // Gets the ViewModel's name
                string fullViewModelName = $"{_viewModel.GetType().Namespace}.{parameter.ToString()}ViewModel";

                // Gets the ViewModel's type from the string
                Type viewModelType = Type.GetType(fullViewModelName);

                // If the ViewModel exists
                if (viewModelType != null)
                {
                    // Creates the type instance (same as _viewModel.SelectedViewModel = new [nome]ViewModel())
                    _viewModel.SelectedViewModel = Activator.CreateInstance(viewModelType) as BaseViewModel;
                }
                else
                {
                    // Throws exception
                    throw new ArgumentException($"ViewModel not found: {parameter.ToString()}");
                }
            }
        }
    }
}
