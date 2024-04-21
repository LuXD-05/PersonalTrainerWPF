using PersonalTrainerApp.Views.SubViews;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTrainerApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private UserControl _selectedSubView;

        /// <summary>
        /// Current view's ViewModel
        /// </summary>
        public UserControl SelectedSubView
        {
            get { return _selectedSubView; }
            set
            {
                _selectedSubView = value;
                OnPropertyChanged(nameof(SelectedSubView));
            }
        }

        public HomeViewModel()
        {
            // Instances the selected ViewModel and sets Home's ViewModel as start
            SelectedSubView = new HomeSubView();

            Height = 790;
            Width = 1310;
            Error = "";
            Title = "Home";
        }

        /// <summary>
        /// Handles the switch between the tabs
        /// </summary>
        public void RadioButtonChanged(object sender, RoutedEventArgs e) 
        {
            if (sender is RadioButton radioButton && radioButton.IsChecked == true)
            {
                // Ottengo il nome della SubView 
                string fullSubViewName = $"{radioButton.Name.Replace("btn", "")}SubView";

                // Ottengo il tipo della SubView
                Type subViewType = Type.GetType($"PersonalTrainerApp.Views.SubViews.{fullSubViewName}");

                // Se la SubView esiste
                if (subViewType != null)
                {
                    // Crea un'istanza del tipo
                    SelectedSubView = Activator.CreateInstance(subViewType) as UserControl;
                }
                else
                {
                    // Eccezione se il tipo non è stato trovato
                    throw new ArgumentException($"SubView not found: {fullSubViewName}");
                }
            }
        }
    }
}
