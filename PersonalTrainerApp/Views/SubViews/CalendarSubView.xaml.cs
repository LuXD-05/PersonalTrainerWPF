using PersonalTrainerApp.Models;
using PersonalTrainerApp.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalTrainerApp.Views.SubViews
{
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

            // Sets the datacontext as the user
            DataContext = (Application.Current.MainWindow.DataContext as MainViewModel).User;

            // Init selected month
            SelectedMonth = calendar.SelectedDate.HasValue ?
                SelectedMonth = char.ToUpper(calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT"))[0]) + calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT")).Substring(1) : 
                SelectedMonth = char.ToUpper(DateTime.Now.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT"))[0]) + DateTime.Now.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT")).Substring(1);
            // Sets datacontext tbkMonth
            tbkMonth.DataContext = this;

            // Init years
            Years = new ObservableCollection<int> { DateTime.Today.Year - 2, DateTime.Today.Year - 1, DateTime.Today.Year, DateTime.Today.Year + 1, DateTime.Today.Year + 2 };
            // Sets datacontext stackpanel
            spYears.DataContext = this;
        }

        /// <summary>
        /// Handlws mousedown
        /// </summary>
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            // If left mouse button clicked, DragMove
            if (e.ChangedButton == MouseButton.Left)
                Application.Current.MainWindow.DragMove();
        }

        /// <summary>
        /// Handles the event generated when the selected month changes
        /// </summary>
        private void ChangeMonth(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string month)
            {
                var d = calendar.DisplayDate.Day;
                var m = DateTime.ParseExact(month, "MMM", CultureInfo.GetCultureInfo("it-IT")).Month;
                var y = calendar.DisplayDate.Year;

                // Sets the displayed date
                calendar.DisplayDate = new DateTime(y, m, d);

                // Changes the selected months if also changes the selected date
                SelectedMonth = char.ToUpper(calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT"))[0]) + calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT")).Substring(1);
            }
        }

        /// <summary>
        /// Handles the event generated when the selected year changes
        /// </summary>
        private void ChangeYear(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is int y)
            {
                var d = calendar.DisplayDate.Day;
                var m = calendar.DisplayDate.Month;

                // Sets the displayed date
                calendar.DisplayDate = new DateTime(y, m, d);
            }
        }

        /// <summary>
        /// Handles the scrolling back of years
        /// </summary>
        private void YearBack(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
                Years[i]--;
        }

        /// <summary>
        /// Handles the scrolling forward of years
        /// </summary>
        private void YearForward(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
                Years[i]++;
        }

        /// <summary>
        /// Changes the ItemsControl's source based on what clicked on the calendar
        /// </summary>
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.Calendar c && c.SelectedDate.HasValue && this.IsLoaded)
            {
                // Updates the itemcontrol with elements from the selected day
                icActivities.ItemsSource = new ObservableCollection<Activity>((this.DataContext as User).Activities.Where(x => x.FullDate.Date == c.SelectedDate.Value.Date).OrderBy(x => x.FullDate));

                // Changes the selectedmonth if the selecteddate's month changes
                SelectedMonth = char.ToUpper(calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT"))[0]) + calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT")).Substring(1);
            }
        }

        /// <summary>
        /// Changes the month based on the displayed date
        /// </summary>
        private void calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            // Changes the selectedmonth if the selecteddate's month changes
            if (calendar.SelectedDate.HasValue)
                SelectedMonth = char.ToUpper(calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT"))[0]) + calendar.DisplayDate.ToString("MMMM", CultureInfo.GetCultureInfo("it-IT")).Substring(1);
        }

        /// <summary>
        /// Updates the ItemsControl's activity when the view is loaded
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // If calendar has a value selected
            if (calendar.SelectedDate.HasValue)
            {
                // Updates the itemcontrol with elements from the selected day
                icActivities.ItemsSource = new ObservableCollection<Activity>((this.DataContext as User).Activities.Where(x => x.FullDate.Date == calendar.SelectedDate.Value.Date).OrderBy(x => x.FullDate));
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
