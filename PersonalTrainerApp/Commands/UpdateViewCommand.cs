using PersonalTrainerApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            /* Gestione con switch (classic)
            switch (parameter.ToString())
            {
                case "Home":
                    _viewModel.SelectedViewModel = new HomeViewModel();
                    break;
                case "Login":
                    _viewModel.SelectedViewModel = new LoginViewModel();
                    break;
                default:
                    throw new NotImplementedException();
            }
            */

            // Check se il parameter ha dentro qualcosa
            if (parameter != null)
            {
                // Ottengo il nome del viewModel 
                string fullViewModelName = $"{_viewModel.GetType().Namespace}.{parameter.ToString()}ViewModel";

                // Ottengo il tipo del viewModel dalla stringa
                Type viewModelType = Type.GetType(fullViewModelName);

                // Se il viewModel esiste
                if (viewModelType != null)
                {
                    // Creo l'istanza del tipo (come fare _viewModel.SelectedViewModel = new [nome]ViewModel())
                    _viewModel.SelectedViewModel = Activator.CreateInstance(viewModelType) as BaseViewModel;
                }
                else
                {
                    // altrimenti eccezione viewModel non trovato
                    throw new ArgumentException($"ViewModel not found: {parameter.ToString()}");
                }
            }
        }
    }
}
