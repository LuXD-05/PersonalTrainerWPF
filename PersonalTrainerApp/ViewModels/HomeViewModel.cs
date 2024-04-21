using PersonalTrainerApp.Views.SubViews;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTrainerApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private UserControl _selectedSubView;

        // ViewModel della View corrente
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
            // Istanzio il ViewModel selezionato e setto quello di Home come quello di start
            SelectedSubView = new HomeSubView();

            Height = 790;
            Width = 1310;
            Error = "";
            Title = "Home";
        }

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
