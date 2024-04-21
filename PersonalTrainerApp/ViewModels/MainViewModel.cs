using PersonalTrainerApp.Commands;
using PersonalTrainerApp.Models;
using System.Windows.Input;

namespace PersonalTrainerApp.ViewModels
{
    /// <summary>
    /// It's MainWindow's datacontext and controls the navigation between views based on the current ViewModel
    /// Here init and apply changes with the PropertyChanged, in BaseViewModel the property is declared
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _selectedViewModel;
        private User _user;

        /// <summary>
        /// View's current ViewModel
        /// </summary>
        public BaseViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;

                // Sets ViewModel's height and width to the Window (if ViewModel's height/width != null, else 0)
                this.Height = _selectedViewModel?.Height ?? 0;
                this.Width = _selectedViewModel?.Width ?? 0;

                // Resets error
                this.Error = string.Empty;

                // Sets window title
                this.Title = _selectedViewModel.Title;

                // Calls OnPropertyChanged
                OnPropertyChanged(nameof(SelectedViewModel));
            }
        }

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        /// <summary>
        /// Allows to change view
        /// </summary>
        public ICommand UpdateViewCommand { get; set; }

        public MainViewModel()
        {
            // Instantiates the command to change views
            UpdateViewCommand = new UpdateViewCommand(this);

            // Instantiates the selected ViewModel and sets Login ViewModel as start
            SelectedViewModel = new LoginViewModel();
        }
    }
}
