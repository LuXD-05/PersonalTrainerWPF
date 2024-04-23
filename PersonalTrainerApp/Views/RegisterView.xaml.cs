using PersonalTrainerApp.Models.Controllers;
using PersonalTrainerApp.Models;
using PersonalTrainerApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace PersonalTrainerApp.Views
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();

            // Sets the datacontext to the mainwindow's datacontext
            DataContext = Application.Current.MainWindow.DataContext;
        }

        /// <summary>
        /// Tries to register the user if it isn't already
        /// </summary>
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Gets the registerd user
            var user = new User(tbUsername.Text, pbPassword.Password);

            // Gets the registered users
            var users = FileManager.GetUsers();

            // If the user isn't registered
            if (!users.Any(x => x.Username == user.Username))
            {
                // Adds the user to the list
                users.Add(user);

                // Updates the db
                FileManager.UpdateDb(users);

                // Sets the user in the datacontext
                (this.DataContext as MainViewModel).User = user;

                // If the view can be changed, changes it with Home
                if ((this.DataContext as MainViewModel).UpdateViewCommand.CanExecute("Home"))
                    (this.DataContext as MainViewModel).UpdateViewCommand.Execute("Home");
            }
            else
            {
                // User already registered
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
