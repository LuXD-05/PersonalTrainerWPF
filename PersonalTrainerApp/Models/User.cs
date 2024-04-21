using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PersonalTrainerApp.Models
{
    /// <summary>
    /// Classe utente
    /// </summary>
    [Serializable]
    public class User : INotifyPropertyChanged
    {
        #region Variables

        private string _username;
        private string _password;
        private DateTime _birthDate;
        private string _image;  // base64string
        private double _height; // cm
        private double _weight; // kg
        private ObservableCollection<Activity> _activities;

        private int _seriesId;

        //[NonSerialized]
        //private ObservableCollection<string> _labels;

        #endregion

        #region Properties

        public string Username
        {
            get { return _username; }
            set 
            { 
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set 
            { 
                _birthDate = value;
                OnPropertyChanged(nameof(BirthDate));
                OnPropertyChanged(nameof(AgeString));
            }
        }
        public string AgeString
        {
            get
            {
                // Se la data di nascita è gia inserita
                if (_birthDate != null && _birthDate != DateTime.MinValue)
                {
                    int age = DateTime.Today.Year - _birthDate.Year;
                    if (DateTime.Today < _birthDate.AddYears(age))
                        age--;
                    return age.ToString() + " anni";
                }
                // Altrimenti
                else
                    return "Data di nascita non inserita";
            }
        }
        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
                OnPropertyChanged(nameof(BitmapImage));
            }
        }
        public BitmapImage BitmapImage
        {
            get
            {
                byte[] bytes;

                // Se _image da db == "" o null, prendo l'immagine di default, altrimenti prendo quella da db
                if (string.IsNullOrEmpty(_image))
                    bytes = Convert.FromBase64String(App.DEFAULT_PFP);
                else
                    bytes = Convert.FromBase64String(_image);

                // Ottengo l'immagine tramite un memorystream e la ritorno
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(bytes);
                bmp.EndInit();
                return bmp;
            }
        }
        public double Height
        {
            get { return _height; }
            set 
            { 
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
        public double Weight // to int? cm
        {
            get { return _weight; }
            set 
            { 
                _weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }
        public ObservableCollection<Activity> Activities
        {
            get { return _activities; }
            set 
            { 
                _activities = value;
                OnPropertyChanged(nameof(Activities));
                OnPropertyChanged(nameof(TodoActivities));
                OnPropertyChanged(nameof(TodayActivities));
                OnPropertyChanged(nameof(ActivitiesDoneThisWeek));
                OnPropertyChanged(nameof(TotalActivitiesDone));
                OnPropertyChanged(nameof(TotalLengthDone));
                OnPropertyChanged(nameof(TotalCaloriesDone));
            }
        }
        public ObservableCollection<Activity> TodoActivities
        {
            get { return new ObservableCollection<Activity>(_activities.Where(x => !x.IsDone).OrderBy(x => x.DataFull)); }
        }
        public ObservableCollection<Activity> TodayActivities
        {
            get { return new ObservableCollection<Activity>(_activities.Where(x => !x.IsDone && x.DataFull >= DateTime.Today && x.DataFull <= DateTime.Today.AddDays(1).AddTicks(-1)).OrderBy(x => x.DataFull)); }
        }
        public ObservableCollection<Activity> ActivitiesDoneThisWeek
        {
            get
            {
                // Trovo 1° e ultimo giorno di settimana corrente
                DateTime start = DateTime.Today.Date.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                DateTime end = start.AddDays(6);
                // Ritorno attività fatte tra start e end
                return new ObservableCollection<Activity>(_activities.Where(x => x.IsDone && x.DataFull >= start && x.DataFull <= end).OrderBy(x => x.DataFull));
            }
        }
        public int TotalActivitiesDone
        {
            get { return _activities.Count(x => x.IsDone); }
        }
        public string TotalLengthDone
        {
            get { return _activities.Where(x => x.IsDone).Sum(x => x.Lunghezza) < 1000 ? _activities.Where(x => x.IsDone).Sum(x => x.Lunghezza) + " m" : _activities.Where(x => x.IsDone).Sum(x => x.Lunghezza) / 1000 + " km"; }
        }
        public string TotalCaloriesDone
        {
            get { return _activities.Where(x => x.IsDone).Sum(x => x.Calorie) < 1000 ? _activities.Where(x => x.IsDone).Sum(x => x.Calorie) + " cal" : _activities.Where(x => x.IsDone).Sum(x => x.Calorie) / 1000 + " kcal"; }
        }
        public int SeriesId
        {
            get { return _seriesId; }
            set 
            { 
                _seriesId = value;
                OnPropertyChanged(nameof(SeriesId));
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Costruttore vuoto
        /// </summary>
        public User() { }

        /// <summary>
        /// Costruttore con username e password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public User(string username, string password)
        {
            _username = username;
            _password = password;

            _activities = new ObservableCollection<Activity>();
        }

        #endregion

        #region Methods



        #endregion

        #region Overrides

        //public override bool Equals(object obj)
        //{
        //    return this._username.Equals((obj as User)._username) && this._password.Equals((obj as User)._password);
        //}

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
