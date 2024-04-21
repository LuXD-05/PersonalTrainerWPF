using Microsoft.Win32;
using PersonalTrainerApp.Models;
using PersonalTrainerApp.Models.Controllers;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PersonalTrainerApp.Views
{
    public partial class EditProfile : UserControl
    {
        public EditProfile(User u)
        {
            InitializeComponent();

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
        /// Chiude la finestra salvando
        /// </summary>
        private void SaveAndCloseSubWindow(object sender, RoutedEventArgs e)
        {
            // Pongo l'error a ""
            string error = "";

            // Prendo valori campi
            DateTime? birthdate = dpBirthDate.SelectedDate;
            double? height = dudHeight.Value;
            double? weight = dudWeight.Value;
            string img = ImgToBase64(imgProfile.Source);

            // Se campi non sono vuoti o null
            if (birthdate != null && height != null && weight != null && !string.IsNullOrEmpty(img))
            {
                // Ottengo utenti in db
                var users = FileManager.GetUsers();

                // Ottengo l'indice dell'utente in questione nella lista
                int i = users.IndexOf(users.Single(x => x.Username == (this.DataContext as User).Username));

                // Aggiorno attributi dell'utente (List)
                users[i].BirthDate = (DateTime)birthdate;
                users[i].Height = (double)height;
                users[i].Weight = (double)weight;
                users[i].Image = img;

                // Aggiorno attributi dell'utente (Model)
                (this.DataContext as User).BirthDate = (DateTime)birthdate;
                (this.DataContext as User).Height = (double)height;
                (this.DataContext as User).Weight = (double)weight;
                (this.DataContext as User).Image = img;

                // Aggiorno il database
                FileManager.UpdateDb(users);
            }
            else
                error = "Non possono esserci campi vuoti o invalidi.";

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

        /// <summary>
        /// Apre un FileDialog per caricare l'immagine
        /// </summary>
        private void EditImage(object sender, RoutedEventArgs e)
        {
            // Inizializzo l'ofd con i filtri per immagine
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            // Se accetto e prendo un'immagine, la imposto
            if (ofd.ShowDialog() == true)
                imgProfile.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Converte un'immagine ImageSource a stringbase64
        /// </summary>
        /// <param name="img">ImageSource da convertire</param>
        /// <returns>Stringa in base 64 rappresentante l'immagine</returns>
        private string ImgToBase64(ImageSource img)
        {
            // Se l'ImageSource passata è una Bitmapimage
            if (img != null && img is BitmapImage bmp)
            {
                // Prendo il suo uri
                Uri uri = bmp.UriSource;

                // Se l'uri è trovato (esiste)
                if (uri != null)
                {
                    // Ottengo il path dell'immagine
                    return Convert.ToBase64String(File.ReadAllBytes(uri.LocalPath));
                }
                // Se l'uri non esiste (foto già diversa da default, non ne vede l'uri)
                else
                {
                    // Leggo la source con memorystream e la ritorno convertita in base 64 string
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)img));
                    using (var ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }

            // Immagine attività default se l'immagine non c'è
            return App.DEFAULT_ACTIVITY;
        }
    }
}
