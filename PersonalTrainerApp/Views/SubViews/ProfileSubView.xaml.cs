using PersonalTrainerApp.Models;
using PersonalTrainerApp.Models.Controllers;
using PersonalTrainerApp.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PersonalTrainerApp.Views.SubViews
{
    public partial class ProfileSubView : UserControl
    {
        private SubWindow _subWindow;

        public ProfileSubView()
        {
            InitializeComponent();

            DataContext = (Application.Current.MainWindow.DataContext as MainViewModel).User;
        }

        /// <summary>
        /// Opens the window for editing credentials
        /// </summary>
        private void OpenEditCredentialsWindow(object sender, MouseButtonEventArgs e)
        {
            // Instantiates the desired subwindow and the user
            _subWindow = new SubWindow(new EditCredentials(this.DataContext as User));

            // Adds handler for when the subwindow closes
            _subWindow.Closed += SubWindow_Closed;

            // Shows the subwindow
            _subWindow.Show();
        }

        /// <summary>
        /// Opens the window for editing the profile
        /// </summary>
        private void OpenEditProfileWindow(object sender, MouseButtonEventArgs e)
        {
            // Instantiates the desired subwindow and the user
            _subWindow = new SubWindow(new EditProfile(this.DataContext as User));

            // Adds handler for when the subwindow closes
            _subWindow.Closed += SubWindow_Closed;

            // Shows the subwindow
            _subWindow.Show();
        }

        /// <summary>
        /// Deletes the account asking for confirmation and exits
        /// </summary>
        private void DeleteAccount(object sender, MouseButtonEventArgs e)
        {
            // Gets the users from the db
            var users = FileManager.GetUsers();

            // Removes the user from the list
            users.Remove(users.Single(x => x.Username == (this.DataContext as User).Username && x.Password == (this.DataContext as User).Password));

            // Updates the db
            FileManager.UpdateDb(users);

            // Logouts
            if ((Application.Current.MainWindow.DataContext as MainViewModel).UpdateViewCommand.CanExecute("Login"))
                (Application.Current.MainWindow.DataContext as MainViewModel).UpdateViewCommand.Execute("Login");
        }

        /// <summary>
        /// Handles subwindow closed
        /// </summary>
        private void SubWindow_Closed(object sender, EventArgs e)
        {
            // Removes handler
            _subWindow.Closed -= SubWindow_Closed;
        }
    }
}