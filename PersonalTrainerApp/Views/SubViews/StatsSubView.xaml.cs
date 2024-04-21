using PersonalTrainerApp.Models;
using PersonalTrainerApp.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTrainerApp.Views.SubViews
{
    /// <summary>
    /// Logica di interazione per StatsSubView.xaml
    /// </summary>
    public partial class StatsSubView : UserControl
    {
        private string _checkedRbtnName = string.Empty;

        public StatsSubView()
        {
            InitializeComponent();

            // Imposto datacontext view a user
            this.DataContext = (Application.Current.MainWindow.DataContext as MainViewModel).User;

            // Imposto datacontext specifico del grafico
            lvcStats.DataContext = new ChartObject(this.DataContext as User, "All", null);

            // Setto il checked radiobutton name iniziale
            _checkedRbtnName = "All";
        }

        /// <summary>
        /// Filtra il grafico in base alle date nei datepicker (considerando anche il checked radiobutton name)
        /// </summary>
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            DateTime? start = dpStart.SelectedDate;
            DateTime? end = dpEnd.SelectedDate;

            // Se data start è valida, data end è valida e data start <= data end
            if (start != null && end != null && start <= end)
            {
                // Rebuildo il chart in base al filtro
                (lvcStats.DataContext as ChartObject).BuildStatsChart(this.DataContext as User, _checkedRbtnName, new Tuple<DateTime, DateTime>((DateTime)start, (DateTime)end));
            }
            else
                MessageBox.Show("Data inizio o fine invalide", "GilTrainer", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Imposta il grafico per visualizzare le statistiche di: Numero attività, Chilometri, Kilocalorie o tutto (considerando anche range)
        /// </summary>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Controllo se evento mandato da un radiobutton checkato (e se chart gia inizializzato)
            if (sender is RadioButton rbtn && rbtn.IsChecked == true && _checkedRbtnName != string.Empty)
            {
                // Salvo il nome del radiobutton checkato
                _checkedRbtnName = rbtn.Name.Remove(0, 4);

                // Rebuildo il chart in base ai filtri (il checked radibutton name e le date se presenti)
                if (dpStart.SelectedDate != null && dpEnd.SelectedDate != null && dpStart.SelectedDate <= dpEnd.SelectedDate)
                    (lvcStats.DataContext as ChartObject).BuildStatsChart(this.DataContext as User, _checkedRbtnName, new Tuple<DateTime, DateTime>((DateTime)dpStart.SelectedDate, (DateTime)dpEnd.SelectedDate));
                else
                    (lvcStats.DataContext as ChartObject).BuildStatsChart(this.DataContext as User, _checkedRbtnName, null);
            }
        }
    }
}
