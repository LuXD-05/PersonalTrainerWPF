using PersonalTrainerApp.Models;
using PersonalTrainerApp.Models.Controllers;
using PersonalTrainerApp.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTrainerApp.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();

            // Sets the datacontext to the mainwindow's datacontext
            DataContext = Application.Current.MainWindow.DataContext;
        }

        /// <summary>
        /// Checks if a user is registered and tries to log in
        /// </summary>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Gets the registered users
            var users = FileManager.GetUsers();

            // If the user is already registered
            if (users.Any(user => user.Username == tbUsername.Text && user.Password == pbPassword.Password))
            {
                // Sets the user in the datacontext
                (this.DataContext as MainViewModel).User = users.Single(x => x.Username.Equals(tbUsername.Text) && x.Password.Equals(pbPassword.Password));

                // If the view can be changed, changes it with Home
                if ((this.DataContext as MainViewModel).UpdateViewCommand.CanExecute("Home"))
                    (this.DataContext as MainViewModel).UpdateViewCommand.Execute("Home");
            }
            else
            {
                // Wrong username and password
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
        /// Handles the MouseDown event
        /// </summary>
        private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // If left mouse button clicked, DragMove
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                Application.Current.MainWindow.DragMove();
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
