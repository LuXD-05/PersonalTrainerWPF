using PersonalTrainerApp.ViewModels;
using System;
using System.Collections.Generic;
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

namespace PersonalTrainerApp
{
    /// <summary>
    /// Applicazione personal trainer
    /// 1) Accesso con login (e poi anche logout)
    /// 2) Home: registrazione percorsi di attività sportive.
    ///    Home: Visualizzazione attività ordinabili per: tempo impiegato o data esecuz
    /// 3) Attività: Descrizione, DataInizio, DataFine, Posizione in Latitudine e longitudine (partenza)
    ///    Attività: Stima calorie consumate dell'attività
    /// 4) Calendario: mensile (riquadri con colori diversi x attività)
    /// 5) *: Attività selezionabili (quindi inserire tool per multi edit/delete...)
    ///    *: Se clicco, si apre il dettaglio con il MapControl + posizione long e lat
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetBinding(Window.WidthProperty, new Binding("Width") { Source = DataContext as MainViewModel, Mode = BindingMode.TwoWay });
            this.SetBinding(Window.HeightProperty, new Binding("Height") { Source = DataContext as MainViewModel, Mode = BindingMode.TwoWay });
            this.SetBinding(Window.MaxWidthProperty, new Binding("Width") { Source = DataContext as MainViewModel });
            this.SetBinding(Window.MaxHeightProperty, new Binding("Height") { Source = DataContext as MainViewModel });
            this.SetBinding(Window.MinWidthProperty, new Binding("Width") { Source = DataContext as MainViewModel });
            this.SetBinding(Window.MinHeightProperty, new Binding("Height") { Source = DataContext as MainViewModel });
        }
    }
}
