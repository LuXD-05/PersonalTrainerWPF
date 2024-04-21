using Microsoft.Win32;
using PersonalTrainerApp.Models.Controllers;
using PersonalTrainerApp.Models;
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
using System.Data.SqlClient;
using System.IO;
using Microsoft.Maps.MapControl.WPF;

namespace PersonalTrainerApp.Views
{
    public partial class EditActivity : UserControl
    {
        User u;
        string oldName;

        public EditActivity(Activity a, User u)
        {
            InitializeComponent();

            // Imposto il datacontext della window all'attività scelta
            this.DataContext = a;

            // Prendo il nome vecchio
            oldName = a.Name;

            // Salvo l'utente in una variabile locale per modificarlo poi nel caso (funge come se si passasse by ref)
            this.u = u;
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
            bool done = cbDone.IsChecked.GetValueOrDefault();

            // Se campi sono tutti pieni (e length e calories e x e y sono dei double validi)
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

                        // Ottengo l'indice dell'attività in questione nella lista
                        var j = users[i].Activities.IndexOf(users[i].Activities.Single(x => x.Name == oldName));

                        // Se il nome non è == a quello di un'altra attività (eccetto quella vecchia)
                        if (!users[i].Activities.Where(x => x != users[i].Activities[j]).Any(x => x.Name == name))
                        {
                            // Aggiorno attributi attività (List)
                            users[i].Activities[j].Name = name;
                            users[i].Activities[j].Coordinate = new Location((double)cy, (double)cx);
                            users[i].Activities[j].Length = dLength;
                            users[i].Activities[j].Calories = dCal;
                            users[i].Activities[j].Type = type;
                            users[i].Activities[j].FullDate = fullDate;
                            users[i].Activities[j].Image = img;
                            users[i].Activities[j].IsDone = done;

                            // Aggiorno attributi attività (Model)
                            (this.DataContext as Activity).Name = name;
                            (this.DataContext as Activity).Coordinate = new Location((double)cy, (double)cx);
                            (this.DataContext as Activity).Length = dLength;
                            (this.DataContext as Activity).Calories = dCal;
                            (this.DataContext as Activity).Type = type;
                            (this.DataContext as Activity).FullDate = fullDate;
                            (this.DataContext as Activity).Image = img;
                            (this.DataContext as Activity).IsDone = done;

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
