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

            // Gets fields values
            DateTime? birthdate = dpBirthDate.SelectedDate;
            double? height = dudHeight.Value;
            double? weight = dudWeight.Value;
            string img = ImgToBase64(imgProfile.Source);

            // If fields != null/empty
            if (birthdate != null && height != null && weight != null && !string.IsNullOrEmpty(img))
            {
                // Gets users in the db
                var users = FileManager.GetUsers();

                // Gets the user's index in the list
                int i = users.IndexOf(users.Single(x => x.Username == (this.DataContext as User).Username));

                // Updates the user's attributes (List)
                users[i].BirthDate = (DateTime)birthdate;
                users[i].Height = (double)height;
                users[i].Weight = (double)weight;
                users[i].Image = img;

                // Updates the user's attributes (Model)
                (this.DataContext as User).BirthDate = (DateTime)birthdate;
                (this.DataContext as User).Height = (double)height;
                (this.DataContext as User).Weight = (double)weight;
                (this.DataContext as User).Image = img;

                // Updates the db
                FileManager.UpdateDb(users);
            }
            else
                error = "Non possono esserci campi vuoti o invalidi.";

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

            // Se != null chiudo la window
            if (w != null)
                w.Close();
        }

        /// <summary>
        /// Apre un FileDialog per caricare l'immagine
        /// </summary>
        private void EditImage(object sender, RoutedEventArgs e)
        {
            // Inits the OpenFileDialog with the img filters
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            // If accepts and gets the image, sets it
            if (ofd.ShowDialog() == true)
                imgProfile.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
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
