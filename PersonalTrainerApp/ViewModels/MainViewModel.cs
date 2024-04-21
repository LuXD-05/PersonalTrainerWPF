using PersonalTrainerApp.Commands;
using PersonalTrainerApp.Models;
using System.Windows.Input;

namespace PersonalTrainerApp.ViewModels
{
    /// <summary>
    /// E' il DataContext della MainWindow e controlla la navigazione tra le varie Views in base al ViewModel corrente
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _selectedViewModel;
        private User _user;

        /// <summary>
        /// ViewModel della View corrente
        /// </summary>
        public BaseViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;

                // Assegno height e width del ViewModel alla Window se il ViewModel e height/width != null, se no 0
                this.Height = _selectedViewModel?.Height ?? 0;
                this.Width = _selectedViewModel?.Width ?? 0;

                // Resetto l'error
                this.Error = string.Empty;

                // Imposto il titolo della finestra
                this.Title = _selectedViewModel.Title;

                // Chiamo l'OnPropertyChanged
                OnPropertyChanged(nameof(SelectedViewModel));
            }
        }

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;

                // Chiamo l'OnPropertyChanged
                OnPropertyChanged(nameof(User));
            }
        }

        /// <summary>
        /// Permette di cambiare la view
        /// </summary>
        public ICommand UpdateViewCommand { get; set; }

        public MainViewModel()
        {
            // Istanzio il Command per cambiare le views
            UpdateViewCommand = new UpdateViewCommand(this);

            // Istanzio il ViewModel selezionato e setto quello di Login come quello di start
            SelectedViewModel = new LoginViewModel();
        }

        // FUNZIONAMENTO --> Qui inizializzo e apporto modifiche con PropertyChanged, Nel BaseViewModel dichiaro la prop, e nel [View]ViewModel imposto
    }
}
