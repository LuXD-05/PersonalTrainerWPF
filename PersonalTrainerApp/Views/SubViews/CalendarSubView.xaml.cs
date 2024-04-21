using PersonalTrainerApp.Models;
using PersonalTrainerApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

namespace PersonalTrainerApp.Views.SubViews
{
    /// <summary>
    /// Logica di interazione per CalendarSubView.xaml
    /// </summary>
    public partial class CalendarSubView : UserControl, INotifyPropertyChanged
    {
        private string _selectedMonth;

        public string SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                _selectedMonth = value;
                OnPropertyChanged(nameof(SelectedMonth));
            }
        }
        public ObservableCollection<int> Years { get; set; }

        public CalendarSubView()
        {
            InitializeComponent();

            // Imposto datacontext view a user
            DataContext = (Application.Current.MainWindow.DataContext as MainViewModel).User;

            // Inizializzo mese selezionato
            SelectedMonth = calendar.SelectedDate.HasValue ?
                SelectedMonth = char.ToUpper(calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT"))[0]) + calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT")).Substring(1) : 
                SelectedMonth = char.ToUpper(DateTime.Now.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT"))[0]) + DateTime.Now.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT")).Substring(1);
            // Imposto datacontext tbkMonth
            tbkMonth.DataContext = this;

            // Inizializzo anni
            Years = new ObservableCollection<int> { DateTime.Today.Year - 2, DateTime.Today.Year - 1, DateTime.Today.Year, DateTime.Today.Year + 1, DateTime.Today.Year + 2 };
            // Imposto datacontext stackpanel
            spYears.DataContext = this;
        }

        /// <summary>
        /// Gestisce l'evento MouseDown del controllo
        /// </summary>
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            // Se cliccato il mouseButton sinistro, esegui il DragMove della MainWindow
            if (e.ChangedButton == MouseButton.Left)
                Application.Current.MainWindow.DragMove();
        }

        /// <summary>
        /// Gestisce l'evento generato al cambio del mese selezionato
        /// </summary>
        private void ChangeMonth(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string month)
            {
                var d = calendar.DisplayDate.Day;
                var m = DateTime.ParseExact(month, "MMM", CultureInfo.GetCultureInfo("it-IT")).Month;
                var y = calendar.DisplayDate.Year;

                // Imposto la data del calendario visualizzata
                calendar.DisplayDate = new DateTime(y, m, d);

                // Cambio il mese selezionato se cambia il mese della data selezionata
                SelectedMonth = char.ToUpper(calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT"))[0]) + calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT")).Substring(1);
            }
        }

        /// <summary>
        /// Gestisce l'evento generato al cambio dell'anno selezionato
        /// </summary>
        private void ChangeYear(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is int y)
            {
                var d = calendar.DisplayDate.Day;
                var m = calendar.DisplayDate.Month;

                // Imposo la data del calendario visualizzata
                calendar.DisplayDate = new DateTime(y, m, d);
            }
        }

        /// <summary>
        /// Gestisce lo scorrimento degli anni visualizzati indietro
        /// </summary>
        private void YearBack(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
                Years[i]--;
        }

        /// <summary>
        /// Gestisce lo scorrimento degli anni visualizzati avanti
        /// </summary>
        private void YearForward(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
                Years[i]++;
        }

        /// <summary>
        /// Cambia la source dell'ItemsControl in base a cosa cliccato sul calendar
        /// </summary>
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.Calendar c && c.SelectedDate.HasValue && this.IsLoaded)
            {
                // Aggiorno l'ItemsControl con elementi del giorno selezionato
                icActivities.ItemsSource = new ObservableCollection<Activity>((this.DataContext as User).Activities.Where(x => x.DataFull.Date == c.SelectedDate.Value.Date).OrderBy(x => x.DataFull));

                // Cambio il mese selezionato se cambia il mese della data selezionata
                SelectedMonth = char.ToUpper(calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT"))[0]) + calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT")).Substring(1);
            }
        }

        /// <summary>
        /// Cambia il mese in base alla data mostrata
        /// </summary>
        private void calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            // Cambio il mese selezionato se cambia il mese della data selezionata
            if (calendar.SelectedDate.HasValue)
                SelectedMonth = char.ToUpper(calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT"))[0]) + calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT")).Substring(1);
        }

        /// <summary>
        /// Aggiorna l'ItemsControl attività quando la view è caricata
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Se il calendar ha un valore selezionato
            if (calendar.SelectedDate.HasValue)
            {
                // Aggiorno l'ItemsControl con elementi del giorno selezionato
                icActivities.ItemsSource = new ObservableCollection<Activity>((this.DataContext as User).Activities.Where(x => x.DataFull.Date == calendar.SelectedDate.Value.Date).OrderBy(x => x.DataFull));
            }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
