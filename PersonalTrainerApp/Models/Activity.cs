using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PersonalTrainerApp.Models
{
    /// <summary>
    /// Classe attività
    /// </summary>
    [Serializable]
    public class Activity : INotifyPropertyChanged
    {
        #region Variables

        public enum ActivityType
        {
            Camminata = 0,
            Corsa = 1,
            Bicicletta = 2
        }

        private string _nome;
        private string _apiActivityName;    // Vedere come legare a enum
        private DateTime _data;
        private Location _coordinate;
        private double _lunghezza;  // m
        private double _calorie;    // cal
        private string _image;      // base64string
        private bool _done;
        private ActivityType _tipo;

        #endregion

        #region Properties

        public string Nome
        {
            get { return _nome; }
            set 
            {
                if (_nome != value)
                {
                    _nome = value;
                    OnPropertyChanged(nameof(Nome));
                }
            }
        }
        public string ApiActivityName
        {
            get { return _apiActivityName; }
            set 
            { 
                if (_apiActivityName != value)
                {
                    _apiActivityName = value;
                    OnPropertyChanged(nameof(ApiActivityName));
                }
            }
        }
        public DateTime DataFull
        {
            get { return _data; }
            set 
            { 
                if (_data != value)
                {
                    _data = value;
                    OnPropertyChanged(nameof(DataFull));
                    OnPropertyChanged(nameof(DatePart));
                    OnPropertyChanged(nameof(TimePart));
                    OnPropertyChanged(nameof(DataOra));
                    OnPropertyChanged(nameof(Data));
                    OnPropertyChanged(nameof(Ora));
                }
            }
        }
        public DateTime DatePart => DataFull.Date;
        public DateTime? TimePart => (DateTime?)DateTime.Today.Add(DataFull.TimeOfDay);
        public string DataOra
        {
            get { return _data.ToString("dd/MM/yyyy HH:mm"); }
        }
        public string Data
        {
            get { return _data.ToString("dd/MM/yyyy"); }
        }
        public string Ora
        {
            get { return _data.ToString("HH:mm"); }
        }
        public Location Coordinate
        {
            get { return _coordinate; }
            set
            {
                if (_coordinate != value)
                {
                    _coordinate = value;
                    OnPropertyChanged(nameof(Coordinate));
                }
            }
        }
        public double Lunghezza
        {
            get { return _lunghezza; }
            set 
            { 
                if (_lunghezza != value)
                {
                    _lunghezza = value;
                    OnPropertyChanged(nameof(Lunghezza));
                    OnPropertyChanged(nameof(LunghezzaString));
                }
            }
        }
        public string LunghezzaString
        {
            get 
            {
                if (_lunghezza < 1000)
                    return _lunghezza + " m";
                else
                    return Math.Round(_lunghezza / 1000, 1) + " km";
            }
        }
        public double Calorie
        {
            get { return _calorie; }
            set 
            { 
                if (_calorie != value)
                {
                    _calorie = value;
                    OnPropertyChanged(nameof(Calorie));
                    OnPropertyChanged(nameof(CalorieString));
                }
            }
        }
        public string CalorieString
        {
            get
            {
                if (_calorie < 1000)
                    return _calorie + " cal";
                else
                    return Math.Round(_calorie / 1000, 1) + " kcal";
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
                    bytes = Convert.FromBase64String(App.DEFAULT_ACTIVITY);
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
        public bool IsDone
        {
            get { return _done; }
            set 
            { 
                if (_done != value)
                {
                    _done = value;
                    OnPropertyChanged(nameof(IsDone));
                }
            }
        }
        public ActivityType Tipo
        {
            get { return _tipo; }
            set 
            { 
                if (_tipo != value)
                {
                    _tipo = value;
                    OnPropertyChanged(nameof(Tipo));
                    OnPropertyChanged(nameof(TipoString));
                    OnPropertyChanged(nameof(TipiString));
                }
            }
        }
        public string TipoString
        {
            get { return Enum.GetName(typeof(ActivityType), _tipo); }
        }
        public List<string> TipiString
        {
            get { return Enum.GetNames(typeof(ActivityType)).ToList(); }
        }
        
        // fare Location

        #endregion

        #region Constructors

        /// <summary>
        /// Costruttore vuoto
        /// </summary>
        public Activity() { }

        /// <summary>
        /// Costruttore parametrizzato full
        /// </summary>
        /// <param name="nome">Nome dell'attività</param>
        /// <param name="data">Data e ora dell'attività</param>
        /// <param name="coord">Coordinata (x,y) dell'attività</param>
        /// <param name="lunghezza">Lunghezza in metri dell'attività</param>
        /// <param name="calorie">Calorie in calorie (non kilo) dell'attività</param>
        /// <param name="tipo">Tipo dell'attività (camminata, corsa o bicicletta)</param>
        /// <param name="done">True se l'attività è completata, altrimenti false</param>
        public Activity(string nome, DateTime data, Location coord, double lunghezza, double calorie, ActivityType tipo, string img, bool done)
        {
            _nome = nome;
            _data = data;
            _coordinate = coord;
            _lunghezza = lunghezza;
            _calorie = calorie;
            _done = false;
            _tipo = tipo;
            _image = img;
            _done = done;
        }

        #endregion

        #region Methods



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
