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

            // Sets the window's datacontext to the chosen activity
            this.DataContext = a;

            // Gets the old activity's name
            oldName = a.Name;

            // Saves the user in a local variable to edit it in case (functions as it was passed by ref)
            this.u = u;
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
        /// Closes the window and saves
        /// </summary>
        private void SaveAndCloseSubWindow(object sender, RoutedEventArgs e)
        {
            // Resets the error
            string error = "";

            // Gets the trimmed fields
            string name = tbName.Text.Trim();
            double? cx = dudX.Value;
            double? cy = dudY.Value;
            string length = tbLength.Text.Trim();
            string cal = tbCalories.Text.Trim();
            string t = (cbbType.SelectedItem as string)?.Trim();
            DateTime fullDate = (dpDate.SelectedDate ?? DateTime.Today).Date + (tpTime.Value ?? DateTime.Now).TimeOfDay;
            string img = ImgToBase64(imgActivity.Source);
            bool done = cbDone.IsChecked.GetValueOrDefault();

            // If the fields are all valid and != null
            if (!string.IsNullOrEmpty(name) && cx != null && cy != null && !string.IsNullOrEmpty(length) && !string.IsNullOrEmpty(cal) && !string.IsNullOrEmpty(t) &&
                !string.IsNullOrEmpty(fullDate.ToString("dd/MM/yyyy")) && !string.IsNullOrEmpty(fullDate.ToString("HH/mm/ss")) && !string.IsNullOrEmpty(img))
            {
                // If length and calories are valid doubles
                if (double.TryParse(length.Replace('.', ','), out double dLength) && double.TryParse(cal.Replace('.', ','), out double dCal))
                {
                    // If the type corresponds to an ActivityType enum value
                    if (Enum.TryParse(t, true, out Activity.ActivityType type))
                    {
                        // Gets users in the db
                        var users = FileManager.GetUsers();

                        // Gets the user's index in the list
                        int i = users.IndexOf(users.Single(x => x.Username == u.Username));

                        // Gets the index of the activity in the list
                        var j = users[i].Activities.IndexOf(users[i].Activities.Single(x => x.Name == oldName));

                        // If Activities doesn't contain the activity name (except for the old one)
                        if (!users[i].Activities.Where(x => x != users[i].Activities[j]).Any(x => x.Name == name))
                        {
                            // Updates activity's attributes (List)
                            users[i].Activities[j].Name = name;
                            users[i].Activities[j].Coordinate = new Location((double)cy, (double)cx);
                            users[i].Activities[j].Length = dLength;
                            users[i].Activities[j].Calories = dCal;
                            users[i].Activities[j].Type = type;
                            users[i].Activities[j].FullDate = fullDate;
                            users[i].Activities[j].Image = img;
                            users[i].Activities[j].IsDone = done;

                            // Updates activity's attributes (Model)
                            (this.DataContext as Activity).Name = name;
                            (this.DataContext as Activity).Coordinate = new Location((double)cy, (double)cx);
                            (this.DataContext as Activity).Length = dLength;
                            (this.DataContext as Activity).Calories = dCal;
                            (this.DataContext as Activity).Type = type;
                            (this.DataContext as Activity).FullDate = fullDate;
                            (this.DataContext as Activity).Image = img;
                            (this.DataContext as Activity).IsDone = done;

                            // Updates the db
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

        /// <summary>
        /// Opens an OpenFileDialog and selects the activity's image
        /// </summary>
        private void ChooseImage(object sender, RoutedEventArgs e)
        {
            // Inits the OpenFileDialog with the img filters
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            // If accepts and gets the image, sets it
            if (ofd.ShowDialog() == true)
                imgActivity.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Converts an image to ImageSource to base64
        /// </summary>
        /// <param name="img">ImageSource to convert</param>
        /// <returns>Base64 string representing the image</returns>
        private string ImgToBase64(ImageSource img)
        {
            // If the ImageSource is a BitmapImage
            if (img != null && img is BitmapImage bmp)
            {
                // Gets the URI
                Uri uri = bmp.UriSource;

                // If the URI exists
                if (uri != null)
                {
                    // Gets the image path
                    return Convert.ToBase64String(File.ReadAllBytes(uri.LocalPath));
                }
                // If the URI doesn't exist
                else
                {
                    // Reads the source with a memorystream and returns it in base64
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)img));
                    using (var ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }

            // Returns the default activity image
            return App.DEFAULT_ACTIVITY;
        }
    }
}
