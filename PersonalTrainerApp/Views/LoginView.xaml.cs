using PersonalTrainerApp.Models;
using PersonalTrainerApp.Models.Controllers;
using PersonalTrainerApp.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTrainerApp.Views
{
    /// <summary>
    /// View finestra di login
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();

            // Imposto il DataContext al DataContext della MainWindow
            DataContext = Application.Current.MainWindow.DataContext;
        }

        /// <summary>
        /// Controlla se un utente è registrato e tenta il login
        /// </summary>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Ottengo gli utenti registrati
            var users = FileManager.GetUsers();

            // Se l'utente è gia registrato
            if (users.Any(user => user.Username == tbUsername.Text && user.Password == pbPassword.Password))
            {
                // Setto l'utente nel datacontext
                (this.DataContext as MainViewModel).User = users.Single(x => x.Username.Equals(tbUsername.Text) && x.Password.Equals(pbPassword.Password));

                // Se posso cambiare la view, la cambio con Home
                if ((this.DataContext as MainViewModel).UpdateViewCommand.CanExecute("Home"))
                    (this.DataContext as MainViewModel).UpdateViewCommand.Execute("Home");
            }
            else
            {
                // Username o Password errati
                (this.DataContext as MainViewModel).Error = "Username o password errati";
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
        /// Chiude l'applicazione
        /// </summary>
        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
