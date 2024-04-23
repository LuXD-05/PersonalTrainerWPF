using Microsoft.SqlServer.Server;
using PersonalTrainerApp.Models;
using PersonalTrainerApp.Models.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PersonalTrainerApp.Views
{
    public partial class EditCredentials : UserControl
    {
        public EditCredentials(User u)
        {
            InitializeComponent();

            // Sets the view's datacontext to the user
            this.DataContext = u;
        }

        /// <summary>
        /// Handles the MouseDown event
        /// </summary>
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            // If the left mouse button is clicked, DragMove the window
            if (e.ChangedButton == MouseButton.Left)
            {
                // Tries to get the window and moves it if != null
                var w = Window.GetWindow(this);
                if (w != null)
                    w.DragMove();
            }
        }

        /// <summary>
        /// Loads usr and pwd to edit in the textboxes
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tbUsername.Text = (this.DataContext as User).Username;
            pbPassword.Password = (this.DataContext as User).Password;
        }

        /// <summary>
        /// Closes the window and saves
        /// </summary>
        private void SaveAndCloseSubWindow(object sender, RoutedEventArgs e)
        {
            // Resets the error
            string error = "";

            // Gets usr and pwd trimmed
            var username = tbUsername.Text.Trim();
            var password = pbPassword.Password.Trim();

            // If usr and pwd != null/empty
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // If usr and pwd != old ones
                if (!((this.DataContext as User).Username == username && (this.DataContext as User).Password == password))
                {
                    // Gets users in the db
                    var users = FileManager.GetUsers();

                    // Se l'utente non esiste già
                    if (!users.Any(x => x.Username == username))
                    {
                        // Gets the user's index in the list
                        int i = users.IndexOf(users.Single(x => x.Username == (this.DataContext as User).Username && x.Password == (this.DataContext as User).Password));

                        // Updates the user's usr and pwd (List)
                        users[i].Username = username;
                        users[i].Password = password;

                        // Updates the user's usr and pwd (Model)
                        (this.DataContext as User).Username = username;
                        (this.DataContext as User).Password = password;

                        // Updates the db
                        FileManager.UpdateDb(users);
                    }
                    else
                        error = "Un utente con questo nome è già registrato.";
                }
                else
                    error = "Inserire un username e una password diversi dai precedenti.";
            }
            else
                error = "Username e password non possono essere vuoti.";

            // If no error, closes window, else shows it
            if (error == "")
                CloseSubWindow(sender, null);
            else
                lblError.Content = error;
        }

        /// <summary>
        /// Closes the window without saving
        /// </summary>
        private void CloseSubWindow(object sender, MouseButtonEventArgs e)
        {
            // Gets the window where the view is located
            var w = Window.GetWindow(this);

            if (w != null)
                w.Close();
        }
    }
}
