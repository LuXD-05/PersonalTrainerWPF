using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PersonalTrainerApp.Models
{
    /// <summary>
    /// Class used to manage the chart
    /// </summary>
    public class ChartObject : INotifyPropertyChanged
    {
        #region Variables

        private SeriesCollection _seriesCollection;
        private ObservableCollection<string> _labels;

        #endregion

        #region Properties

        /// <summary>
        /// Collection to display in the chart
        /// </summary>
        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set 
            { 
                _seriesCollection = value; 
                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        /// <summary>
        /// Labels in the chart
        /// </summary>
        public ObservableCollection<string> Labels
        {
            get { return _labels; }
            set 
            { 
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Costruttore Home
        /// </summary>
        /// <param name="u">User to get activities from</param>
        public ChartObject(User u)
        {
            // Inizializzo liste
            _seriesCollection = new SeriesCollection();
            _labels = new ObservableCollection<string>();

            // Costruisco il chart Home
            BuildHomeChart(u);
        }

        /// <summary>
        /// Stats constructor
        /// </summary>
        /// <param name="u">User to get activities from</param>
        /// <param name="serie">Series to view ("Activities", "Length", "Calories", "All")</param>
        /// <param name="range">Tuple with the smallest and the biggest date of the range</param>
        public ChartObject(User u, string serie, Tuple<DateTime, DateTime> range)
        {
            // Init lists
            _seriesCollection = new SeriesCollection();
            _labels = new ObservableCollection<string>();

            BuildStatsChart(u, serie, range);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds home chart creating a SeriesCollection with some ColumnSeries for n° activities, km and kcal foreach day of the current week
        /// </summary>
        /// <param name="u">User to get activities from</param>
        public void BuildHomeChart(User u)
        {
            // Cleans SeriesCollection and Labels
            _seriesCollection.Clear();
            _labels.Clear();

            // Init lists
            var sc = new SeriesCollection();
            var l = new ObservableCollection<string>();

            if (u.Activities.Count == 0)
                return;

            // N° activities series
            var activitiesSeries = new ColumnSeries
            {
                Title = "Attività",
                Values = new ChartValues<int>(),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF99DD00")),
            };
            // Length done series
            var lengthSeries = new ColumnSeries
            {
                Title = "Chilometri",
                Values = new ChartValues<double>(),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF22BBFF")),
            };
            // Calories burned series
            var caloriesSeries = new ColumnSeries
            {
                Title = "Chilocalorie",
                Values = new ChartValues<double>(),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFA500")),
            };

            // Groups activities done by day
            var activitiesByDate = u.Activities.Where(x => x.IsDone).GroupBy(a => a.FullDate.Day);

            /* " by month
            // Ottengo i giorni del mese corrente
            List<DateTime> currentMonthDays = Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                .Select(day => new DateTime(DateTime.Now.Year, DateTime.Now.Month, day)).ToList();
            */

            // Finds the 1st day of the week (monday)
            DateTime startOfWeek = DateTime.Today.Date.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

            // Creates a list of days of the current week
            List<DateTime> currentWeekDays = Enumerable.Range(0, 7).Select(offset => startOfWeek.AddDays(offset)).ToList();

            // Foreach day of the week
            foreach (DateTime day in currentWeekDays)
            {
                // Adds day to x axis
                l.Add(day.ToString("dddd") + "\r\n" + day.ToString("dd/MM"));

                // Gets the group of activities of the day (if any)
                var dayActivities = activitiesByDate.SingleOrDefault(x => x.Key == day.Day);

                // If any
                if (dayActivities != null)
                {
                    // Adds group data to chart
                    activitiesSeries.Values.Add(dayActivities.Count());
                    lengthSeries.Values.Add(dayActivities.Sum(a => a.Length / 1000));
                    caloriesSeries.Values.Add(dayActivities.Sum(a => a.Calories / 1000));
                }
                else
                {
                    // Adds 0
                    activitiesSeries.Values.Add(0);
                    lengthSeries.Values.Add(0.0);
                    caloriesSeries.Values.Add(0.0);
                }
            }

            // Adds series to the collection
            sc.Add(activitiesSeries);
            sc.Add(lengthSeries);
            sc.Add(caloriesSeries);

            // Adds the new SeriesCollection and Labels
            SeriesCollection = sc;
            Labels = l;
        }

        /// <summary>
        /// Builds stats chart creating a SeriesCollection with some LineSeries, the specified series and between a date range
        /// </summary>
        /// <param name="u">User to get activities from</param>
        /// <param name="serie">Series to view ("Activities", "Length", "Calories", "All")</param>
        /// <param name="range">Tuple with the smallest and the biggest date of the range</param>
        public void BuildStatsChart(User u, string serie, Tuple<DateTime, DateTime> range = null)
        {
            // Cleans SeriesCollection and Labels
            SeriesCollection.Clear();
            Labels.Clear();

            // Init lists
            var sc = new SeriesCollection();
            var l = new ObservableCollection<string>();

            // If there arent done activities
            if (u.Activities.Where(x => x.IsDone).Count() == 0)
                return;

            // Groups activities by day
            var activitiesByDate = new Dictionary<DateTime, List<Activity>>();
            foreach (var a in u.Activities.Where(x => x.IsDone))
            {
                // If already has a day with activity 
                if (activitiesByDate.ContainsKey(a.FullDate.Date))
                {
                    // Adds it to the list
                    activitiesByDate.Single(x => x.Key == a.FullDate.Date).Value.Add(a);
                }
                else
                {
                    // Puts a new dictionary entry
                    activitiesByDate.Add(a.FullDate.Date, new List<Activity> { a });
                }
            }

            // Gets the days based on the range (if range = null, gets all)
            List<DateTime> days;
            if (range == null)
            {
                // Gets the dates' range from the 1st to the last
                days = Enumerable.Range(0, (activitiesByDate.Max(x => x.Key) - activitiesByDate.Min(x => x.Key)).Days + 1)
                                 .Select(offset => activitiesByDate.Min(x => x.Key).AddDays(offset)).ToList();
            }
            else
            {
                // Gets the dates' range from the 1st range's value to its last
                days = Enumerable.Range(0, (range.Item2 - range.Item1).Days + 1)
                                 .Select(offset => range.Item1.AddDays(offset)).ToList();
            }

            LineSeries activitiesSeries;
            LineSeries lengthSeries;
            LineSeries caloriesSeries;

            // Creates the series
            switch (serie)
            {
                #region Activities Only
                
                case "Activities":
                    // N° activities series
                    activitiesSeries = new LineSeries
                    {
                        Title = "Attività",
                        Values = new ChartValues<int>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6699DD00")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF99DD00")),
                        LineSmoothness = 0.2,
                    };

                    // Foreach day of the range
                    foreach (DateTime day in days)
                    {
                        // Adds the day to x axis
                        l.Add(day.ToString("dd/MM"));

                        // If there are any activities for this day
                        if (activitiesByDate.ContainsKey(day.Date))
                        {
                            // Adds its data to the chart
                            activitiesSeries.Values.Add(activitiesByDate[day.Date].Count);
                        }
                        else
                        {
                            // Adds 0
                            activitiesSeries.Values.Add(0);
                        }
                    }

                    // Adds the series to the collection
                    sc.Add(activitiesSeries);

                    break;

                #endregion

                #region Length Only

                case "Length":
                    // Length done series
                    lengthSeries = new LineSeries
                    {
                        Title = "Chilometri",
                        Values = new ChartValues<double>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6622BBFF")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF22BBFF")),
                        LineSmoothness = 0.2,
                    };

                    // Foreach day of the range
                    foreach (DateTime day in days)
                    {
                        // Adds the day to x axis
                        l.Add(day.ToString("dd/MM"));

                        // If there are any activities for this day
                        if (activitiesByDate.ContainsKey(day.Date))
                        {
                            // Adds its data to the chart
                            lengthSeries.Values.Add(activitiesByDate[day.Date].Sum(a => a.Length / 1000));
                        }
                        else
                        {
                            // Adds 0
                            lengthSeries.Values.Add(0.0);
                        }
                    }

                    // Adds the series to the collection
                    sc.Add(lengthSeries);

                    break;

                #endregion

                #region Calories Only

                case "Calories":
                    // Calories burned series
                    caloriesSeries = new LineSeries
                    {
                        Title = "Chilocalorie",
                        Values = new ChartValues<double>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66FFA500")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFA500")),
                        LineSmoothness = 0.2,
                    };

                    // Foreach day of the range
                    foreach (DateTime day in days)
                    {
                        // Adds the day to x axis
                        l.Add(day.ToString("dd/MM"));

                        // If there are any activities for this day
                        if (activitiesByDate.ContainsKey(day.Date))
                        {
                            // Adds its data to the chart
                            caloriesSeries.Values.Add(activitiesByDate[day.Date].Sum(a => a.Calories / 1000));
                        }
                        else
                        {
                            // Adds 0
                            caloriesSeries.Values.Add(0.0);
                        }
                    }

                    // Adds the series to the collection
                    sc.Add(caloriesSeries);

                    break;

                #endregion

                #region All

                case "All":
                    // N° activities series
                    activitiesSeries = new LineSeries
                    {
                        Title = "Attività",
                        Values = new ChartValues<int>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6699DD00")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF99DD00")),
                        LineSmoothness = 0.2,
                    };
                    // Length done series
                    lengthSeries = new LineSeries
                    {
                        Title = "Chilometri",
                        Values = new ChartValues<double>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6622BBFF")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF22BBFF")),
                        LineSmoothness = 0.2,
                    };
                    // Calories burned series
                    caloriesSeries = new LineSeries
                    {
                        Title = "Chilocalorie",
                        Values = new ChartValues<double>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66FFA500")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFA500")),
                        LineSmoothness = 0.2,
                    };

                    // Foreach day of the range
                    foreach (DateTime day in days)
                    {
                        // Adds the day to x axis
                        l.Add(day.ToString("dd/MM"));

                        // If there are any activities for this day
                        if (activitiesByDate.ContainsKey(day.Date))
                        {
                            // Adds its data to the chart
                            activitiesSeries.Values.Add(activitiesByDate[day.Date].Count);
                            lengthSeries.Values.Add(activitiesByDate[day.Date].Sum(a => a.Length / 1000));
                            caloriesSeries.Values.Add(activitiesByDate[day.Date].Sum(a => a.Calories / 1000));
                        }
                        else
                        {
                            // Adds 0
                            activitiesSeries.Values.Add(0);
                            lengthSeries.Values.Add(0.0);
                            caloriesSeries.Values.Add(0.0);
                        }
                    }

                    // Adds the series to the collection
                    sc.Add(activitiesSeries);
                    sc.Add(lengthSeries);
                    sc.Add(caloriesSeries);

                    break;

                #endregion

            }

            // Assegno le nuove SeriesCollection e Labels
            SeriesCollection = sc;
            Labels = l;
        }

        #endregion

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
