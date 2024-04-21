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

        ///// <summary>
        ///// Cambia la modalità da light a dark e viceversa
        ///// </summary>
        //private void ChangeMode(object sender, MouseButtonEventArgs e)
        //{
            
        //}

        /// <summary>
        /// Apre la finestra per l'editing delle credenziali
        /// </summary>
        private void OpenEditCredentialsWindow(object sender, MouseButtonEventArgs e)
        {
            // Istanzio subwindow con view desiderata e user
            _subWindow = new SubWindow(new EditCredentials(this.DataContext as User));

            // Aggiungo handler per quando si chiude la subwindow
            _subWindow.Closed += SubWindow_Closed;

            // Mostro subwindow
            _subWindow.Show();
        }

        /// <summary>
        /// Apre la finestra per l'editing del profilo
        /// </summary>
        private void OpenEditProfileWindow(object sender, MouseButtonEventArgs e)
        {
            // Istanzio subwindow con view desiderata e user
            _subWindow = new SubWindow(new EditProfile(this.DataContext as User));

            // Aggiungo handler per quando si chiude la subwindow
            _subWindow.Closed += SubWindow_Closed;

            // Mostro subwindow
            _subWindow.Show();
        }

        /// <summary>
        /// Cancella l'account chiedendo conferma ed esce
        /// </summary>
        private void DeleteAccount(object sender, MouseButtonEventArgs e)
        {
            // Ottengo gli utenti da db
            var users = FileManager.GetUsers();

            // Rimuovo l'utente dalla lista
            users.Remove(users.Single(x => x.Username == (this.DataContext as User).Username && x.Password == (this.DataContext as User).Password));

            // Aggiorno il database
            FileManager.UpdateDb(users);

            // Eseguo il logout
            if ((Application.Current.MainWindow.DataContext as MainViewModel).UpdateViewCommand.CanExecute("Login"))
                (Application.Current.MainWindow.DataContext as MainViewModel).UpdateViewCommand.Execute("Login");
        }

        /// <summary>
        /// Metodo che gestisce la chiusura della subwindow
        /// </summary>
        private void SubWindow_Closed(object sender, EventArgs e)
        {
            // Rimuovo handler
            _subWindow.Closed -= SubWindow_Closed;
        }
    }
}