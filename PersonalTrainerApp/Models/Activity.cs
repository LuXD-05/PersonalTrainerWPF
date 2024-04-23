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
    /// Class for activity date
    /// </summary>
    [Serializable]
    public class Activity : INotifyPropertyChanged
    {
        #region Variables

        public enum ActivityType
        {
            Camminata = 0,  // Walk
            Corsa = 1,      // Run
            Bicicletta = 2  // Bike
        }

        private string _name;
        private DateTime _date;
        private Location _coordinate;
        private double _length;
        private double _calories;
        private string _image;
        private bool _done;
        private ActivityType _type;

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set 
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Full datetime
        /// </summary>
        public DateTime FullDate
        {
            get { return _date; }
            set 
            { 
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(FullDate));
                    OnPropertyChanged(nameof(DatePart));
                    OnPropertyChanged(nameof(TimePart));
                    OnPropertyChanged(nameof(DateTimeString));
                    OnPropertyChanged(nameof(DateString));
                    OnPropertyChanged(nameof(TimeString));
                }
            }
        }

        /// <summary>
        /// Just the date part of the full date
        /// </summary>
        public DateTime DatePart => FullDate.Date;

        /// <summary>
        /// Just the time part of the full date
        /// </summary>
        public DateTime? TimePart => (DateTime?)DateTime.Today.Add(FullDate.TimeOfDay);

        /// <summary>
        /// Full date in string formatted to "dd/MM/yyyy HH:mm"
        /// </summary>
        public string DateTimeString
        {
            get { return _date.ToString("dd/MM/yyyy HH:mm"); }
        }

        /// <summary>
        /// Full date in string formatted to "dd/MM/yyyy"
        /// </summary>
        public string DateString
        {
            get { return _date.ToString("dd/MM/yyyy"); }
        }

        /// <summary>
        /// Full date in string formatted to "HH:mm"
        /// </summary>
        public string TimeString
        {
            get { return _date.ToString("HH:mm"); }
        }

        /// <summary>
        /// Location object of the MapsUI package
        /// </summary>
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

        public double Length
        {
            get { return _length; }
            set 
            { 
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged(nameof(Length));
                    OnPropertyChanged(nameof(LengthString));
                }
            }
        }

        /// <summary>
        /// Length in string, if (< 1000) in meters else in kilometers
        /// </summary>
        public string LengthString
        {
            get 
            {
                if (_length < 1000)
                    return _length + " m";
                else
                    return Math.Round(_length / 1000, 1) + " km";
            }
        }

        public double Calories
        {
            get { return _calories; }
            set 
            { 
                if (_calories != value)
                {
                    _calories = value;
                    OnPropertyChanged(nameof(Calories));
                    OnPropertyChanged(nameof(CaloriesString));
                }
            }
        }

        /// <summary>
        /// Calories in string, if (< 1000) cal else kcal
        /// </summary>
        public string CaloriesString
        {
            get
            {
                if (_calories < 1000)
                    return _calories + " cal";
                else
                    return Math.Round(_calories / 1000, 1) + " kcal";
            }
        }

        /// <summary>
        /// Image in string
        /// </summary>
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

        /// <summary>
        /// Image in GitmapImage
        /// </summary>
        public BitmapImage BitmapImage
        {
            get
            {
                byte[] bytes;

                // if (_image from db is null or "") gets default image else gets img from db
                if (string.IsNullOrEmpty(_image))
                    bytes = Convert.FromBase64String(App.DEFAULT_ACTIVITY);
                else
                    bytes = Convert.FromBase64String(_image);

                // Gets the img from a MemoryStream and returns it
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

        public ActivityType Type
        {
            get { return _type; }
            set 
            { 
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                    OnPropertyChanged(nameof(TypeString));
                    OnPropertyChanged(nameof(TypesListString));
                }
            }
        }

        /// <summary>
        /// Activity type in string
        /// </summary>
        public string TypeString
        {
            get { return Enum.GetName(typeof(ActivityType), _type); }
        }

        /// <summary>
        /// All activity types in string list
        /// </summary>
        public List<string> TypesListString
        {
            get { return Enum.GetNames(typeof(ActivityType)).ToList(); }
        }

        #endregion

        #region Constructors

        public Activity() { }

        /// <summary>
        /// Full parametrized constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="date">Datetime</param>
        /// <param name="coord">Coordinates (x,y)</param>
        /// <param name="length">Length in meters</param>
        /// <param name="calories">Calories in cal</param>
        /// <param name="type">Activity type (walk, run or bike)</param>
        /// <param name="done">True if completed</param>
        public Activity(string name, DateTime date, Location coord, double length, double calories, ActivityType type, string img, bool done)
        {
            _name = name;
            _date = date;
            _coordinate = coord;
            _length = length;
            _calories = calories;
            _done = false;
            _type = type;
            _image = img;
            _done = done;
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
