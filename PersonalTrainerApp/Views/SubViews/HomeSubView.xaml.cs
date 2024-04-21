using PersonalTrainerApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using PersonalTrainerApp.Models;
using System.Windows.Input;

namespace PersonalTrainerApp.Views.SubViews
{
    /// <summary>
    /// Logica di interazione per HomeSubView.xaml
    /// </summary>
    public partial class HomeSubView : UserControl
    {
        public HomeSubView()
        {
            InitializeComponent();

            // Imposto datacontext view a user
            DataContext = (Application.Current.MainWindow.DataContext as MainViewModel).User;

            // Imposto datacontext specifico del grafico
            lvcHome.DataContext = new ChartObject((Application.Current.MainWindow.DataContext as MainViewModel).User);
        }

        /// <summary>
        /// Evento che gestisce lo scroll orizzontale delle attività nella dashboard
        /// </summary>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta < 0)
            {
                scrollViewer.LineRight();
                scrollViewer.LineRight();
            }
            else
            {
                scrollViewer.LineLeft();
                scrollViewer.LineLeft();
            }
            e.Handled = true;
        }
    }
}
