using PersonalTrainerApp.Models.Controllers;
using PersonalTrainerApp.Models;
using PersonalTrainerApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace PersonalTrainerApp.Views
{
    /// <summary>
    /// View finestra di register
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();

            // Imposto il DataContext al DataContext della MainWindow
            DataContext = Application.Current.MainWindow.DataContext;
        }

        /// <summary>
        /// Tenta di registrare l'utente se non registrato
        /// </summary>
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Ottengo l'utente registrato
            var user = new User(tbUsername.Text, pbPassword.Password);

            // Ottengo gli utenti registrati
            var users = FileManager.GetUsers();

            // Se l'utente non è registrato (se il suo username non c'è)
            if (!users.Any(x => x.Username == user.Username))
            {
                // Aggiungo l'utente alla lista
                users.Add(user);

                // Riaggiungo gli utenti aggiornati a database
                FileManager.UpdateDb(users);

                // Setto l'utente nel datacontext
                (this.DataContext as MainViewModel).User = user;

                // Se posso cambiare la view, la cambio con Home
                if ((this.DataContext as MainViewModel).UpdateViewCommand.CanExecute("Home"))
                    (this.DataContext as MainViewModel).UpdateViewCommand.Execute("Home");
            }
            else
            {
                // Utente già presente
                (this.DataContext as LoginViewModel).Error = "Utente già registrato";
            }
        }

        private void tbUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as MainViewModel).Error != string.Empty)
                (this.DataContext as MainViewModel).Error = string.Empty;
        }

        private void pbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as MainViewModel).Error != string.Empty)
                (this.DataContext as MainViewModel).Error = string.Empty;
        }

        /// <summary>
        /// Gestisce l'evento MouseDown del controllo
        /// </summary>
        private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Se cliccato il mouseButton sinistro, esegui il DragMove della MainWindow
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                Application.Current.MainWindow.DragMove();
        }

        /// <summary>
        /// Chude l'applicazione
        /// </summary>
        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
