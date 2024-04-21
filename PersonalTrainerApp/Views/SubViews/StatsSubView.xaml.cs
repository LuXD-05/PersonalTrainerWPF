using PersonalTrainerApp.Models;
using PersonalTrainerApp.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTrainerApp.Views.SubViews
{
    public partial class StatsSubView : UserControl
    {
        private string _checkedRbtnName = string.Empty;

        public StatsSubView()
        {
            InitializeComponent();

            // Sets the datacontext as the user
            this.DataContext = (Application.Current.MainWindow.DataContext as MainViewModel).User;

            // Sets the specific datacontext for the graph
            lvcStats.DataContext = new ChartObject(this.DataContext as User, "All", null);

            // Sets the initial checked radio button name
            _checkedRbtnName = "All";
        }

        /// <summary>
        /// Filters the chart based on the date in the datepickers (considering also the checked radio button)
        /// </summary>
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            DateTime? start = dpStart.SelectedDate;
            DateTime? end = dpEnd.SelectedDate;

            // If startdate is valid, enddate is valid and startdate <= enddate
            if (start != null && end != null && start <= end)
            {
                // Rebuilds the chart based on the filter
                (lvcStats.DataContext as ChartObject).BuildStatsChart(this.DataContext as User, _checkedRbtnName, new Tuple<DateTime, DateTime>((DateTime)start, (DateTime)end));
            }
            else
                MessageBox.Show("Data inizio o fine invalide", "GilTrainer", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Sets the chart to view stats of: N° activities, km, kcal or all (considering range)
        /// </summary>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Checks if the event is sent by a checked radio button (e se chart gia inizializzato)
            if (sender is RadioButton rbtn && rbtn.IsChecked == true && _checkedRbtnName != string.Empty)
            {
                // Saves the checked radio button name
                _checkedRbtnName = rbtn.Name.Remove(0, 4);

                // Rebuilds the chart based on the filters (checked radibutton name and the dates if present)
                if (dpStart.SelectedDate != null && dpEnd.SelectedDate != null && dpStart.SelectedDate <= dpEnd.SelectedDate)
                    (lvcStats.DataContext as ChartObject).BuildStatsChart(this.DataContext as User, _checkedRbtnName, new Tuple<DateTime, DateTime>((DateTime)dpStart.SelectedDate, (DateTime)dpEnd.SelectedDate));
                else
                    (lvcStats.DataContext as ChartObject).BuildStatsChart(this.DataContext as User, _checkedRbtnName, null);
            }
        }
    }
}
