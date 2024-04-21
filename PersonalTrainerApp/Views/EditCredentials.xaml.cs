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
    /// <summary>
    /// Logica di interazione per EditCredentials.xaml
    /// </summary>
    public partial class EditCredentials : UserControl
    {
        public EditCredentials(User u)
        {
            InitializeComponent();

            // Imposto il datacontext della view all'utente
            this.DataContext = u;
        }

        /// <summary>
        /// Gestisce l'evento MouseDown del controllo
        /// </summary>
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            // Se cliccato il mouseButton sinistro, esegui il DragMove della Window
            if (e.ChangedButton == MouseButton.Left)
            {
                // Provo a ottenere la window e la muovo se != null
                var w = Window.GetWindow(this);
                if (w != null)
                    w.DragMove();
            }
        }

        /// <summary>
        /// Carica username e password da modificare nelle textboxes
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tbUsername.Text = (this.DataContext as User).Username;
            pbPassword.Password = (this.DataContext as User).Password;
        }

        /// <summary>
        /// Chiude la finestra salvando
        /// </summary>
        private void SaveAndCloseSubWindow(object sender, RoutedEventArgs e)
        {
            // Pongo l'error a ""
            string error = "";

            // Prendo username e password trimmati
            var username = tbUsername.Text.Trim();
            var password = pbPassword.Password.Trim();

            // Se username e password non sono vuoti
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // Se username e password non sono == a prima
                if (!((this.DataContext as User).Username == username && (this.DataContext as User).Password == password))
                {
                    // Ottengo utenti in db
                    var users = FileManager.GetUsers();

                    // Se l'utente non esiste già
                    if (!users.Any(x => x.Username == username))
                    {
                        // Ottengo l'indice dell'utente in questione nella lista
                        int i = users.IndexOf(users.Single(x => x.Username == (this.DataContext as User).Username && x.Password == (this.DataContext as User).Password));

                        // Aggiorno username e password dell'utente (List)
                        users[i].Username = username;
                        users[i].Password = password;

                        // Aggiorno username e password dell'utente (Model)
                        (this.DataContext as User).Username = username;
                        (this.DataContext as User).Password = password;

                        // Aggiorno il database
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

            // Se non c'è errore, chiudo la finestra, altrimenti lo mostro
            if (error == "")
                CloseSubWindow(sender, null);
            else
                lblError.Content = error;
        }

        /// <summary>
        /// Chiude la finestra senza salvare
        /// </summary>
        private void CloseSubWindow(object sender, MouseButtonEventArgs e)
        {
            // Ottengo la window in cui si trova la view
            var w = Window.GetWindow(this);

            // Se != null chiudo la window
            if (w != null)
                w.Close();
        }
    }
}
