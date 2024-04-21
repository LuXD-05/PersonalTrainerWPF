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
    public class ChartObject : INotifyPropertyChanged
    {
        #region Variables

        private SeriesCollection _seriesCollection;
        private ObservableCollection<string> _labels;

        #endregion

        #region Properties

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set 
            { 
                _seriesCollection = value; 
                OnPropertyChanged(nameof(SeriesCollection));
            }
        }
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
        /// <param name="u">Utente da cui ottenere le attività</param>
        public ChartObject(User u)
        {
            // Inizializzo liste
            _seriesCollection = new SeriesCollection();
            _labels = new ObservableCollection<string>();

            // Costruisco il chart Home
            BuildHomeChart(u);
        }

        /// <summary>
        /// Costruttore stats
        /// </summary>
        /// <param name="u">Utente da cui ottenere le attività</param>
        /// <param name="serie">Serie da visualizzare (Activities, Length, Calories, All)</param>
        /// <param name="range">Tuple contenente la data minore del range e la data maggiore del range</param>
        public ChartObject(User u, string serie, Tuple<DateTime, DateTime> range)
        {
            // Inizializzo liste
            _seriesCollection = new SeriesCollection();
            _labels = new ObservableCollection<string>();

            // Costruisco il chart Stats
            BuildStatsChart(u, serie, range);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Costruisce il chart della home creando una SeriesCollection con delle ColumnSeries per n attività, chilometri e kilocalorie per ogni giorno della settimana corrente
        /// </summary>
        /// <param name="u">Utente da cui prendere le attività</param>
        public void BuildHomeChart(User u)
        {
            // Pulisco SeriesCollection e Labels
            _seriesCollection.Clear();
            _labels.Clear();

            // Inizializzo liste
            var sc = new SeriesCollection();
            var l = new ObservableCollection<string>();

            if (u.Activities.Count == 0)
                return;

            // Serie n attività
            var activitiesSeries = new ColumnSeries
            {
                Title = "Attività",
                Values = new ChartValues<int>(),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF99DD00")),
            };
            // Serie lunghezza fatta
            var lengthSeries = new ColumnSeries
            {
                Title = "Chilometri",
                Values = new ChartValues<double>(),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF22BBFF")),
            };
            // Serie calorie consumate
            var caloriesSeries = new ColumnSeries
            {
                Title = "Chilocalorie",
                Values = new ChartValues<double>(),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFA500")),
            };

            // Raggruppo le attività fatte per giorno
            var activitiesByDate = u.Activities.Where(x => x.IsDone).GroupBy(a => a.DataFull.Day);

            /* per mese
            // Ottengo i giorni del mese corrente
            List<DateTime> currentMonthDays = Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                .Select(day => new DateTime(DateTime.Now.Year, DateTime.Now.Month, day)).ToList();
            */

            // Trova il primo giorno della settimana corrente (lunedì)
            DateTime startOfWeek = DateTime.Today.Date.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

            // Crea una lista di giorni della settimana corrente
            List<DateTime> currentWeekDays = Enumerable.Range(0, 7).Select(offset => startOfWeek.AddDays(offset)).ToList();

            // Per ogni giorno del mese
            foreach (DateTime day in currentWeekDays)
            {
                // Aggiungo il giorno a asse x
                l.Add(day.ToString("dddd") + "\r\n" + day.ToString("dd/MM"));

                // Prendo il gruppo di attività del giorno se ne sono state fatte
                var dayActivities = activitiesByDate.SingleOrDefault(x => x.Key == day.Day);

                // Se attività fatte
                if (dayActivities != null)
                {
                    // Ne aggiungo i dati al chart
                    activitiesSeries.Values.Add(dayActivities.Count());
                    lengthSeries.Values.Add(dayActivities.Sum(a => a.Lunghezza / 1000));
                    caloriesSeries.Values.Add(dayActivities.Sum(a => a.Calorie / 1000));
                }
                else
                {
                    // Se no aggiungo 0
                    activitiesSeries.Values.Add(0);
                    lengthSeries.Values.Add(0.0);
                    caloriesSeries.Values.Add(0.0);
                }
            }

            // Aggiungo le serie alla raccolta
            sc.Add(activitiesSeries);
            sc.Add(lengthSeries);
            sc.Add(caloriesSeries);

            // Assegno le nuove SeriesCollection e Labels
            SeriesCollection = sc;
            Labels = l;
        }

        /// <summary>
        /// Costruisce il chart delle stats creando una SeriesCollection con delle LineSeries le serie specificate e tra un range di date
        /// </summary>
        /// <param name="u">Utente da cui ottenere le attività</param>
        /// <param name="serie">Serie da visualizzare (Activities, Length, Calories, All)</param>
        /// <param name="range">Tuple contenente la data minore del range e la data maggiore del range</param>
        public void BuildStatsChart(User u, string serie, Tuple<DateTime, DateTime> range = null)
        {
            // Pulisco SeriesCollection e Labels
            SeriesCollection.Clear();
            Labels.Clear();

            // Inizializzo liste
            var sc = new SeriesCollection();
            var l = new ObservableCollection<string>();

            if (u.Activities.Count == 0)
                return;

            // Raggruppo le attività fatte per giorno
            var activitiesByDate = new Dictionary<DateTime, List<Activity>>();
            foreach (var a in u.Activities.Where(x => x.IsDone))
            {
                // Se ho gia un giorno con un'attività la aggiungo alla lista
                if (activitiesByDate.ContainsKey(a.DataFull.Date))
                    activitiesByDate.Single(x => x.Key == a.DataFull.Date).Value.Add(a);
                // Se no metto una nuova voce di dizionario
                else
                    activitiesByDate.Add(a.DataFull.Date, new List<Activity> { a });
            }

            // Ottengo i giorni in base al range (se c'è, se no tutte)
            List<DateTime> days;
            if (range == null)
            {
                // Ottengo il range di date dalla prima all'ultima di quelle in lista
                days = Enumerable.Range(0, (activitiesByDate.Max(x => x.Key) - activitiesByDate.Min(x => x.Key)).Days + 1)
                                 .Select(offset => activitiesByDate.Min(x => x.Key).AddDays(offset)).ToList();
            }
            else
            {
                // Ottengo il range di date dal primo valore del range al secondo
                days = Enumerable.Range(0, (range.Item2 - range.Item1).Days + 1)
                                 .Select(offset => range.Item1.AddDays(offset)).ToList();
            }

            // Dichiaro le serie
            LineSeries activitiesSeries;
            LineSeries lengthSeries;
            LineSeries caloriesSeries;

            // Creo le serie richieste
            switch (serie)
            {
                #region Activities Only
                
                case "Activities":
                    // Serie n attività
                    activitiesSeries = new LineSeries
                    {
                        Title = "Attività",
                        Values = new ChartValues<int>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6699DD00")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF99DD00")),
                        LineSmoothness = 0.2,
                    };

                    // Per ogni giorno del range
                    foreach (DateTime day in days)
                    {
                        // Aggiungo il giorno a asse x
                        l.Add(day.ToString("dd/MM"));

                        // Se ci sono attività per questo giorno
                        if (activitiesByDate.ContainsKey(day.Date))
                        {
                            // Ne aggiungo i dati al chart
                            activitiesSeries.Values.Add(activitiesByDate[day.Date].Count);
                        }
                        else
                        {
                            // Se no aggiungo 0
                            activitiesSeries.Values.Add(0);
                        }
                    }

                    // Aggiungo le serie alla raccolta
                    sc.Add(activitiesSeries);

                    break;

                #endregion

                #region Length Only

                case "Length":
                    // Serie lunghezza fatta
                    lengthSeries = new LineSeries
                    {
                        Title = "Chilometri",
                        Values = new ChartValues<double>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6622BBFF")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF22BBFF")),
                        LineSmoothness = 0.2,
                    };

                    // Per ogni giorno del range
                    foreach (DateTime day in days)
                    {
                        // Aggiungo il giorno a asse x
                        l.Add(day.ToString("dd/MM"));

                        // Se ci sono attività per questo giorno
                        if (activitiesByDate.ContainsKey(day.Date))
                        {
                            // Ne aggiungo i dati al chart
                            lengthSeries.Values.Add(activitiesByDate[day.Date].Sum(a => a.Lunghezza / 1000));
                        }
                        else
                        {
                            // Se no aggiungo 0
                            lengthSeries.Values.Add(0.0);
                        }
                    }

                    // Aggiungo le serie alla raccolta
                    sc.Add(lengthSeries);

                    break;

                #endregion

                #region Calories Only

                case "Calories":
                    // Serie calorie consumate
                    caloriesSeries = new LineSeries
                    {
                        Title = "Chilocalorie",
                        Values = new ChartValues<double>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66FFA500")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFA500")),
                        LineSmoothness = 0.2,
                    };

                    // Per ogni giorno del range
                    foreach (DateTime day in days)
                    {
                        // Aggiungo il giorno a asse x
                        l.Add(day.ToString("dd/MM"));

                        // Se ci sono attività per questo giorno
                        if (activitiesByDate.ContainsKey(day.Date))
                        {
                            // Ne aggiungo i dati al chart
                            caloriesSeries.Values.Add(activitiesByDate[day.Date].Sum(a => a.Calorie / 1000));
                        }
                        else
                        {
                            // Se no aggiungo 0
                            caloriesSeries.Values.Add(0.0);
                        }
                    }

                    // Aggiungo le serie alla raccolta
                    sc.Add(caloriesSeries);

                    break;

                #endregion

                #region All

                case "All":
                    // Serie n attività
                    activitiesSeries = new LineSeries
                    {
                        Title = "Attività",
                        Values = new ChartValues<int>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6699DD00")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF99DD00")),
                        LineSmoothness = 0.2,
                    };
                    lengthSeries = new LineSeries
                    {
                        Title = "Chilometri",
                        Values = new ChartValues<double>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6622BBFF")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF22BBFF")),
                        LineSmoothness = 0.2,
                    };
                    // Serie calorie consumate
                    caloriesSeries = new LineSeries
                    {
                        Title = "Chilocalorie",
                        Values = new ChartValues<double>(),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66FFA500")),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFA500")),
                        LineSmoothness = 0.2,
                    };

                    // Per ogni giorno del range
                    foreach (DateTime day in days)
                    {
                        // Aggiungo il giorno a asse x
                        l.Add(day.ToString("dd/MM"));

                        // Se ci sono attività per questo giorno
                        if (activitiesByDate.ContainsKey(day.Date))
                        {
                            // Ne aggiungo i dati al chart
                            activitiesSeries.Values.Add(activitiesByDate[day.Date].Count);
                            lengthSeries.Values.Add(activitiesByDate[day.Date].Sum(a => a.Lunghezza / 1000));
                            caloriesSeries.Values.Add(activitiesByDate[day.Date].Sum(a => a.Calorie / 1000));
                        }
                        else
                        {
                            // Se no aggiungo 0
                            activitiesSeries.Values.Add(0);
                            lengthSeries.Values.Add(0.0);
                            caloriesSeries.Values.Add(0.0);
                        }
                    }

                    // Aggiungo le serie alla raccolta
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
