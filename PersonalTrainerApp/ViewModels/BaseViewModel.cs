using System.ComponentModel;

namespace PersonalTrainerApp.ViewModels
{
    /// <summary>
    /// Base ViewModel with INotifyPropertyChanged from which MainViewModel and others derive
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        private string _title;
        private string _error;
        private double _height;
        private double _width;

        /// <summary>
        /// View title property
        /// </summary>
        public string Title
        {
            get { return _title; }
            set 
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }


        /// <summary>
        /// Error string property
        /// </summary>
        public string Error
        {
            get { return _error; }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    OnPropertyChanged(nameof(Error));
                }
            }
        }

        /// <summary>
        /// Height property (to adapt window height to view's)
        /// </summary>
        public double Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        // Width property (to adapt window width to view's)
        public double Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}