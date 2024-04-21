using PersonalTrainerApp.Models.Controllers;
using PersonalTrainerApp.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using Microsoft.Maps.MapControl.WPF;

namespace PersonalTrainerApp.Views
{
    /// <summary>
    /// Logica di interazione per NewActivity.xaml
    /// </summary>
    public partial class NewActivity : UserControl
    {
        private User u;

        public NewActivity(User u)
        {
            InitializeComponent();

            // Salvo l'utente in una variabile locale per modificarlo poi nel caso (funge come se si passasse by ref)
            this.u = u;

            // Imposto tipi di default
            cbbType.ItemsSource = Enum.GetNames(typeof(Activity.ActivityType)).ToList();

            // Imposto immagine di default
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = new MemoryStream(Convert.FromBase64String(App.DEFAULT_ACTIVITY));
            bmp.EndInit();
            imgActivity.Source = bmp;
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

            // Prendo campi trimmati
            string name = tbName.Text.Trim();
            double? cx = dudX.Value;
            double? cy = dudY.Value;
            string length = tbLength.Text.Trim();
            string cal = tbCalories.Text.Trim();
            string t = (cbbType.SelectedItem as string)?.Trim();
            DateTime fullDate = (dpDate.SelectedDate ?? DateTime.Today).Date + (tpTime.Value ?? DateTime.Now).TimeOfDay;
            string img = ImgToBase64(imgActivity.Source);

            // Se campi sono tutti pieni (e length e calories sono dei double validi)
            if (!string.IsNullOrEmpty(name) && cx != null && cy != null && !string.IsNullOrEmpty(length) && !string.IsNullOrEmpty(cal) && !string.IsNullOrEmpty(t) && 
                !string.IsNullOrEmpty(fullDate.ToString("dd/MM/yyyy")) && !string.IsNullOrEmpty(fullDate.ToString("HH/mm/ss")) && !string.IsNullOrEmpty(img))
            {
                // Se lunghezza e calorie sono dei double validi
                if (double.TryParse(length.Replace('.', ','), out double dLength) && double.TryParse(cal.Replace('.', ','), out double dCal))
                {
                    // Se il tipo corrisponde a un valore di enum ActivityType
                    if (Enum.TryParse(t, true, out Activity.ActivityType type))
                    {
                        // Gets users in the db
                        var users = FileManager.GetUsers();

                        // Gets the user's index in the list
                        int i = users.IndexOf(users.Single(x => x.Username == u.Username));

                        // Se l'attività non esiste già per l'utente
                        if (!users[i].Activities.Any(x => x.Name == name))
                        {
                            Activity a;

                            // Creo l'attività (con isdone true se creata prima di adesso)
                            if (fullDate < DateTime.Now)
                                a = new Activity(name, fullDate, new Location((double)cy, (double)cx), dLength, dCal, type, img, true);
                            else
                                a = new Activity(name, fullDate, new Location((double)cy, (double)cx), dLength, dCal, type, img, false);

                            // Aggiungo l'attività all'utente (Model)
                            u.Activities.Add(a);

                            // Aggiungo l'attività all'utente (List)
                            users[i].Activities.Add(a);

                            // Aggiorno il database
                            FileManager.UpdateDb(users);
                        }
                        else
                            error = "L'utente contiene già un'attività con nome uguale.";
                    }
                    else
                        error = "Il tipo selezionato è sconosciuto.";
                }
                else
                    error = "Length e calorie devono essere numeri.";
            }
            else
                error = "Nessun campo può essere vuoto.";

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
        /// Apre l'ofd e sceglie l'immagine dell'attività
        /// </summary>
        private void ChooseImage(object sender, RoutedEventArgs e)
        {
            // Inizializzo l'ofd con i filtri per immagine
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            // Se accetto e prendo un'immagine, la imposto
            if (ofd.ShowDialog() == true)
                imgActivity.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
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
                // Se l'uri non esiste (foto già diversa da default, non ne vede l'uri) // NON SERVE QUI (perche uri mai != null) ?
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
