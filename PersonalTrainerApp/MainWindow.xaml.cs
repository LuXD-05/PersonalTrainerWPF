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
    /// Main window / main ContentControl backend
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Sets the datacontext as a new MainViewModel
            DataContext = new MainViewModel();
        }

        /// <summary>
        /// Programmatically sets some bindings to the dimensions of the window (seemed to not work in set in the view)
        /// </summary>
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
